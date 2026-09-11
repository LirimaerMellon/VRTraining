using UnityEngine;

namespace VRTraining.Highlighting
{
    /// <summary>
    /// Адаптер поверх платного ассета Highlight Plus. Ассет не входит в проект
    /// по умолчанию (ТЗ разрешает его использовать, но не требует), поэтому весь
    /// код, обращающийся к его API, спрятан за символом компиляции HIGHLIGHT_PLUS.
    ///
    /// Чтобы включить: импортируйте Highlight Plus из Asset Store, затем добавьте
    /// "HIGHLIGHT_PLUS" в Project Settings > Player > Scripting Define Symbols.
    /// Без этого символа компонент компилируется, но ничего не делает — проект
    /// собирается и без установленного ассета. По умолчанию подсветкой занимается
    /// OutlineAdapter, реализующий тот же интерфейс IHighlightable.
    /// </summary>
    public class HighlightPlusAdapter : MonoBehaviour, IHighlightable
    {
        [Tooltip("Идентификатор цели, совпадающий с полем target в StepDefinition.")]
        [SerializeField] private string targetId;

#if HIGHLIGHT_PLUS
        private HighlightPlus.HighlightEffect _effect;

        private void Awake()
        {
            _effect = GetComponent<HighlightPlus.HighlightEffect>();
        }
#endif

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
#if HIGHLIGHT_PLUS
            if (_effect == null) return;
            if (isHighlighted) _effect.HighlightOn();
            else _effect.HighlightOff();
#else
            Debug.LogWarning("HighlightPlusAdapter используется без установленного ассета Highlight Plus " +
                             "(нет символа компиляции HIGHLIGHT_PLUS). Используйте OutlineAdapter вместо него.");
#endif
        }
    }
}
