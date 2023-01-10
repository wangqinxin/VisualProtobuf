using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public enum ProtobufInstanceType
    {
        Single,
        List,
        Map,
        Table,
    }

    [System.Serializable]
    public class ProtobufInstanceMeta
    {
        public string guid;

        public string syntax;

        public ProtobufInstanceType type;

        public string messageType;

        public ProtobufInstanceMeta(ProtobufInstanceType type)
        {
            guid = System.Guid.NewGuid().ToString();
            syntax = "proto3";
            this.type = type;
        }

        public static ProtobufInstanceMeta FromJson(string json)
        {
            return JsonUtility.FromJson<ProtobufInstanceMeta>(json);
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        public override string ToString()
        {
            var lines = ToLines();
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public List<string> ToLines()
        {
            var lines = new List<string>();
            using var reader = new StringReader(ToJson());
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lines.Add(line);
            }
            lines.RemoveAt(0);
            lines.RemoveAt(lines.Count - 1);
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = "// " + lines[i].Trim().TrimEnd(',');
            }
            return lines;
        }

        public static ProtobufInstanceMeta ReadMetaData(string filePath)
        {
            using var streamReader = File.OpenText(filePath);
            var lines = ListPool<string>.Get();
            while (true)
            {
                var line = streamReader.ReadLine();
                if (line == null || !line.StartsWith("//")) break;
                lines.Add(line);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            for (int i = 0; i < lines.Count; i++)
            {
                sb.Append(lines[i][3..]);
                sb.AppendLine(i == lines.Count - 1 ? "" : ",");
            }
            ListPool<string>.Release(lines);
            sb.AppendLine("}");
            return FromJson(sb.ToString());
        }

        public ProtobufInstanceMeta Duplicate()
        {
            var meta = new ProtobufInstanceMeta(type)
            {
                messageType = messageType
            };
            return meta;
        }
    }

    public class ProtobufInstance : ScriptableObject, IFileProvider
    {
        readonly static byte[] EmptyContentBytes = Encoding.UTF8.GetBytes("{}");

        public string FileExtension => "json";

        public ProtobufInstanceMeta metaData;

        public string fsPath;

        public bool isFolder;

        public void CreateMeta(ProtobufInstanceType type)
        {
            metaData = new ProtobufInstanceMeta(type);
        }

        public void Init(string fsPath)
        {
            this.fsPath = fsPath;
            this.name = Path.GetFileNameWithoutExtension(fsPath);
            if (Directory.Exists(fsPath))
            {
                isFolder = true;
            }
            else
            {
                isFolder = false;
                if (File.Exists(fsPath))
                {
                    metaData = ProtobufInstanceMeta.ReadMetaData(fsPath);
                }
                else
                {
                    if (metaData == null) metaData = new ProtobufInstanceMeta(ProtobufInstanceType.Single);
                    var bytes = metaData.ToBytes();
                    using var writer = File.Create(fsPath);
                    writer.Write(bytes, 0, bytes.Length);
                    writer.Write(EmptyContentBytes);
                }
            }
        }

        public IFileProvider Duplicate(string newPath)
        {
            var instance = CreateInstance<ProtobufInstance>();
            instance.metaData = metaData.Duplicate();
            instance.fsPath = newPath;
            instance.isFolder = isFolder;

            var newLines = instance.metaData.ToLines();

            var contentLine = 0;
            var lines = File.ReadAllLines(fsPath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("//"))
                    contentLine++;
                else
                    break;
            }
            newLines.AddRange(lines[contentLine..]);
            File.WriteAllLines(newPath, newLines);
            return instance;
        }

        public void SetMessageType(string messageType)
        {
            metaData.messageType = messageType;
            using var writer = File.OpenWrite(fsPath);
            var bytes = metaData.ToBytes();
            writer.Write(bytes, 0, bytes.Length);
            writer.Write(EmptyContentBytes);
        }

        public TextWriter CreateWriter()
        {
            var writer = File.CreateText(fsPath);
            writer.Write(metaData.ToString());
            return writer;
        }
    }
}