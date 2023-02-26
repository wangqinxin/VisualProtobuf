using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class RepeatedField : ListView
    {
        const float k_DefaultSingleItemHeight = 22f;

        public FieldDescriptor Descriptor { get; set; }

        public IMessage Message { get; set; }

        public RepeatedField(FieldDescriptor descriptor, IMessage message)
        {
            Descriptor = descriptor;
            Message = message;

            headerTitle = descriptor.GetDisplayName();
            itemsSource = descriptor.GetValue<List<object>>(message);
            reorderable = true;
            showBorder = true;
            showFoldoutHeader = true;
            showAddRemoveFooter = true;
            showBoundCollectionSize = true;
            reorderMode = ListViewReorderMode.Animated;
            showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            makeItem = MakeItem;
            bindItem = BindItem;
            itemsAdded += OnItemAdded;
            itemsRemoved += OnItemRemoved;
            itemIndexChanged += OnItemIndexChanged;
            fixedItemHeight = GetItemHeight();
            RegisterCallback<ChangeEvent<IMessage>>(OnFieldValueChanged);
        }


        VisualElement MakeItem()
        {
            if (Descriptor.FieldType == FieldType.Message)
            {
                return new MessageField(null,null);
            }
            else
            {
               return InstanceFieldHelpers.CreateSingularFieldElement(Descriptor, null);
            }
        }

        void BindItem(VisualElement element, int index)
        {
            if (element is IProtobufVisualField field)
            {
                field.SetLabel("Element" + index);
                field.SetValue(itemsSource[index], false);
                field.OnValueChanged = (preVal, newVal) =>
                {
                    itemsSource[index] = newVal;
                };
            }
        }

        float GetItemHeight()
        {
            if (Descriptor.FieldType != FieldType.Message)
            {
                return fixedItemHeight;
            }
            var fieldCount = Descriptor.MessageType.Fields.InFieldNumberOrder().Count;
            return k_DefaultSingleItemHeight * fieldCount;
        }

        void OnItemAdded(IEnumerable<int> indices)
        {
            foreach (var index in indices)
            {
                if (Descriptor.FieldType == FieldType.Message)
                {
                    itemsSource[index] = ProtobufDatabase.CreateMessage(Descriptor.MessageType);
                }
                else
                {
                    itemsSource[index] = Descriptor.GetDefaultSingularValue();
                }
            }
            SendValueChangeEvent();
        }

        void OnItemRemoved(IEnumerable<int> indices)
        {
            SendValueChangeEvent();
        }

        void OnItemIndexChanged(int oldIndex,int newIndex)
        {
            SendValueChangeEvent();
        }
       
        void OnFieldValueChanged(ChangeEvent<IMessage> changeEvent)
        {
            SendValueChangeEvent();
        }

        void SendValueChangeEvent()
        {
            using var evt = ChangeEvent<IMessage>.GetPooled(Message, Message);
            evt.target = parent;
            SendEvent(evt);
        }
    }
}