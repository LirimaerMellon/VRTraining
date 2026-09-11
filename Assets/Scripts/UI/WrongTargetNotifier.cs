using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core;
using VRTraining.Events;

namespace VRTraining.UI
{
    /// <summary>
    /// Показывает короткое предупреждение на экране при ошибке шага
    /// (StepStatus.CompletedWithError) и при закрытии группы из-за нарушения
    /// порядка (OnGroupSequenceViolated). Ничего не знает о ScenarioController
    /// напрямую — только слушает события, как и остальные независимые
    /// UI/звук/подсветка системы.
    /// </summary>
    public class WrongTargetNotifier : MonoBehaviour
    {
        [SerializeField] private GameObject warningRoot;
        [SerializeField] private Text warningText;
        [SerializeField] private float showDuration = 2.2f;

        private Coroutine _hideRoutine;

        private void OnEnable()
        {
            ScenarioEvents.OnStepCompleted += HandleStepCompleted;
            ScenarioEvents.OnGroupSequenceViolated += HandleGroupSequenceViolated;
            if (warningRoot != null) warningRoot.SetActive(false);
        }

        private void OnDisable()
        {
            ScenarioEvents.OnStepCompleted -= HandleStepCompleted;
            ScenarioEvents.OnGroupSequenceViolated -= HandleGroupSequenceViolated;
        }

        private void HandleStepCompleted(StepResult result)
        {
            if (result.Status != StepStatus.CompletedWithError) return;
            ShowWarning($"Неверно! Ожидалось: {result.Step.description}");
        }

        private void HandleGroupSequenceViolated(int groupIndex)
        {
            ShowWarning("Нарушен порядок выполнения! Этап завершён, переходим к следующему.");
        }

        private void ShowWarning(string message)
        {
            if (warningRoot == null || warningText == null) return;

            warningText.text = message;
            warningRoot.SetActive(true);

            if (_hideRoutine != null) StopCoroutine(_hideRoutine);
            _hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(showDuration);
            if (warningRoot != null) warningRoot.SetActive(false);
        }
    }
}
