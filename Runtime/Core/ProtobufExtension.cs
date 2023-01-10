using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf
{
    public static class ProtobufExtension 
    {
        public static T GetValue<T>(this FieldDescriptor fieldDescriptor,IMessage message)
        {
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

        public static void SetValue<T> (this FieldDescriptor fieldDescriptor,IMessage message,T v)
        {
            if (fieldDescriptor.Accessor == null) return;
            fieldDescriptor.Accessor.SetValue(message, v);
        }
    }
}