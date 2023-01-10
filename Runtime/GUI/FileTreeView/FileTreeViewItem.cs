using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public class FileTreeViewItem<T> : VisualElement where T : ScriptableObject, IFileProvider
    {
        public int index;
        public FileTreeViewNode<T> data;

        protected readonly Image m_Icon;
        protected readonly Label m_TitleLabel;

        protected TextField m_RenameField;
        protected int m_TitleClickCount;

        protected bool m_Selected;
        public bool Selected
        {
            get { return m_Selected; }
            set
            {
                m_Selected = value;
                m_TitleClickCount = 0;
            }
        }

        public FileTreeViewItem()
        {
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;
            m_Icon = new Image();

            Add(m_Icon);

            m_TitleLabel = new Label();
            m_TitleLabel.RegisterCallback<ClickEvent>(OnLabelClicked);
            m_TitleLabel.RegisterCallback<BlurEvent>(OnLabelBlur);
            Add(m_TitleLabel);
        }

        internal void SetItemData(FileTreeViewNode<T> data, int index)
        {
            this.index = index;
            this.data = data;

            m_TitleLabel.text = data.Name;
            if (data.IsFolder)
            {
                if (data.HasChildren)
                {
                    m_Icon.image = data.IsExpanded ? ProtobufStyleAssets.Active.IconFolderOpened : ProtobufStyleAssets.Active.IconFolder;
                }
                else
                {
                    m_Icon.image = ProtobufStyleAssets.Active.IconFolderEmpty;
                }
            }
            else
            {
                m_Icon.image = ProtobufStyleAssets.Active.IconInstance;
            }
            if (data.IsNewCreated)
            {
                data.IsNewCreated = false;
                EnterRenameMode();
            }
        }

        protected virtual void OnLabelClicked(ClickEvent clickEvent)
        {
            m_TitleClickCount++;
            if (m_TitleClickCount >= 2)
            {
                EnterRenameMode();
            }
        }

        void OnLabelBlur(BlurEvent blurEvent)
        {
            m_TitleClickCount = 0;
        }

        protected virtual TextField CreateRenameField()
        {
            var renameField = new TextField();
            renameField.focusable = true;
            renameField.selectAllOnFocus = true;
            renameField.SetValueWithoutNotify(data.Name);
            renameField.RegisterCallback<BlurEvent>((evt) =>
            {
                ExitRenameMode();
            });
            return renameField;
        }

        public void EnterRenameMode()
        {
            m_TitleClickCount = 0;
            // rename tree item
            if (m_RenameField == null)
            {
                m_RenameField = CreateRenameField();
            }
            Remove(m_TitleLabel);
            Add(m_RenameField);
            m_RenameField.Focus();
        }

        private void ExitRenameMode()
        {
            m_TitleClickCount = 0;
            var newName = m_RenameField.value.Trim();
            if (data.Rename(newName))
            {
                m_TitleLabel.text = newName;
            }
            Remove(m_RenameField);
            m_RenameField = null;
            Add(m_TitleLabel);
        }
    }
}
