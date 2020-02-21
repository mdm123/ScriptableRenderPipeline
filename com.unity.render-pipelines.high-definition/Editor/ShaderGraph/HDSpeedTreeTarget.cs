using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEditor.ShaderGraph.Drawing.Controls;

namespace UnityEditor.Rendering.HighDefinition.ShaderGraph
{
    class HDSpeedTreeTarget : ITargetImplementation
    {
        public Type targetType => typeof(SpeedTreeTarget);
        public string displayName => "HDRP";
        public string passTemplatePath => string.Empty;
        public string sharedTemplateDirectory => $"{HDUtils.GetHDRenderPipelinePath()}Editor/ShaderGraph/Templates";

        #region Fields
        public static FieldDescriptor SpeedTreeV7 = new FieldDescriptor("SpeedTree Asset", "Version 7", "SPEEDTREE_V7");
        public static FieldDescriptor SpeedTreeV8 = new FieldDescriptor("SpeedTree Asset", "Version 8", "SPEEDTREE_V8");
        #endregion

        public static KeywordDescriptor SpeedTreeVersion = new KeywordDescriptor()
        {
            displayName = "SpeedTree Asset Version",
            referenceName = "SPEEDTREE",
            type = KeywordType.Enum,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
            entries = new KeywordEntry[]
            {
                    new KeywordEntry() { displayName = "Version 7", referenceName = "V7" },
                    new KeywordEntry() { displayName = "Version 8", referenceName = "V8" },
            }
        };

        public static KeywordDescriptor LodFadePercentage = new KeywordDescriptor()
        {
            displayName = "LOD Fade Percentage",
            referenceName = "LOD_FADE_PERCENTAGE",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.MultiCompile,
            scope = KeywordScope.Local,
        };

        public static KeywordDescriptor SpeedTreeUpAxis = new KeywordDescriptor()
        {
            displayName = "SpeedTree Up Axis",
            referenceName = "SPEEDTREE",
            type = KeywordType.Enum,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
            entries = new KeywordEntry[]
            {
                new KeywordEntry() { displayName = "Y up", referenceName="Y_UP" },
                new KeywordEntry() { displayName = "Z up", referenceName="Z_UP" },
            }
        };

        public static KeywordDescriptor SpeedTree7GeomType = new KeywordDescriptor()
        {
            displayName = "Tree Geom Type",
            referenceName = "GEOM_TYPE",
            type = KeywordType.Enum,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
            entries = new KeywordEntry[]
            {
                new KeywordEntry() { displayName = "Branch", referenceName="BRANCH" },
                new KeywordEntry() { displayName = "Branch Detail", referenceName="BRANCH_DETAIL" },
                new KeywordEntry() { displayName = "Frond", referenceName="FROND" },
                new KeywordEntry() { displayName = "Leaf", referenceName="LEAF" },
                new KeywordEntry() { displayName = "Mesh", referenceName="MESH" },
                new KeywordEntry() { displayName = "Null", referenceName="NULL"},       // Included as a dummy for SpeedTree 8
            }
        };

        public const int kNullGeomType = 5;

        public static KeywordDescriptor SpeedTree8WindQuality = new KeywordDescriptor()
        {
            displayName = "Wind Quality",
            referenceName = "_WINDQUALITY",
            type = KeywordType.Enum,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
            entries = new KeywordEntry[]
            {
                new KeywordEntry() { displayName = "None", referenceName="NONE" },
                new KeywordEntry() { displayName = "Fastest", referenceName="FASTEST" },
                new KeywordEntry() { displayName = "Fast", referenceName="FAST" },
                new KeywordEntry() { displayName = "Better", referenceName="BETTER" },
                new KeywordEntry() { displayName = "Best", referenceName="BEST" },
                new KeywordEntry() { displayName = "Palm", referenceName="PALM" },
                new KeywordEntry() { displayName = "Null", referenceName="NULL" },     // Included as a dummy for SpeedTree 7
            }
        };

        public const int kNullWindQuality = 6;

        public static KeywordDescriptor EnableWind = new KeywordDescriptor()
        {
            displayName = "Enable Wind",
            referenceName = "ENABLE_WIND",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
        };

