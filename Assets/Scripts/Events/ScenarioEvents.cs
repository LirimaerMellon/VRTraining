using System;
using VRTraining.Core;

namespace VRTraining.Events
{
    /// <summary>
    /// Центральная событийная шина всего тренажёра. Это единственная точка связи
    /// между независимыми системами: источники действий игрока публикуют события,
    /// а сценарий, UI, звук и подсветка на них подписываются. Никто из подписчиков
    /// не знает друг о друге и не хранит прямых ссылок — только зависимость от
    /// этого статического класса. Так реализован принцип "события вместо DI".
    /// </summary>
    public static class ScenarioEvents
    {
        /// <summary>Игрок совершил действие (пришёл в точку, взял объект, кликнул, нажал кнопку).</summary>
        public static event Action<PlayerAction> OnPlayerAction;
        public static void RaisePlayerAction(PlayerAction action) => OnPlayerAction?.Invoke(action);

        /// <summary>Активирована новая группа шагов — можно показать инфо-сообщение/подсветку.</summary>
        public static event Action<int, StepGroupDefinition> OnGroupActivated;
        public static void RaiseGroupActivated(int groupIndex, StepGroupDefinition group) => OnGroupActivated?.Invoke(groupIndex, group);

        /// <summary>Активирован конкретный шаг внутри группы — цель для подсветки.</summary>
        public static event Action<int, int, StepDefinition> OnStepActivated;
        public static void RaiseStepActivated(int groupIndex, int stepIndex, StepDefinition step) => OnStepActivated?.Invoke(groupIndex, stepIndex, step);

        /// <summary>Шаг завершён (успешно, с ошибкой или пропущен) — сигнал для звука и UI.</summary>
        public static event Action<StepResult> OnStepCompleted;
        public static void RaiseStepCompleted(StepResult result) => OnStepCompleted?.Invoke(result);

        /// <summary>
        /// Нарушен порядок выполнения шагов — по ТЗ вся текущая группа шагов
        /// закрывается (непройденные шаги помечаются как Skipped через
        /// OnStepCompleted) и сценарий переходит к следующей группе.
        /// </summary>
        public static event Action<int> OnGroupSequenceViolated;
        public static void RaiseGroupSequenceViolated(int groupIndex) => OnGroupSequenceViolated?.Invoke(groupIndex);

        /// <summary>Сценарий полностью завершён — итоговые результаты по каждому шагу.</summary>
        public static event Action<StepResult[]> OnScenarioCompleted;
        public static void RaiseScenarioCompleted(StepResult[] results) => OnScenarioCompleted?.Invoke(results);

        /// <summary>Пользователь нажал "Попытаться ещё" на экране результатов.</summary>
        public static event Action OnRestartRequested;
        public static void RaiseRestartRequested() => OnRestartRequested?.Invoke();

        /// <summary>Пользователь нажал "Возврат в Лобби" на экране результатов.</summary>
        public static event Action OnReturnToLobbyRequested;
        public static void RaiseReturnToLobbyRequested() => OnReturnToLobbyRequested?.Invoke();

        /// <summary>
        /// Сбрасывает все подписки. Обязательно вызывается при смене сцены
        /// (см. SceneLoader), иначе статические события будут держать ссылки
        /// на уничтоженные объекты предыдущей сцены и приводить к утечкам/ошибкам.
        /// </summary>
        public static void ClearAllSubscriptions()
        {
            OnPlayerAction = null;
            OnGroupActivated = null;
            OnStepActivated = null;
            OnStepCompleted = null;
            OnGroupSequenceViolated = null;
            OnScenarioCompleted = null;
            OnRestartRequested = null;
            OnReturnToLobbyRequested = null;
        }
    }
}
