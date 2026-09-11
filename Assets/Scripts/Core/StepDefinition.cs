using System;

namespace VRTraining.Core
{
    /// <summary>
    /// Описание одного шага сценария: какое действие и над каким объектом
    /// (target id) ожидается от пользователя. Чистые данные, без ссылок на Unity API,
    /// поэтому легко сериализуются из JSON через JsonUtility.
    /// </summary>
    [Serializable]
    public class StepDefinition
    {
        public int id;
        public string description;
        public ActionType expectedAction;

        /// <summary>
        /// Идентификатор ожидаемой цели действия. Должен совпадать со значением
        /// targetId, которое публикует соответствующий PlayerActionSource в сцене.
        /// </summary>
        public string target;
    }
}
