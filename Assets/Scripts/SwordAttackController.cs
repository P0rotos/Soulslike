using UnityEngine;

public class SwordAttackController : MonoBehaviour, IDamage{
    /*public float rotspeed = 180f;
    public float maxAngle = -180f;*/
    public PlayerController _player;
    [SerializeField] private float _dmg = 1f;
    [SerializeField] private Vector3 _offset = Vector3.forward;
    [SerializeField] private bool _type = false; //false == physical, true == magical

    public float dmg { get => _dmg; set => _dmg = value; }
    public Vector3 offset { get => _offset; set => _offset = value; }
    public bool type { get => _type; set => _type = value; }
    public PlayerController player { get => _player; set => _player = value; }
    
    void Start(){
        Debug.Log("SwordAttackController created at: " + Time.time);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        _player = playerObj.GetComponent<PlayerController>();
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
