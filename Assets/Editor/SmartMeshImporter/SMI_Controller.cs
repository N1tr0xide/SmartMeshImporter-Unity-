using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class SMI_Controller : MonoBehaviour
{
    private static StreamReader reader;
    private static List<GameObject> objects = new List<GameObject>();

    public static void OnMeshPathChanged(string path)
    {
        if (!path.Contains("Assets/")) return;
       
        string[] fileNames = Directory.GetFiles(path, "*.fbx");
        if(fileNames == null)
        {
            Debug.LogWarning("No files found");
            return;
        }

        foreach (var item in fileNames)
        {
            print(item);
            var sourceFile = new FileInfo(item);
            if (sourceFile != null & sourceFile.Exists) reader = new StreamReader(path);

            if (reader == null)
            {
                Debug.Log(sourceFile + " not found");
                return;
            }

            string name = sourceFile.Name;
            string nameShort = name.Substring(0, name.Length - 4); //name without extension
            GameObject gObj = new GameObject(nameShort);
            objects.Add(gObj);
        }

        foreach (var item in objects)
        {
            print(item.name);
        }
    }

    public static void SceneRebuilt(string meshPath)
    {
        print(meshPath);
    }
}
