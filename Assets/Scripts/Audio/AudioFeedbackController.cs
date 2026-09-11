using UnityEngine;
using VRTraining.Core;
using VRTraining.Events;

namespace VRTraining.Audio
{
    /// <summary>
    /// Проигрывает звуковой сигнал успеха или ошибки при завершении шага.
    /// Единственная система, отвечающая за звук — ничего не знает ни про UI,
    /// ни про подсветку, только слушает OnStepCompleted.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioFeedbackController : MonoBehaviour
    {
        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            ScenarioEvents.OnStepCompleted += HandleStepCompleted;
            ScenarioEvents.OnGroupSequenceViolated += HandleGroupSequenceViolated;
        }

        private void OnDisable()
        {
            ScenarioEvents.OnStepCompleted -= HandleStepCompleted;
            ScenarioEvents.OnGroupSequenceViolated -= HandleGroupSequenceViolated;
        }

        private void HandleStepCompleted(StepResult result)
        {
            // Пропущенные шаги (Skipped) — следствие закрытия группы при нарушении
            // порядка, отдельный сигнал для них не проигрываем: сигнал ошибки уже
            // проигрывается один раз через HandleGroupSequenceViolated.
            switch (result.Status)
            {
                case StepStatus.Completed:
                    PlayClip(successClip);
                    break;
                case StepStatus.CompletedWithError:
                    PlayClip(errorClip);
                    break;
            }
        }

        private void HandleGroupSequenceViolated(int groupIndex)
        {
            PlayClip(errorClip);
        }

        private void PlayClip(AudioClip clip)
        {
            if (clip == null) return;
            _audioSource.PlayOneShot(clip);
        }
    }
}
