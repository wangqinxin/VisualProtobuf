using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.EditorCoroutines.Editor;

namespace VisualProtobuf.UIElements
{
    public class ProtobufInstanceEditorView : ProtobufInstanceView
    {
        private DropdownField m_SelectedSchemaDropDown;

        public ProtobufInstanceEditorView() : base()
        {
        }

        protected override VisualElement CreateHeader()
        {
            var header = new VisualElement();

            m_SelectedSchemaDropDown = new DropdownField(kInstanceViewTitle);
            var textElement = m_SelectedSchemaDropDown.ElementAt(0);
            var fontStyle = textElement.style.unityFontStyleAndWeight;
            fontStyle.value = FontStyle.Bold;
            textElement.style.unityFontStyleAndWeight = fontStyle;
            m_SelectedSchemaDropDown.choices = new List<string>() { "All", "visualprotobuf/sample", "visualprotobuf/sample2", "test", "test2" };
            m_SelectedSchemaDropDown.index = 0;
            header.Add(m_SelectedSchemaDropDown);

            var toolbar = new ProtobufToolbar();
            toolbar.AppendAddMenuAction("Folder", (action) => { CreateFolder(); });
            toolbar.AppendAddMenuSeparator();
            toolbar.AppendAddMenuAction("Instance", (action) => { CreateInstance(); });
            toolbar.AppendAddMenuAction("Instance Variant", (action) => { CreateInstanceVariant(); });
            toolbar.AppendAddMenuSeparator();
            toolbar.AppendAddMenuAction("Instance List", (action) => { CreateInstanceList(); });
            toolbar.AppendAddMenuAction("Instance Map", (action) => { CreateInstanceMap(); });
            toolbar.AppendAddMenuAction("Instance Table", (action) => { CreateInstanceTable(); });
            toolbar.SetDeleteButtonClickAction(InternalRemoveSelections);
            toolbar.SetRefreshButtonClickAction(RefreshAll);
            header.Add(toolbar);
            return header;
        }

        protected override FileTreeView<ProtobufInstance> CreateTreeView()
        {
            return new FileTreeEditorView<ProtobufInstance>();
        }


        protected override void OnTreeViewContextClick(ContextClickEvent evt)
        {
            SetSelectionToHoveredIndex();

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create/Folder"), false, CreateFolder);
            menu.AddSeparator("Create/");
            menu.AddItem(new GUIContent("Create/Instance"), false, CreateInstance);
            menu.AddItem(new GUIContent("Create/Instance Variant"), false, CreateInstanceVariant);
            menu.AddSeparator("Create/");
            menu.AddItem(new GUIContent("Create/Instance List"), false, CreateInstanceList);
            menu.AddItem(new GUIContent("Create/Instance Map"), false, CreateInstanceMap);
            menu.AddItem(new GUIContent("Create/Instance Table"), false, CreateInstanceTable);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Rename"), false, RenameSelection);
            menu.AddItem(new GUIContent("Delete"), false, InternalRemoveSelections);
            menu.AddItem(new GUIContent("Refresh"), false, RefreshSelection);
#if UNITY_EDITOR_OSX
            menu.AddItem(new GUIContent("Reveal in Finder"), false, OpenTreeItem);
#else
            menu.AddItem(new GUIContent("Show in Explorer"), false, OpenTreeItem);
#endif 
            menu.ShowAsContext();
        }

        protected override void OnSelectedFileChanged(ProtobufInstance[] instances)
        {
            Selection.objects = instances;
        }

        // Context menu
        void InternalRemoveSelections()
        {
            var removes = new StringBuilder();
            foreach (var selectedNode in treeView.selectedNodes)
            {
                var subPath = selectedNode.FsPath.Replace(ProtobufDatabase.InstanceRootPath, ProtobufDatabase.InstanceFolderName);
                removes.AppendLine(subPath);
            }
            if (EditorUtility.DisplayDialog("Delete selected assets?", removes.ToString(), "Delete", "Cancel"))
            {
                RemoveSelections();
            }
        }

        void OpenTreeItem()
        {
            if (treeView.selectedNode == null) return;
            EditorUtility.RevealInFinder(treeView.selectedNode.FsPath);
        }
    }
}