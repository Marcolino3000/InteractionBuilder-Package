using Runtime.Scripts.Interactables;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(InteractableState))]
    public class InteractableInspector : StateOwnerInspector
    {
        private bool showStaticData = true;
        private SerializedProperty spriteProp;
        private SerializedProperty locationOnMapProp;
        private SerializedProperty discoveryTypeProp;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            spriteProp = serializedObject.FindProperty("Sprite");
            locationOnMapProp = serializedObject.FindProperty("LocationOnMap");
            discoveryTypeProp = serializedObject.FindProperty("AwarenessLevel");
        }

        public override void OnInspectorGUI()
        {
           base.OnInspectorGUI();
           
            showStaticData = EditorGUILayout.Foldout(showStaticData, "Static Data");
            
            if (showStaticData)
            {
                EditorGUILayout.PropertyField(spriteProp);
                EditorGUILayout.PropertyField(locationOnMapProp);
                EditorGUILayout.PropertyField(discoveryTypeProp);
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}