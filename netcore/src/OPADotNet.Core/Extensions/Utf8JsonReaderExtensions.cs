using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OPADotNet.Extensions
{
    internal static class Utf8JsonReaderExtensions
    {
        public static void CheckType(ref this Utf8JsonReader reader, JsonTokenType type)
        {
            if (reader.TokenType != type)
            {
                throw new JsonException();
            }
        }

        public static void ReadThrow(ref this Utf8JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new JsonException();
            }
        }

        public static void ReadCheckType(ref this Utf8JsonReader reader, JsonTokenType type)
        {
            if (!reader.Read() || reader.TokenType != type)
            {
                throw new JsonException();
            }
        }

        public static string GetPropertyName(ref this Utf8JsonReader reader)
        {
            reader.CheckType(JsonTokenType.PropertyName);
            return reader.GetString();
        }

        public static string ReadPropertyName(ref this Utf8JsonReader reader)
        {
            reader.ReadCheckType(JsonTokenType.PropertyName);
            return reader.GetString();
        }

        public static void ReadExpectPropertyName(ref this Utf8JsonReader reader, string name)
        {
            string propertyName = reader.ReadPropertyName();
            if (propertyName != name)
            {
                throw new JsonException($"Expected property name '{name}' but got: {propertyName}");
            }
        }

        public static void ExpectPropertyName(ref this Utf8JsonReader reader, string name)
        {
            string propertyName = reader.GetPropertyName();
            if (propertyName != name)
            {
                throw new JsonException($"Expected property name '{name}' but got: {propertyName}");
            }
        }

        public static string ReadGetString(ref this Utf8JsonReader reader)
        {
            reader.ReadCheckType(JsonTokenType.String);
            return reader.GetString();
        }

        public static decimal ReadGetDecimal(ref this Utf8JsonReader reader)
        {
            reader.ReadCheckType(JsonTokenType.Number);
            return reader.GetDecimal();
        }

        public static bool ReadGetBoolean(ref this Utf8JsonReader reader)
        {
            reader.ReadThrow();

            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }
            else if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }
            else
            {
                throw new JsonException($"Expected boolean but got '{reader.TokenType}'");
            }
        }

        public static void ReadExpectStringValue(ref this Utf8JsonReader reader, string value)
        {
            string v = reader.ReadGetString();
            if (v != value)
            {
                throw new JsonException($"Expected string value: '{value}', but got: '{v}'");
            }
        }

        public static void ReadUntil(ref this Utf8JsonReader reader, JsonTokenType type)
        {
            bool foundEnd = false;
            while (reader.Read())
            {
                if (reader.TokenType == type)
                {
                    foundEnd = true;
                    break;
                }
            }

            if (!foundEnd)
            {
                throw new JsonException($"Could not find a token of type: {type}");
            }
        }
    }
}
