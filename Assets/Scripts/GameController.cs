using UnityEngine;

public class GameController : MonoBehaviour{

    public static GameController instance;
    private GameObject positionRef;

    [Header("Prefabs")]
    public GameObject prefabEnemy;

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
        positionRef = GameObject.Find("PositionRef");
        Vector3 spawnPosition = new Vector3(6f, 0f, 0f);
        //GameObject enemy = Instantiate<GameObject>(prefabEnemy, spawnPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
