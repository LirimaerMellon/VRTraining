using UnityEngine;
using UnityEngine.SceneManagement;
using VRTraining.Events;

namespace VRTraining.SceneManagement
{
    /// <summary>
    /// Persistent-синглтон, отвечающий за переходы между сценами. Перед загрузкой
    /// новой сцены очищает все подписки ScenarioEvents — иначе статические
    /// события держали бы ссылки на уничтоженные объекты предыдущей сцены,
    /// что привело бы к утечкам и ложным вызовам после смены сцены.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName)
        {
            ScenarioEvents.ClearAllSubscriptions();
            SceneManager.LoadScene(sceneName);
        }
    }
}
