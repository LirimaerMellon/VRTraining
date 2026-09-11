namespace VRTraining.Core
{
    /// <summary>
    /// Итог выполнения одного шага — используется и для UI-статусов по ходу
    /// сценария, и для финального экрана результатов.
    /// </summary>
    public struct StepResult
    {
        public int GroupIndex;
        public int StepIndex;
        public StepDefinition Step;
        public StepStatus Status;
        public ViolationType Violation;

        public StepResult(int groupIndex, int stepIndex, StepDefinition step, StepStatus status, ViolationType violation)
        {
            GroupIndex = groupIndex;
            StepIndex = stepIndex;
            Step = step;
            Status = status;
            Violation = violation;
        }
    }
}
