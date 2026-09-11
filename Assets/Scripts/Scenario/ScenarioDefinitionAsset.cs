using UnityEngine;
using VRTraining.Core;

namespace VRTraining.Scenario
{
    /// <summary>
    /// ScriptableObject-обёртка над ScenarioDefinition. Позволяет назначить JSON
    /// сценария прямо в инспекторе (через TextAsset), что удобно для дизайнера
    /// сценариев и не требует хардкода путей в коде.
    /// </summary>
    [CreateAssetMenu(fileName = "ScenarioDefinitionAsset", menuName = "VR Training/Scenario Definition")]
    public class ScenarioDefinitionAsset : ScriptableObject
    {
        [Tooltip("JSON-файл сценария, перетащенный в инспектор.")]
        [SerializeField] private TextAsset sourceFile;

        [Tooltip("Альтернатива: JSON сценария прямо текстом (используется, если sourceFile не задан).")]
        [SerializeField, TextArea(5, 20)] private string sourceJson;

        [Tooltip("Ещё одна альтернатива: имя файла в любой папке Resources (без расширения).")]
        [SerializeField] private string resourcesPath = "example_scenario";

        /// <summary>
        /// Парсит и возвращает описание сценария. Каждый вызов создаёт новый экземпляр
        /// данных, чтобы повторный запуск (Restart) не переиспользовал состояние
        /// предыдущего прохождения.
        /// </summary>
        public ScenarioDefinition Load()
        {
            string json = ResolveJson();
            return JsonUtility.FromJson<ScenarioDefinition>(json);
        }

        private string ResolveJson()
        {
            if (sourceFile != null) return sourceFile.text;
            if (!string.IsNullOrEmpty(sourceJson)) return sourceJson;

            TextAsset fromResources = Resources.Load<TextAsset>(resourcesPath);
            if (fromResources != null) return fromResources.text;

            Debug.LogError($"ScenarioDefinitionAsset: не удалось найти JSON сценария " +
                            $"(sourceFile, sourceJson и Resources/{resourcesPath} пусты).");
            return "{}";
        }
    }
}
