namespace VRTraining.Core
{
    /// <summary>
    /// Тип действия, которое может выполнить пользователь в сцене тренировки.
    /// </summary>
    public enum ActionType
    {
        MoveToPoint = 0,
        GrabObject = 1,
        ClickObject = 2,
        PressUIButton = 3
    }
}
