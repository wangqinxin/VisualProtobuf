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

        public NumberBaseField(IMessage message, FieldDescriptor descriptor)
        {
            Message = message;
            Descriptor = descriptor;
            var field = CreateFiled();
            field.label = Descriptor.Name;
            field.value = GetValue();
            field.RegisterCallback<ChangeEvent<T>>(OnValueChanged);
            Add(field);
        }

        protected virtual BaseField<T> CreateFiled()
        {
            return null;
        }

        protected virtual T GetValue()
        {
            return Descriptor.GetValue<T>(Message);
        }

        protected virtual void SetValue(T val)
        {
            Descriptor.SetValue(Message, val);
        }

        void OnValueChanged(ChangeEvent<T> changeEvent)
        {
            SetValue(changeEvent.newValue);

            using (var evt = ChangeEvent<IMessage>.GetPooled(Message, Message))
            {
                evt.target = this;
                SendEvent(evt);
            }
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
            return new UnityEngine.UIElements.IntegerField();
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
    }

}