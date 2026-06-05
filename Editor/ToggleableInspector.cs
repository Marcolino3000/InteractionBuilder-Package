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
        private SerializedProperty SpriteRotationOnProp;
        private SerializedProperty SpriteRotationOffProp;
    
        protected override void OnEnable()
        {
            base.OnEnable();
        
            toggleStateProp = serializedObject.FindProperty("ToggleState");
            StatusOnSpriteProp = serializedObject.FindProperty("StatusSpriteOn");
            StatusOffSpriteProp = serializedObject.FindProperty("SpriteRotationOff");
            SpriteRotationOnProp = serializedObject.FindProperty("SpriteRotationOn");
            SpriteRotationOffProp = serializedObject.FindProperty("SpriteRotationOff");
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
            
            EditorGUILayout.PropertyField(SpriteRotationOnProp);
            EditorGUILayout.PropertyField(SpriteRotationOffProp);
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}