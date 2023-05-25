using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace CustomEditors
{
    [CustomEditor(typeof(SizeTargetToText), true)]
    [CanEditMultipleObjects]
    public class SizeTargetToTextEditor : ContentSizeFitterEditor
    {
        SerializedProperty m_target;

        protected override void OnEnable()
        {
            m_target = serializedObject.FindProperty("target");
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(m_target, true);
            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
