# Upgrading to version 7.2.0 of the Universal Render Pipeline

On this page, you will find information about upgrading from an older version of the Universal Render Pipeline (URP) to the current version.

## Building your Project for consoles

To build a Project for the **PlayStation 4** or **Xbox One**, you need to install an additional package for each platform you want to support.

For more information, see the documentation on [Building for Consoles](Building-For-Consoles.md).

## Require Depth Texture
In previous versions of URP, if post-processing was enabled it would cause the pipeilne to always require depth. We have improved the post-processing integration to only require depth from the pipeline when Depth of Field, Motion Blur or SMAA effects are enabled. This improves performance in many cases.

Because Cameras that use post-processing no longer require depth by default, you must now manually indicate that Cameras require depth if you are using it for other effects, such as soft particles.

To make all Cameras require depth, enable the the `Depth Texture` option in the [Pipeline Asset](universalrp-asset.md). To make an individual Camera require depth, set `Depth Texture` option to `On` in the [Camera Inspector](camera-component-reference.md).

## Sampling shadows from the Main Light
In previous versions of URP, if shadow cascades were enabled for the main Light, shadows would be resolved in a screen space pass. The pipeline now always resolves shadows while rendering opaques or transparent objects. This allows for consistency and solved many issues regarding shadows.

Four new defines have been added to the URP shaders:

MAIN_LIGHT_CALCULATE_SHADOWS
Defined when shadows on main light are enabled and shadows enabled in the material

ADDITIONAL_LIGHT_CALCULATE_SHADOWS
Defined when shadows on additional lights are enabled and shadows enabled in the material

REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
Defined when shadows on main light are enabled, shadows enabled in the material and cascades set to none. Used to determine whether shadow coordinates need to be passed from the vertex shader to fragment shader.

REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
Defined when shadows there are additional lights or shadow cascades set to two or four. 
Used to determine whether the world space position needs to be passed from the vertex shader to fragment shader.

These defines then get used in the following places.

In Varyings struct that gets passed from vertex to fragment:
```
#if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    float3 positionWS               : TEXCOORD2;
#endif

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord              : TEXCOORD7;
#endif
```

In vertex shaders:
```
#if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    output.positionWS = vertexInput.positionWS;
#endif

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    output.shadowCoord = GetShadowCoord(vertexInput);
#endif
```

In fragment shaders
```
#if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    inputData.positionWS = input.positionWS;
#endif

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    inputData.shadowCoord = input.shadowCoord;
#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
#else
    inputData.shadowCoord = float4(0, 0, 0, 0);
#endif
```


If have custom hlsl shaders and sample `_ScreenSpaceShadowmapTexture` texture, you must upgrade them to sample shadows by using the `GetMainLight` function instead. 

For example:

```
float4 shadowCoord = TransformWorldToShadowCoord(positionWorldSpace);
Light mainLight = GetMainLight(inputData.shadowCoord);

// now you can use shadow to apply realtime occlusion
half shadow = mainLight.shadowAttenuation;
```

You must also define the following in your .shader file to make sure your custom shader can receive shadows correctly:

```
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
```

## Transparent receiving shadows
Transparent objects can now receive shadows when using shadow cascades. You can also optionally disable shadow receiving for transparent to improve performance. To do so, disable `Transparent Receive Shadows` in the Forward Renderer asset.