using Codice.CM.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SMI_Window : EditorWindow
{
    private VisualTreeAsset inpectorXML;
    private VisualElement root;
    private TextField meshPath;

    [MenuItem("Window/Smart Mesh Importer Window")]
    public static void ImporterWindow()
    {
        SMI_Window wnd = GetWindow<SMI_Window>();
        wnd.titleContent = new GUIContent("Smart Mesh Importer");
    }

    public void CreateGUI()
    {
        inpectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/SmartMeshImporter/SMI_Window_UXML.uxml");
        root = inpectorXML.CloneTree();
        rootVisualElement.Add(root);

        meshPath = root.Q<TextField>("MeshFolderPathField");
        meshPath.RegisterValueChangedCallback(evt => { SMI_Controller.OnMeshPathChanged(evt.newValue); });

        var button = root.Q<Button>("RebuiltSceneBtn");
        button.clicked += OnRebuiltSceneButton;    
    }

    private void OnRebuiltSceneButton()
    {
        if (meshPath == null || meshPath.text == "")
        {
            Debug.LogWarning("Mesh Folder Path is Empty");
            return;
        }

        SMI_Controller.SceneRebuilt(meshPath.text);
    }
}
