using UnityEngine;
using VRTraining.Core;
using VRTraining.Events;

namespace VRTraining.Actions
{
    /// <summary>
    /// Базовый класс для любого источника игровых действий на сцене. Ничего не знает
    /// о сценарии, группах или шагах — он лишь публикует факт своего действия
    /// в общую шину ScenarioEvents, когда это действие происходит. Всю логику
    /// "правильно это или нет" решает ScenarioController, который слушает события.
    /// Такое разделение позволяет добавлять новые виды действий, не трогая
    /// остальной код (принцип открытости/закрытости — SOLID).
    /// </summary>
    public abstract class PlayerActionSource : MonoBehaviour
    {
        [Tooltip("Идентификатор цели действия. Должен совпадать со значением target " +
                 "в StepDefinition того шага, который это действие должно завершать.")]
        [SerializeField] private string targetId;

        public string TargetId => targetId;

        /// <summary>Тип действия, которое публикует конкретный наследник.</summary>
        public abstract ActionType ActionType { get; }

        /// <summary>Публикует факт совершения действия в общую событийную шину.</summary>
        protected void PublishAction()
        {
            ScenarioEvents.RaisePlayerAction(new PlayerAction(ActionType, targetId));
        }

        /// <summary>
        /// Активирует это действие "прицелом" (CrosshairInteractor) — упрощённый
        /// способ взаимодействия для тестирования мышью/клавиатурой: навёл прицел
        /// на объект и нажал ЛКМ, без необходимости освобождать курсор или
        /// переключать управление XR-контроллером.
        /// </summary>
        public void TriggerFromCrosshair()
        {
            PublishAction();
        }
    }
}
