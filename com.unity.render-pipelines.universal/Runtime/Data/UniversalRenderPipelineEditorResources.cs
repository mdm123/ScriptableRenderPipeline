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

                //UpdateImportDependency(RenderPipelineAsset.DefaultMaterialImportDependency, Instance.materials.lit);
                //UpdateImportDependency(RenderPipelineAsset.SpeedTree7ShaderImportDependency, Instance.shaders.defaultSpeedTree7PS);
                //UpdateImportDependency(RenderPipelineAsset.SpeedTree8ShaderImportDependency, Instance.shaders.defaultSpeedTree8PS);
                //UpdateImportDependency(RenderPipelineAsset.AutodeskInteractiveMaterialImportDependency, Instance.shaders.autodeskInteractivePS);
                //UpdateImportDependency(RenderPipelineAsset.AutodeskInteractiveMaskedMaterialImportDependency, Instance.shaders.autodeskInteractiveMaskedPS);
                //UpdateImportDependency(RenderPipelineAsset.AutodeskInteractiveTransparentMaterialImportDependency, Instance.shaders.autodeskInteractiveTransparentPS);

                UpdateImportDependency("DefaultMaterialImportDependency", Instance.materials.lit);
                UpdateImportDependency("SpeedTree7ShaderImportDependency", Instance.shaders.defaultSpeedTree7PS);
                UpdateImportDependency("SpeedTree8ShaderImportDependency", Instance.shaders.defaultSpeedTree8PS);
                UpdateImportDependency("AutodeskInteractiveMaterialImportDependency", Instance.shaders.autodeskInteractivePS);
                UpdateImportDependency("AutodeskInteractiveMaskedMaterialImportDependency", Instance.shaders.autodeskInteractiveMaskedPS);
                UpdateImportDependency("AutodeskInteractiveTransparentMaterialImportDependency", Instance.shaders.autodeskInteractiveTransparentPS);
            }
            
            return Instance;
        }

        internal static void SaveSettings()
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { Instance }, k_SettingsPath, true);
        }

        internal static void UpdateImportDependency(string dependencyKey, Object obj)
        {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out long fileID))
            {
                var hash = Hash128.Compute(guid + "_" + fileID);
                AssetDatabaseExperimental.RegisterCustomDependency(dependencyKey, hash);
                AssetDatabase.Refresh();
            }
        }
    }

    static class UniversalRenderPipelineEditorResourcesIMGUIRegister
    {
        private static void DrawObjectPropertyWithImportDependency(SerializedProperty property, string dependencyKey)
        {
            // TODO : handle ObjectSelector events to only apply changes when the ObjectSelector closes.
            // Also add alert popup : changing this value will trigger a reimport of some assets, may take some time..
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property);
            if (EditorGUI.EndChangeCheck())
                UniversalRenderPipelineEditorResources.UpdateImportDependency(dependencyKey, property.objectReferenceValue);
        }


        [SettingsProvider]
        public static SettingsProvider CreateRenderPipelineEditorResourcesProvider()
        {
            var provider = new SettingsProvider("Project/Universal Render Pipeline", SettingsScope.Project)
            {
                label = "URP Editor Resources",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = UniversalRenderPipelineEditorResources.getSerializedObject();

                    DrawObjectPropertyWithImportDependency(settings.FindProperty("materials.lit"), RenderPipelineAsset.DefaultMaterialImportDependency);
                    EditorGUILayout.PropertyField(settings.FindProperty("materials.particleLit"));

                    DrawObjectPropertyWithImportDependency(settings.FindProperty("shaders.defaultSpeedTree7PS"), RenderPipelineAsset.SpeedTree7ShaderImportDependency);
                    DrawObjectPropertyWithImportDependency(settings.FindProperty("shaders.defaultSpeedTree8PS"), RenderPipelineAsset.SpeedTree8ShaderImportDependency);

                    DrawObjectPropertyWithImportDependency(settings.FindProperty("shaders.autodeskInteractivePS"), RenderPipelineAsset.AutodeskInteractiveMaterialImportDependency);
                    DrawObjectPropertyWithImportDependency(settings.FindProperty("shaders.autodeskInteractiveTransparentPS"), RenderPipelineAsset.AutodeskInteractiveTransparentMaterialImportDependency);
                    DrawObjectPropertyWithImportDependency(settings.FindProperty("shaders.autodeskInteractiveMaskedPS"), RenderPipelineAsset.AutodeskInteractiveMaskedMaterialImportDependency);

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

