using UnityEngine;
using UnityEngine.UI; // Required for Image component
using TMPro;        // Required for TextMeshProUGUI

public class PlayerHealth : MonoBehaviour{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float maxHealth;
    private PlayerController playerController;

    public Image healthBarFill;
    public TextMeshProUGUI healthText; // Optional, for showing numerical health
    void Start(){
        GameObject playerObj = GameObject.Find("Player");    
        if (playerObj != null){
            playerController = playerObj.GetComponent<PlayerController>();
            maxHealth = playerController.health;
            healthBarFill.fillAmount = playerController.health / maxHealth;
        }
    }

    // Update is called once per frame
    void Update(){    
        if (healthBarFill != null && playerController != null){
            healthBarFill.fillAmount = playerController.health / maxHealth;
        }
    }
}
