using UnityEngine;
using VRTraining.Core;

namespace VRTraining.Actions
{
    /// <summary>
    /// Объект, по которому нужно кликнуть: единственный официальный способ —
    /// навести прицел (CrosshairInteractor) и нажать ЛКМ. Никакой отдельной
    /// логики клика тут больше нет специально, чтобы не было второго
    /// независимого источника того же действия (см. GrabbableActionSource).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ClickableObject : PlayerActionSource
    {
        public override ActionType ActionType => ActionType.ClickObject;
    }
}
