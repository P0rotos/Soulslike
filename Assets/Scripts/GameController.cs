using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour{

    public static GameController instance;
    private GameObject positionRef;

    [Header("Prefabs")]
    public GameObject prefabEnemy;

    [SerializeField] private TextMeshProUGUI attackTypeText;
    private PlayerController player;

    void Awake(){
        if(instance != null){
            Debug.Log("GameController: Error, hay más de un GameController instanciado");
            return;
        }
        instance = this;
        Application.targetFrameRate = 60; // Set your desired framerate
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        player = FindFirstObjectByType<PlayerController>();
        attackTypeText.text = "Attack Type: Sword";
        // positionRef = GameObject.Find("PositionRef");
        // Vector3 spawnPosition = new Vector3(6f, 0f, 0f);
        //GameObject enemy = Instantiate<GameObject>(prefabEnemy, spawnPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update(){
        switch ((char)player.attackType)
        {
            case (char)0:
                attackTypeText.text = "Attack Type: Sword";
                break;
            case (char)1:
                attackTypeText.text = "Attack Type: Dagger";
                break;
            case (char)2:
                attackTypeText.text = "Attack Type: Bow";
                break;
            case (char)3:
                attackTypeText.text = "Attack Type: Magic";
                break;
            default:
                attackTypeText.text = "Attack Type: Unknown";
                break;
        }
    }
}
