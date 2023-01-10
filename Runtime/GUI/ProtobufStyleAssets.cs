using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualProtobuf.UIElements
{
    public class ProtobufStyleAssets : ScriptableObject
    {
        protected static System.Func<ProtobufStyleAssets> createInstanceFunc;

        static ProtobufStyleAssets m_Instance;
        public static ProtobufStyleAssets Active
        {
            get
            {
                if (m_Instance == null)
                {
                    if (createInstanceFunc != null) m_Instance = createInstanceFunc.Invoke();
                    if (m_Instance == null) m_Instance = CreateInstance<ProtobufStyleAssets>();
                }
                return m_Instance;
            }
        }

        [SerializeField]
        protected Texture2D m_IconProtobufFile;
        public virtual Texture2D IconProtobufFile => m_IconProtobufFile;

        [SerializeField]
        protected Texture2D m_IconMessageType;
        public virtual Texture2D IconMessageType => m_IconMessageType;

        [SerializeField]
        protected Texture2D m_IconInstance;
        public virtual Texture2D IconInstance => m_IconInstance;

        [SerializeField]
        protected Texture2D m_IconInstanceVariant;
        public virtual Texture2D IconInstanceVariant => m_IconInstanceVariant;

        [SerializeField]
        protected Texture2D m_IconAdd;
        public virtual Texture2D IconAdd => m_IconAdd;

        [SerializeField]
        protected Texture2D m_IconDelete;
        public virtual Texture2D IconDelete => m_IconDelete;

        [SerializeField]
        protected Texture2D m_IconRefresh;
        public virtual Texture2D IconRefresh => m_IconRefresh;

        [SerializeField]
        protected Texture2D m_IconFolder;
        public virtual Texture2D IconFolder => m_IconFolder;

        [SerializeField]
        protected Texture2D m_IconFolderEmpty;
        public virtual Texture2D IconFolderEmpty => m_IconFolderEmpty;

        [SerializeField]
        protected Texture2D m_IconFolderOpened;
        public virtual Texture2D IconFolderOpened => m_IconFolderOpened;
    }
}