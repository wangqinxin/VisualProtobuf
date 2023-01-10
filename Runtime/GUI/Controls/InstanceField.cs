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
        public InstanceField(IMessage message)
        {
            Add(new MessageField(message));
        }
    }
} 