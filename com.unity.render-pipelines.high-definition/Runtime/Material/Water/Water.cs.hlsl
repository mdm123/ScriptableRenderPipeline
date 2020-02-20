//
// This file was automatically generated. Please don't edit by hand.
//

#ifndef WATER_CS_HLSL
#define WATER_CS_HLSL
//
// UnityEngine.Rendering.HighDefinition.Water+MaterialFeatureFlags:  static fields
//
#define MATERIALFEATUREFLAGS_WATER_CINEMATIC (1)
#define MATERIALFEATUREFLAGS_WATER_SUBSURFACE_SCATTERING (2)

//
// UnityEngine.Rendering.HighDefinition.Water+SurfaceData:  static fields
//
#define DEBUGVIEW_WATER_SURFACEDATA_MATERIAL_FEATURES (1500)
#define DEBUGVIEW_WATER_SURFACEDATA_BASE_COLOR (1501)
#define DEBUGVIEW_WATER_SURFACEDATA_NORMAL (1502)
#define DEBUGVIEW_WATER_SURFACEDATA_NORMAL_VIEW_SPACE (1503)
#define DEBUGVIEW_WATER_SURFACEDATA_IRIS_NORMAL (1504)
#define DEBUGVIEW_WATER_SURFACEDATA_IRIS_NORMAL_VIEW_SPACE (1505)
#define DEBUGVIEW_WATER_SURFACEDATA_GEOMETRIC_NORMAL (1506)
#define DEBUGVIEW_WATER_SURFACEDATA_GEOMETRIC_NORMAL_VIEW_SPACE (1507)
#define DEBUGVIEW_WATER_SURFACEDATA_SMOOTHNESS (1508)
#define DEBUGVIEW_WATER_SURFACEDATA_AMBIENT_OCCLUSION (1509)
#define DEBUGVIEW_WATER_SURFACEDATA_SPECULAR_OCCLUSION (1510)
#define DEBUGVIEW_WATER_SURFACEDATA_IOR (1511)
#define DEBUGVIEW_WATER_SURFACEDATA_MASK (1512)
#define DEBUGVIEW_WATER_SURFACEDATA_DIFFUSION_PROFILE_HASH (1513)
#define DEBUGVIEW_WATER_SURFACEDATA_SUBSURFACE_MASK (1514)

//
// UnityEngine.Rendering.HighDefinition.Water+BSDFData:  static fields
//
#define DEBUGVIEW_WATER_BSDFDATA_MATERIAL_FEATURES (1550)
#define DEBUGVIEW_WATER_BSDFDATA_DIFFUSE_COLOR (1551)
#define DEBUGVIEW_WATER_BSDFDATA_FRESNEL0 (1552)
#define DEBUGVIEW_WATER_BSDFDATA_IOR (1553)
#define DEBUGVIEW_WATER_BSDFDATA_AMBIENT_OCCLUSION (1554)
#define DEBUGVIEW_WATER_BSDFDATA_SPECULAR_OCCLUSION (1555)
#define DEBUGVIEW_WATER_BSDFDATA_NORMAL_WS (1556)
#define DEBUGVIEW_WATER_BSDFDATA_NORMAL_VIEW_SPACE (1557)
#define DEBUGVIEW_WATER_BSDFDATA_DIFFUSE_NORMAL_WS (1558)
#define DEBUGVIEW_WATER_BSDFDATA_DIFFUSE_NORMAL_VIEW_SPACE (1559)
#define DEBUGVIEW_WATER_BSDFDATA_GEOMETRIC_NORMAL (1560)
#define DEBUGVIEW_WATER_BSDFDATA_GEOMETRIC_NORMAL_VIEW_SPACE (1561)
#define DEBUGVIEW_WATER_BSDFDATA_PERCEPTUAL_ROUGHNESS (1562)
#define DEBUGVIEW_WATER_BSDFDATA_MASK (1563)
#define DEBUGVIEW_WATER_BSDFDATA_DIFFUSION_PROFILE_INDEX (1564)
#define DEBUGVIEW_WATER_BSDFDATA_SUBSURFACE_MASK (1565)
#define DEBUGVIEW_WATER_BSDFDATA_ROUGHNESS (1566)

// Generated from UnityEngine.Rendering.HighDefinition.Water+SurfaceData
// PackingRules = Exact
struct SurfaceData
{
    uint materialFeatures;
    float3 baseColor;
    float3 normalWS;
    float3 irisNormalWS;
    float3 geomNormalWS;
    float perceptualSmoothness;
    float ambientOcclusion;
    float specularOcclusion;
    float IOR;
    float2 mask;
    uint diffusionProfileHash;
    float subsurfaceMask;
};

