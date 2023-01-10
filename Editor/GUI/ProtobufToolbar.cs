using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace VisualProtobuf.UIElements
{
    public class ProtobufToolbar : VisualElement
    {
        private ToolbarMenu m_AddMenu;
        private ToolbarButton m_DeleteBtn;
        private ToolbarButton m_RefreshBtn;

        public ProtobufToolbar()
        {
            var toolbar = new Toolbar();

            m_AddMenu = new ToolbarMenu();
            m_AddMenu.style.minWidth = 36;

            var textElement = m_AddMenu.ElementAt(0);
            textElement.style.backgroundImage = ProtobufStyleAssets.Active.IconAdd;
            textElement.style.width = 16;
            textElement.style.height = 16;
            toolbar.Add(m_AddMenu);

            m_DeleteBtn = new ToolbarButton();
            m_DeleteBtn.style.minWidth = 36;
            m_DeleteBtn.Add(new Image() { image = ProtobufStyleAssets.Active.IconDelete });
            toolbar.Add(m_DeleteBtn);

            m_RefreshBtn = new ToolbarButton();
            m_RefreshBtn.style.minWidth = 36;
            m_RefreshBtn.style.borderLeftWidth = 0;
            m_RefreshBtn.Add(new Image() { image = ProtobufStyleAssets.Active.IconRefresh });
            toolbar.Add(m_RefreshBtn);

            var searchField = new ToolbarSearchField();
            searchField.style.flexGrow = 1;
            toolbar.Add(searchField);

            Add(toolbar);
        }

        public void AppendAddMenuAction(string actionName, Action<DropdownMenuAction> action, Func<DropdownMenuAction, DropdownMenuAction.Status> actionStatusCallback, object userData = null)
        {
            if (m_AddMenu == null) return;
            m_AddMenu.menu.AppendAction(actionName, action, actionStatusCallback, userData);
        }

        public void AppendAddMenuAction(string actionName, Action<DropdownMenuAction> action, DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal)
        {
            if (m_AddMenu == null) return;
            m_AddMenu.menu.AppendAction(actionName, action, status);
        }

        public void AppendAddMenuSeparator(string subMenuPath = null)
        {
            if (m_AddMenu == null) return;
            m_AddMenu.menu.AppendSeparator(subMenuPath);
        }

        public void SetDeleteButtonClickAction(Action deleteAction)
        {
            m_DeleteBtn.clicked += deleteAction;
        }

        public void SetRefreshButtonClickAction(Action refreshAction)
        {
            m_RefreshBtn.clicked += refreshAction;
        }
    }
}