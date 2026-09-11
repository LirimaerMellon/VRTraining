using System.Collections.Generic;
using UnityEngine;
using VRTraining.Core;
using VRTraining.Events;

namespace VRTraining.Scenario
{
    /// <summary>
    /// Конечный автомат сценария тренировки: держит текущую группу и шаг,
    /// сверяет входящие действия игрока через ActionValidator, решает переходы
    /// между группами и завершение сценария. Не занимается ни звуком, ни UI,
    /// ни подсветкой напрямую — только публикует события ScenarioEvents,
    /// на которые эти системы подписаны независимо друг от друга.
    /// </summary>
    public class ScenarioController : MonoBehaviour
    {
        [SerializeField] private ScenarioDefinitionAsset scenarioAsset;

        private ScenarioDefinition _scenario;
        private int _currentGroupIndex;
        private int _currentStepIndex;
        private bool _finished;
        private readonly List<StepResult> _results = new List<StepResult>();

        private void OnEnable()
        {
            ScenarioEvents.OnPlayerAction += HandlePlayerAction;
            ScenarioEvents.OnRestartRequested += StartScenario;
        }

        private void OnDisable()
        {
            ScenarioEvents.OnPlayerAction -= HandlePlayerAction;
            ScenarioEvents.OnRestartRequested -= StartScenario;
        }

        private void Start()
        {
            StartScenario();
        }

        /// <summary>Запускает сценарий с самого начала. Используется и при первом старте, и при Restart.</summary>
        public void StartScenario()
        {
            _scenario = scenarioAsset.Load();
            _results.Clear();
            _finished = false;
            _currentGroupIndex = 0;
            ActivateGroup(_currentGroupIndex);
        }

        private void ActivateGroup(int groupIndex)
        {
            _currentStepIndex = 0;
            StepGroupDefinition group = _scenario.groups[groupIndex];
            ScenarioEvents.RaiseGroupActivated(groupIndex, group);
            ScenarioEvents.RaiseStepActivated(groupIndex, 0, group.steps[0]);
        }

        private void HandlePlayerAction(PlayerAction action)
        {
            if (_finished) return;

            StepGroupDefinition group = _scenario.groups[_currentGroupIndex];
            ActionValidationResult validation = ActionValidator.Validate(_scenario, _currentGroupIndex, _currentStepIndex, action);

            switch (validation)
            {
                case ActionValidationResult.Success:
                    CompleteStep(StepStatus.Completed, ViolationType.None);
                    AdvanceStep(group);
                    break;

                case ActionValidationResult.WrongTarget:
                case ActionValidationResult.SequenceViolation:
                    // И "не тот объект" (тип действия верный, цель — нет), и "не по
                    // порядку" (совпало с другим шагом этой же группы) — это ошибка
                    // ПОПЫТКИ, а не провал шага. Группу и шаг НЕ закрываем, дальше не
                    // переходим — просто показываем предупреждение и ждём, пока игрок
                    // выполнит именно текущий ожидаемый шаг.
                    NotifyMistake(group);
                    break;

                case ActionValidationResult.NotRelevant:
                default:
                    // Действие не относится к текущему шагу — просто игнорируем его.
                    break;
            }
        }

        private void CompleteStep(StepStatus status, ViolationType violation)
        {
            StepGroupDefinition group = _scenario.groups[_currentGroupIndex];
            StepDefinition step = group.steps[_currentStepIndex];
            var result = new StepResult(_currentGroupIndex, _currentStepIndex, step, status, violation);
            _results.Add(result);
            ScenarioEvents.RaiseStepCompleted(result);
        }

        private void AdvanceStep(StepGroupDefinition group)
        {
            _currentStepIndex++;
            if (_currentStepIndex >= group.steps.Count)
            {
                AdvanceGroup();
            }
            else
            {
                ScenarioEvents.RaiseStepActivated(_currentGroupIndex, _currentStepIndex, group.steps[_currentStepIndex]);
            }
        }

        private void NotifyMistake(StepGroupDefinition group)
        {
            // Ни WrongTarget, ни SequenceViolation не считаются завершением шага —
            // текущий шаг остаётся текущим, ничего не помечается ни выполненным, ни
            // проваленным. Специально НЕ используем OnStepCompleted здесь: это событие
            // означает "шаг завершён", и StepGroupInfoPanel из-за него навсегда
            // помечал бы ещё не пройденный шаг как проваленный.
            StepDefinition step = group.steps[_currentStepIndex];
            ScenarioEvents.RaiseSequenceViolationWarning(step);
        }

        private void AdvanceGroup()
        {
            _currentGroupIndex++;
            if (_currentGroupIndex >= _scenario.groups.Count)
            {
                FinishScenario();
            }
            else
            {
                ActivateGroup(_currentGroupIndex);
            }
        }

        private void FinishScenario()
        {
            _finished = true;
            ScenarioEvents.RaiseScenarioCompleted(_results.ToArray());
        }
    }
}
