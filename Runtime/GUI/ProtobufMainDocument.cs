using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VisualProtobuf.UIElements;

namespace VisualProtobuf
{
    [RequireComponent(typeof(UIDocument))]
    public class ProtobufMainDocument : MonoBehaviour
    {
        private UIDocument m_Document;

        private void Awake()
        {
            m_Document = GetComponent<UIDocument>();
            var rootPath = System.IO.Path.GetFullPath(Application.dataPath + "/../Config");
            Debug.LogError("rootPath:" + rootPath);
            ProtobufDatabase.Initialize(rootPath);
        }

        private void Start()
        {
            if (m_Document == null) return;
            m_Document.visualTreeAsset = new VisualTreeAsset();
            var configView = new ProtobufMainView(false);
            m_Document.rootVisualElement.Add(configView);
        }
    }
}