using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting.APIUpdating;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Class <c>ScriptableRendererData</c> contains resources for a <c>ScriptableRenderer</c>.
    /// <seealso cref="ScriptableRenderer"/>
    /// </summary>
    [MovedFrom("UnityEngine.Rendering.LWRP")] public abstract class ScriptableRendererData : ScriptableObject
    {
        internal bool isInvalidated { get; set; }

        /// <summary>
        /// Creates the instance of the ScriptableRenderer.
        /// </summary>
        /// <returns>The instance of ScriptableRenderer</returns>
        protected abstract ScriptableRenderer Create();

        [SerializeField] List<ScriptableRendererFeature> m_RendererFeatures = new List<ScriptableRendererFeature>(10);
        [SerializeField] List<string> m_RendererFeatureMap = new List<string>(10);

        /// <summary>
        /// List of additional render pass features for this renderer.
        /// </summary>
        public List<ScriptableRendererFeature> rendererFeatures
        {
            get => m_RendererFeatures;
        }

        internal ScriptableRenderer InternalCreateRenderer()
        {
            isInvalidated = false;
            return Create();
        }

        protected virtual void OnValidate()
        {
            isInvalidated = true;
            if(m_RendererFeatures.Contains(null))
                ValidateRendererFeatures();
        }

        protected virtual void OnEnable()
        {
            isInvalidated = true;
        }

#if UNITY_EDITOR
        internal virtual Material GetDefaultMaterial(DefaultMaterialType materialType)
        {
            return null;
        }

        internal virtual Shader GetDefaultShader()
        {
            return null;
        }

        internal void ValidateRendererFeatures()
        {
            // Get all Subassets
            var subassets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            var linkedGuids = new List<string>();
            var loadedAssets = new Dictionary<string, object>();
            var mapValid = m_RendererFeatureMap?.Count == m_RendererFeatures?.Count;

            var debugOutput = $"{name} Render Feature Validation\n-Valid Subassets:";

            foreach (var asset in subassets)
            {
                if (asset == null) continue;
                if (asset.GetType().BaseType == typeof(ScriptableRendererFeature))
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long localId);
                    loadedAssets.Add(guid, asset);
                    debugOutput += $"--{asset.name} guid={guid}\n";
                }
            }

            for (var i = 0; i < m_RendererFeatures?.Count; i++)
            {
                if(!m_RendererFeatures[i]) continue;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_RendererFeatures[i], out var guid, out long localId))
                {
                    linkedGuids.Add(guid);
                }
            }

            var mapDebug = mapValid ? "Linking" : "Map missing, will attempt to re-map";
            debugOutput += $"Feature List Status({mapDebug}):\n";

            for (var i = 0; i < m_RendererFeatures?.Count; i++)
            {
                if (m_RendererFeatures[i] == null)
                {
                    if (mapValid)
                    {
                        var guid = m_RendererFeatureMap[i];
                        if(guid != null)
                            m_RendererFeatures[i] =
                            AssetDatabase.LoadAssetAtPath<ScriptableRendererFeature>(
                                AssetDatabase.GUIDToAssetPath(guid));
                        debugOutput += $"--{i}:Repaired";
                    }
                    else
                    {
                        m_RendererFeatures[i] =
                            AssetDatabase.LoadAssetAtPath<ScriptableRendererFeature>(
                                AssetDatabase.GUIDToAssetPath(GetUnusedGUID(ref linkedGuids, ref loadedAssets)));
                        debugOutput += $"--{i}:Missing - attempting to fix...";
                    }
                }
                debugOutput += m_RendererFeatures[i] != null ? $"--{i}:Linked" : $"--{i}:Missing";
            }
            Debug.Log(debugOutput);

            if (!mapValid)
            {
                CreateMap();
            }
        }

        private string GetUnusedGUID(ref List<string> usedGuids, ref Dictionary<string, object> assets)
        {
            foreach (var asset in assets)
            {
                var alreadyLinked = usedGuids.Any(used => asset.Key == used);

                if (alreadyLinked) continue;
                usedGuids.Add(asset.Key);
                return asset.Key;
            }

            return null;
        }

        internal void CreateMap()
        {
            m_RendererFeatureMap.Clear();
            for (int i = 0; i < rendererFeatures.Count; i++)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_RendererFeatures[i], out var guid, out long localId);
                m_RendererFeatureMap.Add(guid);
            }
        }
#endif
    }
}

