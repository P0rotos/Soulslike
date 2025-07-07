using UnityEngine;

public class BowAttackController : MonoBehaviour, IDamage
{
    /*public float rotspeed = 180f;
    public float maxAngle = -180f;*/
    private Vector3 startPosition;
    public PlayerController _player;
    [SerializeField] private float maxDistance = 2f;
    [SerializeField] private float _dmg = 1f;
    [SerializeField] private Vector3 _offset = Vector3.forward;
    [SerializeField] private bool _type = false; //false == physical, true == magical
    [SerializeField] private float speed = 5f;

    public Vector3 direction = Vector3.right;
    public float dmg { get => _dmg; set => _dmg = value; }
    public Vector3 offset { get => _offset; set => _offset = value; }
    public bool type { get => _type; set => _type = value; }
    public PlayerController player { get => _player; set => _player = value; }

    void Start()
    {
        Debug.Log("SwordAttackController created at: " + Time.time);
        startPosition = transform.position;
        if (player != null)
        {
            Vector3 target = player.transform.position + offset;
            direction = (target - player.transform.position).normalized;
        }
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        if (Vector3.Distance(startPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        Debug.Log("SwordAttackController destroyed at: " + Time.time);
        if (player != null)
        {
            player.endAttackDistance();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other) {
        // Optionally, add a tag check to avoid destroying on player or other arrows
        if (other.gameObject != player.gameObject) {
            Destroy(gameObject);
        }
    }
    /*void Update(){
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z-rotspeed-(Time.deltaTime*rotspeed));
        if (transform.rotation.eulerAngles.z <= maxAngle){
            Debug.Log("SwordAttackController: Destroying sword attack");
            Destroy(gameObject);
        }
    }*/
}
