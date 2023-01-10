using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class StringField : TextField, IProtobufField
    {
        public FieldDescriptor Descriptor { get; set; }

        public IMessage Message { get; set; }

        public StringField(IMessage message, FieldDescriptor descriptor)
        {
            Message = message;
            Descriptor = descriptor;
            label = Descriptor.Name;
            value = Descriptor.GetValue<string>(message);
            RegisterCallback<ChangeEvent<string>>(OnValueChanged);
        }

        void OnValueChanged(ChangeEvent<string> changeEvent)
        {
            Descriptor.SetValue(Message, changeEvent.newValue);

            using (var evt = ChangeEvent<IMessage>.GetPooled(Message,Message))
            {
                evt.target = this;
                SendEvent(evt);
            }
        }
    }
}