using UnityEditor.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{

    class SerializedHDRenderPipelineAsset
    {
        public SerializedObject serializedObject;

        public SerializedProperty renderPipelineResources;
        public SerializedProperty defaultMaterialQualityLevel;
        public SerializedProperty availableMaterialQualityLevels;
        public SerializedProperty renderPipelineRayTracingResources;
        public SerializedProperty diffusionProfileSettingsList;
        public SerializedProperty allowShaderVariantStripping;
        public SerializedProperty enableSRPBatcher;
        public SerializedProperty shaderVariantLogLevel;
        public SerializedRenderPipelineSettings renderPipelineSettings;
        public SerializedFrameSettings defaultFrameSettings;
        public SerializedFrameSettings defaultBakedOrCustomReflectionFrameSettings;
        public SerializedFrameSettings defaultRealtimeReflectionFrameSettings;


        public SerializedHDRenderPipelineAsset(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;

            defaultMaterialQualityLevel = serializedObject.FindProperty("m_DefaultMaterialQualityLevel");
            availableMaterialQualityLevels = serializedObject.Find((HDRenderPipelineAsset s) => s.availableMaterialQualityLevels);

            renderPipelineResources = serializedObject.FindProperty("m_RenderPipelineResources");
            renderPipelineRayTracingResources = serializedObject.FindProperty("m_RenderPipelineRayTracingResources");
            diffusionProfileSettingsList = serializedObject.Find((HDRenderPipelineAsset s) => s.diffusionProfileSettingsList);
            allowShaderVariantStripping = serializedObject.Find((HDRenderPipelineAsset s) => s.allowShaderVariantStripping);
            enableSRPBatcher = serializedObject.Find((HDRenderPipelineAsset s) => s.enableSRPBatcher);
            shaderVariantLogLevel = serializedObject.Find((HDRenderPipelineAsset s) => s.shaderVariantLogLevel);

            renderPipelineSettings = new SerializedRenderPipelineSettings(serializedObject.FindProperty("m_RenderPipelineSettings"));
            defaultFrameSettings = new SerializedFrameSettings(serializedObject.FindProperty("m_RenderingPathDefaultCameraFrameSettings"), null); //no overrides in HDRPAsset
            defaultBakedOrCustomReflectionFrameSettings = new SerializedFrameSettings(serializedObject.FindProperty("m_RenderingPathDefaultBakedOrCustomReflectionFrameSettings"), null); //no overrides in HDRPAsset
            defaultRealtimeReflectionFrameSettings = new SerializedFrameSettings(serializedObject.FindProperty("m_RenderingPathDefaultRealtimeReflectionFrameSettings"), null); //no overrides in HDRPAsset
        }

        public void Update()
        {
            serializedObject.Update();
        }

        public void Apply()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
