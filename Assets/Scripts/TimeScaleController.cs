using UnityEngine;

/// <summary>
/// Simple utility to control Time.timeScale for testing the FirstPersonController
/// with time stopping mechanics. Attach to any GameObject in the scene.
/// Press T to toggle time freeze, R to reset to normal time.
/// </summary>
public class TimeScaleController : MonoBehaviour
{
    [Header("Time Control")]
    [Tooltip("Whether time is currently frozen")]
    public bool timeFrozen = false;

    [Header("Key Bindings")]
    [Tooltip("Key to toggle time freeze")]
    public KeyCode toggleTimeKey = KeyCode.T;
    
    [Tooltip("Key to reset time to normal")]
    public KeyCode resetTimeKey = KeyCode.R;

    private void Update()
    {
        // Toggle time freeze
        if (Input.GetKeyDown(toggleTimeKey))
        {
            ToggleTimeFreeze();
        }

        // Reset time to normal
        if (Input.GetKeyDown(resetTimeKey))
        {
            ResetTime();
        }
    }

    /// <summary>
    /// Toggle between frozen and normal time
    /// </summary>
    public void ToggleTimeFreeze()
    {
        timeFrozen = !timeFrozen;
        Time.timeScale = timeFrozen ? 0f : 1f;
        Debug.Log($"Time {(timeFrozen ? "FROZEN" : "RESUMED")} - TimeScale: {Time.timeScale}");
    }

    /// <summary>
    /// Reset time to normal (timeScale = 1)
    /// </summary>
    public void ResetTime()
    {
        timeFrozen = false;
        Time.timeScale = 1f;
        Debug.Log("Time RESET - TimeScale: 1.0");
    }

    /// <summary>
    /// Set time scale to a specific value
    /// </summary>
    public void SetTimeScale(float scale)
    {
        Time.timeScale = Mathf.Max(0f, scale);
        timeFrozen = Time.timeScale == 0f;
        Debug.Log($"Time scale set to: {Time.timeScale}");
    }

    private void OnGUI()
    {
        // Display time state in the game view
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = timeFrozen ? Color.red : Color.green;
        
        string status = timeFrozen ? "TIME FROZEN" : "TIME NORMAL";
        GUI.Label(new Rect(10, 10, 300, 30), status, style);
        GUI.Label(new Rect(10, 35, 300, 30), $"TimeScale: {Time.timeScale:F2}", style);
        
        // Instructions
        GUIStyle instructionStyle = new GUIStyle();
        instructionStyle.fontSize = 14;
        instructionStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 65, 400, 20), $"Press {toggleTimeKey} to toggle time freeze", instructionStyle);
        GUI.Label(new Rect(10, 85, 400, 20), $"Press {resetTimeKey} to reset time", instructionStyle);
    }
}
