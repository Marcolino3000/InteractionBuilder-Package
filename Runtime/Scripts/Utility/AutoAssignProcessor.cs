#if UNITY_EDITOR
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Runtime.Scripts.Utility   
{
    [InitializeOnLoad]
    public static class AutoAssignProcessor
    {
        static AutoAssignProcessor()
        {
            EditorApplication.delayCall += AssignReferences;
            EditorApplication.projectChanged += AssignReferences;
        }

        [InitializeOnLoadMethod]
        private static void AssignReferences()
        {
            var allMonoBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).Cast<Object>();
            var allScriptableObjects = Resources.FindObjectsOfTypeAll<ScriptableObject>();
            var allObjects = allMonoBehaviours.Concat(allScriptableObjects);
            
            foreach (var obj in allObjects)
            {
                var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var field in fields)
                {
                    var autoAssignAttribute = field.GetCustomAttribute<AutoAssignAttribute>();
                    if (autoAssignAttribute == null || field.GetValue(obj) != null) continue;
                
                    var sceneObjects = Object.FindObjectsByType(field.FieldType, FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                    var resourcesObjects = Resources.FindObjectsOfTypeAll(field.FieldType);
                    var all = sceneObjects.Concat(resourcesObjects).ToArray();
                    
                    if (all is not { Length: 1 })
                        continue;
                    
                    field.SetValue(obj, all[0]);
                    EditorUtility.SetDirty(obj);
                }
            }
        }
    }
}
#endif