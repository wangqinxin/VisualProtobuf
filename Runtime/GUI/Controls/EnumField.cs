using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class EnumField : DropdownField, IProtobufField
    {
        public FieldDescriptor Descriptor { get; set; }
        public IMessage Message { get; set; }
        public System.Action<object, object> OnValueChanged { get; set; }

        public EnumField(IMessage message, FieldDescriptor descriptor)
        {
            Message = message;
            Descriptor = descriptor;
            label = Descriptor.GetDisplayName();

            choices = new List<string>();
            foreach (var enumValue in descriptor.EnumType.Values)
            {
                choices.Add(enumValue.Name);
            }

            index = Descriptor.GetValue<int>(message);
            labelElement.RegisterCallback<ChangeEvent<string>>(OnLabelValueChanged);
            RegisterCallback<ChangeEvent<string>>(OnFieldValueChanged);
        }

        void OnLabelValueChanged(ChangeEvent<string> changeEvent)
        {
            changeEvent.StopPropagation();
        }

        void OnFieldValueChanged(ChangeEvent<string> changeEvent)
        {
            if (OnValueChanged != null) {
                var previousIndex = choices.IndexOf(changeEvent.previousValue);
                OnValueChanged.Invoke(previousIndex, index);
            }
            else
                Descriptor.SetValue(Message, index);

            using var evt = ChangeEvent<IMessage>.GetPooled(Message, Message);
            evt.target = parent;
            SendEvent(evt);
        }

        public void SetValue(object value, bool notify)
        {
            if (value is int index)
            {
                if (notify)
                    this.index = index;
                else
                {
                    if (index >= 0 && index < choices.Count)
                    {
                        SetValueWithoutNotify(choices[index]);
                    }
                }

            }
        }

        public object GetValue()
        {
            return index;
        }

        public void SetLabel(string label)
        {
            this.label = label;
        }
    }
}