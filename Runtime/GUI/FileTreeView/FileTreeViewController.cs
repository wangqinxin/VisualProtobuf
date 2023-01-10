using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public class FileTreeViewController<T> : TreeViewController where T : ScriptableObject, IFileProvider
    {
        public FileTreeViewNode<T> root;
        public IList<int> selectedIds;
        public int selectedId => selectedIds.Count > 0 ? selectedIds[0] : -1;

        public FileTreeViewController()
        {
            selectedIds = new List<int>();
        }

        public void SetRootPath(string rootPath)
        {
            root = CreateRootNode(rootPath);
            RebuildTree();
        }

        public virtual FileTreeViewNode<T> CreateRootNode(string rootPath)
        {
            return FileTreeViewNode<T>.CreateRootNode(rootPath);
        }

        public override IEnumerable<int> GetAllItemIds(IEnumerable<int> rootIds = null)
        {
            if (root == null) return new int[0];
            if (rootIds == null)
            {
                rootIds = new List<int>() { root.Id };
            }
            var nodes = new List<FileTreeViewNode<T>>();
            foreach (var rootId in rootIds)
            {
                var rootNode = FileTreeViewNode<T>.GetNodeByID(rootId);
                if (rootNode == null) continue;
                rootNode.GetAllChildrenNodes(ref nodes);
            }
            var nodeIds = new int[nodes.Count];
            for (int i = 0; i < nodes.Count; i++)
            {
                nodeIds[i] = nodes[i].Id;
            }
            return nodeIds;
        }

        public override IEnumerable<int> GetChildrenIds(int id)
        {
            var node = FileTreeViewNode<T>.GetNodeByID(id);
            if (node == null || !node.HasChildren) return new int[0];
            var childIds = new int[node.Children.Count];
            for (int i = 0; i < node.Children.Count; i++)
            {
                childIds[i] = node.Children[i].Id;
            }
            return childIds;
        }

        public override int GetParentId(int id)
        {
            var node = FileTreeViewNode<T>.GetNodeByID(id);
            if (node != null && node.Parent != null) return node.Parent.Id;
            return -1;
        }

        public override bool HasChildren(int id)
        {
            var node = FileTreeViewNode<T>.GetNodeByID(id);
            return node?.HasChildren ?? false;
        }

        public void OnSelectionChanged(IList<int> ids)
        {
            selectedIds = ids;
        }

        public override void Move(int id, int newParentId, int childIndex = -1, bool rebuildTree = true)
        {
            var node = FileTreeViewNode<T>.GetNodeByID(id);
            if (node == null || node.Parent == null || node.Parent.Id == newParentId) return;
            var newParent = FileTreeViewNode<T>.GetNodeByID(newParentId);
            if (newParent == null || !newParent.IsFolder) return;
            node.Move(newParent);
            if (rebuildTree) RebuildTree();
        }

        public FileTreeViewNode<T> TryAddItem(string itemName, System.Func<T> creater = null)
        {
            var selected = FileTreeViewNode<T>.GetNodeByID(selectedId);
            if (selected == null) return null;
            var node = selected.AddChild(itemName, creater);
            if (node != null) RebuildTree();
            return node;
        }

        public void RemoveSelectedItems()
        {
            foreach (var selectedId in selectedIds)
            {
                TryRemoveItem(selectedId, false);
            }
            RebuildTree();
        }

        public override bool TryRemoveItem(int id, bool rebuildTree = true)
        {
            var node = FileTreeViewNode<T>.GetNodeByID(id);
            if (node == null || node.Parent == null) return false;
            return node.RemoveSelf();
        }

        public void RenameItem(int id, string newName)
        {
            var node = FileTreeViewNode<T>.GetNodeByID(id);
            if (node == null) return;
            node.Rename(newName);
        }

        public List<int> Duplicate(List<int> ids, int parentId = int.MinValue)
        {
            var nodeIds = new List<int>();
            if (parentId != int.MinValue)
            {
                var parent = FileTreeViewNode<T>.GetNodeByID(parentId);
                if (parent == null) return nodeIds;
                if (!parent.IsFolder && parent.Parent != null)
                {
                    parentId = parent.Parent.Id;
                }
            }
            foreach (var id in ids)
            {
                var node = FileTreeViewNode<T>.GetNodeByID(id);
                if (node == null || node.Parent == null) continue;
                var dupNode = node.Duplicate(parentId == int.MinValue ? node.Parent.Id : parentId);
                if (dupNode != null) nodeIds.Add(dupNode.Id);
            }
            RebuildTree();
            return nodeIds;
        }
    }
}