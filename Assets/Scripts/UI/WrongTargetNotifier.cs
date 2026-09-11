using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core;
using VRTraining.Events;

namespace VRTraining.UI
{
    /// <summary>
    /// Показывает короткое предупреждение на экране, если игрок выполнил
    /// действие с неверной целью (подошёл не к тому месту/предмету) —
    /// StepStatus.CompletedWithError. Ничего не знает о ScenarioController
    /// напрямую, только слушает OnStepCompleted, как и остальные независимые
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
            ScenarioEvents.OnSequenceViolationWarning += HandleSequenceViolationWarning;
            if (warningRoot != null) warningRoot.SetActive(false);
        }

        private void OnDisable()
        {
            ScenarioEvents.OnStepCompleted -= HandleStepCompleted;
            ScenarioEvents.OnSequenceViolationWarning -= HandleSequenceViolationWarning;
        }

        private void HandleStepCompleted(StepResult result)
        {
            if (result.Status != StepStatus.CompletedWithError) return;
            ShowWarning(result.Step.description);
        }

        private void HandleSequenceViolationWarning(StepDefinition expectedStep)
        {
            ShowWarning(expectedStep.description);
        }

        private void ShowWarning(string expectedStepDescription)
        {
            if (warningRoot == null || warningText == null) return;

            warningText.text = $"Неверно! Ожидалось: {expectedStepDescription}";
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
