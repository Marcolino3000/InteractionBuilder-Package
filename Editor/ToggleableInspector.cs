using Runtime.Scripts.Interactables;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(Toggleable))]
    public class ToggleableInspector : InteractableInspector
    {
        private SerializedProperty toggleStateProp;
    
        protected override void OnEnable()
        {
            base.OnEnable();
        
            toggleStateProp = serializedObject.FindProperty("ToggleState");
        }
    
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            // serializedObject.Update();
        
            var toggleable = (Toggleable)target;
        
            EditorGUILayout.PropertyField(toggleStateProp);
            EditorGUILayout.LabelField("Status", toggleable.StatusDescription);
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}