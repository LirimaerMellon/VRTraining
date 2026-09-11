using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core;
using VRTraining.Events;

namespace VRTraining.UI
{
    /// <summary>
    /// Инфо-панель текущей группы шагов: показывает название группы, вводное
    /// сообщение о порядке ожидаемых действий (появляется при активации группы)
    /// и статус каждого шага по мере прохождения.
    /// </summary>
    public class StepGroupInfoPanel : MonoBehaviour
    {
        [SerializeField] private Text groupTitleText;
        [SerializeField] private Text introMessageText;
        [SerializeField] private Text stepsListText;

        private StepGroupDefinition _currentGroup;
        private StepStatus[] _stepStatuses;

        private void OnEnable()
        {
            ScenarioEvents.OnGroupActivated += HandleGroupActivated;
            ScenarioEvents.OnStepCompleted += HandleStepCompleted;
        }

        private void OnDisable()
        {
            ScenarioEvents.OnGroupActivated -= HandleGroupActivated;
            ScenarioEvents.OnStepCompleted -= HandleStepCompleted;
        }

        private void HandleGroupActivated(int groupIndex, StepGroupDefinition group)
        {
            _currentGroup = group;
            _stepStatuses = new StepStatus[group.steps.Count];

            if (groupTitleText != null) groupTitleText.text = group.groupName;
            if (introMessageText != null) introMessageText.text = group.introMessage;
            RefreshStepsList();
        }

        private void HandleStepCompleted(StepResult result)
        {
            // StepResult всегда приходит для текущей активной группы — предыдущая
            // группа уже закрыта, а следующая ещё не могла ничего завершить.
            if (_stepStatuses == null || result.StepIndex >= _stepStatuses.Length) return;

            _stepStatuses[result.StepIndex] = result.Status;
            RefreshStepsList();
        }

        private void RefreshStepsList()
        {
            if (stepsListText == null || _currentGroup == null) return;

            var sb = new StringBuilder();
            for (int i = 0; i < _currentGroup.steps.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {_currentGroup.steps[i].description} — {StatusToText(_stepStatuses[i])}");
            }
            stepsListText.text = sb.ToString();
        }

        private static string StatusToText(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Completed: return "выполнено";
                case StepStatus.CompletedWithError: return "выполнено с ошибкой";
                case StepStatus.Skipped: return "пропущено";
                default: return "ожидание";
            }
        }
    }
}
