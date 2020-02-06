#if UNITY_EDITOR //file must be in realtime assembly folder to be found in HDRPAsset
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditorInternal;

namespace UnityEngine.Rendering.HighDefinition
{
    [HelpURL(Documentation.baseURL + Documentation.version + Documentation.subURL + "HDRP-Asset" + Documentation.endURL)]
    public partial class HDRenderPipelineEditorResources : ScriptableObject
    {
        public static readonly string k_SettingsFromPackagePath = "Packages/com.unity.render-pipelines.high-definition/Editor/RenderPipelineResources/HDRenderPipelineEditorResources.asset";
        public static readonly string k_SettingsPath = "ProjectSettings/HDRenderPipelineEditorResources.asset";

        public GameObject defaultScene;
        
        public GameObject defaultDXRScene;
        
        public VolumeProfile defaultSkyAndFogProfile;
        
        public VolumeProfile defaultDXRSkyAndFogProfile;
        
        public VolumeProfile defaultDXRSettings;
        
        [SerializeField]
        internal DiffusionProfileSettings[] defaultDiffusionProfileSettingsList;
        
        [Reload("Editor/RenderPipelineResources/DefaultSettingsVolumeProfile.asset")]
        public VolumeProfile defaultSettingsVolumeProfile;

        [Serializable]
        public sealed class ShaderResources
        {
            public Shader terrainDetailLitShader;
            public Shader terrainDetailGrassShader;
            public Shader terrainDetailGrassBillboardShader;
        }

        [Serializable]
        public sealed class MaterialResources
        {
            // Defaults
            
            public Material defaultDiffuseMat;
            
            public Material defaultMirrorMat;
            
            public Material defaultDecalMat;
            
            public Material defaultParticleMat;
            
            public Material defaultTerrainMat;
            
            public Material GUITextureBlit2SRGB;
        }

        [Serializable]
        public sealed class TextureResources
        {
        }

        [Serializable]
        public sealed class ShaderGraphResources
        {
            public Shader autodeskInteractive;
            public Shader autodeskInteractiveMasked;
            public Shader autodeskInteractiveTransparent;
        }

        [Serializable]
        public sealed class LookDevResources
        {
            [Reload("Editor/RenderPipelineResources/DefaultLookDevProfile.asset")]
            public VolumeProfile defaultLookDevVolumeProfile;
        }

        public ShaderResources shaders;
        public MaterialResources materials;
        public TextureResources textures;
        public ShaderGraphResources shaderGraphs;
        public LookDevResources lookDev;

        private static HDRenderPipelineEditorResources _instance;
        private static SerializedObject _instanceSerializedObject;

        internal static SerializedObject getSerializedObject()
        {
            if (_instanceSerializedObject == null)
            {
                if (_instance == null)
                    _instance = GetInstance();

                _instanceSerializedObject = new SerializedObject(_instance);
            }

            return _instanceSerializedObject;
        }

        public static HDRenderPipelineEditorResources GetInstance()
        {
            if (_instance != null)
                return _instance;

            var objects = InternalEditorUtility.LoadSerializedFileAndForget(k_SettingsPath);
            if (objects.Length > 0)
            {
                _instance = objects.First() as HDRenderPipelineEditorResources;
            }
            else
            {
                // UniversalRenderPipelineEditorResources was deleted or moved, regenerate it from package.
                objects = InternalEditorUtility.LoadSerializedFileAndForget(k_SettingsFromPackagePath);
                if (objects.Length > 0)
                {
                    _instance = objects.First() as HDRenderPipelineEditorResources;
                    SaveSettings();
                }
                else
                {
                    //Was removed from package or other error, generate one from scratch ??
                }

                UpdateImportDependency(RenderPipelineAsset.DefaultMaterialImportDependency, _instance.materials.defaultDiffuseMat);
                // No SpeedTree shader in HDRP ?
                //UpdateImportDependency(RenderPipelineAsset.SpeedTree7ShaderImportDependency, _instance.shaders.defaultSpeedTree7PS);
                //UpdateImportDependency(RenderPipelineAsset.SpeedTree8ShaderImportDependency, _instance.shaders.defaultSpeedTree8PS);
                UpdateImportDependency(RenderPipelineAsset.AutodeskInteractiveMaterialImportDependency, _instance.shaderGraphs.autodeskInteractive);
                UpdateImportDependency(RenderPipelineAsset.AutodeskInteractiveMaskedMaterialImportDependency, _instance.shaderGraphs.autodeskInteractiveMasked);
                UpdateImportDependency(RenderPipelineAsset.AutodeskInteractiveTransparentMaterialImportDependency, _instance.shaderGraphs.autodeskInteractiveTransparent);
            }

            return _instance;
        }

        internal static void SaveSettings()
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { _instance }, k_SettingsPath, true);
        }

        internal static void UpdateImportDependency(string dependencyKey,Object obj)
        {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var guid, out long fileID))
            {
                var hash = Hash128.Compute(guid + "_" + fileID);
                AssetDatabaseExperimental.RegisterCustomDependency(dependencyKey, hash);
                AssetDatabase.Refresh();
            }
        }
    }

    

    static class HDRenderPipelineEditorResourcesIMGUIRegister
    {
        private static void DrawObjectPropertyWithImportDependency(SerializedProperty property, string dependencyKey)
        {
            // TODO : handle ObjectSelector events to only apply changes when the ObjectSelector closes.
            // Also add alert popup : changing this value will trigger a reimport of some assets, may take some time..
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(property);
            if (EditorGUI.EndChangeCheck())
                HDRenderPipelineEditorResources.UpdateImportDependency(dependencyKey, property.objectReferenceValue);
        }

        [SettingsProvider]
        internal static SettingsProvider CreateRenderPipelineEditorResourcesProvider()
        {
            var provider = new SettingsProvider("Project/HDRP", SettingsScope.Project)
            {
                label = "HDRP Editor Resources",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = HDRenderPipelineEditorResources.getSerializedObject();

                    DrawObjectPropertyWithImportDependency(settings.FindProperty("materials.defaultDiffuseMat"), RenderPipelineAsset.DefaultMaterialImportDependency);

                    EditorGUILayout.PropertyField(settings.FindProperty("materials.defaultParticleMat"));

                    DrawObjectPropertyWithImportDependency(settings.FindProperty("materials.autodeskInteractive"), RenderPipelineAsset.AutodeskInteractiveMaterialImportDependency);
                    DrawObjectPropertyWithImportDependency(settings.FindProperty("materials.autodeskInteractiveTransparent"), RenderPipelineAsset.AutodeskInteractiveTransparentMaterialImportDependency);
                    DrawObjectPropertyWithImportDependency(settings.FindProperty("materials.autodeskInteractiveMasked"), RenderPipelineAsset.AutodeskInteractiveMaskedMaterialImportDependency);
                    // And so on...

                    if (settings.hasModifiedProperties)
                    {
                        settings.ApplyModifiedProperties();
                        HDRenderPipelineEditorResources.SaveSettings();
                    }
                    
                }
            };

            return provider;
        }
    }
}
#endif
