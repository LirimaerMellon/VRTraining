using UnityEngine;
using VRTraining.Core;
using VRTraining.Events;

namespace VRTraining.Highlighting
{
    /// <summary>
    /// Слушает события сценария и включает/выключает подсветку нужного объекта
    /// через абстракцию IHighlightable. Не знает, какая именно реализация
    /// подсветки стоит на объекте (OutlineAdapter или HighlightPlusAdapter).
    /// </summary>
    public class StepHighlightController : MonoBehaviour
    {
        private string _currentTargetId;

        private void OnEnable()
        {
            ScenarioEvents.OnStepActivated += HandleStepActivated;
            ScenarioEvents.OnStepCompleted += HandleStepCompleted;
            ScenarioEvents.OnGroupSequenceViolated += HandleGroupSequenceViolated;
        }

        private void OnDisable()
        {
            ScenarioEvents.OnStepActivated -= HandleStepActivated;
            ScenarioEvents.OnStepCompleted -= HandleStepCompleted;
            ScenarioEvents.OnGroupSequenceViolated -= HandleGroupSequenceViolated;
        }

        private void HandleStepActivated(int groupIndex, int stepIndex, StepDefinition step)
        {
            ClearCurrentHighlight();
            _currentTargetId = step.target;
            SetHighlight(_currentTargetId, true);
        }

        private void HandleStepCompleted(StepResult result)
        {
            ClearCurrentHighlight();
        }

        private void HandleGroupSequenceViolated(int groupIndex)
        {
            ClearCurrentHighlight();
        }

        private void ClearCurrentHighlight()
        {
            if (string.IsNullOrEmpty(_currentTargetId)) return;
            SetHighlight(_currentTargetId, false);
            _currentTargetId = null;
        }

        private static void SetHighlight(string targetId, bool state)
        {
            var highlightables = HighlightableRegistry.Get(targetId);
            for (int i = 0; i < highlightables.Count; i++)
            {
                highlightables[i].SetHighlighted(state);
            }
        }
    }
}
