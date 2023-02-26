using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class InstanceField : VisualElement, IProtobufVisualRoot
    {
        public static readonly string ussClassName = "instnce_field";

        internal Dictionary<string, IProtobufVisualField> allVisualFields;

        public InstanceField(IMessage message)
        {
            allVisualFields = new Dictionary<string, IProtobufVisualField>();

            AddToClassList(ussClassName);

            var fields = message.Descriptor.Fields;
            foreach (var fieldDesc in fields.InFieldNumberOrder())
            {
                Add(this.CreateFieldElement(null, fieldDesc, message));
            }

            OnPostCreateUIElements();
        }


        public void RegisterVisualField(IProtobufVisualField field)
        {
            allVisualFields.TryAdd(field.FieldPath, field);
        }

        public bool TryGetVisualFieldByPath(string fieldPath, out IProtobufVisualField field)
        {
            return allVisualFields.TryGetValue(fieldPath, out field);
        }

        void OnPostCreateUIElements()
        {
            foreach (var field in allVisualFields.Values)
            {
                field.BindAssociatedFields();
            }
        }
    }
}