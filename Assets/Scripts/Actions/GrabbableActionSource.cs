using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VRTraining.Core;

namespace VRTraining.Actions
{
    /// <summary>
    /// Объект, который можно "взять" — единственный официальный способ это сделать
    /// в этом проекте: навести прицел (CrosshairInteractor) и нажать ЛКМ.
    /// XRGrabInteractable оставлен на объекте только чтобы к нему можно было
    /// физически прикрепиться визуально/для будущего VR-режима, но его
    /// собственное событие selectEntered НЕ используется для публикации
    /// действия — иначе XR Device Simulator мог бы независимо от прицела сам
    /// "схватить" объект своим лучом/контроллером на тот же клик ЛКМ и вызвать
    /// действие ДВАЖДЫ за один клик, ломая порядок шагов сценария.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabbableActionSource : PlayerActionSource
    {
        public override ActionType ActionType => ActionType.GrabObject;
    }
}
