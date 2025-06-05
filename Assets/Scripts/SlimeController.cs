using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour, IDamage
{
    [Header("Stats")]
    [SerializeField] private float vit = 10f;
    [SerializeField] private float _str = 1f;
    [SerializeField] private float mind;
    [SerializeField] private float def;
    [SerializeField] private float mdef;
    [SerializeField] private float mov = 8.0f;
    [SerializeField] private float detectionRadius = 5f;
    public float str => _str;

    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;
    private bool isPushedBack = false;
    private UnityEngine.AI.NavMeshAgent agent;
    
    void OnValidate(){
        if (agent != null){
            agent.speed = mov / 4f; 
        }
    }

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){   
        agent.speed = mov / 4f;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        //rb.linearDamping = 0f;
    }

    // Update is called once per frame
    void Update(){
        if (player != null){
            float distance = Vector2.Distance(transform.position, player.position);
            Animate(distance);
        }
        if (vit <= 0){
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        Debug.Log(gameObject.name + " entered trigger with " + other.gameObject.name);
        // You can add your game logic here, e.g., collect item, activate switch
        if (other.CompareTag("PlayerAttack")){
            // Get the damage from the attack object
            float dmg = 1f;
            var attack = other.GetComponent<SwordAttackController>();
            if (attack != null)
                vit -= attack.dmg;
            Debug.Log($"{gameObject.name} took {attack.dmg} damage! Remaining HP: {vit}");


            // Calculate pushback direction (from enemy to player)    
            Vector2 pushDirection = (transform.position - other.transform.position).normalized;
            float pushForce = 5.0f; // Adjust this value as needed
            StartCoroutine(PushbackCoroutine(pushDirection, pushForce, 0.2f));
        }
    }

    void OnTriggerStay2D(Collider2D other){
        // Debug.Log(gameObject.name + " is staying in trigger with " + other.gameObject.name);
    }

    IEnumerator PushbackCoroutine(Vector2 direction, float force, float duration){
        isPushedBack = true;
        rb.linearVelocity = direction * force;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isPushedBack = false;
    }

    public void Animate(float distance){    
        if (isPushedBack) return;      
        if (distance < detectionRadius){
            // Move towards the player
            Vector2 lastMoveDirection = (player.position - transform.position).normalized;
            agent.SetDestination(player.transform.position);
            anim.SetBool("Run", true);//anim.SetInteger("Run", animDirection(moveInput.normalized));
            anim.SetFloat("X", lastMoveDirection.x);
            anim.SetFloat("Y", lastMoveDirection.y);
        }else{
            agent.SetDestination(transform.position);
            anim.SetBool("Run", false);
            rb.linearVelocity = Vector2.zero;
        }
    }
}
