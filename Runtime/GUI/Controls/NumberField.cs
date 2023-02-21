using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public abstract class NumberBaseField<T> : VisualElement, IProtobufField where T : struct, System.IComparable<T>
    {
        public FieldDescriptor Descriptor { get; set; }

        public IMessage Message { get; set; }

        private readonly T m_MinValue;
        public T MinValue => m_MinValue;

        private readonly T m_MaxValue;
        public T MaxValue => m_MaxValue;

        private readonly bool m_AsSlider;
        public bool AsSlider => m_AsSlider;

        public System.Action<object, object> OnValueChanged { get; set; }

        private BaseField<T> m_BaseField;
        protected BaseField<T> BaseField => m_BaseField;

        public NumberBaseField(IMessage message, FieldDescriptor descriptor)
        {
            Message = message;
            Descriptor = descriptor;
            TryGetValueRange(out m_MinValue, out m_MaxValue, out m_AsSlider);

            m_BaseField = CreateFiled();
            m_BaseField.label = Descriptor.GetDisplayName();
            var displayTooltip = descriptor.GetDisplayTooltip();
            if (!string.IsNullOrEmpty(displayTooltip)) m_BaseField.tooltip = displayTooltip;
            m_BaseField.value = GetValue();
            m_BaseField.RegisterCallback<ChangeEvent<T>>(OnFieldValueChanged);
            Add(m_BaseField);
        }

        protected virtual BaseField<T> CreateFiled()
        {
            return null;
        }

        protected virtual void TryGetValueRange(out T min, out T max, out bool slider)
        {
            Descriptor.TryGetValueRange<T>(out min, out max, out slider);
        }

        protected virtual T GetValue()
        {
            return Descriptor.GetValue<T>(Message);
        }

        protected bool Validate(T val)
        {
            return val.CompareTo(MinValue) >= 0 && val.CompareTo(MaxValue) <= 0;
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

        public void SetValue(object value, bool notify)
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
        bool m_IsPercent;
        int? m_PercentPrecision;

        public FloatField(IMessage message, FieldDescriptor descriptor) : base(message, descriptor)
        {
            m_IsPercent = descriptor.IsPercent();
            m_PercentPrecision = null;
            if (descriptor.TryGetPercentPrecision(out int precision))
            {
                m_PercentPrecision = precision;
            }
        }

        protected override BaseField<float> CreateFiled()
        {
            if (Descriptor.IsPercent())
            {
                var slider = new Slider
                {
                    showInputField = true,
                    lowValue = 0,
                    highValue = 100
                };
                var percentSymbol = new Label("%");
                percentSymbol.style.alignSelf = Align.Center;
                slider.Add(percentSymbol);
                return slider;
            }
            if (AsSlider)
            {
                return new Slider
                {
                    showInputField = true,
                    lowValue = MinValue,
                    highValue = MaxValue
                };
            }
            return new UnityEngine.UIElements.FloatField();
        }

        protected override void SetValue(float val)
        {
            if (m_IsPercent)
            {
                val *= 0.01f;
                if (m_PercentPrecision != null)
                {
                    val = (float)System.Math.Round(val, m_PercentPrecision.Value);
                    BaseField.SetValueWithoutNotify(val * 100);
                }
            }
            base.SetValue(val);
        }

        protected override float GetValue()
        {
            var val = base.GetValue();
            if (m_IsPercent) val *= 100;
            return val;
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
            if (AsSlider)
            {
                return new SliderInt()
                {
                    showInputField = true,
                    lowValue = MinValue,
                    highValue = MaxValue,
                };
            }
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

        protected override void TryGetValueRange(out long min, out long max, out bool slider)
        {
            Descriptor.TryGetValueRange<uint>(out uint umin, out uint umax, out slider);
            min = umin;
            max = umax;
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

        protected override void TryGetValueRange(out long min, out long max, out bool slider)
        {
            Descriptor.TryGetValueRange<long>(out min, out max, out slider);
            min = 0;
        }
    }

}