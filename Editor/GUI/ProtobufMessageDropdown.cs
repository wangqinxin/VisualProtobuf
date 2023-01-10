using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace VisualProtobuf.UIElements
{
    public class ProtobufMessageDropdown : AdvancedDropdown
    {
        private System.Action<string> m_OnMessageSelected;

        public ProtobufMessageDropdown(AdvancedDropdownState state) : base(state) { }

        public ProtobufMessageDropdown(AdvancedDropdownState state, System.Action<string> onMessageSelected) : base(state)
        {
            m_OnMessageSelected = onMessageSelected;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new ProtobufMessageDropdownItem("ProtobufMessage");

            var messageTypes = ProtobufDatabase.GetMessageTypes();
            foreach (var msgType in messageTypes)
            {
                root.AddChildByFullName(msgType.FullName);
            }
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is ProtobufMessageDropdownItem msgItem)
            {
                m_OnMessageSelected?.Invoke(msgItem.fullName);
            }
        }
    }

    public class ProtobufMessageDropdownItem : AdvancedDropdownItem
    {
        public string fullName;
        public bool isEndItem;
        public Dictionary<string, ProtobufMessageDropdownItem> childrenMap;

        public ProtobufMessageDropdownItem(string name) : base(name)
        {
            childrenMap = new Dictionary<string, ProtobufMessageDropdownItem>();
        }

        public void AddChildByFullName(string fullName)
        {
            var nameArray = fullName.Split('.');
            var parent = this;
            for (int i = 0; i < nameArray.Length - 1; i++)
            {
                parent = parent.TryAddChild(nameArray[i]);
            }
            var item = parent.TryAddChild(nameArray[nameArray.Length - 1]);
            item.fullName = fullName;
            item.icon = ProtobufEditorStyleAssets.Active.IconMessageType;
            item.isEndItem = true;
        }

        ProtobufMessageDropdownItem TryAddChild(string childName)
        {
            if (!childrenMap.TryGetValue(childName, out var child) || child.isEndItem)
            {
                child = new ProtobufMessageDropdownItem(childName);
                childrenMap.TryAdd(childName, child);
                AddChild(child);
            }
            return child;
        }
    }
}