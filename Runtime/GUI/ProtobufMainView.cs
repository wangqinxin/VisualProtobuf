using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Google.Protobuf.Reflection;
using VisualProtobuf.Extensions;

namespace VisualProtobuf.UIElements
{
    public class ProtobufMainView : VisualElement
    {
        private readonly bool m_EditMode;

        public ProtobufMainView(bool editMode = true)
        {
            style.width = Length.Percent(100);
            style.height = Length.Percent(100);
            m_EditMode = editMode;
            
            Add(CreateView());
        }

        protected virtual VisualElement CreateView()
        {
            var schemaView = CreateScheamView(m_EditMode);
            var messageView = CreateMessageView();

            var paneSplitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Vertical);
            paneSplitView.Add(schemaView);
            paneSplitView.Add(messageView);

            return paneSplitView;
        }

        protected virtual VisualElement CreateScheamView(bool editMode)
        {
            return new ProtobufSchemaView(editMode);
        }

        protected virtual VisualElement CreateMessageView()
        {
            return new ProtobufInstanceView();
        }
    }
}