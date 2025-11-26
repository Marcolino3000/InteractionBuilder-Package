using UnityEngine;

namespace Runtime.Scripts.Utility
{
    [CreateAssetMenu(menuName = "Utility/GUI Style")]
    public class GuiStyleSettings : ScriptableObject
    {
        public int fontSize;

        public static int FontSize;

        public static GUIStyle GetStyle()
        {
            var style = GUIStyle.none;
            style.fontSize = FontSize;

            
            return style;
        }

        public static GUISkin GetSkin()
        {
            var skin = GUI.skin;
            skin.label.fontSize = FontSize; 
            skin.button.fontSize = FontSize;
            return skin;
        }
        
        private void OnValidate()
        {
            FontSize = fontSize;
        }
    }
}