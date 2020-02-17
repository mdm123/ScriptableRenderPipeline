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
        [SerializeField] List<long> m_RendererFeatureMap = new List<long>(10);

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

        internal bool ValidateRendererFeatures()
        {
            // Get all Subassets
            var subassets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            var linkedIds = new List<long>();
            var loadedAssets = new Dictionary<long, object>();
            var mapValid = m_RendererFeatureMap != null || m_RendererFeatureMap?.Count == m_RendererFeatures?.Count;

            var debugOutput = $"{name} Render Feature Validation\n-Valid Subassets:";

            // Collect valid, compiled sub-assets
            foreach (var asset in subassets)
            {
                if (asset == null) continue;
                if (asset.GetType().BaseType == typeof(ScriptableRendererFeature))
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long localId);
                    loadedAssets.Add(localId, asset);
                    debugOutput += $"--{asset.name} guid={guid}\n";
                }
            }

            // Collect assets that are connected to the list
            for (var i = 0; i < m_RendererFeatures?.Count; i++)
            {
                if(!m_RendererFeatures[i]) continue;
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_RendererFeatures[i], out var guid, out long localId))
                {
                    linkedIds.Add(localId);
                }
            }

            var mapDebug = mapValid ? "Linking" : "Map missing, will attempt to re-map";
            debugOutput += $"Feature List Status({mapDebug}):\n";

            // Try fix missing references
            for (var i = 0; i < m_RendererFeatures?.Count; i++)
            {
                if (m_RendererFeatures[i] == null)
                {
                    if (mapValid)
                    {
                        var localId = m_RendererFeatureMap[i];
                        loadedAssets.TryGetValue(localId, out var asset);
                        m_RendererFeatures[i] = (ScriptableRendererFeature)asset;

                        debugOutput += $"--{i}:Repaired";
                    }
                    else
                    {
                        m_RendererFeatures[i] = (ScriptableRendererFeature)GetUnusedAsset(ref linkedIds, ref loadedAssets);
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

            if (!m_RendererFeatures.Contains(null)) return true;

            Debug.LogError("Still missing features");
            return false;
        }

        private static object GetUnusedAsset(ref List<long> usedIds, ref Dictionary<long, object> assets)
        {
            foreach (var asset in assets)
            {
                var alreadyLinked = usedIds.Any(used => asset.Key == used);

                if (alreadyLinked) continue;
                usedIds.Add(asset.Key);
                return asset.Value;
            }

            return null;
        }

        [ContextMenu("Re-makeMap")]
        private void CreateMap()
        {
            m_RendererFeatureMap.Clear();
            for (var i = 0; i < rendererFeatures.Count; i++)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_RendererFeatures[i], out var guid, out long localId);
                m_RendererFeatureMap.Add(localId);
            }
        }
#endif
    }
}

