using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Text.RegularExpressions;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.Rendering.Universal
{
    [CustomEditor(typeof(ScriptableRendererData), true)]
    [MovedFrom("UnityEditor.Rendering.LWRP")] public class ScriptableRendererDataEditor : Editor
    {
        class Styles
        {
            public static readonly GUIContent RenderFeatures =
                new GUIContent("Renderer Features",
                "Features to include in this renderer.\nTo add or remove features, use the plus and minus at the bottom of this box.");

            public static readonly GUIContent PassNameField =
                new GUIContent("Name", "Render pass name. This name is the name displayed in Frame Debugger.");

            public static GUIStyle BoldLabelSimple;

            static Styles()
            {
                BoldLabelSimple = new GUIStyle(EditorStyles.label);
                BoldLabelSimple.fontStyle = FontStyle.Bold;
            }
        }

        SavedBool[] m_Foldouts;
        private int m_IsRenaming = -1;
        private const string RenameControl = "render_feature_rename";
        SerializedProperty m_RenderPasses;

        private void OnEnable()
        {
            m_RenderPasses = serializedObject.FindProperty("m_RendererFeatures");
            CreateFoldoutBools();
        }

        public override void OnInspectorGUI()
        {
            if(m_RenderPasses == null)
                OnEnable();
            serializedObject.Update();

            if(m_RenderPasses.arraySize != m_Foldouts.Length)
                CreateFoldoutBools();

            DrawRendererFeatureList();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawRendererFeatureList()
        {
            EditorGUILayout.LabelField(Styles.RenderFeatures, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            //Draw List
            CoreEditorUtils.DrawSplitter();
            for (int i = 0; i < m_RenderPasses.arraySize; i++)
            {
                var prop = m_RenderPasses.GetArrayElementAtIndex(i);
                DrawRendererFeature(i, ref prop);
                CoreEditorUtils.DrawSplitter();
            }
            EditorGUILayout.Space();

            //Add renderer
            using (var hscope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Renderer Feature", EditorStyles.miniButton))
                {
                    AddPass();
                }
            }
        }

        private void DrawRendererFeature(int index, ref SerializedProperty prop)
        {
            var obj = prop.objectReferenceValue;
            if (obj == null)
            {
                EditorGUILayout.LabelField("Missing");
                return;
            }

            var serializedFeature = new SerializedObject(obj);
            var enabled = serializedFeature.FindProperty("enabled");

            Editor editor = CreateEditor(obj);
            var displayContent = prop.isExpanded;

            if (m_IsRenaming == index)
            {
                EditorGUI.BeginChangeCheck();
                GUI.SetNextControlName(RenameControl);
                var newName = EditorGUI.DelayedTextField(EditorGUILayout.GetControlRect(), obj.name);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(obj, "Rename Render Feature");
                    obj.name = ValidatePassName(newName);
                    RenameComponent(serializedFeature);
                    m_IsRenaming = -1;
                }
                if (GUI.GetNameOfFocusedControl() != RenameControl)
                {
                    Debug.Log("Lost focus");
                }
            }
            else
            {
                displayContent = CoreEditorUtils.DrawHeaderToggle(
                    ObjectNames.GetInspectorTitle(obj),
                    prop,
                    enabled,
                    pos => OnContextClick(pos, index)
                );
            }
            //ObjectEditor
            if (displayContent)
            {
                editor.DrawDefaultInspector();
            }

            if (serializedFeature.hasModifiedProperties)
            {
                serializedFeature.ApplyModifiedProperties();
                EditorUtility.SetDirty(obj);
            }
        }

        private void OnContextClick(Vector2 position, int id)
        {
            var menu = new GenericMenu();

            if (id == 0)
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Up"));
            else
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Up"), false, () => MoveComponent(id, -1));

            if (id == m_RenderPasses.arraySize - 1)
                menu.AddDisabledItem(EditorGUIUtility.TrTextContent("Move Down"));
            else
                menu.AddItem(EditorGUIUtility.TrTextContent("Move Down"), false, () => MoveComponent(id, 1));

            menu.AddSeparator(string.Empty);
            menu.AddItem(EditorGUIUtility.TrTextContent("Rename"), false, () => { m_IsRenaming = id; });

            menu.AddSeparator(string.Empty);
            menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, () => RemoveComponent(id));

            menu.DropDown(new Rect(position, Vector2.zero));
        }

        private void CreateFoldoutBools()
        {
            m_Foldouts = new SavedBool[m_RenderPasses.arraySize];
            for (var i = 0; i < m_RenderPasses.arraySize; i++)
            {
                var name = m_RenderPasses.serializedObject.targetObject.name;
                m_Foldouts[i] =
                    new SavedBool($"{name}.ELEMENT{i.ToString()}.PassFoldout", false);
            }
        }

        internal void RenameComponent(SerializedObject obj)
        {
            obj.Update();
            Debug.Log("Renaming");
            if (EditorUtility.IsPersistent(obj.targetObject))
            {
                EditorUtility.SetDirty(obj.targetObject);
                AssetDatabase.SaveAssets();
            }
            obj.ApplyModifiedProperties();
        }

        internal void AddComponent(object type)
        {
            serializedObject.Update();

            var component = CreateInstance((string)type);
            component.name = $"New{(string)type}";
            Undo.RegisterCreatedObjectUndo(component, "Add Renderer Feature");

            // Store this new effect as a subasset so we can reference it safely afterwards
            // Only when we're not dealing with an instantiated asset
            if (EditorUtility.IsPersistent(target))
                AssetDatabase.AddObjectToAsset(component, target);

            // Grow the list first, then add - that's how serialized lists work in Unity
            m_RenderPasses.arraySize++;
            var componentProp = m_RenderPasses.GetArrayElementAtIndex(m_RenderPasses.arraySize - 1);
            componentProp.objectReferenceValue = component;

            // Force save / refresh
            if (EditorUtility.IsPersistent(target))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void AddPass()
        {
            var menu = new GenericMenu();
            var types = TypeCache.GetTypesDerivedFrom<ScriptableRendererFeature>();
            foreach (Type type in types)
            {
                string path = GetMenuNameFromType(type);
                menu.AddItem(new GUIContent(path), false, AddComponent, type.Name);
            }
            menu.ShowAsContext();
        }

        internal void RemoveComponent(int id)
        {
            var property = m_RenderPasses.GetArrayElementAtIndex(id);
            var component = property.objectReferenceValue;
            property.objectReferenceValue = null;

            // remove the array index itself from the list
            m_RenderPasses.DeleteArrayElementAtIndex(id);
            serializedObject.ApplyModifiedProperties();

            // Destroy the setting object after ApplyModifiedProperties(). If we do it before, redo
            // actions will be in the wrong order and the reference to the setting object in the
            // list will be lost.
            Undo.SetCurrentGroupName($"Delete {component.name}");
            Undo.DestroyObjectImmediate(component);

            // Force save / refresh
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        internal void MoveComponent(int id, int offset)
        {
            Undo.SetCurrentGroupName($"Move Render Feature");
            serializedObject.Update();
            m_RenderPasses.MoveArrayElement(id, id + offset);
            serializedObject.ApplyModifiedProperties();
            // Force save / refresh
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        private string GetMenuNameFromType(Type type)
        {
            var path = type.Name;
            if (type.Namespace != null)
            {
                if (type.Namespace.Contains("Experimental"))
                    path += " (Experimental)";
            }

            // Inserts blank space in between camel case strings
            return Regex.Replace(Regex.Replace(path, "([a-z])([A-Z])", "$1 $2", RegexOptions.Compiled),
                "([A-Z])([A-Z][a-z])", "$1 $2", RegexOptions.Compiled);
        }

        private string ValidatePassName(string name)
        {
            name = Regex.Replace(name, @"[^a-zA-Z0-9 ]", "");
            return name;
        }
    }
}
