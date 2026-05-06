using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Scripts.Interactables
{
    public class CursorSetter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int standardCursorSize = 24;
        [SerializeField] private int interactionCursorSize = 48;
        
        [Header("References")]
        [SerializeField] private Texture2D standardCursor;
        [SerializeField] private Texture2D handCursor;
        [SerializeField] private Texture2D inspectCursor;
        [SerializeField] private Texture2D moveCursor;
        [SerializeField] private Texture2D goThroughDoorCursor;
        
        private Texture2D resizedStandardCursor;
        private Texture2D resizedHandCursor;
        
        private Texture2D resizedMoveCursor;
        private Texture2D resizedInspectCursor;
        private Texture2D resizedGoThroughDoorCursor;
        private bool scene2;

        #region Setup

        private void Awake()
        {
            Initialize();
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if(scene.name == "Scene 2 from package")
                scene2 = true;
        }

        private void Initialize()
        {
            resizedStandardCursor = ResizeCursor(standardCursor, standardCursorSize);
            resizedHandCursor = ResizeCursor(handCursor, interactionCursorSize);
            resizedMoveCursor = ResizeCursor(moveCursor, standardCursorSize);
            resizedInspectCursor = ResizeCursor(inspectCursor, standardCursorSize);
            resizedGoThroughDoorCursor = ResizeCursor(goThroughDoorCursor, standardCursorSize);
            
            SetCursorUpperLeftHotspot(resizedStandardCursor);
        }

        #endregion

        #region Helpers

        public void SetStandardCursor()
        {
            SetCursor(resizedStandardCursor);
        }

        public void SetCursor(InteractionType interactionType)
        {
            SetCursorByType(interactionType);
            // SetCursor(resizedHandCursor);
        }

        private void SetCursorByType(InteractionType type)
        {
            switch (type)
            {
                case InteractionType.None:
                    SetCursor(resizedStandardCursor);
                    break;
                case InteractionType.Move:
                    SetCursor(resizedMoveCursor);
                    break;
                case InteractionType.Inspect:
                    SetCursor(resizedInspectCursor);
                    break;
                case InteractionType.GoThroughDoor:
                    SetCursor(resizedGoThroughDoorCursor);
                    break;
            }
        }

        private void SetCursor(Texture2D texture)
        {
            if (!scene2)
            {
                SetCursorUpperLeftHotspot(resizedStandardCursor);
                return;
            }
            
            Vector2 hotspot = new Vector2(texture.width / 4f, texture.height / 4f);
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        }
        
        private void SetCursorUpperLeftHotspot(Texture2D texture)
        {
            Vector2 hotspot = Vector2.zero;
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        }

        private Texture2D ResizeCursor(Texture2D original, int size)
        {
            if (original == null) return null;
            Texture2D resized = new Texture2D(size, size, original.format, false);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float u = x / (float)size;
                    float v = y / (float)size;
                    resized.SetPixel(x, y, original.GetPixelBilinear(u, v));
                }
            }
            resized.Apply();
            return resized;
        }

        #endregion

        public void SetCursor(bool hoveringInteractable, InteractionType interactionType)
        {
            SetCursor(hoveringInteractable ? resizedHandCursor : resizedStandardCursor);
        }
    }
    
    
    public enum InteractionType
    {
        None,
        Move,
        Inspect,
        GoThroughDoor
    }
}