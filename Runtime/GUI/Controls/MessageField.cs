using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class MessageField : VisualElement, IProtobufField
    {
        public FieldDescriptor Descriptor { get; set; }
        public IMessage Message { get; set; }

        public MessageField(IMessage message, FieldDescriptor descriptor)
        {
            Descriptor = descriptor;
            Message = message;

            var fields = descriptor.MessageType.Fields;
            foreach (var fieldDesc in fields.InFieldNumberOrder())
            {
                Add(CreateFieldGUI(fieldDesc, message));
            }
        }

        public MessageField(IMessage message)
        {
            Message = message;

            var fields = Message.Descriptor.Fields;
            foreach (var fieldDesc in fields.InFieldNumberOrder())
            {
                Add(CreateFieldGUI(fieldDesc, message));
            }
        }

        VisualElement CreateFieldGUI(FieldDescriptor descriptor, IMessage message)
        {
            switch (descriptor.FieldType)
            {
                case FieldType.Double:
                    return new DoubleField(message, descriptor);
                case FieldType.Float:
                    return new FloatField(message, descriptor);
                case FieldType.Int32:
                case FieldType.SInt32:
                case FieldType.SFixed32:
                    return new IntField(message, descriptor);
                case FieldType.Int64:
                case FieldType.SInt64:
                case FieldType.SFixed64:
                    return new LongField(message, descriptor);
                case FieldType.UInt32:
                case FieldType.Fixed32:
                    return new UintField(message, descriptor);
                case FieldType.UInt64:
                case FieldType.Fixed64:
                    return new UlongField(message, descriptor);
                case FieldType.Bool:
                    return new BoolField(message, descriptor);
                case FieldType.String:
                    return new StringField(message, descriptor);
                case FieldType.Enum:
                    return new EnumField(message, descriptor);
                //case FieldType.Message:
                //    return new MessageField(message, descriptor);
                default: return null;
            }
        }
    }
}