using UnityEngine;
using UnityEngine.UI;
using VRTraining.SceneManagement;

namespace VRTraining.UI
{
    /// <summary>
    /// Контроллер сцены "Лобби": кнопка перехода в сцену тренировки.
    /// Работает и с мышью, и с XR-контроллерами — оба варианта используют
    /// один и тот же Button.onClick через стандартный EventSystem.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class LobbyUIController : MonoBehaviour
    {
        [SerializeField] private string trainingSceneName = "Training";

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(StartTraining);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(StartTraining);
        }

        private void StartTraining()
        {
            SceneLoader.Instance.LoadScene(trainingSceneName);
        }
    }
}
