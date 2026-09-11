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
                    // Неверная цель (подошёл не туда/кликнул не то/нажал не ту кнопку) —
                    // по ТЗ шаг завершается с отметкой об ошибке, и сценарий идёт дальше.
                    CompleteStep(StepStatus.CompletedWithError, ViolationType.WrongTarget);
                    AdvanceStep(group);
                    break;

                case ActionValidationResult.SequenceViolation:
                    // Нарушение порядка выполнения — по ТЗ закрываем всю текущую
                    // группу шагов и переходим к следующей.
                    CloseGroupOnSequenceViolation(group);
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

        private void CloseGroupOnSequenceViolation(StepGroupDefinition group)
        {
            // По ТЗ: при нарушении порядка выполнения вся группа закрывается.
            // Уже пройденные шаги сохраняют свой статус (они попали в _results
            // раньше, здесь их не трогаем), а все ещё не пройденные шаги —
            // включая тот, что игрок пытался сделать не по порядку, — помечаются
            // как "пропущенные".
            ScenarioEvents.RaiseGroupSequenceViolated(_currentGroupIndex);

            for (int i = _currentStepIndex; i < group.steps.Count; i++)
            {
                StepDefinition step = group.steps[i];
                var result = new StepResult(_currentGroupIndex, i, step, StepStatus.Skipped, ViolationType.SequenceViolation);
                _results.Add(result);
                ScenarioEvents.RaiseStepCompleted(result);
            }

            AdvanceGroup();
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
