using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public class ProtobufInstanceView : VisualElement
    {
        protected const string kInstanceViewTitle = "Instances";
        protected const string kInstanceDefaultFolderName = "New Folder";
        protected const string kInstanceDefaultFileName = "New Instance.json";

        public readonly FileTreeView<ProtobufInstance> treeView;

        public ProtobufInstanceView()
        {
            Add(CreateHeader());

            treeView = CreateTreeView();
            treeView.SetRootPath(ProtobufDatabase.InstanceRootPath);
            treeView.RegisterCallback<ContextClickEvent>(OnTreeViewContextClick);
            treeView.onSelectedFileChanged += OnSelectedFileChanged;
            Add(treeView);
        }

        protected virtual VisualElement CreateHeader()
        {
            return new GroupBox(kInstanceViewTitle);
        }

        protected virtual FileTreeView<ProtobufInstance> CreateTreeView()
        {
            return new FileTreeView<ProtobufInstance>();
        }

        protected void RebuildView()
        {
            treeView.Rebuild();
        }

        protected virtual void OnTreeViewContextClick(ContextClickEvent evt)
        {

        }

        protected virtual void OnSelectedFileChanged(ProtobufInstance[] instances)
        {
           
        }

        protected void SetSelectionToHoveredIndex()
        {
            treeView.ClearSelection();
            var index = treeView.GetHoveredIndex();
            if (index == -1) return;
            treeView.SetSelection(index);
        }

        // Context Menu
        protected virtual void CreateFolder()
        {
            treeView.AddItem(kInstanceDefaultFolderName);
        }

        protected virtual void CreateInstance()
        {
            treeView.AddItem(kInstanceDefaultFileName, () =>
            {
                var instance = new ProtobufInstance();
                instance.CreateMeta(ProtobufInstanceType.Single);
                return instance;
            });
        }

        protected virtual void CreateInstanceVariant()
        {

        }

        protected virtual void CreateInstanceList()
        {

        }

        protected virtual void CreateInstanceMap()
        {

        }

        protected virtual void CreateInstanceTable()
        {

        }

        protected virtual void RenameSelection()
        {
            treeView.RenameSelection();
        }

        protected virtual void RemoveSelections()
        {
            treeView.RemoveSelections();
        }

        protected virtual void RefreshAll()
        {
            treeView.RefreshAll();
        }

        protected virtual void RefreshSelection()
        {
            treeView.RefreshSelection();
        }
    }
}