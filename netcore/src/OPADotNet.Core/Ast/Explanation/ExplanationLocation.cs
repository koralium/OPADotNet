using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace OPADotNet.Core.Ast.Explanation
{
    [DebuggerDisplay("File = {File}, Row = {Row}")]
    public struct ExplanationLocation : IEquatable<ExplanationLocation>, IComparable<ExplanationLocation>
    {
        [JsonPropertyName("file")]
        public string File { get; set; }

        [JsonPropertyName("row")]
        public int Row { get; set; }

        public int CompareTo(ExplanationLocation other)
        {
            int result = File.CompareTo(other.File);

            if (result != 0)
            {
                return result;
            }

            return Row.CompareTo(other.Row);
        }

        public override bool Equals(object obj)
        {
            if (obj is ExplanationLocation explanationLocation)
            {
                return Equals(explanationLocation);
            }
            return false;
        }

        public bool Equals(ExplanationLocation other)
        {
            return Equals(File, other.File) &&
                Equals(Row, other.Row);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(File, Row);
        }
    }
}
