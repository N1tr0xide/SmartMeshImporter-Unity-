using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SmartMeshImporter
{
    public class SMIFbxProcessor : MonoBehaviour
    {
        public static void ProcessFBX(string fbxPath, string outputFolder)
        {
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
            if (!Directory.Exists(outputFolder + "/textures")) Directory.CreateDirectory(outputFolder + "/textures");
            
            // Extract textures from the FBX file
            ModelImporter modelImporter = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
            if (modelImporter == null) return;
            modelImporter.ExtractTextures(outputFolder + "/textures");

            // Load all assets from the FBX
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            List<Material> materials = new List<Material>();

            foreach (Object asset in assets)
            {
                if (asset is not Material material) continue;
                
                string materialPath = Path.Combine(outputFolder, material.name + ".mat");
                materialPath = AssetDatabase.GenerateUniqueAssetPath(materialPath);
                AssetDatabase.ExtractAsset(material, materialPath);
                materials.Add(material);
            }

            // Assign textures to extracted materials
            
            AssignTexturesToMaterials(materials, outputFolder + "/textures");
        }

        private static void AssignTexturesToMaterials(List<Material> materials, string textureFolder)
        {
            Dictionary<string, Texture> textures = LoadTextures(textureFolder);

            foreach (Material material in materials)
            {
                foreach (var textureEntry in textures)
                {
                    Texture texture = textureEntry.Value;
                    material.SetTexture("_BaseMap", texture);
                    
                    /*
                    if (texture.name.ToLower().Contains("diffuse") || texture.name.ToLower().Contains("albedo") || texture.name.ToLower().Contains("_basecolor"))
                    {
                        material.SetTexture("_MainTex", texture);
                    }
                    else if (texture.name.ToLower().Contains("normal"))
                    {
                        material.SetTexture("_BumpMap", texture);
                        material.EnableKeyword("_NORMALMAP");
                    }
                    else if (texture.name.ToLower().Contains("roughness"))
                    {
                        material.SetTexture("_GlossMap", texture);
                        material.EnableKeyword("_GLOSSY_REFLECTIONS_OFF");
                    }
                    else if (texture.name.ToLower().Contains("metallic"))
                    {
                        material.SetTexture("_MetallicGlossMap", texture);
                        material.EnableKeyword("_METALLICGLOSSMAP");
                    }
                    else if (texture.name.ToLower().Contains("ao") || texture.name.ToLower().Contains("ambientocclusion"))
                    {
                        material.SetTexture("_OcclusionMap", texture);
                    }*/

                    Debug.Log($"Assigned {texture.name} to {material.name}");
                    AssetDatabase.SaveAssets();
                }
                
                AssetDatabase.SaveAssets();
            }
            
        }

        private static Dictionary<string, Texture> LoadTextures(string folderPath)
        {
            Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

            // Load all extracted textures
            string[] textureFiles = Directory.GetFiles(folderPath, "*.*");

            foreach (string file in textureFiles)
            {
                string extension = Path.GetExtension(file).ToLower();
                if (extension != ".png" && extension != ".jpg" && extension != ".jpeg") continue;
                
                Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(file);
                if (texture == null) continue;
                textures[file] = texture;
            }

            return textures;
        }
    }
}