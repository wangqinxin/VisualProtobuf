using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualProtobuf.UIElements
{
    public interface IFileProvider
    {
        string FileExtension { get; }

        void Init(string fsPath);

        IFileProvider Duplicate(string newPath);
    }

    public class FileTreeViewNode<T> where T : ScriptableObject, IFileProvider
    {
        public const int kRootID = -1;
        private static int m_IDCounter = -1;
        private static Dictionary<int, FileTreeViewNode<T>> m_NodeDict = new Dictionary<int, FileTreeViewNode<T>>();

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string FsPath { get; private set; }
        public bool IsFolder { get; private set; }
        public bool IsExpanded { get; set; }

        public bool IsNewCreated { get; set; }

        public T FileProvider { get; private set; }

        public FileTreeViewNode<T> Parent { get; private set; }

        public List<FileTreeViewNode<T>> Children { get; private set; }

        public bool HasChildren => Children != null && Children.Count > 0;

        internal FileTreeViewNode(string fsPath, FileTreeViewNode<T> parent, T fileProvider)
        {
            Id = m_IDCounter++;
            Parent = parent;
            FsPath = fsPath;
            Name = Path.GetFileNameWithoutExtension(fsPath);
            FileProvider = fileProvider;
            FileProvider.Init(FsPath);
            IsFolder = File.GetAttributes(fsPath).HasFlag(FileAttributes.Directory);
            IsNewCreated = false;
            if (IsFolder) Children = new List<FileTreeViewNode<T>>();
            if (parent != null && parent.IsFolder && parent.Children != null)
            {
                parent.Children.Add(this);
                parent.SortChildren();
            }
            BuildNode();
            m_NodeDict.Add(Id, this);
        }

        public static FileTreeViewNode<T> CreateRootNode(string rootPath)
        {
            if (!Directory.Exists(rootPath)) return null;
            ClearAllNodes();
            return new FileTreeViewNode<T>(rootPath, null, ScriptableObject.CreateInstance<T>());
        }
        public static void ClearAllNodes()
        {
            m_NodeDict.Clear();
            m_IDCounter = -1;
        }

        public static FileTreeViewNode<T> GetNodeByID(int id)
        {
            if (m_NodeDict.TryGetValue(id, out var node))
            {
                return node;
            }
            return null;
        }

        private void BuildNode()
        {
            if (!IsFolder) return;
            Children.Clear();

            var dirs = Directory.GetDirectories(FsPath);
            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var dirName = Path.GetFileName(dir);
                if (dirName.StartsWith(".")) continue;
                new FileTreeViewNode<T>(dir, this, ScriptableObject.CreateInstance<T>());
            }

            var files = Directory.GetFiles(FsPath, $"*.{FileProvider.FileExtension}", SearchOption.TopDirectoryOnly);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.StartsWith(".")) continue;
                new FileTreeViewNode<T>(file, this, ScriptableObject.CreateInstance<T>());
            }

            SortChildren();
        }

        public void GetAllChildrenNodes(ref List<FileTreeViewNode<T>> nodes)
        {
            if (Children == null) return;
            foreach (var child in Children)
            {
                nodes.Add(child);
                child.GetAllChildrenNodes(ref nodes);
            }
        }

        public FileTreeViewNode<T> AddChild(string nodeName, System.Func<T> creater = null)
        {
            if (!IsFolder) return null;
            if (Children == null) Children = new List<FileTreeViewNode<T>>();
            try
            {
                var newNodePath = FileSystemUtility.GetUniquePath(FsPath, nodeName);
                if (creater == null)
                {
                    Directory.CreateDirectory(newNodePath);
                    return new FileTreeViewNode<T>(newNodePath, this, ScriptableObject.CreateInstance<T>()) { IsNewCreated = true };
                }
                else
                {
                    var fileProvider = creater.Invoke();
                    return new FileTreeViewNode<T>(newNodePath, this, fileProvider) { IsNewCreated = true };
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Try add file node failed, {e.Message}");
                return null;
            }
        }

        protected virtual void SortChildren()
        {
            Children.Sort((a, b) =>
            {
                if (a.IsFolder && !b.IsFolder) return -1;
                if (!a.IsFolder && b.IsFolder) return 1;
                FileSystemUtility.TryGetNameEndNumber(a.Name, out int numberA);
                FileSystemUtility.TryGetNameEndNumber(b.Name, out int numberB);
                if (numberA == numberB) return a.Name.CompareTo(b.Name);
                return numberA.CompareTo(numberB);
            });
        }

        public bool Rename(string newName)
        {
            if (string.IsNullOrEmpty(newName)) return false;
            if (Name.Equals(newName)) return false;
            try
            {
                var directory = Path.GetDirectoryName(FsPath);
                string newPath;
                if (Path.HasExtension(FsPath))
                {
                    var ext = Path.GetExtension(FsPath);
                    newPath = Path.Combine(directory, newName + ext);
                }
                else
                {
                    newPath = Path.Combine(directory, newName);
                }

                if (IsFolder)
                {
                    if (Directory.Exists(newPath)) return false;
                    Directory.Move(FsPath, newPath);
                }
                else
                {
                    if (File.Exists(newPath)) return false;
                    File.Move(FsPath, newPath);
                }
                Name = newName;
                FsPath = newPath;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Rename protobuf instance failed:" + e.Message);
                return false;
            }
        }

        public bool RemoveSelf()
        {
            if (Id == kRootID) return false;
            if (Parent == null) return false;
            try
            {
                if (IsFolder)
                {
                    if (Directory.Exists(FsPath)) Directory.Delete(FsPath, true);

                    foreach (var child in Children)
                    {
                        if (m_NodeDict.ContainsKey(child.Id)) m_NodeDict.Remove(child.Id);
                    }
                }
                else
                {
                    if (File.Exists(FsPath)) File.Delete(FsPath);
                }

                Parent.Children.Remove(this);
                if (m_NodeDict.ContainsKey(Id)) m_NodeDict.Remove(Id);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Remove file node failed ,{e.Message}");
                return false;
            }
        }

        public bool Move(FileTreeViewNode<T> newParent)
        {
            if (newParent == null || newParent == Parent) return false;
            try
            {
                if (IsFolder)
                {
                    var newPath = Path.Combine(newParent.FsPath, Name);
                    if (Directory.Exists(newPath)) return false;
                    Directory.Move(FsPath, newPath);
                    FsPath = newPath;
                }
                else
                {
                    var fileName = Path.GetFileName(FsPath);
                    var newPath = Path.Combine(newParent.FsPath, fileName);
                    if (File.Exists(newPath)) return false;
                    File.Move(FsPath, newPath);
                    FsPath = newPath;
                }
                Parent.Children.Remove(this);
                newParent.Children.Add(this);
                newParent.SortChildren();
                Parent = newParent;
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Move file tree node failed, {e.Message}");
                return false;
            }
        }

        public void Refresh()
        {
            BuildNode();
        }

        public FileTreeViewNode<T> Duplicate(int parentId)
        {
            if (IsFolder) return null;
            var parent = GetNodeByID(parentId);
            if (parent == null || !parent.IsFolder) return null;
            try
            {
                var fileName = Path.GetFileName(FsPath);
                var newPath = FileSystemUtility.GetUniquePath(parent.FsPath, fileName);
                var t = (T)FileProvider.Duplicate(newPath);
                return new FileTreeViewNode<T>(newPath, parent, t);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Duplicate file tree node failed, {e.Message}");
                return null;
            }
        }
    }
}