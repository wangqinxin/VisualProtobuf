using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public class ProtobufSchemaEditorView : ProtobufSchemaView
    {
        public ProtobufSchemaEditorView(bool editMode = true) : base(editMode)
        {
        }

        protected override VisualElement CreateHeader()
        {
            var header = new VisualElement();
            var title = new Label(kSchemaViewTitle);
            var fontStyle = title.style.unityFontStyleAndWeight;
            fontStyle.value = FontStyle.Bold;
            title.style.unityFontStyleAndWeight = fontStyle;
            header.Add(title);

            var genCodeBtn = new Button();
            genCodeBtn.text = "GenerateCode";
            genCodeBtn.RegisterCallback<ClickEvent>(OnClickGenerateCode);
            header.Add(genCodeBtn);

            var toolbar = new ProtobufToolbar();
            toolbar.AppendAddMenuAction("Add Schema", (action) => { });
            header.Add(toolbar);
            return header;
        }

        void OnClickGenerateCode(ClickEvent clickEvent)
        {
            ProtobufDatabase.ReloadSchemas();
        }
    }
}