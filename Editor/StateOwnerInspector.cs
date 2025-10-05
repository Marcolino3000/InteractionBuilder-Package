using Runtime.Scripts.Interactables;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(WorldStateOwner))]
    public class StateOwnerInspector : UnityEditor.Editor
    {
        protected SerializedProperty saveStateToCurrentSceneProp;
        
        protected virtual void OnEnable()
        {
            saveStateToCurrentSceneProp = serializedObject.FindProperty("_SaveStateToCurrentScene");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(saveStateToCurrentSceneProp);
        }
    }
}