using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.EditorCoroutines.Editor;

namespace VisualProtobuf.UIElements
{
    public class FileTreeEditorViewItem<T> : FileTreeViewItem<T> where T : ScriptableObject, IFileProvider
    {
        private EditorCoroutine m_LabelClickCoroutine;

        public FileTreeEditorViewItem() : base()
        {
            m_Icon.style.width = 16;
            m_Icon.style.minWidth = 16;
            m_Icon.style.height = 16;
        }

        protected override void OnLabelClicked(ClickEvent clickEvent)
        {
            if (m_LabelClickCoroutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(m_LabelClickCoroutine);
            }
            if (clickEvent.clickCount == 2)
            {
                if (!data.IsFolder)
                {
                    ProtobufEditorUtilities.OpenInstanceWithVscode(data.FsPath, 0, true);
                }
                //EditorUtility.OpenWithDefaultApp(data.path);
                return;
            }
            m_LabelClickCoroutine = EditorCoroutineUtility.StartCoroutine(ILabelClicked(), this);
        }

        IEnumerator ILabelClicked()
        {
            yield return new EditorWaitForSeconds(0.35f);

            m_TitleClickCount++;
            if (m_TitleClickCount >= 2)
            {
                EnterRenameMode();
            }

            m_LabelClickCoroutine = null;
        }

        protected override TextField CreateRenameField()
        {
            var renameField = base.CreateRenameField();

            renameField.style.paddingLeft = 1;
            renameField.style.marginLeft = 0;

            var input = renameField.ElementAt(0);
            input.style.borderBottomLeftRadius = 0;
            input.style.borderBottomRightRadius = 0;
            input.style.borderTopLeftRadius = 0;
            input.style.borderTopRightRadius = 0;

            input.style.borderRightWidth = 0;
            input.style.borderBottomWidth = 0;
            input.style.borderLeftWidth = 0;
            input.style.borderTopWidth = 0;

            input.style.paddingLeft = 0;
            input.style.paddingRight = 0;
            input.style.paddingTop = 0;
            input.style.paddingBottom = 0;
            return renameField;
        }
    }
}