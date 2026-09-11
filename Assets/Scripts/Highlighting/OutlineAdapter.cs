using UnityEngine;

namespace VRTraining.Highlighting
{
    /// <summary>
    /// Базовая реализация IHighlightable "из коробки", без платных ассетов:
    /// включает объекту цветное свечение (emission) через MaterialPropertyBlock,
    /// выключает — возвращая чёрный emission. Работает с любым материалом URP Lit,
    /// у которого включена галочка Emission. Требование ТЗ разрешает подсветку
    /// через Highlight Plus/Outline, но не обязывает — это самостоятельная,
    /// не завязанная на сторонний ассет реализация того же интерфейса.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class OutlineAdapter : MonoBehaviour, IHighlightable
    {
        [Tooltip("Идентификатор цели, совпадающий с полем target в StepDefinition.")]
        [SerializeField] private string targetId;

        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField, Range(0f, 5f)] private float emissionIntensity = 1.5f;

        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        private Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            HighlightableRegistry.Register(targetId, this);
        }

        private void OnDisable()
        {
            HighlightableRegistry.Unregister(targetId, this);
        }

        public void SetHighlighted(bool isHighlighted)
        {
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(EmissionColorId, isHighlighted ? highlightColor * emissionIntensity : Color.black);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
