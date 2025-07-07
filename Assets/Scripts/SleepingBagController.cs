using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SleepingBagController : MonoBehaviour{  
    [SerializeField] private Image darkOverlay; // Assign in Inspector
    [SerializeField] private float pauseDuration = 2f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.health = player.stats.vit; // Restore health to max
                player.UpdateHealthUI();
                StartCoroutine(PauseAndDarken());
            }
        }
    }

    private IEnumerator PauseAndDarken()
    {
        if (darkOverlay != null)
        {
            darkOverlay.color = new Color(0, 0, 0, 0.5f); // Semi-transparent black
        }
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(pauseDuration);
        Time.timeScale = 1f;
        if (darkOverlay != null)
        {
            darkOverlay.color = new Color(0, 0, 0, 0f); // Hide overlay
        }
    }
}
