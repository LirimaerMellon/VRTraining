using System;
using System.Collections.Generic;

namespace VRTraining.Core
{
    /// <summary>
    /// Полное описание сценария тренировки: список групп шагов по порядку.
    /// Загружается из JSON (см. ScenarioDefinitionAsset), поэтому не содержит
    /// никакой логики — только данные.
    /// </summary>
    [Serializable]
    public class ScenarioDefinition
    {
        public string scenarioName;
        public List<StepGroupDefinition> groups = new List<StepGroupDefinition>();
    }
}
