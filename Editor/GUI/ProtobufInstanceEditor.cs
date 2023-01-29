using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.IMGUI.Controls;
using Google.Protobuf;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ProtobufInstance))]
    public class ProtobufInstanceEditor : Editor
    {
        public StyleSheet protobufInstanceEditorStyleSheet;

        SerializedProperty foldOutAll;
        SerializedProperty selectedLanguages;
        SerializedProperty autoSave;

        private ProtobufInstance m_Instance;
        private ProtobufInstanceMeta m_MetaData;

        private VisualElement m_InspectorRoot;

        private void OnEnable()
        {
            m_Instance = target as ProtobufInstance;
            m_MetaData = m_Instance.metaData;
        }

        protected override void OnHeaderGUI()
        {
            GUILayout.BeginHorizontal("In BigTitle");
            GUILayout.Space(38);
            GUILayout.BeginVertical();
            GUILayout.Space(21);
            GUILayout.BeginHorizontal();

            GUILayoutUtility.GetRect(10, 10, 16, 16, EditorStyles.layerMaskField);

            GUILayout.FlexibleSpace();


            //var selectIndex = EditorGUILayout.Popup(selectedLanguages.intValue, , GUILayout.MaxWidth(65));
            //if (selectedLanguages.intValue != selectIndex)
            //{
            //    selectedLanguages.intValue = selectIndex;
            //}

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            Rect fullRect = GUILayoutUtility.GetLastRect();
            Rect iconRect = new Rect(fullRect.x + 6, fullRect.y + 6, 32, 32);
            var icon = m_Instance.isFolder ? ProtobufEditorStyleAssets.Active.IconFolder : ProtobufEditorStyleAssets.Active.IconInstance;
            GUI.Label(iconRect, icon);
            var titleRect = new Rect(fullRect.x + 44, fullRect.y + 6, fullRect.width - 44, 18);
            string fileName = target.name;
            EditorStyles.largeLabel.CalcMinMaxWidth(new GUIContent(fileName), out float minWidth, out float maxWidth);

            titleRect.width = minWidth + 10;
            GUI.Label(titleRect, fileName, EditorStyles.largeLabel);

            //if (m_Instance.metaData != null)
            //{
            //    var msgType = m_Instance.metaData.messageType;
            //    EditorStyles.largeLabel.CalcMinMaxWidth(new GUIContent(msgType), out minWidth, out maxWidth);
            //    var msgTypeRect = new Rect(titleRect);
            //    msgTypeRect.y += 18;
            //    msgTypeRect.width = minWidth + 10;
            //    GUI.Label(msgTypeRect, msgType, EditorStyles.largeLabel);
            //}

            //Auto Toggle
            var rect = new Rect(titleRect.x + 3, titleRect.y + 23f, 16, 16);

            //var foldoutVal = GUI.Toggle(rect, foldOutAll.boolValue, "", ProtobufMessageEditorConst.foldToggleStyle);
            //if (foldoutVal != foldOutAll.boolValue)
            //{
            //    foldOutAll.boolValue = foldoutVal;

            //}

            rect.x += 18;
            rect.y += 1;
            rect.width = 80;

            //autoSave.boolValue = GUI.Toggle(rect, autoSave.boolValue, "AutoSave");
            //if (!autoSave.boolValue)
            //{
            //    rect.x += rect.width;
            //    rect.height = 17;
            //    rect.width = 48;
            //    GUI.enabled = serializer?.hasChangedProperty ?? true;
            //    if (GUI.Button(rect, "Save"))
            //    {
            //        //SaveModifications
            //    }
            //    GUI.enabled = true;
            //}
        }

        public override VisualElement CreateInspectorGUI()
        {
            m_InspectorRoot = new VisualElement();
            if (m_Instance.isFolder) return m_InspectorRoot;
            DrawInspectorGUI();
            m_InspectorRoot.RegisterCallback<ChangeEvent<IMessage>>(OnInstanceValueChanged);
            return m_InspectorRoot;
        }

        void DrawInspectorGUI()
        {
            m_InspectorRoot.Clear();
            var messageType = m_Instance.metaData.messageType;
            if (string.IsNullOrEmpty(messageType))
            {
                m_InspectorRoot.Add(CreateNullTypeGUI());
            }
            else
            {
                var msgDesc = ProtobufDatabase.FindMessageType(messageType);
                if (msgDesc != null)
                {
                    using var jsonReader = System.IO.File.OpenText(m_Instance.fsPath);
                    var msg = ProtobufDatabase.CreateMessage(msgDesc,jsonReader);
                    var instanceField = new InstanceField(msg);
                    instanceField.styleSheets.Add(protobufInstanceEditorStyleSheet);
                    m_InspectorRoot.Add(instanceField);
                }
                else
                {
                    m_InspectorRoot.Add(new Label($"Invalid messageType {messageType}"));
                }
            }
        }

        VisualElement CreateNullTypeGUI()
        {
            var btn = new Button();
            btn.style.marginTop = 5;
            btn.style.minWidth = 200;
            btn.style.maxWidth = 300;
            btn.style.alignSelf = Align.Center;
            btn.text = "Set Protobuf Message";
            btn.RegisterCallback<ClickEvent>((e) =>
            {
                var dropdown = new ProtobufMessageDropdown(new AdvancedDropdownState(), (messageType) =>
                {
                    m_Instance.SetMessageType(messageType);
                    DrawInspectorGUI();
                    Repaint();
                });
                var rect = new Rect(btn.worldBound.position, btn.worldBound.size);
                dropdown.Show(rect);
            });
            return btn;
        }

        void OnInstanceValueChanged(ChangeEvent<IMessage> changeEvent)
        {
            var instance = changeEvent.newValue;
            if (instance != null)
            {
                var writer = m_Instance.CreateWriter();
                var indentWriter = new IndentedTextWriter(writer, IndentedTextWriter.DefaultTabString);
                var jsonSettings = JsonFormatter.Settings.Default.WithFormatIndented(true);
                var jsonFormatter = new JsonFormatter(jsonSettings);
                jsonFormatter.Format(instance, indentWriter);
                writer.Dispose();
            }
            changeEvent.StopPropagation();
        }
    }
}
