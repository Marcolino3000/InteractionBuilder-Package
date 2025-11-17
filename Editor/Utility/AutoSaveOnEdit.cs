using UnityEditor;
using UnityEngine;

public class AutoSaveOnEdit : EditorWindow
{
    [MenuItem("Window/My Window")]
    static void Init()
    {
        AutoSaveOnEdit window = (AutoSaveOnEdit)EditorWindow.GetWindow(typeof(AutoSaveOnEdit));
        window.Show();
        EditorApplication.projectChanged += SaveAssets;
        
        Debug.Log("init called");
    }
    private static void SaveAssets()
    {
        AssetDatabase.SaveAssets();
        Debug.Log("save assets called");
    }
}