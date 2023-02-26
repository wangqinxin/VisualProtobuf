using System;
using System.Collections.Generic;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using UnityEngine.UIElements;

namespace VisualProtobuf.UIElements
{
    public class MessageField : VisualElement, IProtobufVisualField
    {
        public static readonly string ussClassName = "unity-base-field";
        public static readonly string labelUssClassName = ussClassName + "__label";
        public static readonly string messageFieldUssClassName = "message_field";
        public static readonly string messageFieldContainerUssClassName = messageFieldUssClassName + "_container";

        public FieldDescriptor Descriptor { get; set; }
        public IMessage Message { get; set; }
        public IProtobufVisualRoot Root { get; set; }
        public IProtobufVisualField Parent { get; set; }
        public HashSet<string> AssociatedFields { get; set; }
        public string FieldPath { get; set; }
        public Action<object, object> OnValueChanged { get; set; }
        private MessageFieldHeader m_FieldHeader;

        VisualElement m_ContentContainer;



        public MessageField(IMessage message, FieldDescriptor descriptor = null)
        {
            Descriptor = descriptor;
            Message = message;

            RegisterCallback<ChangeEvent<IMessage>>(OnFieldValueChanged);

            AddToClassList(ussClassName);
            AddToClassList(messageFieldUssClassName);
            style.flexDirection = FlexDirection.Column;

        }

        internal void BuildUIElement()
        {
            Clear();

            if (Descriptor == null)
            {
                if (Message == null) return;
                CreateFieldUI(this, Message.Descriptor, Message);
                return;
            }

            if (Descriptor.HasValue<IMessage>(Message))
            {
                CreateNotNullValueUI();
            }
            else
            {
                CreateNullValueUI();
            }
        }

        void CreateNullValueUI()
        {
            m_FieldHeader = new MessageFieldHeader(Descriptor.GetDisplayName());
            m_FieldHeader.SetValueIsNull(true);
            Add(m_FieldHeader);

            var createButton = new Button();
            createButton.style.backgroundImage = ProtobufStyleAssets.Active.IconAdd;
            createButton.RegisterCallback<ClickEvent>(OnClickCreateMessage);
            m_FieldHeader.AddControlElement(createButton);
        }

        void CreateNotNullValueUI()
        {
            m_FieldHeader = new MessageFieldHeader(Descriptor.GetDisplayName());
            m_FieldHeader.SetValueIsNull(false);
            m_FieldHeader.SetValueWithoutNotify(true); //TODO message default value
            m_FieldHeader.RegisterCallback<ChangeEvent<bool>>(OnFieldHeaderChecked);
            Add(m_FieldHeader);

            var deleteButton = new Button();
            deleteButton.style.backgroundImage = ProtobufStyleAssets.Active.IconDelete;
            deleteButton.RegisterCallback<ClickEvent>(OnClickDeleteMessage);
            m_FieldHeader.AddControlElement(deleteButton);

            CreateMessageContentUI(m_FieldHeader.value);
        }

        void OnFieldHeaderChecked(ChangeEvent<bool> changeEvent)
        {
            CreateMessageContentUI(changeEvent.newValue);
        }

        void CreateMessageContentUI(bool display)
        {
            if (display)
            {
                if (m_ContentContainer == null)
                {
                    var message = Descriptor.GetValue<IMessage>(Message);
                    if (message == null) return;

                    m_ContentContainer = new VisualElement();
                    m_ContentContainer.AddToClassList(messageFieldContainerUssClassName);
                    Add(m_ContentContainer);

                    CreateFieldUI(m_ContentContainer, Descriptor.MessageType, message);
                }
                Add(m_ContentContainer);
            }
            else
            {
                if (m_ContentContainer != null)
                    Remove(m_ContentContainer);
            }
        }

        void OnClickCreateMessage(ClickEvent clickEvent)
        {
            var msg = ProtobufDatabase.CreateMessage(Descriptor.MessageType);
            Descriptor.SetValue(Message, msg);

            using var evt = ChangeEvent<IMessage>.GetPooled(Message, Message);
            evt.target = parent;
            SendEvent(evt);

            BuildUIElement();
        }

        void OnClickDeleteMessage(ClickEvent clickEvent)
        {
            Descriptor.SetValue<IMessage>(Message, null);

            using var evt = ChangeEvent<IMessage>.GetPooled(Message, Message);
            evt.target = parent;
            SendEvent(evt);

            BuildUIElement();
        }

        void CreateFieldUI(VisualElement fieldRootElement, MessageDescriptor messageDescriptor, IMessage message)
        {
            var fields = messageDescriptor.Fields;
            foreach (var fieldDesc in fields.InFieldNumberOrder())
            {
                fieldRootElement.Add(InstanceFieldHelpers.CreateFieldElement(Root, this, fieldDesc, message));
            }
        }


        void OnFieldValueChanged(ChangeEvent<IMessage> changeEvent)
        {
            if (OnValueChanged != null)
                OnValueChanged.Invoke(changeEvent.previousValue, changeEvent.newValue);

            changeEvent.StopPropagation();

            this.OnPostValueChanged();

            using var evt = ChangeEvent<IMessage>.GetPooled(Message, Message);
            evt.target = parent;
            SendEvent(evt);
        }

        public void SetValue(object value, bool notify)
        {
            if (value is IMessage message)
            {
                Message = message;
                BuildUIElement();
            }
        }

        public object GetValue()
        {
            return Message;
        }

        public void SetLabel(string label)
        {
            if (m_FieldHeader != null) m_FieldHeader.label = label;
        }

        class MessageFieldHeader : BaseBoolField
        {
            public static readonly string messageFieldHeaderUssClassName = messageFieldUssClassName + "_header";
            public static readonly string headerCheckmarkUssClassName = messageFieldHeaderUssClassName + "__checkmark";
            public static readonly string headerLabelUssClassName = messageFieldHeaderUssClassName + "__label";
            public static readonly string messageFieldNullHeaderUssClassName = messageFieldHeaderUssClassName + "__notnull";
            public static readonly string messageFieldControlUssClassName = messageFieldHeaderUssClassName + "__control";

            internal VisualElement m_ControlElement;

            public MessageFieldHeader(string label) : base(null)
            {
                text = label;

                AddToClassList(messageFieldHeaderUssClassName);

                m_ControlElement = new VisualElement();
                m_ControlElement.AddToClassList(messageFieldControlUssClassName);

                m_CheckMark.AddToClassList(headerCheckmarkUssClassName);
                Add(m_ControlElement);
            }

            protected override void InitLabel()
            {
                base.InitLabel();
                m_Label.AddToClassList(headerLabelUssClassName);
            }

            internal void AddControlElement(VisualElement element)
            {
                m_ControlElement.Add(element);
            }

            internal void SetValueIsNull(bool isNull)
            {
                if (isNull)
                    RemoveFromClassList(messageFieldNullHeaderUssClassName);
                else
                    AddToClassList(messageFieldNullHeaderUssClassName);
            }
        }
    }
}