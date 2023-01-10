using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf.Reflection;

namespace VisualProtobuf.UIElements
{
    public class ProtobufMainEditorView : ProtobufMainView
    {
        public ProtobufMainEditorView(bool editMode = true)
            : base(editMode)
        {
        }

        protected override VisualElement CreateScheamView(bool editMode)
        {
            return new ProtobufSchemaEditorView(editMode);
        }

        protected override VisualElement CreateMessageView()
        {
            return new ProtobufInstanceEditorView();
        }
    }
}