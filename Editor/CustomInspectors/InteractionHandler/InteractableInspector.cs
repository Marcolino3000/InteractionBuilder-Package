using Runtime.Scripts.Interactables;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(InteractableState))]
    public class InteractableInspector : StateOwnerInspector
    {
        private bool showStaticData = true;
        private SerializedProperty spriteProp;
        private SerializedProperty discoveryTypeProp;
        private SerializedProperty interactionType;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            spriteProp = serializedObject.FindProperty("Sprite");
            discoveryTypeProp = serializedObject.FindProperty("AwarenessLevel");
            interactionType = serializedObject.FindProperty("InteractionType");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
           
            showStaticData = EditorGUILayout.Foldout(showStaticData, "Static Data");
            
            if (showStaticData)
            {
                EditorGUILayout.PropertyField(spriteProp);
                EditorGUILayout.PropertyField(discoveryTypeProp);
                EditorGUILayout.PropertyField(interactionType);
            }
        
            serializedObject.ApplyModifiedProperties();
        }
    }
}