        public static KeywordDescriptor EnableBillboard = new KeywordDescriptor()
        {
            displayName = "Billboard Mode",
            referenceName = "EFFECT_BILLBOARD",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
        };

        public static KeywordDescriptor BillboardFaceCam = new KeywordDescriptor()
        {
            displayName = "Billboard Faces Camera",
            referenceName = "BILLBOARD_FACE_CAMERA_POS",
            type = KeywordType.Boolean,
            definition = KeywordDefinition.ShaderFeature,
            scope = KeywordScope.Local,
        };
        /*
        public static PragmaDescriptor EnableWind = new PragmaDescriptor()
        {
            value = "shader_feature_local ENABLE_WIND"
        };
        public static PragmaDescriptor EnableBillboard = new PragmaDescriptor()
        {
            value = "shader_feature_local EFFECT_BILLBOARD"
        };
        */

        public static PragmaDescriptor EnableDebug = new PragmaDescriptor()
        {
            value = "enable_d3d11_debug_symbols"
        };

        public bool IsValid(IMasterNode masterNode)
        {
            return (masterNode is PBRMasterNode ||
                    masterNode is UnlitMasterNode ||
                    masterNode is HDUnlitMasterNode ||
                    masterNode is HDLitMasterNode ||
                    masterNode is FabricMasterNode);
        }
        public bool IsPipelineCompatible(RenderPipelineAsset currentPipeline)
        {
            return currentPipeline is HDRenderPipelineAsset;
        }

        private SubShaderDescriptor UpdateSubShader(ref SubShaderDescriptor origDescriptor)
        {
            SubShaderDescriptor modDescriptor = origDescriptor;

            modDescriptor.renderQueueOverride = "AlphaTest";

            foreach(PassCollection.Item p in modDescriptor.passes)
            {
                p.descriptor.keywords.Add(SpeedTreeVersion);
                p.descriptor.keywords.Add(LodFadePercentage);
                p.descriptor.defines.Add(SpeedTreeUpAxis, 0);
                p.descriptor.keywords.Add(EnableWind);
                p.descriptor.keywords.Add(EnableBillboard);
                p.descriptor.keywords.Add(BillboardFaceCam);

                p.descriptor.keywords.Add(SpeedTree7GeomType);
                p.descriptor.keywords.Add(SpeedTree8WindQuality);

                p.descriptor.pragmas.Add(EnableDebug);

                p.descriptor.includes.Add("Packages/com.unity.render-pipelines.core/ShaderLibrary/SpeedTree/SpeedTreeCommon.hlsl", IncludeLocation.Pregraph);
            }

            modDescriptor.customEditorOverride = @"CustomEditor ""UnityEditor.Rendering.HighDefinition.SpeedTreeLitGUI""";

            return modDescriptor;
        }

        public void SetupTarget(ref TargetSetupContext context)
        {
            context.AddAssetDependencyPath(AssetDatabase.GUIDToAssetPath("4592b595eeb00ee42868a87a4901d29b")); // SpeedTreeTarget
            context.AddAssetDependencyPath(AssetDatabase.GUIDToAssetPath("e0988759073f96945ba34b15eed233e0")); // HDSpeedTreeTarget

            switch (context.masterNode)
            {
                case PBRMasterNode pbrMasterNode:
                    context.SetupSubShader(UpdateSubShader(ref HDSubShaders.PBR));
                    break;
                case UnlitMasterNode unlitMasterNode:
                    context.SetupSubShader(UpdateSubShader(ref HDSubShaders.Unlit));
                    break;
                case HDUnlitMasterNode hdUnlitMasterNode:
                    context.SetupSubShader(UpdateSubShader(ref HDSubShaders.HDUnlit));
                    hdUnlitMasterNode.alphaTest = new ToggleData(true);
                    break;
                case HDLitMasterNode hdLitMasterNode:
                    context.SetupSubShader(UpdateSubShader(ref HDSubShaders.HDLit));
                    hdLitMasterNode.alphaTest = new ToggleData(true);
                    break;
                case FabricMasterNode fabricMasterNode:
                    context.SetupSubShader(UpdateSubShader(ref HDSubShaders.Fabric));
                    fabricMasterNode.alphaTest = new ToggleData(true);
                    break;
            }
        }
    }
}
