using UnityEngine;
using System.Collections.Generic;

namespace ChronoSniper
{
    public enum GameState
    {
        Planning,    // Player is setting up bounce points
        Executing,   // Bullet is traveling
        Replay,      // Showing kill cam
        Win,
        Lose
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private int totalEnemies = 5;
        [SerializeField] private float replayDelay = 1f;

        [Header("References")]
        [SerializeField] private BulletController bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;

        private GameState currentState = GameState.Planning;
        private List<Enemy> enemies = new List<Enemy>();
        private int enemiesKilled = 0;
        private BulletController activeBullet;

        public GameState CurrentState => currentState;
        public int TotalEnemies => totalEnemies;
        public int EnemiesKilled => enemiesKilled;
        public bool BulletFired { get; private set; }

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
            RegisterAllEnemies();
            ChangeState(GameState.Planning);
        }

        private void RegisterAllEnemies()
        {
            enemies.Clear();
            enemies.AddRange(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
            totalEnemies = enemies.Count;
        }

        public void ChangeState(GameState newState)
        {
            currentState = newState;
            OnStateChanged(newState);
        }

        private void OnStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Planning:
                    TimeController.Instance?.SetTimeScale(0f);
                    UIManager.Instance?.ShowPlanningUI();
                    break;

                case GameState.Executing:
                    TimeController.Instance?.SetTimeScale(1f);
                    UIManager.Instance?.ShowExecutingUI();
                    break;

                case GameState.Replay:
                    Invoke(nameof(StartReplay), replayDelay);
                    break;

                case GameState.Win:
                    UIManager.Instance?.ShowWinUI();
                    break;

                case GameState.Lose:
                    UIManager.Instance?.ShowLoseUI();
                    break;
            }
        }

        public void FireBullet(Vector3 direction)
        {
            if (BulletFired) return;

            BulletFired = true;
            activeBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            activeBullet.Initialize(direction);
            
            ChangeState(GameState.Executing);
        }

        public void OnEnemyKilled()
        {
            enemiesKilled++;

            if (enemiesKilled >= totalEnemies)
            {
                // All enemies killed - WIN
                Invoke(nameof(TriggerWin), 0.5f);
            }
        }

        public void OnBulletStopped()
        {
            if (enemiesKilled < totalEnemies)
            {
                // Not all enemies killed - LOSE
                Invoke(nameof(TriggerLose), 0.5f);
            }
        }

        private void TriggerWin()
        {
            ChangeState(GameState.Replay);
        }

        private void TriggerLose()
        {
            ChangeState(GameState.Lose);
        }

        private void StartReplay()
        {
            ReplayManager.Instance?.StartReplay();
        }

        public void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}
