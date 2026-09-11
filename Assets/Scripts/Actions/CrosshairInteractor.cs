using UnityEngine;
using UnityEngine.InputSystem;

namespace VRTraining.Actions
{
    /// <summary>
    /// Простой "прицел" для тестирования мышью/клавиатурой без VR-контроллеров:
    /// по нажатию ЛКМ пускает луч из центра камеры (туда же, где нарисован
    /// прицел на HUD) и, если попал в объект с PlayerActionSource, активирует
    /// его. Не зависит от блокировки курсора, XR Device Simulator, лучей
    /// контроллеров и т.п. — работает всегда, пока камера смотрит на цель.
    /// </summary>
    public class CrosshairInteractor : MonoBehaviour
    {
        [SerializeField] private Camera sourceCamera;
        [SerializeField] private float maxDistance = 5f;

        private void Awake()
        {
            // Не полагаемся только на ручную ссылку в инспекторе — если она почему-то
            // не задана (например, объект создан не через редактор), находим камеру
            // игрока сами по стандартному тегу MainCamera.
            if (sourceCamera == null)
            {
                sourceCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (sourceCamera == null)
            {
                sourceCamera = Camera.main;
                if (sourceCamera == null) return;
            }

            var mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;

            // Проверяем ВСЕ объекты вдоль луча, а не только первый попавшийся —
            // если между камерой и целью случайно окажется коллайдер руки/контроллера,
            // это не должно мешать клику по объекту, стоящему дальше.
            RaycastHit[] hits = Physics.RaycastAll(
                sourceCamera.transform.position,
                sourceCamera.transform.forward,
                maxDistance);

            if (hits.Length == 0) return;

            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (RaycastHit hit in hits)
            {
                var source = hit.collider.GetComponentInParent<PlayerActionSource>();
                if (source != null)
                {
                    source.TriggerFromCrosshair();
                    return;
                }
            }
        }
    }
}
