#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"

#if SHADERPASS == SHADERPASS_FORWARD
    #define HAS_LIGHTLOOP
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Water/Water.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoop.hlsl"
#else
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Water/Water.hlsl"
#endif

#if SHADERPASS != SHADERPASS_DEPTH_ONLY || defined(WRITE_NORMAL_BUFFER)
    #define ATTRIBUTES_NEED_NORMAL
    #define VARYINGS_NEED_POSITION_WS
    #define ATTRIBUTES_NEED_TEXCOORD0
    #define VARYINGS_NEED_TEXCOORD0
#endif

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/VaryingMesh.hlsl"
#include "WaterLitData.hlsl"

#if SHADERPASS == SHADERPASS_SHADOWS || SHADERPASS == SHADERPASS_DEPTH_ONLY
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassDepthOnly.hlsl"
#elif SHADERPASS == SHADERPASS_FORWARD
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassForward.hlsl"
#endif
