using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public abstract class NumberBaseField<T> : VisualElement, IProtobufField
    {
        public FieldDescriptor Descriptor { get; set; }

        public IMessage Message { get; set; }

        public System.Action<object, object> OnValueChanged { get; set; }

        private BaseField<T> m_BaseField;

        public NumberBaseField(IMessage message, FieldDescriptor descriptor)
        {
            Message = message;
            Descriptor = descriptor;
            m_BaseField = CreateFiled();
            m_BaseField.label = Descriptor.GetDisplayName();
            m_BaseField.value = GetValue();
            m_BaseField.RegisterCallback<ChangeEvent<T>>(OnFieldValueChanged);
            Add(m_BaseField);
        }

        protected virtual BaseField<T> CreateFiled()
        {
            return null;
        }

        protected virtual T GetValue()
        {
            return Descriptor.GetValue<T>(Message);
        }

        protected virtual bool Validate(T val)
        {
            return true;
        }

        protected virtual void SetValue(T val)
        {
            Descriptor.SetValue(Message, val);
        }

        void OnFieldValueChanged(ChangeEvent<T> changeEvent)
        {
            if (!Validate(changeEvent.newValue))
            {
                m_BaseField.SetValueWithoutNotify(changeEvent.previousValue);
                return;
            }
            if (OnValueChanged != null)
                OnValueChanged.Invoke(changeEvent.previousValue, changeEvent.newValue);
            else
                SetValue(changeEvent.newValue);

            changeEvent.StopPropagation();

            using var evt = ChangeEvent<IMessage>.GetPooled(Message, Message);
            evt.target = parent;
            SendEvent(evt);
        }

        public void SetLabel(string label)
        {
            m_BaseField.label = label;
        }

        public void SetValue(object value,bool notify)
        {
            if (value is T val)
            {
                if (notify)
                    m_BaseField.value = val;
                else
                    m_BaseField.SetValueWithoutNotify(val);
            }
        }

        object IProtobufField.GetValue()
        {
            return m_BaseField.value;
        }
    }

    public class FloatField : NumberBaseField<float>
    {
        public FloatField(IMessage message, FieldDescriptor descriptor) : base(message, descriptor)
        {
        }

        protected override BaseField<float> CreateFiled()
        {
            return new UnityEngine.UIElements.FloatField();
        }
    }

    public class DoubleField : NumberBaseField<double>
    {
        public DoubleField(IMessage message, FieldDescriptor descriptor) : base(message, descriptor)
        {

        }

        protected override BaseField<double> CreateFiled()
        {
            return new UnityEngine.UIElements.DoubleField();
        }
    }

    public class IntField : NumberBaseField<int>
    {
        public IntField(IMessage message, FieldDescriptor descriptor) : base(message, descriptor)
        {

        }

        protected override BaseField<int> CreateFiled()
        {
            return new IntegerField();
        }
    }

    public class LongField : NumberBaseField<long>
    {
        public LongField(IMessage message, FieldDescriptor descriptor) : base(message, descriptor)
        {

        }

        protected override BaseField<long> CreateFiled()
        {
            return new UnityEngine.UIElements.LongField();
        }
    }

    public class UintField : LongField
    {
        public UintField(IMessage message, FieldDescriptor descriptor) : base(message, descriptor)
        {

        }

        protected override long GetValue()
        {
            return System.Convert.ToInt64(Descriptor.GetValue<uint>(Message));
        }

        protected override void SetValue(long val)
        {
            Descriptor.SetValue<uint>(Message, System.Convert.ToUInt32(val));
        }

        protected override bool Validate(long val)
        {
            return val >= uint.MinValue && val <= uint.MaxValue;
        }
    }


    public class UlongField : LongField
    {
        public UlongField(IMessage message, FieldDescriptor descriptor) : base(message, descriptor)
        {

        }

        protected override long GetValue()
        {
            return System.Convert.ToInt64(Descriptor.GetValue<ulong>(Message));
        }

        protected override void SetValue(long val)
        {
            Descriptor.SetValue<ulong>(Message, System.Convert.ToUInt64(val));
        }

        protected override bool Validate(long val)
        {
            return val >= (long)ulong.MinValue && val <= long.MaxValue;
        }
    }

}