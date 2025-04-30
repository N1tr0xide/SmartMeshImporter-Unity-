using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SmartMeshImporter
{
    public class SmiWindow : EditorWindow
    {
        private VisualTreeAsset _inspectorXML;
        private VisualElement _root;
        private TextField _meshPath;

        [MenuItem("Window/Smart Mesh Importer Window")]
        public static void ImporterWindow()
        {
            SmiWindow wnd = GetWindow<SmiWindow>();
            wnd.titleContent = new GUIContent("Smart Mesh Importer");
        }

        public void CreateGUI()
        {
            _inspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/SmartMeshImporter/SMI_Window_UXML.uxml");
            _root = _inspectorXML.CloneTree();
            rootVisualElement.Add(_root);

            _meshPath = _root.Q<TextField>("MeshFolderPathField");
            _meshPath.RegisterValueChangedCallback(evt => { SmiController.OnMeshPathChanged(evt.newValue); });

            var button = _root.Q<Button>("RebuiltSceneBtn");
            button.clicked += OnRebuiltSceneButton;

            var matsExtractBtn = _root.Q<Button>("ExtractButton");
            matsExtractBtn.clicked += OnMatsExtractBtn;
        }

        private void OnRebuiltSceneButton()
        {
            if (_meshPath == null || _meshPath.text == "")
            {
                Debug.LogWarning("Mesh Folder Path is Empty");
                return;
            }

            SmiController.SceneRebuilt();
        }
    
        private void OnMatsExtractBtn()
        {
            SmiController.ExtractMaterialsAndTextures();
        }
    }
}
