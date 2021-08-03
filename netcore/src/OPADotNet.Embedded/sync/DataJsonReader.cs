/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using OPADotNet.Embedded.sync;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OPADotNet.Embedded.Sync
{
    /// <summary>
    /// Reads input data json
    /// </summary>
    public static class DataJsonReader
    {
        public static string FilterJson(Stream input, DataSetNode dataSetNode)
        {
            using MemoryStream memoryStream = new MemoryStream();
            var writer = new Utf8JsonWriter(memoryStream);
            var reader = new Utf8JsonStreamReader(input, 4096);
            FilterJson(ref reader, writer, dataSetNode);
            writer.Flush();
            memoryStream.Position = 0;
            using StreamReader streamReader = new StreamReader(memoryStream);
            return streamReader.ReadToEnd();
        }

        public static string FilterJson(string input, DataSetNode dataSetNode)
        {
            MemoryStream inputStream = new MemoryStream();

            var bytes = Encoding.UTF8.GetBytes(input);
            inputStream.Write(bytes);
            inputStream.Position = 0;

            return FilterJson(inputStream, dataSetNode);
        }

        private static void FilterJson(ref Utf8JsonStreamReader reader, Utf8JsonWriter writer, DataSetNode dataSetNode)
        {
            reader.Read();

            //If the data node is either the end node or used in a policy, copy all the values under it.
            if (dataSetNode.Children.Count == 0 || dataSetNode.UsedInPolicy)
            {
                Copy(ref reader, writer);
                return;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                writer.WriteStartObject();

                bool hasIterator = false;
                if (dataSetNode.Children.TryGetValue("$0", out var iteratorNode) && iteratorNode.IsVariable)
                {
                    hasIterator = true;
                }

                var depth = reader.CurrentDepth;
                reader.Read();
                while (depth < reader.CurrentDepth)
                {
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException("Expected property name");
                    }
                    var propertyName = reader.GetString();

                    if (hasIterator)
                    {
                        Copy(ref reader, writer); //Copy the property name
                        FilterJson(ref reader, writer, iteratorNode);
                        reader.Read();
                    }
                    //Check if there is a child with the property name
                    else if (dataSetNode.Children.TryGetValue(propertyName, out var node))
                    {
                        Copy(ref reader, writer); //Copy the property name
                        FilterJson(ref reader, writer, node);
                        reader.Read();
                    }
                    else //Otherwise try and skip
                    {
                        reader.Read();
                        if (reader.TokenType == JsonTokenType.StartArray || reader.TokenType == JsonTokenType.StartObject)
                        {
                            reader.Skip();
                        }
                        else
                        {
                            //Skip the field
                            reader.Read();
                        }
                    }   
                }
                writer.WriteEndObject();
            }
            else if  (reader.TokenType == JsonTokenType.StartArray)
            {
                writer.WriteStartArray();

                bool hasIterator = false;
                if (dataSetNode.Children.TryGetValue("$0", out var iteratorNode) && iteratorNode.IsVariable)
                {
                    hasIterator = true;
                }

                if (!hasIterator && dataSetNode.Children.Count > 0)
                {
                    throw new InvalidOperationException("Found array access without the use of a variable");
                }

                var depth = reader.CurrentDepth;
                do
                {
                    FilterJson(ref reader, writer, iteratorNode);
                } while (depth < reader.CurrentDepth);

                writer.WriteEndArray();
            }
            else
            {
                Copy(ref reader, writer);
            }
        }

        private static void Copy(ref Utf8JsonStreamReader reader, Utf8JsonWriter writer)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                case JsonTokenType.StartArray:
                    CopyObject(ref reader, writer);
                    break;
                case JsonTokenType.Number:
                    CopyNumber(ref reader, writer);
                    break;
                case JsonTokenType.String:
                    CopyString(ref reader, writer);
                    break;
                case JsonTokenType.Null:
                    CopyNull(ref reader, writer);
                    break;
                case JsonTokenType.False:
                    writer.WriteBooleanValue(false);
                    break;
                case JsonTokenType.True:
                    writer.WriteBooleanValue(true);
                    break;
                case JsonTokenType.PropertyName:
                    CopyPropertyName(ref reader, writer);
                    break;
            }
        }

        private static void CopyPropertyName(ref Utf8JsonStreamReader reader, Utf8JsonWriter writer)
        {
            var propertyName = reader.GetString();
            writer.WritePropertyName(propertyName);
        }

        private static void CopyNull(ref Utf8JsonStreamReader reader, Utf8JsonWriter writer)
        {
            writer.WriteNullValue();
        }

        private static void CopyString(ref Utf8JsonStreamReader reader, Utf8JsonWriter writer)
        {
            var val = reader.GetString();
            writer.WriteStringValue(val);
        }

        private static void CopyNumber(ref Utf8JsonStreamReader reader, Utf8JsonWriter writer)
        {
            var dec = reader.GetDecimal();
            writer.WriteNumberValue(dec);
        }

        private static void CopyObject(ref Utf8JsonStreamReader reader, Utf8JsonWriter writer)
        {
            bool isObject = false;
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                writer.WriteStartObject();
                isObject = true;
            }
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                writer.WriteStartArray();
            }
            else
            {
                throw new JsonException("Expected array or object.");
            }
            int startDepth = reader.CurrentDepth;
            do
            {
                bool result = reader.Read();
                Copy(ref reader, writer);
            }
            while (startDepth < reader.CurrentDepth);

            if (isObject)
            {
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteEndArray();
            }
        }

        //From https://stackoverflow.com/questions/54983533/parsing-a-json-file-with-net-core-3-0-system-text-json
        private ref struct Utf8JsonStreamReader
        {
            private readonly Stream _stream;
            private readonly int _bufferSize;

            private SequenceSegment? _firstSegment;
            private int _firstSegmentStartIndex;
            private SequenceSegment? _lastSegment;
            private int _lastSegmentEndIndex;

            private Utf8JsonReader _jsonReader;
            private bool _keepBuffers;
            private bool _isFinalBlock;

            public Utf8JsonStreamReader(Stream stream, int bufferSize)
            {
                _stream = stream;
                _bufferSize = bufferSize;

                _firstSegment = null;
                _firstSegmentStartIndex = 0;
                _lastSegment = null;
                _lastSegmentEndIndex = -1;

                _jsonReader = default;
                _keepBuffers = false;
                _isFinalBlock = false;
            }

            public bool Read()
            {
                // read could be unsuccessful due to insufficient bufer size, retrying in loop with additional buffer segments
                while (!_jsonReader.Read())
                {
                    if (_isFinalBlock)
                        return false;

                    MoveNext();
                }

                return true;
            }

            private void MoveNext()
            {
                var firstSegment = _firstSegment;
                _firstSegmentStartIndex += (int)_jsonReader.BytesConsumed;

                // release previous segments if possible
                if (!_keepBuffers)
                {
                    while (firstSegment?.Memory.Length <= _firstSegmentStartIndex)
                    {
                        _firstSegmentStartIndex -= firstSegment.Memory.Length;
                        firstSegment.Dispose();
                        firstSegment = (SequenceSegment?)firstSegment.Next;
                    }
                }

                // create new segment
                var newSegment = new SequenceSegment(_bufferSize, _lastSegment);

                if (firstSegment != null)
                {
                    _firstSegment = firstSegment;
                    newSegment.Previous = _lastSegment;
                    _lastSegment?.SetNext(newSegment);
                    _lastSegment = newSegment;
                }
                else
                {
                    _firstSegment = _lastSegment = newSegment;
                    _firstSegmentStartIndex = 0;
                }

                // read data from stream
                _lastSegmentEndIndex = _stream.Read(newSegment.Buffer.Memory.Span);
                _isFinalBlock = _lastSegmentEndIndex < newSegment.Buffer.Memory.Length;
                _jsonReader = new Utf8JsonReader(new ReadOnlySequence<byte>(_firstSegment, _firstSegmentStartIndex, _lastSegment, _lastSegmentEndIndex), _isFinalBlock, _jsonReader.CurrentState);
            }

            public void Dispose() => _lastSegment?.Dispose();

            public int CurrentDepth => _jsonReader.CurrentDepth;
            public bool HasValueSequence => _jsonReader.HasValueSequence;
            public long TokenStartIndex => _jsonReader.TokenStartIndex;
            public JsonTokenType TokenType => _jsonReader.TokenType;
            public ReadOnlySequence<byte> ValueSequence => _jsonReader.ValueSequence;
            public ReadOnlySpan<byte> ValueSpan => _jsonReader.ValueSpan;

            public bool GetBoolean() => _jsonReader.GetBoolean();
            public byte GetByte() => _jsonReader.GetByte();
            public byte[] GetBytesFromBase64() => _jsonReader.GetBytesFromBase64();
            public string GetComment() => _jsonReader.GetComment();
            public DateTime GetDateTime() => _jsonReader.GetDateTime();
            public DateTimeOffset GetDateTimeOffset() => _jsonReader.GetDateTimeOffset();
            public decimal GetDecimal() => _jsonReader.GetDecimal();
            public double GetDouble() => _jsonReader.GetDouble();
            public Guid GetGuid() => _jsonReader.GetGuid();
            public short GetInt16() => _jsonReader.GetInt16();
            public int GetInt32() => _jsonReader.GetInt32();
            public long GetInt64() => _jsonReader.GetInt64();
            public sbyte GetSByte() => _jsonReader.GetSByte();
            public float GetSingle() => _jsonReader.GetSingle();
            public string GetString() => _jsonReader.GetString();
            public uint GetUInt32() => _jsonReader.GetUInt32();
            public ulong GetUInt64() => _jsonReader.GetUInt64();
            public bool TryGetDecimal(out byte value) => _jsonReader.TryGetByte(out value);
            public bool TryGetBytesFromBase64(out byte[] value) => _jsonReader.TryGetBytesFromBase64(out value);
            public bool TryGetDateTime(out DateTime value) => _jsonReader.TryGetDateTime(out value);
            public bool TryGetDateTimeOffset(out DateTimeOffset value) => _jsonReader.TryGetDateTimeOffset(out value);
            public bool TryGetDecimal(out decimal value) => _jsonReader.TryGetDecimal(out value);
            public bool TryGetDouble(out double value) => _jsonReader.TryGetDouble(out value);
            public bool TryGetGuid(out Guid value) => _jsonReader.TryGetGuid(out value);
            public bool TryGetInt16(out short value) => _jsonReader.TryGetInt16(out value);
            public bool TryGetInt32(out int value) => _jsonReader.TryGetInt32(out value);
            public bool TryGetInt64(out long value) => _jsonReader.TryGetInt64(out value);
            public bool TryGetSByte(out sbyte value) => _jsonReader.TryGetSByte(out value);
            public bool TryGetSingle(out float value) => _jsonReader.TryGetSingle(out value);
            public bool TryGetUInt16(out ushort value) => _jsonReader.TryGetUInt16(out value);
            public bool TryGetUInt32(out uint value) => _jsonReader.TryGetUInt32(out value);
            public bool TryGetUInt64(out ulong value) => _jsonReader.TryGetUInt64(out value);

            public void Skip()
            {
                if (TokenType == JsonTokenType.PropertyName)
                {
                    bool result = Read();
                }

                if (TokenType == JsonTokenType.StartObject || TokenType == JsonTokenType.StartArray)
                {
                    int depth = CurrentDepth;
                    do
                    {
                        bool result = Read();
                        Debug.Assert(result);
                    }
                    while (depth < CurrentDepth);
                }
            }

            private sealed class SequenceSegment : ReadOnlySequenceSegment<byte>, IDisposable
            {
                internal IMemoryOwner<byte> Buffer { get; }
                internal SequenceSegment? Previous { get; set; }
                private bool _disposed;

                public SequenceSegment(int size, SequenceSegment? previous)
                {
                    Buffer = MemoryPool<byte>.Shared.Rent(size);
                    Previous = previous;

                    Memory = Buffer.Memory;
                    RunningIndex = previous?.RunningIndex + previous?.Memory.Length ?? 0;
                }

                public void SetNext(SequenceSegment next) => Next = next;

                public void Dispose()
                {
                    if (!_disposed)
                    {
                        _disposed = true;
                        Buffer.Dispose();
                        Previous?.Dispose();
                    }
                }
            }
        }

    }
}
