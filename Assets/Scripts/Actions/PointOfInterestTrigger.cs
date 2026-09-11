using UnityEngine;
using VRTraining.Core;

namespace VRTraining.Actions
{
    /// <summary>
    /// Точка интереса — невидимая зона-триггер. Публикует MoveToPoint, когда в неё
    /// входит игрок. Коллайдер должен быть настроен как Trigger (см. Reset()).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PointOfInterestTrigger : PlayerActionSource
    {
        [Tooltip("Тег объекта игрока (обычно на корне XR Origin), который должен войти в зону.")]
        [SerializeField] private string playerTag = "Player";

        public override ActionType ActionType => ActionType.MoveToPoint;

        // Автоматически включаем Is Trigger при добавлении компонента в редакторе,
        // чтобы разработчик сцены не забыл это сделать руками.
        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Публикуем действие при КАЖДОМ заходе в зону — это нормально и
            // безопасно повторять сколько угодно раз (в т.ч. после "Попробовать
            // ещё" в рамках той же загруженной сцены): ActionValidator сам решает,
            // что делать с повтором — если шаг уже пройден, действие просто
            // игнорируется, ошибки не будет (см. ActionValidator.Validate).
            if (!other.CompareTag(playerTag)) return;
            PublishAction();
        }
    }
}
