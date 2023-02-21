using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf
{
    public static class ProtobufExtension
    {
        public static T GetValue<T>(this FieldDescriptor fieldDescriptor, IMessage message)
        {
            if (message == null) return default;
            if (fieldDescriptor.Accessor != null)
            {
                var value = fieldDescriptor.Accessor.GetValue(message);
                if (value != null && value is T t)
                {
                    return t;
                }
            }
            return default(T);
        }

        public static void SetValue<T>(this FieldDescriptor fieldDescriptor, IMessage message, T v)
        {
            if (fieldDescriptor.Accessor == null || message == null) return;
            fieldDescriptor.Accessor.SetValue(message, v);
        }

        public static bool HasValue<T>(this FieldDescriptor fieldDescriptor, IMessage message)
        {
            if (fieldDescriptor.Accessor == null || message == null) return false;
            return fieldDescriptor.Accessor.HasValue(message);
        }

        public static string GetDisplayName(this IDescriptor descriptor)
        {
            return descriptor.GetName(LanguageCode.ZH_CN);
            return UnderscoresToCamelCase(descriptor.Name, true, false);
        }

        public static string GetDisplayTooltip(this IDescriptor descriptor)
        {
            return descriptor.GetTooltip(LanguageCode.ZH_CN);
        }

        public static object GetDefaultSingularValue(this FieldDescriptor fieldDescriptor)
        {
            switch (fieldDescriptor.FieldType)
            {
                case FieldType.Bool:
                    return default(bool);
                case FieldType.Bytes:
                    return ByteString.Empty;
                case FieldType.String:
                    return string.Empty;
                case FieldType.Double:
                    return default(double);
                case FieldType.SInt32:
                case FieldType.Int32:
                case FieldType.SFixed32:
                case FieldType.Enum:
                    return default(int);
                case FieldType.Fixed32:
                case FieldType.UInt32:
                    return default(uint);
                case FieldType.Fixed64:
                case FieldType.UInt64:
                    return default(ulong);
                case FieldType.SFixed64:
                case FieldType.Int64:
                case FieldType.SInt64:
                    return default(long);
                case FieldType.Float:
                    return default(float);
                case FieldType.Message:
                case FieldType.Group:
                    return null;
                default: return null;
            }
        }

        static string UnderscoresToCamelCase(string input, bool capNextLetter, bool preservePeriod)
        {
            var result = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (char.IsLower(c))
                {
                    if (capNextLetter)
                    {
                        if (i != 0) result.Append(" ");
                        result.Append(char.ToUpper(c));
                    }
                    else
                    {
                        result.Append(c);
                    }
                    capNextLetter = false;
                }
                else if (char.IsUpper(c))
                {
                    if (i == 0 && !capNextLetter)
                    {
                        result.Append(char.ToLower(c));
                    }
                    else
                    {
                        result.Append(" ");
                        result.Append(c);
                    }
                    capNextLetter = false;
                }
                else if (char.IsDigit(c))
                {
                    result.Append(c);
                    capNextLetter = true;
                }
                else
                {
                    capNextLetter = true;
                    if (c == '.' && preservePeriod)
                    {
                        result.Append('.');
                    }
                }
            }
            if (input.Length > 0 && input[input.Length - 1] == '#')
            {
                result.Append('_');
            }
            return result.ToString();
        }
    }
}