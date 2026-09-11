namespace VRTraining.Core
{
    /// <summary>
    /// Факт совершения игроком одного действия: что сделал (Type) и над каким
    /// объектом (TargetId). Это единственное, что источники действий (PlayerActionSource
    /// и наследники) публикуют в событийную шину — они ничего не знают о сценарии.
    /// </summary>
    public readonly struct PlayerAction
    {
        public readonly ActionType Type;
        public readonly string TargetId;

        public PlayerAction(ActionType type, string targetId)
        {
            Type = type;
            TargetId = targetId;
        }
    }
}
