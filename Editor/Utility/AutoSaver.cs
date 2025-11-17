using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor.Utility
{
[CreateAssetMenu(menuName = "Tools/AutoSaver")]
    public class AutoSaver : ScriptableObject
    {
        private void OnEnable()
        {
            EditorApplication.projectChanged += SaveAssets;
            // Debug.Log("onenable called");
        }

        private void SaveAssets()
        {
            AssetDatabase.SaveAssets();
            // EditorApplication.ExecuteMenuItem("File/Save");    
            Debug.Log("Auto Saver: save assets called");
            //
            // string[] path = EditorSceneManager.GetActiveScene().path.Split(char.Parse("/"));
            // path[path.Length - 1] = "AutoSave_" + path[path.Length - 1];
            // bool saveOK = EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            // Debug.Log("Saved Scene " + (saveOK ? "OK" : "Error!"));
        }
    }
}