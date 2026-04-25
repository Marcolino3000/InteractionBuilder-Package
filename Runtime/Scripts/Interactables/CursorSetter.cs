using Runtime.Scripts.Core;
using UnityEngine;

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

        #region Setup

        public void Initialize()
        {
            resizedStandardCursor = ResizeCursor(standardCursor, standardCursorSize);
            resizedHandCursor = ResizeCursor(handCursor, interactionCursorSize);
            SetStandardCursor();
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
                    SetCursor(moveCursor);
                    break;
                case InteractionType.Inspect:
                    SetCursor(inspectCursor);
                    break;
                case InteractionType.GoThroughDoor:
                    SetCursor(goThroughDoorCursor);
                    break;
            }
        }

        private void SetCursor(Texture2D texture)
        {
            Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
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