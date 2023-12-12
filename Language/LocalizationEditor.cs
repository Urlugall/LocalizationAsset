using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Language
{
    [CustomEditor(typeof(Localization))]
    public class LocalizationEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(10);
            
            var localization = (Localization)target;
            if(GUILayout.Button("Create Localization File"))
            {
                localization.CheckLocalizationFile();
                AssetDatabase.Refresh();
            }
        }
    }
}

#endif