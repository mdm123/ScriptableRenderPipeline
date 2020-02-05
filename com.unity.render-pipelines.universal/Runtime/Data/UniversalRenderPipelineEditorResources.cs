using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditorInternal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.LWRP
{
    [Obsolete("LWRP -> Universal (UnityUpgradable) -> UnityEngine.Rendering.Universal.UniversalRenderPipelineEditorResources", true)]
    public class LightweightRenderPipelineEditorResources
    {
    }
}

namespace UnityEngine.Rendering.Universal
{
    [MovedFrom("UnityEngine.Rendering.LWRP")] public class UniversalRenderPipelineEditorResources : ScriptableObject
    {
        public static readonly string k_SettingsFromPackagePath = "Packages/com.unity.render-pipelines.universal/Runtime/Data/UniversalRenderPipelineEditorResources.asset";
        public static readonly string k_SettingsPath = "ProjectSettings/UniversalRenderPipelineEditorResources.asset";

        [Serializable]
        public sealed class ShaderResources
        {
            public Shader autodeskInteractivePS;

            public Shader autodeskInteractiveTransparentPS;

            public Shader autodeskInteractiveMaskedPS;

            public Shader terrainDetailLitPS;

            public Shader terrainDetailGrassPS;

            public Shader terrainDetailGrassBillboardPS;

            public Shader defaultSpeedTree7PS;

            public Shader defaultSpeedTree8PS;
        }

        [Serializable]
        public sealed class MaterialResources
        {
            public Material lit;

            public Material particleLit;

            public Material terrainLit;
        }

        public ShaderResources shaders;
        public MaterialResources materials;


        private static UniversalRenderPipelineEditorResources Instance;
        private static SerializedObject instanceSerializedObject;

        internal static SerializedObject getSerializedObject()
        {
            if (instanceSerializedObject == null)
            {
                if (Instance == null)
                    Instance = GetInstance();

                instanceSerializedObject = new SerializedObject(Instance);
            }

            return instanceSerializedObject;
        }

        internal static UniversalRenderPipelineEditorResources GetInstance()
        {
            if (Instance != null)
                return Instance;

            var objects = InternalEditorUtility.LoadSerializedFileAndForget(k_SettingsPath);
            if (objects.Length>0)
            {
                Instance = objects.First() as UniversalRenderPipelineEditorResources;
            }
            else
            {
                // UniversalRenderPipelineEditorResources was deleted or moved, regenerate it from package.
                objects = InternalEditorUtility.LoadSerializedFileAndForget(k_SettingsFromPackagePath);
                if (objects.Length > 0)
                {
                    Instance = objects.First() as UniversalRenderPipelineEditorResources;
                    SaveSettings();
                }
                else
                {
                    //Was removed from package or other error, generate one from scratch ??
                }

                // custom dependencies should also be updated here.
            }
            
            return Instance;
        }

        public static void SaveSettings()
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { Instance }, k_SettingsPath, true);
        }
    }

    static class UniversalRenderPipelineEditorResourcesIMGUIRegister
    {
        private static readonly string defaultMaterialAssetImportDependency = "DefaultMaterialAssetImportDependency";

        [SettingsProvider]
        public static SettingsProvider CreateRenderPipelineEditorResourcesProvider()
        {
            var provider = new SettingsProvider("Project/Universal Render Pipeline", SettingsScope.Project)
            {
                label = "Universal Render Pipeline Editor Resources",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                
                    var settings = UniversalRenderPipelineEditorResources.getSerializedObject();
                    EditorGUI.BeginChangeCheck();
                    var litMaterialProp = settings.FindProperty("materials.lit");
                    EditorGUILayout.PropertyField(litMaterialProp);
                    if (EditorGUI.EndChangeCheck())
                    {
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(litMaterialProp.objectReferenceValue,out var guid,out long fileID);
                        var hash = Hash128.Compute(guid + "_" + fileID);
                        AssetDatabaseExperimental.RegisterCustomDependency(defaultMaterialAssetImportDependency, hash);
                        AssetDatabase.Refresh();
                    }

                    EditorGUILayout.PropertyField(settings.FindProperty("materials.particleLit"));
                    EditorGUILayout.PropertyField(settings.FindProperty("shaders.defaultSpeedTree7PS"));
                    EditorGUILayout.PropertyField(settings.FindProperty("shaders.defaultSpeedTree8PS"));
                    EditorGUILayout.PropertyField(settings.FindProperty("shaders.autodeskInteractivePS"));
                    EditorGUILayout.PropertyField(settings.FindProperty("shaders.autodeskInteractiveTransparentPS"));
                    EditorGUILayout.PropertyField(settings.FindProperty("shaders.autodeskInteractiveMaskedPS"));

                    if (settings.hasModifiedProperties)
                    {
                        settings.ApplyModifiedProperties();
                        UniversalRenderPipelineEditorResources.SaveSettings();
                    }
                
                }
            };

            return provider;
        }
    }
}

