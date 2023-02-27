using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace WARP.Core.API {
public class ImageExtractor {
  [MenuItem("Ecilos/Extract/Textures")]
  public static void Extract() {
    foreach (string guid in Selection.assetGUIDs) {
      ExtractAsset(guid);
    }
  }

  public static void ExtractAsset(string guid, string extension = "jpg",
                                  int quality = 100) {
    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

    AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);

    IEnumerable<Object> materials =
        AssetDatabase.LoadAllAssetsAtPath(assetImporter.assetPath)
            .ToList()
            .Where(x => x is Material || x is Texture2D);

    foreach (Object obj in materials) {
      if (obj is Material material) {
        string[] texturePropertyNames = material.GetTexturePropertyNames();

        foreach (string texturePropertyName in texturePropertyNames) {
          Texture texture = material.GetTexture(texturePropertyName);

          if (texture == null)
            // Empty slot.
            continue;

          if (texture is Texture2D texture2D)
            WriteTexture(assetPath, texture2D, extension, quality);
          else
            Debug.LogWarning(
                $"Extraction of object's \"{assetImporter.assetPath}\"'s texture type {texture.GetType().Name} is unsupported for texture property \"{texturePropertyName}\"! Only Texture2D is yet supported.");
        }
      } else if (obj is Texture2D texture2D)
        WriteTexture(assetPath, texture2D, extension, quality);
    }
  }

  private static void WriteTexture(string assetPath, Texture2D texture,
                                   string extension, int quality = 100) {
    byte[] bytes;

    if (extension == "jpg")
      bytes = texture.EncodeToJPG(quality);
    else
      throw new ArgumentException(
          "Only \"jpg\" extension currently supported!");

    File.WriteAllBytes(BuildTexturePath(assetPath, texture.name, extension),
                       bytes);
  }

  public static string BuildTexturePath(string assetPath, string textureName,
                                        string extension) {
    string assetFolder = assetPath.Substring(0, assetPath.LastIndexOf('/'));
    string assetPathWithoutExtension =
        assetPath.Substring(0, assetPath.LastIndexOf('.'));
    // string assetName = assetPath.Substring(assetFolder.Length + 1);

    Directory.CreateDirectory(assetPathWithoutExtension);

    return $"{assetPathWithoutExtension}/{textureName}.{extension}";
  }
}
}