using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Reflection;

namespace VisualProtobuf
{
    public class ProtobufDatabase
    {
        public const string SchemaFolderName = "Schemas";
        public const string SchemaExtension = "proto";
        public const string InstanceFolderName = "Instances";
        public const string InstanceExtension = "json";

        private static string m_RootPath;
        public static string RootPath => m_RootPath;

        private static string m_SchemaRootPath;
        public static string SchemaRootPath => m_SchemaRootPath;

        private static string m_InstanceRootPath;
        public static string InstanceRootPath => m_InstanceRootPath;

        private static ProtobufDescriptor m_ProtobufDescriptor;

        public static ProtobufDescriptor ProtobufDescriptor => m_ProtobufDescriptor;

        public static MessageDescriptor SelectedMessageDescriptor;

        private static bool m_Initialized;

        public static void Initialize(string rootPath)
        {
            if (m_Initialized) return;
            m_RootPath = rootPath;
            m_SchemaRootPath = Path.Combine(RootPath, SchemaFolderName);
            m_InstanceRootPath = Path.Combine(RootPath, InstanceFolderName);
            if (!Directory.Exists(m_SchemaRootPath)) Directory.CreateDirectory(m_SchemaRootPath);
            if (!Directory.Exists(m_InstanceRootPath)) Directory.CreateDirectory(m_InstanceRootPath);

            // Schemas
            ReloadSchemas();
#if UNITY_EDITOR
            WatchProtobufSchema();
#endif
            m_Initialized = true;
        }

        public static void ReloadSchemas()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            m_ProtobufDescriptor = new ProtobufDescriptor(SchemaRootPath);
            stopwatch.Stop();
            Debug.Log($"Load proto success, total {m_ProtobufDescriptor.Files.Count} files,use {stopwatch.ElapsedMilliseconds} ms.");
        }

        public static IList<MessageDescriptor> GetMessageTypes(System.Predicate<MessageDescriptor> match = null)
        {
            return m_ProtobufDescriptor.GetMessageTypes(match);
        }

        public static MessageDescriptor FindMessageType(string fullName)
        {
            var msgDescs = m_ProtobufDescriptor.GetMessageTypes((msgDesc) => { return msgDesc.FullName == fullName; });
            if (msgDescs == null) return null;
            if (msgDescs.Count == 0) return null;
            return msgDescs[0];
        }

#if UNITY_EDITOR
        public static void WatchProtobufSchema()
        {
            var watcher = new FileSystemWatcher(SchemaRootPath);
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Size;
            watcher.Changed += OnSchemaChanged;
            watcher.Created += OnSchemaCreated;
            watcher.Deleted += OnSchemaDeleted;
            watcher.Renamed += OnSchemaRenamed;
            watcher.Filter = $"*.{SchemaExtension}";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private static void OnSchemaChanged(object sender, FileSystemEventArgs e)
        {
            Debug.Log("OnSchemaChanged:" + e.FullPath);
            ReloadSchemas();
        }

        private static void OnSchemaCreated(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("OnSchemaCreated:" + e.FullPath);
            ReloadSchemas();
        }

        private static void OnSchemaDeleted(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("OnSchemaDeleted:" + e.FullPath);
            ReloadSchemas();
        }

        private static void OnSchemaRenamed(object sender, FileSystemEventArgs e)
        {
            Debug.LogError("OnSchemaRenamed:" + e.FullPath);
            ReloadSchemas();
        }
#endif
    }
}