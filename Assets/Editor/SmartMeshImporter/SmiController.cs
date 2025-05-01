using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
            
            if (_fileNames.Length == 0)
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

        public static void SceneRebuilt(TextAsset jsonFile)
        {
            if(jsonFile == null) return;
            string path = AssetDatabase.GetAssetPath(jsonFile); //Read json data
            if (!path.Contains(".json"))
            {
                Debug.LogWarning("Text asset is not a .json file.");
                return;
            }

            SceneData sceneData = SceneDataReader.GetSceneData(path);
            if (sceneData == null) return;

            Dictionary<string, GameObject> fbxGameObjects = new System.Collections.Generic.Dictionary<string, GameObject>();
            Dictionary<string, GameObject> addedGameObjs = new System.Collections.Generic.Dictionary<string, GameObject>();
            string modelsPath = "Assets" + _modelsPath.Replace(Application.dataPath, "");

            //Instantiate all gameObjects and apply transformations.
            foreach (MeshObjectData objectData in sceneData.objectDatas)
            {
                GameObject fbxGameObj;

                if (fbxGameObjects.TryGetValue(objectData.base_name, out GameObject obj))
                {
                    fbxGameObj = obj;
                }
                else
                {
                    string fbxPath = $"{modelsPath}/{objectData.base_name}.fbx";
                    fbxGameObj = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
                    if (fbxGameObj == null)
                    {
                        Debug.LogWarning("failed to load fbx: " + fbxPath);
                        continue;
                    }
                    fbxGameObjects.Add(objectData.base_name, fbxGameObj);
                }

                GameObject newObj = Instantiate(fbxGameObj);
                newObj.name = objectData.name;
                newObj.transform.position = new Vector3(objectData.location[0], objectData.location[2], objectData.location[1]);
                newObj.transform.localScale = new Vector3(objectData.scale[0], objectData.scale[2], objectData.scale[1]);
                newObj.transform.rotation = Quaternion.Euler(new Vector3(objectData.rotation[0], objectData.rotation[2], objectData.rotation[1]));
                addedGameObjs.Add(objectData.name, newObj);
            }

            //Set parents for each object
            foreach (MeshObjectData objectData in sceneData.objectDatas)
            {
                if(objectData.parent == "null") continue;
                if (!addedGameObjs.TryGetValue(objectData.name, out GameObject obj)) continue;
                if (!addedGameObjs.TryGetValue(objectData.parent, out GameObject parentObj)) continue;
                obj.transform.parent = parentObj.transform;
            }
        }

        public static void ExtractMaterialsAndTextures()
        {
            if (_fileNames == null || _fileNames.Length == 0)
            {
                Debug.LogWarning("No Fbx files found. Path might not exists or might not contain any .fbx files.");
                return;
            }

            string modelsPath = "Assets" + _modelsPath.Replace(Application.dataPath, "");
            string materialsPath = modelsPath + "/Materials";
            if (!Directory.Exists(materialsPath)) Directory.CreateDirectory(materialsPath);
            
            foreach (string file in _fileNames)
            {
                string filePath = "Assets" + file.Replace(Application.dataPath, "");
                SmiFbxProcessor.ProcessFBX(filePath, materialsPath);
            }
        }
    }
}
