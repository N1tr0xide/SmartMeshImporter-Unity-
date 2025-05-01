using System.IO;
using UnityEditor;
using UnityEngine;

namespace SmartMeshImporter
{
    public static class SmiFbxProcessor
    {
        /// Extract textures from the fbx file by switching between import settings.
        /// <param name="fbxPath">path of fbx file</param>
        /// <param name="materialOutputFolder">Folder to put materials in</param>
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

            modelImporter.materialLocation = ModelImporterMaterialLocation.InPrefab;
            AssetDatabase.WriteImportSettingsIfDirty(fbxPath);
            AssetDatabase.ImportAsset(fbxPath, ImportAssetOptions.ForceUpdate); //reimport twice because unity.

            // Extract Materials
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);

            foreach (UnityEngine.Object asset in assets)
            {
                if (asset is not Material material) continue;
                string materialPath = Path.Combine(materialOutputFolder, fbxName + "-" + material.name + ".mat");
                materialPath = AssetDatabase.GenerateUniqueAssetPath(materialPath);
                AssetDatabase.ExtractAsset(material, materialPath);
            }
        }
    }
}