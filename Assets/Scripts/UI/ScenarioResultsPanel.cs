using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VRTraining.Core;
using VRTraining.Events;
using VRTraining.SceneManagement;

namespace VRTraining.UI
{
    /// <summary>
    /// Финальный экран сценария: список всех шагов с их статусом и кнопки
    /// "Попытаться ещё" (Restart) и "Возврат в Лобби". Панель скрыта, пока
    /// сценарий не завершится (OnScenarioCompleted).
    /// </summary>
    public class ScenarioResultsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Text resultsText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnToLobbyButton;
        [SerializeField] private string lobbySceneName = "Lobby";

        private void Awake()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
        }

        private void OnEnable()
        {
            ScenarioEvents.OnScenarioCompleted += HandleScenarioCompleted;
            if (restartButton != null) restartButton.onClick.AddListener(HandleRestartClicked);
            if (returnToLobbyButton != null) returnToLobbyButton.onClick.AddListener(HandleReturnToLobbyClicked);
        }

        private void OnDisable()
        {
            ScenarioEvents.OnScenarioCompleted -= HandleScenarioCompleted;
            if (restartButton != null) restartButton.onClick.RemoveListener(HandleRestartClicked);
            if (returnToLobbyButton != null) returnToLobbyButton.onClick.RemoveListener(HandleReturnToLobbyClicked);
        }

        private void HandleScenarioCompleted(StepResult[] results)
        {
            if (resultsText != null) resultsText.text = BuildResultsText(results);
            if (panelRoot != null) panelRoot.SetActive(true);
        }

        private static string BuildResultsText(StepResult[] results)
        {
            var sb = new StringBuilder();
            int currentGroup = -1;
            foreach (StepResult result in results)
            {
                if (result.GroupIndex != currentGroup)
                {
                    currentGroup = result.GroupIndex;
                    sb.AppendLine();
                    sb.AppendLine($"Группа {currentGroup + 1}");
                }
                sb.AppendLine($"  {result.StepIndex + 1}. {result.Step.description} — {StatusToText(result.Status)}");
            }
            return sb.ToString();
        }

        private static string StatusToText(StepStatus status)
        {
            switch (status)
            {
                case StepStatus.Completed: return "выполнено";
                case StepStatus.CompletedWithError: return "выполнено с ошибкой";
                case StepStatus.Skipped: return "пропущено";
                default: return "не начато";
            }
        }

        private void HandleRestartClicked()
        {
            if (panelRoot != null) panelRoot.SetActive(false);
            ScenarioEvents.RaiseRestartRequested();
        }

        private void HandleReturnToLobbyClicked()
        {
            SceneLoader.Instance.LoadScene(lobbySceneName);
        }
    }
}
