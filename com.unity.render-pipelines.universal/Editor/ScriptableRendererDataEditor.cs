using System;
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

        private bool m_MissingRenderers;
        private SerializedProperty m_RenderPasses;
        private SerializedProperty m_RenderPassMap;

        private void OnValidate()
        {
            throw new NotImplementedException();
        }

        private void OnEnable()
        {
            m_RenderPasses = serializedObject.FindProperty("m_RendererFeatures");
            m_RenderPassMap = serializedObject.FindProperty("m_RendererFeatureMap");
        }

        public override void OnInspectorGUI()
        {
            if(m_RenderPasses == null)
                OnEnable();
            serializedObject.Update();
            DrawRendererFeatureList();

            if(serializedObject.hasModifiedProperties)
                serializedObject.ApplyModifiedProperties();
        }

        private void DrawRendererFeatureList()
        {
            EditorGUILayout.LabelField(Styles.RenderFeatures, EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (m_RenderPasses.arraySize == 0)
            {
                EditorGUILayout.HelpBox("No Renderer Features added", MessageType.Info);
            }
            else
            {
                //Draw List
                CoreEditorUtils.DrawSplitter();
                for (int i = 0; i < m_RenderPasses.arraySize; i++)
                {
                    var prop = m_RenderPasses.GetArrayElementAtIndex(i);
                    DrawRendererFeature(i, ref prop);
                    CoreEditorUtils.DrawSplitter();
                }
            }
            EditorGUILayout.Space();

            //Add renderer
            if (GUILayout.Button("Add Renderer Feature", EditorStyles.miniButton))
            {
                AddPassMenu();
            }


            //Fix Renderers
            if (m_MissingRenderers)
            {
                EditorGUILayout.HelpBox("You have missing RendererFeature references, we can attempt to fix these or you can choose to do it manually via the Debug Inspector.", MessageType.Error);
                if (!GUILayout.Button("Fix Renderer Features", EditorStyles.miniButton)) return;
                var data = target as ScriptableRendererData;
                m_MissingRenderers = !data.ValidateRendererFeatures();
            }
        }

        private void DrawRendererFeature(int index, ref SerializedProperty prop)
        {
            var obj = prop.objectReferenceValue;
            if (obj == null)
            {
                EditorGUILayout.LabelField("Missing Render Feature");
                m_MissingRenderers = true;
                return;
            }

            var serializedFeature = new SerializedObject(obj);
            var enabled = serializedFeature.FindProperty("enabled");
            var title = ObjectNames.GetInspectorTitle(obj);


            var editor = CreateEditor(obj);
            var displayContent = CoreEditorUtils.DrawHeaderToggle(
                title,
                prop,
                enabled,
                pos => OnContextClick(pos, serializedFeature, index)
            );
            // ObjectEditor
            if (displayContent)
            {
                EditorGUILayout.DelayedTextField(serializedFeature.FindProperty("m_Name"));
                editor.OnInspectorGUI();
                //editor.DrawDefaultInspector();
            }

            //Save the changed data
            if (!serializedFeature.hasModifiedProperties) return;

            serializedFeature.ApplyModifiedProperties();
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
        }

        private void OnContextClick(Vector2 position, SerializedObject obj, int id)
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
            menu.AddItem(EditorGUIUtility.TrTextContent("Remove"), false, () => RemoveComponent(id));

            menu.DropDown(new Rect(position, Vector2.zero));
        }

        private void AddPassMenu()
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

        private void AddComponent(object type)
        {
            serializedObject.Update();

            var component = CreateInstance((string)type);
            component.name = $"New{(string)type}";
            Undo.RegisterCreatedObjectUndo(component, "Add Renderer Feature");

            // Store this new effect as a subasset so we can reference it safely afterwards
            // Only when we're not dealing with an instantiated asset
            if (EditorUtility.IsPersistent(target))
                AssetDatabase.AddObjectToAsset(component, target);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(component, out var guid, out long localId);
            Debug.Log($"new feature | guid={guid} | localID={localId}");

            // Grow the list first, then add - that's how serialized lists work in Unity
            m_RenderPasses.arraySize++;
            var componentProp = m_RenderPasses.GetArrayElementAtIndex(m_RenderPasses.arraySize - 1);
            componentProp.objectReferenceValue = component;

            // Update GUID Map
            m_RenderPassMap.arraySize++;
            var guidProp = m_RenderPassMap.GetArrayElementAtIndex(m_RenderPassMap.arraySize - 1);
            guidProp.longValue = localId;

            serializedObject.ApplyModifiedProperties();

            // Force save / refresh
            if (EditorUtility.IsPersistent(target))
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveComponent(int id)
        {
            var property = m_RenderPasses.GetArrayElementAtIndex(id);
            var component = property.objectReferenceValue;
            property.objectReferenceValue = null;

            // remove the array index itself from the list
            m_RenderPasses.DeleteArrayElementAtIndex(id);
            m_RenderPassMap.DeleteArrayElementAtIndex(id);
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

        private void MoveComponent(int id, int offset)
        {
            Undo.SetCurrentGroupName($"Move Render Feature");
            serializedObject.Update();
            m_RenderPasses.MoveArrayElement(id, id + offset);
            m_RenderPassMap.MoveArrayElement(id, id + offset);
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

        private string ValidateName(string name)
        {
            name = Regex.Replace(name, @"[^a-zA-Z0-9 ]", "");
            return name;
        }
    }
}
