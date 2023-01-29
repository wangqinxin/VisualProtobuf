using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class BoolField : BaseBoolField, IProtobufField
    {
        public new static readonly string ussClassName = "bool_field";
        public new static readonly string inputUssClassName = ussClassName + "__input";
        public static readonly string checkmarkUssClassName = ussClassName + "__checkmark";

        public FieldDescriptor Descriptor { get; set; }

        public IMessage Message { get; set; }

        public System.Action<object, object> OnValueChanged { get; set; }

        public BoolField(string label) : base(label)
        {
            this.label = label;
        }

        public BoolField(IMessage message, FieldDescriptor descriptor) : this(descriptor.GetDisplayName())
        {
            Message = message;
            Descriptor = descriptor;

            AddToClassList(ussClassName);
            m_CheckMark.AddToClassList(checkmarkUssClassName);
            m_CheckMark.parent.AddToClassList(inputUssClassName);
            
            value = Descriptor.GetValue<bool>(message);
            RegisterCallback<ChangeEvent<bool>>(OnFieldValueChanged);
        }

        void OnFieldValueChanged(ChangeEvent<bool> changeEvent)
        {
            if (OnValueChanged != null)
                OnValueChanged.Invoke(changeEvent.previousValue, changeEvent.newValue);
            else
                Descriptor.SetValue(Message, changeEvent.newValue);

            using var evt = ChangeEvent<IMessage>.GetPooled(Message, Message);
            evt.target = parent;
            SendEvent(evt);
        }

        public void SetValue(object value, bool notify)
        {
            if (notify)
                this.value = System.Convert.ToBoolean(value);
            else
                SetValueWithoutNotify(System.Convert.ToBoolean(value));
        }

        public object GetValue()
        {
            return value;
        }

        public void SetLabel(string label)
        {
            this.label = label;
        }
    }
}