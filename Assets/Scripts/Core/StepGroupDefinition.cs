using System;
using System.Collections.Generic;

namespace VRTraining.Core
{
    /// <summary>
    /// Группа связанных по смыслу шагов (например "Проверка документов").
    /// Шаги внутри группы должны выполняться строго по порядку.
    /// </summary>
    [Serializable]
    public class StepGroupDefinition
    {
        public string groupName;
        public List<StepDefinition> steps = new List<StepDefinition>();

        /// <summary>
        /// Информационное сообщение, показываемое пользователю при активации группы.
        /// </summary>
        public string introMessage;
    }
}
