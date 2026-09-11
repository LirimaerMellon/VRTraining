using System;
using System.Collections.Generic;

namespace VRTraining.Highlighting
{
    /// <summary>
    /// Реестр "targetId -> подсвечиваемые объекты с этим id". Заполняется самими
    /// адаптерами подсветки при включении/выключении (OnEnable/OnDisable).
    /// Нужен, чтобы StepHighlightController мог найти объект по строковому id
    /// из StepDefinition.target, не имея прямых ссылок на объекты сцены —
    /// это и есть событийная связь без DI-контейнеров.
    /// </summary>
    public static class HighlightableRegistry
    {
        private static readonly Dictionary<string, List<IHighlightable>> ByTargetId = new Dictionary<string, List<IHighlightable>>();
        private static readonly IReadOnlyList<IHighlightable> Empty = Array.Empty<IHighlightable>();

        public static void Register(string targetId, IHighlightable highlightable)
        {
            if (string.IsNullOrEmpty(targetId)) return;

            if (!ByTargetId.TryGetValue(targetId, out List<IHighlightable> list))
            {
                list = new List<IHighlightable>();
                ByTargetId[targetId] = list;
            }
            list.Add(highlightable);
        }

        public static void Unregister(string targetId, IHighlightable highlightable)
        {
            if (string.IsNullOrEmpty(targetId)) return;
            if (ByTargetId.TryGetValue(targetId, out List<IHighlightable> list))
            {
                list.Remove(highlightable);
            }
        }

        public static IReadOnlyList<IHighlightable> Get(string targetId)
        {
            if (string.IsNullOrEmpty(targetId)) return Empty;
            return ByTargetId.TryGetValue(targetId, out List<IHighlightable> list) ? list : Empty;
        }
    }
}
