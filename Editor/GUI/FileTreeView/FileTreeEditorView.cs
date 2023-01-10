using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public class FileTreeEditorView<T> : FileTreeView<T> where T : ScriptableObject, IFileProvider
    {
        public FileTreeEditorView() : base()
        {
            fixedItemHeight = 16;
        }

        protected override VisualElement MakeTreeItem()
        {
            return new FileTreeEditorViewItem<T>();
        }
    }
}