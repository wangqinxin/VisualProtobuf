using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VisualProtobuf.UIElements
{
    public class ProtobufEditorStyleAssets : ProtobufStyleAssets
    {
        const string kIconAdd = "d_createaddnew.png";
        const string kIconFolder = "d_Folder Icon";
        const string kIconFolderEmpty = "d_FolderEmpty Icon";
        const string kIconFolderOpened = "d_FolderOpened Icon";


        [InitializeOnLoadMethod]
        static void Initialize()
        {
            createInstanceFunc = () =>
            {
                return CreateInstance<ProtobufEditorStyleAssets>();
            };
        }

        public override Texture2D IconAdd
        {
            get
            {
                if (m_IconAdd == null)
                {
                    m_IconAdd = EditorGUIUtility.Load(kIconAdd) as Texture2D;
                }
                return m_IconAdd;
            }
        }

        public override Texture2D IconFolder
        {
            get
            {
                if (m_IconFolder == null)
                {
                    m_IconFolder = EditorGUIUtility.Load(kIconFolder) as Texture2D;
                }
                return m_IconFolder;
            }
        }

        public override Texture2D IconFolderEmpty
        {
            get
            {
                if (m_IconFolderEmpty == null)
                {
                    m_IconFolderEmpty = EditorGUIUtility.Load(kIconFolderEmpty) as Texture2D;
                }
                return m_IconFolderEmpty;
            }
        }

        public override Texture2D IconFolderOpened
        {
            get
            {
                if (m_IconFolderOpened == null)
                {
                    m_IconFolderOpened = EditorGUIUtility.Load(kIconFolderOpened) as Texture2D;
                }
                return m_IconFolderOpened;
            }
        }
    }
}