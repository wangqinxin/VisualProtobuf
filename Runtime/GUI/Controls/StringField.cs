using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class StringField : TextField, IProtobufField
    {
        public FieldDescriptor Descriptor { get; set; }

        public IMessage Message { get; set; }

        public System.Action<object, object> OnValueChanged { get; set; }

        public StringField(IMessage message, FieldDescriptor descriptor)
        {
            Message = message;
            Descriptor = descriptor;
            label = Descriptor.GetDisplayName();
            value = Descriptor.GetValue<string>(message);
            labelElement.RegisterCallback<ChangeEvent<string>>(OnLabelValueChanged);
            RegisterCallback<ChangeEvent<string>>(OnFieldValueChanged);
        }

        void OnLabelValueChanged(ChangeEvent<string> changeEvent)
        {
            changeEvent.StopPropagation();
        }

        void OnFieldValueChanged(ChangeEvent<string> changeEvent)
        {
            if (OnValueChanged != null)
                OnValueChanged.Invoke(changeEvent.previousValue, changeEvent.newValue);
            else
                Descriptor.SetValue(Message, changeEvent.newValue);

            changeEvent.StopPropagation();

            using var evt = ChangeEvent<IMessage>.GetPooled(Message, Message);
            evt.target = parent;
            SendEvent(evt);
        }

        public void SetLabel(string label)
        {
            this.label = label;
        }

        public void SetValue(object value, bool notify)
        {
            if (notify)
                this.value = System.Convert.ToString(value);
            else
                SetValueWithoutNotify(System.Convert.ToString(value));
        }

        public object GetValue()
        {
            return value;
        }
    }
}