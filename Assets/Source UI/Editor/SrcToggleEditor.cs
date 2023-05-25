using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(SrcToggle))]
    [CanEditMultipleObjects]
    public class SrcToggleEditor : ToggleEditor
    {
        SerializedProperty m_useCheckBox;
        SerializedProperty m_targetImage;
        SerializedProperty m_normalSprite;
        SerializedProperty m_selectedSprite;


        protected override void OnEnable()
        {
            m_useCheckBox = serializedObject.FindProperty("m_useCheckBox");
            m_targetImage = serializedObject.FindProperty("targetImage");
            m_normalSprite = serializedObject.FindProperty("normalSprite");
            m_selectedSprite = serializedObject.FindProperty("selectedSprite");

            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            SrcToggle toggle = serializedObject.targetObject as SrcToggle;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_useCheckBox, new GUIContent("Use Check Box"));
            EditorGUILayout.PropertyField(m_targetImage, new GUIContent("Target Image"));
            EditorGUILayout.PropertyField(m_normalSprite, new GUIContent("Normal Sprite"));
            EditorGUILayout.PropertyField(m_selectedSprite, new GUIContent("Selected Sprite"));
            if (EditorGUI.EndChangeCheck())
            {
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(toggle.gameObject.scene);

                toggle.UseCheckBox = m_useCheckBox.boolValue;
                toggle.targetImage = m_targetImage.objectReferenceValue as Image;
                toggle.normalSprite = m_normalSprite.objectReferenceValue as Sprite;
                toggle.selectedSprite = m_selectedSprite.objectReferenceValue as Sprite;
            }

            EditorGUILayout.Space(20);

            base.OnInspectorGUI();
        }
    }
}
