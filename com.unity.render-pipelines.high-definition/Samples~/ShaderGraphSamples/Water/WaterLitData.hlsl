#define HAVE_MESH_MODIFICATION
#define HAVE_VERTEX_MODIFICATION

float  _AlbedoAffectEmissive;
float  _EmissiveExposureWeight;
float3 _EmissiveColor;

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Water/Water.cs.hlsl"

AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters)
{
    return input;
}

void ApplyVertexModification(AttributesMesh input, float3 normalWS, inout float3 positionRWS, float3 timeParameters)
{
    positionRWS = input.positionOS;
    positionRWS -= _WorldSpaceCameraPos;
}

float3 GetDebugNormals(float3 worldPos)
{
    float3 worldPosDdx = normalize(ddx(worldPos));
    float3 worldPosDdy = normalize(ddy(worldPos));
    return normalize(cross(worldPosDdx, worldPosDdy));
}

void GetSurfaceAndBuiltinData(inout FragInputs input, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
{
    float3 surfaceNormals = GetDebugNormals(posInput.positionWS);

    surfaceData.materialFeatures = 0;
    surfaceData.baseColor = float3(10,0,0);
    surfaceData.normalWS = surfaceNormals;
    surfaceData.irisNormalWS = surfaceNormals;
    surfaceData.geomNormalWS = surfaceNormals;
    surfaceData.perceptualSmoothness = 0.2f;
    surfaceData.ambientOcclusion = 0.0f;
    surfaceData.specularOcclusion = 1.0f;
    surfaceData.IOR = 0.f;
    surfaceData.mask = float2(1, 1);
    surfaceData.diffusionProfileHash = 0;
    surfaceData.subsurfaceMask = 1.0f;

    builtinData.opacity = 1;
    builtinData.bakeDiffuseLighting = float3(0,0,0);
    builtinData.backBakeDiffuseLighting = float3(0,0,0);
    builtinData.shadowMask0 = 0.0f;
    builtinData.shadowMask1 = 0.0f;
    builtinData.shadowMask2 = 0.0f;
    builtinData.shadowMask3 = 0.0f;
    builtinData.emissiveColor = float3(0,0,0);
    builtinData.motionVector = float2(0,0);
    builtinData.distortion = float2(0,0);
    builtinData.distortionBlur = 0;
    builtinData.renderingLayers = 0;
    builtinData.depthOffset = 0.0f;
}
