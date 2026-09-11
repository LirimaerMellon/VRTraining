using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core;

namespace VRTraining.Actions
{
    /// <summary>
    /// UI-кнопка сцены тренировки, участвующая в сценарии (не путать с кнопками
    /// экрана результатов — те не публикуют PlayerAction, а напрямую вызывают
    /// ScenarioEvents.RaiseRestartRequested/RaiseReturnToLobbyRequested).
    /// Публикует PressUIButton по стандартному Button.onClick — работает
    /// одинаково для мыши и для XR-контроллеров.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIButtonActionSource : PlayerActionSource
    {
        private Button _button;

        public override ActionType ActionType => ActionType.PressUIButton;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(PublishAction);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(PublishAction);
        }
    }
}
