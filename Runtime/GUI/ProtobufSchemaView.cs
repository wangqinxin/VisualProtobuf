using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class ProtobufSchemaView : VisualElement
    {
        protected const string kSchemaViewTitle = "Schemas";

        private readonly ProtobufSchemaTreeView m_TreeView;

        private IList<TreeViewItemData<ProtobufSchemaTreeNode>> m_TreeRoots;

        public ProtobufSchemaView(bool editMode = true)
        {
            m_TreeRoots = new List<TreeViewItemData<ProtobufSchemaTreeNode>>();

            BuildTreeData();

            Add(CreateHeader());

            m_TreeView = new ProtobufSchemaTreeView(editMode);
            m_TreeView.SetRootItems(m_TreeRoots);
            Add(m_TreeView);
        }

        protected virtual VisualElement CreateHeader()
        {
            return new GroupBox(kSchemaViewTitle);
        }

        void BuildTreeData()
        {
            m_TreeRoots.Clear();
            var descriptor = ProtobufDatabase.ProtobufDescriptor;
            if (descriptor == null || descriptor.Files == null || descriptor.Files.Count == 0)
            {
                //Empty View
                return;
            }
            var msgTypes = descriptor.GetMessageTypes();
            var rootNode = new ProtobufSchemaTreeNode("root", ".", ISchemaNodeType.Namespace);
            foreach (var msgType in msgTypes)
            {
                var node = new ProtobufSchemaTreeNode(msgType.Name, msgType.FullName, ISchemaNodeType.Type);
                rootNode.AddChild(node);
            }
            m_TreeRoots = rootNode.BuildTreeItemData();
        }



        protected enum ISchemaNodeType
        {
            Namespace = 0,
            Type = 1,
            Enum = 2,
        }

        protected class ProtobufSchemaTreeNode
        {
            public int id;

            public string name;

            public string fullName;

            public ISchemaNodeType type;

            public bool populated;

            public List<ProtobufSchemaTreeNode> children;

            static int index;

            internal ProtobufSchemaTreeNode(string name, string fullName, ISchemaNodeType type)
            {
                this.id = index++;
                this.name = name;
                this.fullName = fullName;
                this.type = type;
                children = new List<ProtobufSchemaTreeNode>();
            }

            internal void AddChild(ProtobufSchemaTreeNode node)
            {
                var fullPathSlices = node.fullName.Split('.');
                var parent = this;
                var fullPath = new System.Text.StringBuilder();
                for (int i = 0; i < fullPathSlices.Length - 1; i++)
                {
                    var sliceName = fullPathSlices[i];
                    fullPath.Append(sliceName);
                    var child = parent.FindChild(sliceName);
                    if (child == null)
                    {
                        var childNode = new ProtobufSchemaTreeNode(sliceName, fullPath.ToString(), ISchemaNodeType.Namespace);
                        parent.children.Add(childNode);
                        parent = childNode;
                    }
                    else
                    {
                        parent = child;
                    }
                    fullPath.Append(".");
                }
                parent.children.Add(node);
            }

            internal ProtobufSchemaTreeNode FindChild(string name)
            {
                return children.Find(node => node.name == name);
            }

            internal List<TreeViewItemData<ProtobufSchemaTreeNode>> BuildTreeItemData()
            {
                if (children.Count == 0) return null;
                var itemData = new List<TreeViewItemData<ProtobufSchemaTreeNode>>();
                foreach (var child in children)
                {
                    itemData.Add(new TreeViewItemData<ProtobufSchemaTreeNode>(child.id, child, child.BuildTreeItemData()));
                }
                return itemData;
            }
        }

        protected class ProtobufSchemaTreeView : TreeView
        {
            internal ProtobufSchemaTreeView(bool editMode = true)
            {
                if (editMode)
                {
                    fixedItemHeight = 16;
                }
                makeItem += MakeTreeItem;
                bindItem += BindTreeItem;
                selectionChanged += OnSelectionChanged;
            }

            VisualElement MakeTreeItem()
            {
                return new ProtobufScehemaTreeItemView();
            }

            void BindTreeItem(VisualElement element, int index)
            {
                if (element is ProtobufScehemaTreeItemView itemView)
                {
                    var itemData = GetItemDataForIndex<ProtobufSchemaTreeNode>(index);
                    itemView.SetItemData(itemData);
                }
            }

            void OnSelectionChanged(IEnumerable<object> selections)
            {
                foreach (var selection in selections)
                {
                    Debug.LogError(selection.GetType());
                }
            }
        }

        protected class ProtobufScehemaTreeItemView : VisualElement
        {
            private Image m_Icon;
            private Label m_Label;

            internal ProtobufScehemaTreeItemView()
            {
                style.flexDirection = FlexDirection.Row;
                style.alignItems = Align.Center;
                m_Icon = new Image();
                m_Icon.style.maxWidth = 16;
                m_Icon.style.maxHeight = 16;
                m_Label = new Label();
                Add(m_Icon);
                Add(m_Label);
            }

            internal void SetItemData(ProtobufSchemaTreeNode node)
            {
                if (node.type == ISchemaNodeType.Type)
                {
                    m_Icon.image = ProtobufStyleAssets.Active.IconMessageType;
                }
                else
                {
                    m_Icon.image = null;
                }
                m_Label.text = node.name;
            }
        }
    }
}