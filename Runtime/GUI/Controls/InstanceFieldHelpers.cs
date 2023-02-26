using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public static class InstanceFieldHelpers
    {
        internal static string GetParentFieldPath(this IProtobufVisualField field)
        {
            return field.Parent == null ? "" : field.Parent.FieldPath;
        }

        internal static void BindAssociatedFields(this IProtobufVisualField field)
        {
            if (field.Descriptor.TryGetConditionExpression(out var logicExpression))
            {
                foreach (var variable in logicExpression.Variables)
                {
                    var variablePath = field.GetParentFieldPath() + "." + variable.Name;
                    if (field.Root.TryGetVisualFieldByPath(variablePath, out var associatedField))
                    {
                        associatedField.AssociatedFields ??= new HashSet<string>();
                        associatedField.AssociatedFields.Add(field.FieldPath);
                    }
                }
            }
        }

        internal static void OnPostValueChanged(this IProtobufVisualField field)
        {
            if (field.AssociatedFields == null || field.AssociatedFields.Count == 0) return;
            foreach (var association in field.AssociatedFields)
            {
                if (field.Root.TryGetVisualFieldByPath(association, out var associatedField))
                {
                    associatedField.VerifyConditionResult();
                }
            }
        }

        internal static void VerifyConditionResult(this IProtobufVisualField field)
        {
            if (field is VisualElement fieldElement)
            {
                var result = field.Descriptor.GetConditionResult(field.Message);
                fieldElement.style.display = result ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        internal static VisualElement CreateFieldElement(this IProtobufVisualRoot root, IProtobufVisualField parent, FieldDescriptor descriptor, IMessage message)
        {
            VisualElement fieldElement = descriptor.IsRepeated ?
                new RepeatedField(descriptor, message) :
                CreateSingularFieldElement(descriptor, message);

            if (fieldElement is IProtobufVisualField field)
            {
                field.Parent = parent;
                field.Root = root;
                field.FieldPath = field.GetParentFieldPath() + "." + field.Descriptor.Name;
                field.VerifyConditionResult();
                root.RegisterVisualField(field);
            }

            // MessageField build elemetns after construct.
            if (fieldElement is MessageField messageField)
            {
                messageField.BuildUIElement();
            }

            return fieldElement;
        }

        internal static VisualElement CreateSingularFieldElement(FieldDescriptor descriptor, IMessage message)
        {
            switch (descriptor.FieldType)
            {
                case FieldType.Double:
                    return new DoubleField(message, descriptor);
                case FieldType.Float:
                    return new FloatField(message, descriptor);
                case FieldType.Int32:
                case FieldType.SInt32:
                case FieldType.SFixed32:
                    return new IntField(message, descriptor);
                case FieldType.Int64:
                case FieldType.SInt64:
                case FieldType.SFixed64:
                    return new LongField(message, descriptor);
                case FieldType.UInt32:
                case FieldType.Fixed32:
                    return new UintField(message, descriptor);
                case FieldType.UInt64:
                case FieldType.Fixed64:
                    return new UlongField(message, descriptor);
                case FieldType.Bool:
                    return new BoolField(message, descriptor);
                case FieldType.String:
                    return new StringField(message, descriptor);
                case FieldType.Enum:
                    return new EnumField(message, descriptor);
                case FieldType.Message:
                    return new MessageField(message, descriptor);
                default: return null;
            }
        }
    }
}