// Generated from UnityEngine.Rendering.HighDefinition.Water+BSDFData
// PackingRules = Exact
struct BSDFData
{
    uint materialFeatures;
    float3 diffuseColor;
    float3 fresnel0;
    float IOR;
    float ambientOcclusion;
    float specularOcclusion;
    float3 normalWS;
    float3 diffuseNormalWS;
    float3 geomNormalWS;
    float perceptualRoughness;
    float2 mask;
    uint diffusionProfileIndex;
    float subsurfaceMask;
    float roughness;
};

//
// Debug functions
//
void GetGeneratedSurfaceDataDebug(uint paramId, SurfaceData surfacedata, inout float3 result, inout bool needLinearToSRGB)
{
    switch (paramId)
    {
        case DEBUGVIEW_WATER_SURFACEDATA_MATERIAL_FEATURES:
            result = GetIndexColor(surfacedata.materialFeatures);
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_BASE_COLOR:
            result = surfacedata.baseColor;
            needLinearToSRGB = true;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_NORMAL:
            result = surfacedata.normalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_NORMAL_VIEW_SPACE:
            result = surfacedata.normalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_IRIS_NORMAL:
            result = surfacedata.irisNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_IRIS_NORMAL_VIEW_SPACE:
            result = surfacedata.irisNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_GEOMETRIC_NORMAL:
            result = surfacedata.geomNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_GEOMETRIC_NORMAL_VIEW_SPACE:
            result = surfacedata.geomNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_SMOOTHNESS:
            result = surfacedata.perceptualSmoothness.xxx;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_AMBIENT_OCCLUSION:
            result = surfacedata.ambientOcclusion.xxx;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_SPECULAR_OCCLUSION:
            result = surfacedata.specularOcclusion.xxx;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_IOR:
            result = surfacedata.IOR.xxx;
            needLinearToSRGB = true;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_MASK:
            result = float3(surfacedata.mask, 0.0);
            needLinearToSRGB = true;
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_DIFFUSION_PROFILE_HASH:
            result = GetIndexColor(surfacedata.diffusionProfileHash);
            break;
        case DEBUGVIEW_WATER_SURFACEDATA_SUBSURFACE_MASK:
            result = surfacedata.subsurfaceMask.xxx;
            break;
    }
}

//
// Debug functions
//
void GetGeneratedBSDFDataDebug(uint paramId, BSDFData bsdfdata, inout float3 result, inout bool needLinearToSRGB)
{
    switch (paramId)
    {
        case DEBUGVIEW_WATER_BSDFDATA_MATERIAL_FEATURES:
            result = GetIndexColor(bsdfdata.materialFeatures);
            break;
        case DEBUGVIEW_WATER_BSDFDATA_DIFFUSE_COLOR:
            result = bsdfdata.diffuseColor;
            needLinearToSRGB = true;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_FRESNEL0:
            result = bsdfdata.fresnel0;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_IOR:
            result = bsdfdata.IOR.xxx;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_AMBIENT_OCCLUSION:
            result = bsdfdata.ambientOcclusion.xxx;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_SPECULAR_OCCLUSION:
            result = bsdfdata.specularOcclusion.xxx;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_NORMAL_WS:
            result = bsdfdata.normalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_NORMAL_VIEW_SPACE:
            result = bsdfdata.normalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_DIFFUSE_NORMAL_WS:
            result = bsdfdata.diffuseNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_DIFFUSE_NORMAL_VIEW_SPACE:
            result = bsdfdata.diffuseNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_GEOMETRIC_NORMAL:
            result = bsdfdata.geomNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_GEOMETRIC_NORMAL_VIEW_SPACE:
            result = bsdfdata.geomNormalWS * 0.5 + 0.5;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_PERCEPTUAL_ROUGHNESS:
            result = bsdfdata.perceptualRoughness.xxx;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_MASK:
            result = float3(bsdfdata.mask, 0.0);
            break;
        case DEBUGVIEW_WATER_BSDFDATA_DIFFUSION_PROFILE_INDEX:
            result = GetIndexColor(bsdfdata.diffusionProfileIndex);
            break;
        case DEBUGVIEW_WATER_BSDFDATA_SUBSURFACE_MASK:
            result = bsdfdata.subsurfaceMask.xxx;
            break;
        case DEBUGVIEW_WATER_BSDFDATA_ROUGHNESS:
            result = bsdfdata.roughness.xxx;
            break;
    }
}


#endif
