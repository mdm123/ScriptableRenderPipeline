using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// You can add a <c>ScriptableRendererFeature</c> to the <c>ScriptableRenderer</c>. Use this scriptable renderer feature to inject render passes into the renderer.
    /// </summary>
    /// <seealso cref="ScriptableRenderer"/>
    /// <seealso cref="ScriptableRenderPass"/>
    [ExcludeFromPreset]
    [MovedFrom("UnityEngine.Rendering.LWRP")] public abstract class ScriptableRendererFeature : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool m_Active = true;
        /// <summary>
        /// This returns the active state of this ScriptableRenderFeature, which is set using ScriptableRenderFeature.SetActive.
        /// </summary>
        public bool isActive => m_Active;

        /// <summary>
        /// Initializes this feature's resources. This is called every time serialization happens.
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Injects one or multiple <c>ScriptableRenderPass</c> in the renderer.
        /// </summary>
        /// <param name="renderPasses">List of render passes to add to.</param>
        /// <param name="renderingData">Rendering state. Use this to setup render passes.</param>
        public abstract void AddRenderPasses(ScriptableRenderer renderer,
            ref RenderingData renderingData);

        void OnEnable()
        {
            Create();
        }

        void OnValidate()
        {
            Create();
        }

        /// <summary>
        /// Activates/Deactivates the ScriptableRenderFeature, depending on the given true or false value.
        /// While active this feature will be added to the renderer it is attached to, and will be skipped when inactive.
        /// </summary>
        /// <param name="active">Activate or deactivate the ScriptableRenderFeature, where true activates the ScriptableRenderFeature and false deactivates the ScriptableRenderFeature.</param>
        public void SetActive(bool active)
        {
            m_Active = active;
        }
    }
}
