using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SmartMeshImporter
{
    [System.Serializable]
    public class MeshObjectData
    {
        public string name;
        public string parent;
        public List<float> location;
        public List<float> rotation;
        public List<float> scale;
        public string base_name;
    }

    [System.Serializable]
    public class SceneData
    {
        public List<MeshObjectData> objectDatas = new List<MeshObjectData>();
    }

    public static class SceneDataReader
    {
        public static SceneData GetSceneData(string jsonFilePath)
        {
            if (File.Exists(jsonFilePath))
            {
                string json = File.ReadAllText(jsonFilePath);
                return JsonUtility.FromJson<SceneData>(json);
            }

            Debug.LogWarning("JSON file not found: " + jsonFilePath);
            return null;
        }
    }
}