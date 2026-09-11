namespace VRTraining.Highlighting
{
    /// <summary>
    /// Абстракция над конкретным способом подсветки объекта. Позволяет подменить
    /// реализацию (свой outline через материал, Highlight Plus, встроенный
    /// Outline-компонент и т.д.), не трогая StepHighlightController —
    /// принцип инверсии зависимостей (SOLID).
    /// </summary>
    public interface IHighlightable
    {
        void SetHighlighted(bool isHighlighted);
    }
}
