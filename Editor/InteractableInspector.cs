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

        protected override void OnEnable()
        {
            base.OnEnable();
            
            spriteProp = serializedObject.FindProperty("Sprite");
            locationOnMapProp = serializedObject.FindProperty("LocationOnMap");
        }

        public override void OnInspectorGUI()
        {
           base.OnInspectorGUI();
           
            showStaticData = EditorGUILayout.Foldout(showStaticData, "Static Data");
            
            if (showStaticData)
            {
                EditorGUILayout.PropertyField(spriteProp);
                EditorGUILayout.PropertyField(locationOnMapProp);
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}