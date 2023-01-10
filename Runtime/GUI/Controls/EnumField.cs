using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class EnumField : DropdownField , IProtobufField
    {
        public FieldDescriptor Descriptor { get; set; }

        public IMessage Message { get; set; }

        public EnumField(IMessage message, FieldDescriptor descriptor)
        {
            Message = message;
            Descriptor = descriptor;
            label = Descriptor.Name;

            choices = new List<string>();
            foreach(var enumValue in descriptor.EnumType.Values)
            {
                choices.Add(enumValue.Name);
            }
            
            index = Descriptor.GetValue<int>(message);
            RegisterCallback<ChangeEvent<string>>(OnValueChanged);
        }

        void OnValueChanged(ChangeEvent<string> changeEvent)
        {
            Descriptor.SetValue(Message, index);

            using (var evt = ChangeEvent<IMessage>.GetPooled(Message, Message))
            {
                evt.target = this;
                SendEvent(evt);
            }
        }
    }
}