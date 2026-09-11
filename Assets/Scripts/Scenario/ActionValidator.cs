using VRTraining.Core;

namespace VRTraining.Scenario
{
    /// <summary>
    /// Результат сверки действия игрока с ожидаемым шагом.
    /// </summary>
    public enum ActionValidationResult
    {
        /// <summary>Действие вообще не относится ни к одному шагу сценария — игнорируется.</summary>
        NotRelevant,
        /// <summary>Действие в точности совпадает с ожидаемым шагом.</summary>
        Success,
        /// <summary>Тип действия верный, но цель — нет.</summary>
        WrongTarget,
        /// <summary>Действие совпадает с каким-то другим (ещё не наступившим) шагом сценария — нарушение порядка.</summary>
        SequenceViolation
    }

    /// <summary>
    /// Чистая логика сверки действия игрока с шагами сценария. Не зависит от Unity API
    /// (никаких MonoBehaviour) — легко покрывается юнит-тестами без запуска сцены.
    /// </summary>
    public static class ActionValidator
    {
        public static ActionValidationResult Validate(ScenarioDefinition scenario, int currentGroupIndex, int currentStepIndex, PlayerAction action)
        {
            StepGroupDefinition currentGroup = scenario.groups[currentGroupIndex];
            StepDefinition currentStep = currentGroup.steps[currentStepIndex];

            if (Matches(currentStep, action))
                return ActionValidationResult.Success;

            // Действие совпало с каким-то ДРУГИМ шагом — не важно, из текущей группы,
            // из уже пройденной или из ещё не начатой. Игрок мог потыкаться на
            // совершенно другом столе, к которому дойдёт только позже, — это тоже
            // нарушение порядка, а не "нейтральное" действие, которое можно
            // проигнорировать молча.
            for (int g = 0; g < scenario.groups.Count; g++)
            {
                StepGroupDefinition group = scenario.groups[g];
                for (int s = 0; s < group.steps.Count; s++)
                {
                    if (g == currentGroupIndex && s == currentStepIndex) continue;
                    if (!Matches(group.steps[s], action)) continue;

                    // Шаг УЖЕ пройден (более ранняя группа, либо более ранний шаг
                    // этой же группы) — это не ошибка, просто повтор, игнорируем молча.
                    bool alreadyPassed = g < currentGroupIndex || (g == currentGroupIndex && s < currentStepIndex);
                    return alreadyPassed ? ActionValidationResult.NotRelevant : ActionValidationResult.SequenceViolation;
                }
            }

            // Тип действия тот, что ожидался, но цель не совпала — неверная цель.
            if (action.Type == currentStep.expectedAction)
                return ActionValidationResult.WrongTarget;

            // Действие вообще не связано ни с одним шагом сценария — игнорируем.
            return ActionValidationResult.NotRelevant;
        }

        private static bool Matches(StepDefinition step, PlayerAction action)
        {
            return step.expectedAction == action.Type && step.target == action.TargetId;
        }
    }
}
