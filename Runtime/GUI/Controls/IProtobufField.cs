using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public interface IProtobufField
    {
        FieldDescriptor Descriptor { get; set; }

        IMessage Message { get; set; }

        System.Action<object, object> OnValueChanged { get; set; }

        void SetLabel(string label);

        void SetValue(object value, bool notify);

        object GetValue();
    }
}