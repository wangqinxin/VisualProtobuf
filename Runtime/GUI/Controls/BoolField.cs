using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{

    public class BoolField : Toggle , IProtobufField
    {
        public FieldDescriptor Descriptor { get; set; }

        public IMessage Message { get; set; }

        public BoolField(IMessage message, FieldDescriptor descriptor)
        {
            Message = message;
            Descriptor = descriptor;
            label = Descriptor.Name;
            value = Descriptor.GetValue<bool>(message);
            RegisterCallback<ChangeEvent<bool>>(OnValueChanged);
        }

        void OnValueChanged(ChangeEvent<bool> changeEvent)
        {
            Descriptor.SetValue(Message, changeEvent.newValue);

            using (var evt = ChangeEvent<IMessage>.GetPooled(Message, Message))
            {
                evt.target = this;
                SendEvent(evt);
            }
        }
    }
}