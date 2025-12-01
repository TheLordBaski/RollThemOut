using UnityEngine;

namespace ChronoSniper
{
    public class TimeController : MonoBehaviour
    {
        public static TimeController Instance { get; private set; }

        [Header("Time Settings")]
        [SerializeField] private float normalTimeScale = 1f;
        [SerializeField] private float pausedTimeScale = 0f;

        private float currentTimeScale = 0f;
        private bool isPaused = true;

        public bool IsPaused => isPaused;
        public float CurrentTimeScale => currentTimeScale;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetTimeScale(pausedTimeScale);
        }

        public void SetTimeScale(float scale)
        {
            currentTimeScale = scale;
            Time.timeScale = scale;
            isPaused = scale <= 0f;
        }

        public void PauseTime()
        {
            SetTimeScale(pausedTimeScale);
        }

        public void ResumeTime()
        {
            SetTimeScale(normalTimeScale);
        }

        public void ToggleTime()
        {
            if (isPaused)
                ResumeTime();
            else
                PauseTime();
        }
    }
}
