using System;
using UnityEditor;
using UnityEngine;

public class ReadWriteAuto : EditorWindow
{
    private void OnEnable()
    {
        ReadWrite();
    }
    private void ReadWrite()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
            if (modelImporter != null && !modelImporter.isReadable)
            {
                modelImporter.isReadable = true;
                modelImporter.SaveAndReimport(); 
            }

        }
    }
}