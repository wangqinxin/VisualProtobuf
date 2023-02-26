using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public interface IProtobufVisualRoot
    {
        void RegisterVisualField(IProtobufVisualField field);
        bool TryGetVisualFieldByPath(string fieldPath, out IProtobufVisualField field);
    }

    public interface IProtobufVisualField
    {
        FieldDescriptor Descriptor { get; set; }

        IMessage Message { get; set; }

        IProtobufVisualField Parent { get; set; }

        IProtobufVisualRoot Root { get; set; }
        // Used for condition valid or formula.
        HashSet<string> AssociatedFields { get; set; }

        string FieldPath { get; set; }

        System.Action<object, object> OnValueChanged { get; set; }

        void SetLabel(string label);

        void SetValue(object value, bool notify);

        object GetValue();
    }
}