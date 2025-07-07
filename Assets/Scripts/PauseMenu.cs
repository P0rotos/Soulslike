using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private TextMeshProUGUI movText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI dexterityText;
    [SerializeField] private TextMeshProUGUI mindText;
    [SerializeField] private TextMeshProUGUI defText;
    [SerializeField] private TextMeshProUGUI mdefText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI pointsText;
    private PlayerController player;
    void Start()
    {
        pauseMenuUI.SetActive(false);
        player = FindFirstObjectByType<PlayerController>();
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        UpdateStatsUI();
    }
    private void UpdateStatsUI()
    {
        if (player != null)
        {
            healthText.text = $"{player.stats.vit}";
            strengthText.text = $"{player.stats.str}";
            dexterityText.text = $"{player.stats.dex}";
            mindText.text = $"{player.stats.mind}";
            movText.text = $"{player.stats.mov}";
            defText.text = $"{player.stats.def}";
            mdefText.text = $"{player.stats.mdef}";
            pointsText.text = $"{player.XP / 10}";
        }
    }
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
    public void QuitGame()
    {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1f; // Resume the game
    }
    public void IncreaseMov()
    {
        if (player.XP < 10)
        {
            Debug.Log("Not enough points to increase movement speed.");
            return;
        }
        player.XP -= 10; // Deduct points
        player.stats.mov += 1.0f;
        player.speed = player.stats.mov / 4f;
        player.dashspeed = player.stats.mov * 1.875f; // 15.0f when mov is 8.0f
        player.rollspeed = player.stats.mov * 1.0f; // 8.0f when mov is 8.0f
        UpdateStatsUI();
    }
    public void IncreaseHealth(){
        if (player.XP < 10){
            Debug.Log("Not enough points to increase movement speed.");
            return;
        }
        player.XP -= 10; // Deduct points
        player.stats.vit += 1.0f;
        player.health = player.health + 1.0f; // Increase current health by 1
        player.UpdateHealthUI();
        UpdateStatsUI();
    }

    public void IncreaseStrength(){
        if (player.XP < 10){
            Debug.Log("Not enough points to increase movement speed.");
            return;
        }
        player.XP -= 10; // Deduct points
        player.stats.str += 1.0f;
        UpdateStatsUI();
    }

    public void IncreaseDexterity(){
        if (player.XP < 10){
            Debug.Log("Not enough points to increase movement speed.");
            return;
        }
        player.XP -= 10; // Deduct points
        player.stats.dex += 1.0f;
        UpdateStatsUI();
    }

    public void IncreaseMind(){
        if (player.XP < 10){
            Debug.Log("Not enough points to increase movement speed.");
            return;
        }
        player.XP -= 10; // Deduct points
        player.stats.mind += 1.0f;
        UpdateStatsUI();
    }

    public void IncreaseDefense(){
        if (player.XP < 10){
            Debug.Log("Not enough points to increase movement speed.");
            return;
        }
        player.XP -= 10; // Deduct points
        player.stats.def += 1.0f;
        UpdateStatsUI();
    }

    public void IncreaseMagicDefense(){
        if (player.XP < 10){
            Debug.Log("Not enough points to increase movement speed.");
            return;
        }
        player.XP -= 10; // Deduct points
        player.stats.mdef += 1.0f;
        UpdateStatsUI();
    }
}
