using UnityEngine;
using UnityEngine.UI;

namespace ChronoSniper
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject planningPanel;
        [SerializeField] private GameObject executingPanel;
        [SerializeField] private GameObject replayPanel;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        [Header("Planning UI")]
        [SerializeField] private Text instructionsText;
        [SerializeField] private Text bouncePointCountText;

        [Header("Executing UI")]
        [SerializeField] private Text enemiesKilledText;

        [Header("Win/Lose UI")]
        [SerializeField] private Button restartButton;

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
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            HideAllPanels();
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (GameManager.Instance == null) return;

            // Update enemies killed counter
            if (enemiesKilledText != null)
            {
                enemiesKilledText.text = $"Enemies: {GameManager.Instance.EnemiesKilled}/{GameManager.Instance.TotalEnemies}";
            }
        }

        private void HideAllPanels()
        {
            if (planningPanel != null) planningPanel.SetActive(false);
            if (executingPanel != null) executingPanel.SetActive(false);
            if (replayPanel != null) replayPanel.SetActive(false);
            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
        }

        public void ShowPlanningUI()
        {
            HideAllPanels();
            if (planningPanel != null)
            {
                planningPanel.SetActive(true);
            }
        }

        public void ShowExecutingUI()
        {
            HideAllPanels();
            if (executingPanel != null)
            {
                executingPanel.SetActive(true);
            }
        }

        public void ShowReplayUI()
        {
            HideAllPanels();
            if (replayPanel != null)
            {
                replayPanel.SetActive(true);
            }
        }

        public void ShowWinUI()
        {
            HideAllPanels();
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
        }

        public void ShowLoseUI()
        {
            HideAllPanels();
            if (losePanel != null)
            {
                losePanel.SetActive(true);
            }
        }

        private void OnRestartClicked()
        {
            GameManager.Instance?.RestartGame();
        }
    }
}
