using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VisualProtobuf
{
    public static class ProtobufEditorUtilities
    {
        public static void OpenWithVscode(string rootPath, string filePath, int line, bool reuse = false)
        {
            string args = rootPath;
            if (line > 0)
                args += $" -g \"{filePath}:{line}\"";
            else
                args += $" \"{filePath}\"";
            if (reuse)
            {
                args += " -r ";
            }
            Process.Start("code", args);
        }

        public static void OpenInstanceWithVscode(string filePath, int line, bool reuse = false)
        {
            CheckVscodeWorkspaceSettings();
            OpenWithVscode(ProtobufDatabase.InstanceRootPath, filePath, line, reuse);
        }

        const string kVscodeWorkspaceFileAssociations = "files.associations";
        static void CheckVscodeWorkspaceSettings()
        {
            var codeSettingsRootPath = Path.Combine(ProtobufDatabase.InstanceRootPath, ".vscode");
            if (!Directory.Exists(codeSettingsRootPath)) Directory.CreateDirectory(codeSettingsRootPath);
            var codeSettingsPath = Path.Combine(codeSettingsRootPath, "settings.json");
            if (File.Exists(codeSettingsPath)) return;

            var settings = new JObject();
            var associations = new JObject();
            associations.Add("*.json", "jsonc");
            settings.Add(kVscodeWorkspaceFileAssociations, associations);
            File.WriteAllText(codeSettingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}