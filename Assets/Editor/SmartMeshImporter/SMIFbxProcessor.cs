using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.Progress;

namespace SmartMeshImporter
{
    public class SMIFbxProcessor : MonoBehaviour
    {
        /// Extract textures from the fbx file
        /// <param name="fbxPath">path of fbx file</param>
        public static void ProcessFBX(string fbxPath, string materialOutputFolder)
        {
            ModelImporter modelImporter = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
            if (modelImporter == null) return;

            FileInfo sourceFile = new FileInfo(fbxPath);
            if (!sourceFile.Exists) return;
            string fbxName = sourceFile.Name.Substring(0, sourceFile.Name.Length - 4); //name without extension

            //Extract textures
            modelImporter.materialLocation = ModelImporterMaterialLocation.External;
            modelImporter.materialName = ModelImporterMaterialName.BasedOnModelNameAndMaterialName;
            modelImporter.materialSearch = ModelImporterMaterialSearch.Local;
            AssetDatabase.WriteImportSettingsIfDirty(fbxPath);
            AssetDatabase.ImportAsset(fbxPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();

            modelImporter.materialLocation = ModelImporterMaterialLocation.InPrefab;
            AssetDatabase.WriteImportSettingsIfDirty(fbxPath);
            AssetDatabase.ImportAsset(fbxPath, ImportAssetOptions.ForceUpdate); //reimport twice because unity.
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Extract Materials
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);

            foreach (Object asset in assets)
            {
                if (asset is not Material material) continue;
                string materialPath = Path.Combine(materialOutputFolder, fbxName + "-" + material.name + ".mat");
                materialPath = AssetDatabase.GenerateUniqueAssetPath(materialPath);
                AssetDatabase.ExtractAsset(material, materialPath);
            }
        }

        public static void MoveTexturesFolders(string modelsPath, string destPath)
        {

            string[] textureDirectories = Directory.GetDirectories(modelsPath, "*.fbm");

            foreach (string dir in textureDirectories)
            {
                string newDir = dir.Replace(modelsPath, "");
                print(newDir);
                Directory.Move(dir, destPath + newDir);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}