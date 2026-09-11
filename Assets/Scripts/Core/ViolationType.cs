namespace VRTraining.Core
{
    /// <summary>
    /// Тип нарушения, зафиксированного при выполнении шага.
    /// </summary>
    public enum ViolationType
    {
        None = 0,
        WrongTarget = 1,
        SequenceViolation = 2
    }
}
