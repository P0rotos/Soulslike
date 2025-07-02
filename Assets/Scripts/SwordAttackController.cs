using UnityEngine;

public class SwordAttackController : MonoBehaviour, IDamage{
    /*public float rotspeed = 180f;
    public float maxAngle = -180f;*/
    public PlayerController player;
    [SerializeField] private float _dmg = 1f;
    [SerializeField] private Vector3 _offset = Vector3.forward;

    public float dmg => _dmg;
    public Vector3 offset => _offset;

    public float time = 0.2f;
    
    void Start(){
        Debug.Log("SwordAttackController created at: " + Time.time);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        player = playerObj.GetComponent<PlayerController>();
        Destroy(GameObject, time);
    }
    
    void Update(){
        if (player.transform != null){
            transform.position = player.transform.position + offset;
        }
    }

    void OnDestroy() {
        Debug.Log("SwordAttackController destroyed at: " + Time.time);
        if (player != null) {
            player.OnAttackEnded();
        }
    }
    /*void Update(){
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z-rotspeed-(Time.deltaTime*rotspeed));
        if (transform.rotation.eulerAngles.z <= maxAngle){
            Debug.Log("SwordAttackController: Destroying sword attack");
            Destroy(gameObject, time);
        }
    }*/
}
