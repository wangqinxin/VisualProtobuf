using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public class FileTreeView<T> : TreeView where T : ScriptableObject, IFileProvider
    {
        public List<int> selectedIds;
        public int selectedId => selectedIds.Count > 0 ? selectedIds[0] : -1;

        public List<FileTreeViewNode<T>> selectedNodes;
        public FileTreeViewNode<T> selectedNode => selectedNodes.Count > 0 ? selectedNodes[0] : null;

        public System.Action<T[]> onSelectedFileChanged;

        private readonly VisualElement m_ContentContainer;
        private string m_RootPath;
        private List<int> copyedIds;

        public FileTreeView()
        {
            reorderable = true;
            selectionType = SelectionType.Multiple;
            selectionChanged += OnSelectionChanged;
            makeItem += MakeTreeItem;
            bindItem += BindTreeItem;
            var scrollView = this.Q<ScrollView>();
            m_ContentContainer = scrollView?.contentContainer;

            copyedIds = new List<int>();
            selectedIds = new List<int>();
            selectedNodes = new List<FileTreeViewNode<T>>();
            UpdateSelections();
            SetViewController(CreateViewController());
        }

        protected override CollectionViewController CreateViewController()
        {
            return new FileTreeViewController<T>();
        }


        public void SetRootPath(string rootPath)
        {
            m_RootPath = rootPath;
            if (viewController is FileTreeViewController<T> fileTreeViewController)
            {
                fileTreeViewController.SetRootPath(rootPath);
            }
        }

        protected virtual VisualElement MakeTreeItem()
        {
            return new FileTreeViewItem<T>();
        }

        protected void BindTreeItem(VisualElement element, int index)
        {
            if (element is FileTreeViewItem<T> item)
            {
                var id = GetIdForIndex(index);
                var node = FileTreeViewNode<T>.GetNodeByID(id);
                if (node == null) return;
                node.IsExpanded = IsExpanded(id);
                item.SetItemData(node, index);
            }
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            if (evt is ClickEvent clickEvent && m_ContentContainer != null)
            {
                var contentRect = m_ContentContainer.localBound;
                var position = clickEvent.localPosition;
                if (position.x >= 0 && position.y >= 0 && !contentRect.Contains(position))
                {
                    ClearSelection();
                }
            }
            else if (evt is KeyUpEvent keyUpEvent)
            {
                if (keyUpEvent.ctrlKey)
                {
                    switch (keyUpEvent.keyCode)
                    {
                        case KeyCode.C: CopySelections(); break;
                        case KeyCode.V: PasteSelections(); break;
                        case KeyCode.D: DuplicateSelections(); break;
                    }
                }
            }

        }

        internal void OnSelectionChanged(IEnumerable<object> selections)
        {
            UpdateSelections();
            if (viewController is FileTreeViewController<T> treeViewController)
            {
                treeViewController.OnSelectionChanged(selectedIds);
            }
        }

        void UpdateSelections()
        {
            foreach (var id in selectedIds)
            {
                var viewItem = GetRootElementForId(id)?.Q<FileTreeViewItem<T>>();
                if (viewItem != null) viewItem.Selected = false;
            }
            selectedIds.Clear();
            selectedNodes.Clear();
            var selectedFiles = ListPool<T>.Get();
            foreach (var index in selectedIndices)
            {
                var id = GetIdForIndex(index);
                selectedIds.Add(id);
                var node = FileTreeViewNode<T>.GetNodeByID(id);
                if (node != null)
                {
                    selectedNodes.Add(node);
                    selectedFiles.Add(node.FileProvider);
                }
                var viewItem = GetRootElementForId(id)?.Q<FileTreeViewItem<T>>();
                if (viewItem != null) viewItem.Selected = true;
            }
            onSelectedFileChanged?.Invoke(selectedFiles.ToArray());
            ListPool<T>.Release(selectedFiles);
        }

        internal FileTreeViewItem<T> GetHoveredItem()
        {
            return this.Query<FileTreeViewItem<T>>().Hovered().First();
        }

        internal int GetHoveredIndex()
        {
            var item = GetHoveredItem();
            if (item == null) return -1;
            return item.index;
        }

        internal void AddItem(string itemName, System.Func<T> creater = null)
        {
            if (viewController is FileTreeViewController<T> treeViewController)
            {
                var item = treeViewController.TryAddItem(itemName, creater);
                if (item != null)
                {
                    RefreshItems();
                    SetSelection(viewController.GetIndexForId(item.Id));
                }
            }
        }

        internal void RenameSelection()
        {
            var item = GetRootElementForIndex(selectedIndex)?.Q<FileTreeViewItem<T>>();
            if (item != null) item.EnterRenameMode();
        }

        internal void RemoveSelections()
        {
            if (viewController is FileTreeViewController<T> treeViewController)
            {
                treeViewController.RemoveSelectedItems();
                RefreshItems();
                ClearSelection();
            }
        }

        internal void RefreshAll()
        {
            SetRootPath(m_RootPath);
            RefreshItems();
        }

        internal void RefreshSelection()
        {
            if (selectedNode == null)
            {
                RefreshAll();
                return;
            }
            selectedNode.Refresh();
            viewController.RebuildTree();
            RefreshItems();
        }

        private void CopySelections()
        {
            copyedIds.Clear();
            copyedIds.AddRange(selectedIds);
        }

        private void PasteSelections()
        {
            if (copyedIds.Count == 0) return;
            if (selectedIds.Count > 1)
            {
                Debug.LogWarning("Selected item is more than one,cannot paste.");
                return;
            }
            if (viewController is FileTreeViewController<T> treeViewController)
            {
                var nodeIds = treeViewController.Duplicate(copyedIds, selectedId);
                RefreshItems();
                SetSelectionById(nodeIds);
            }
        }

        private void DuplicateSelections()
        {
            if (selectedIds.Count == 0) return;
            if (viewController is FileTreeViewController<T> treeViewController)
            {
                var nodeIds = treeViewController.Duplicate(selectedIds);
                RefreshItems();
                SetSelectionById(nodeIds);
            }
        }
    }
}