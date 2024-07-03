#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Playables;

namespace FPCSwimmer
{

    [CustomEditor(typeof(FPCSwimmer))]
    public class FPCSwimmerEditor : Editor
    {
        private Texture2D m_Logo;

        void OnEnable()
        {
            m_Logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/FPCSwimmer/Core/Resources/BackgroundLogoFPCSwimmer.png", typeof(Texture2D));
        }

        public override void OnInspectorGUI()
        {
            var style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset()
                {
                    left = -20,
                    right = -10,
                    top = 0,
                    bottom = 20
                }
            };

            var rect = GUILayoutUtility.GetAspectRect(2);
            GUI.DrawTexture(rect, m_Logo, ScaleMode.ScaleToFit, true, 2);
            base.OnInspectorGUI();
        }
    }

} // namespace

#endif