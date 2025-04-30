using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SmartMeshImporter
{
    public class SmiController : MonoBehaviour
    {
        private static string _modelsPath;
        private static string[] _fileNames;
    
        public static void OnMeshPathChanged(string path)
        {
            if (path.Contains("Assets")) path = path.Replace("Assets", "");
            _modelsPath = Application.dataPath + path;
            
            try
            {
                _fileNames = Directory.GetFiles(_modelsPath, "*.fbx");
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case ArgumentException:
                        Debug.LogWarning("Invalid path: Path contains invalid characters");
                        break;
                    case DirectoryNotFoundException:
                        Debug.LogWarning("Invalid path: Path was not found, or it does not exists");
                        break;
                }

                _fileNames = null;
                return;
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (_fileNames == null || _fileNames.Length == 0)
            {
                Debug.LogWarning("No Fbx files found. Path might not exists or might not contain any .fbx files.");
                return;
            }
        
            //Print fbx files.
            Debug.LogWarning("FBX MODELS FOUND:");

            foreach (string item in _fileNames)
            {
                FileInfo sourceFile = new FileInfo(item);
                if (!sourceFile.Exists) continue;
                print(sourceFile.Name.Substring(0, sourceFile.Name.Length - 4)); //name without extension
            }
        }

        public static void SceneRebuilt()
        {
            //GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets" + item.Replace(Application.dataPath, ""));
            //_objects.Add(obj);

            //string name = sourceFile.Name;
            //string nameShort = name.Substring(0, name.Length - 4); //name without extension
            //GameObject gObj = new GameObject(nameShort);
            //_objects.Add(gObj);
        }

        public static void ExtractMaterialsAndTextures()
        {
            print("button pressed");
            if (_fileNames == null || _fileNames.Length == 0)
            {
                Debug.LogWarning("No Fbx files found. Path might not exists or might not contain any .fbx files.");
                return;
            }

            string modelsPath = "Assets" + _modelsPath.Replace(Application.dataPath, "");
            string materialsPath = modelsPath + "/Materials";
            string texturesPath = modelsPath + "/Textures";
            if (!Directory.Exists(materialsPath)) Directory.CreateDirectory(materialsPath);
            if (!Directory.Exists(texturesPath)) Directory.CreateDirectory(texturesPath);

            foreach (string file in _fileNames)
            {
                string filePath = "Assets" + file.Replace(Application.dataPath, "");
                SMIFbxProcessor.ProcessFBX(filePath, materialsPath);
            }

            SMIFbxProcessor.MoveTexturesFolders(modelsPath, texturesPath);
        }
    }
}
