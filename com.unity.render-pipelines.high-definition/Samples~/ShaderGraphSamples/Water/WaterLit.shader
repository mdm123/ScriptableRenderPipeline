Shader "HDRP/Environment/WaterLit"
{
    Properties
    {
        // Stencil state
        // Forward
        [HideInInspector] _StencilRef("_StencilRef", Int) = 2 // StencilLightingUsage.RegularLighting
        [HideInInspector] _StencilWriteMask("_StencilWriteMask", Int) = 3 // StencilMask.Lighting

        // Blending state
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _CullMode("__cullmode", Float) = 2.0
        [HideInInspector] _ZTestDepthEqualForOpaque("_ZTestDepthEqualForOpaque", Int) = 4 // Less equal
        [HideInInspector] _ZTestGBuffer("_ZTestGBuffer", Int) = 4

        // HACK: GI Baking system relies on some properties existing in the shader ("_MainTex", "_Cutoff" and "_Color") for opacity handling, so we need to store our version of those parameters in the hard-coded name the GI baking system recognizes.
        [HideInInspector] _MainTex("Albedo", 2D) = "white" {}
        [HideInInspector] _Color("Color", Color) = (1,1,1,1)

        [HideInInspector] _EnableFogOnTransparent("Enable Fog", Float) = 1.0
        [HideInInspector] _BlendMode("__blendmode", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _AlphaSrcBlend("__alphaSrc", Float) = 1.0
        [HideInInspector] _AlphaDstBlend("__alphaDst", Float) = 0.0
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
    #pragma multi_compile _ _ALPHATEST_ON
    #pragma shader_feature _SURFACE_TYPE_TRANSPARENT
    #pragma shader_feature_local _ENABLE_FOG_ON_TRANSPARENT
    #pragma shader_feature_local _ _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY

    #pragma vertex Vert
    #pragma fragment Frag

    ENDHLSL

    SubShader
    {
        // This tags allow to use the shader replacement features
        Tags
        {
            "RenderPipeline" = "HDRenderPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "SplatCount" = "8"
            "MaskMapR" = "Metallic"
            "MaskMapG" = "AO"
            "MaskMapB" = "Height"
            "MaskMapA" = "Smoothness"
            "DiffuseA" = "Smoothness (becomes Density when Mask map is assigned)"   // when MaskMap is disabled
            "DiffuseA_MaskMapUsed" = "Density"                                      // when MaskMap is enabled
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode" = "Forward" } // This will be only for transparent object based on the RenderQueue index

            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                WriteMask[_StencilWriteMask]
                Ref[_StencilRef]
                Comp Always
                Pass Replace
            }

            // In case of forward we want to have depth equal for opaque mesh
            Cull[_CullMode]

            HLSLPROGRAM

            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            // Setup DECALS_OFF so the shader stripper can remove variants
            #pragma multi_compile DECALS_OFF DECALS_3RT DECALS_4RT

            // Supported shadow modes per light type
            #pragma multi_compile SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH

            #pragma multi_compile USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST

            #define SHADERPASS SHADERPASS_FORWARD
            #include "WaterLitTemplate.hlsl"

            ENDHLSL
        }

        // Needed to avoid: Assertion failed on expression: '0 == m_CurrentBufferBindMask[kUnityPerDraw]'
        UsePass "Hidden/Nature/Terrain/Utilities/PICKING"
        UsePass "Hidden/Nature/Terrain/Utilities/SELECTION"
    }
}
