using System.Collections.Generic;
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
        [SerializeField] List<int> m_RendererFeatureMap = new List<int>(10);

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
            var objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));

            for (int i = 0; i < m_RendererFeatureMap.Count; i++)
            {
                if (m_RendererFeatures[i] == null)
                {
                    foreach (var asset in objs)
                    {
                        if (asset == null) continue;
                        if (asset.GetInstanceID() == m_RendererFeatureMap[i])
                        {
                            m_RendererFeatures[i] = asset as ScriptableRendererFeature;
                            EditorUtility.SetDirty(this);
                            EditorUtility.RequestScriptReload();
                        }
                    }
                }
            }

            foreach (var obj in objs)
            {
                if (obj == null) continue;
                if (obj.GetType().BaseType == typeof(ScriptableRendererFeature))
                {

                    Debug.Log($"ScriptableRenderer {obj.name} id:{obj.GetInstanceID()}");
                }
            }
        }
#endif
    }
}

