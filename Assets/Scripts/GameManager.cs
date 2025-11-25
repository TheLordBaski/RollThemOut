using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Player Reference")]
    [SerializeField] private PlayerController player;
    
    [Header("Game Settings")]
    [SerializeField] private float gameTime = 0f;
    [SerializeField] private int score = 0;
    [SerializeField] private int itemsAttached = 0;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private TextMeshProUGUI massText;
    
    private bool gameActive = true;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }
    }
    
    void Update()
    {
        if (!gameActive) return;
        
        gameTime += Time.deltaTime;
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
        
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
        
        if (itemCountText != null && player != null)
        {
            itemCountText.text = $"Items: {itemsAttached}";
        }
        
        if (massText != null && player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                massText.text = $"Mass: {rb.mass:F1}kg";
            }
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
    }
    
    public void OnItemAttached()
    {
        itemsAttached++;
        AddScore(10); // Bonus for collecting items
    }
    
    public void OnItemDetached()
    {
        itemsAttached--;
    }
    
    public void GameOver()
    {
        gameActive = false;
        Debug.Log($"Game Over! Final Score: {score}");
    }
    
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}

