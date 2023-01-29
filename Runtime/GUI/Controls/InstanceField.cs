using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class InstanceField : VisualElement
    {
        public static readonly string ussClassName = "instnce_field";

        public InstanceField(IMessage message)
        {
            AddToClassList(ussClassName);

            var fields = message.Descriptor.Fields;
            foreach (var fieldDesc in fields.InFieldNumberOrder())
            {
                Add(InstanceFieldHelpers.CreateFieldElement(fieldDesc, message));
            }
        }
    }
} 