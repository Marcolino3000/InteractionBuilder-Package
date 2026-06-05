using Runtime.Scripts.Interactables;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(Toggleable))]
    public class ToggleableInspector : InteractableInspector
    {
        private SerializedProperty toggleStateProp;
        private SerializedProperty StatusOnSpriteProp;
        private SerializedProperty StatusOffSpriteProp;
    
        protected override void OnEnable()
        {
            base.OnEnable();
        
            toggleStateProp = serializedObject.FindProperty("ToggleState");
            StatusOnSpriteProp = serializedObject.FindProperty("StatusOnSprite");
            StatusOffSpriteProp = serializedObject.FindProperty("StatusOffSprite");
        }
    
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            // serializedObject.Update();
        
            var toggleable = (Toggleable)target;
        
            EditorGUILayout.PropertyField(toggleStateProp);
            EditorGUILayout.LabelField("Status", toggleable.StatusDescription);
            
            EditorGUILayout.PropertyField(StatusOnSpriteProp);
            EditorGUILayout.PropertyField(StatusOffSpriteProp);
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}