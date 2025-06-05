using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SlimeBossController : MonoBehaviour, IDamage
{
    [Header("Stats")]
    [SerializeField] private float vit = 10f;
    [SerializeField] private float _str = 2f;
    [SerializeField] private float mind;
    [SerializeField] private float def;
    [SerializeField] private float mdef;
    [SerializeField] private float mov = 8.0f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float attackTimer = 5f; 
    public float str => _str;
    private bool attackflag = false;

    CapsuleCollider2D boxCollider;
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
        boxCollider = GetComponent<CapsuleCollider2D>();
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
            anim.SetBool("Died", true);
            return;
        }
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (anim.GetInteger("Attack") == 2 && state.IsName("attackDown") && state.normalizedTime >= 1f) {
            anim.SetInteger("Attack", 0);
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        Debug.Log(gameObject.name + " entered trigger with " + other.gameObject.name);
        // You can add your game logic here, e.g., collect item, activate switch
        if (other.CompareTag("PlayerAttack")){
            // Get the damage from the attack object
            var attack = other.GetComponent<SwordAttackController>();
            if (attack != null)
                vit -= attack.dmg;
            Debug.Log($"{gameObject.name} took {attack.dmg} damage! Remaining HP: {vit}");


            // Calculate pushback direction (from enemy to player)    
            Vector2 pushDirection = (transform.position - other.transform.position).normalized;
            float pushForce = 4.0f; // Adjust this value as needed
            StartCoroutine(PushbackCoroutine(pushDirection, pushForce, 0.2f));
        }
    }

    IEnumerator PushbackCoroutine(Vector2 direction, float force, float duration){
        isPushedBack = true;
        rb.linearVelocity = direction * force;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isPushedBack = false;
    }

    public void Animate(float distance){   
        if (distance < detectionRadius){
            attackTimer -= Time.deltaTime;
            if (isPushedBack) return;  
            if (!attackflag){
                if (attackTimer <= 0f){
                    anim.SetInteger("Attack", 1);
                    mov = 10.0f;
                    agent.speed = mov / 4f; 
                    attackflag = true;
                    attackTimer = 3f; 
                }
            }else{                    
                if (attackTimer <= 0f){
                    anim.SetInteger("Attack", 2);
                    mov = 8.0f;
                    agent.speed = mov / 4f; 
                    attackflag = false;
                    attackTimer = 5f; 
                }
            }
            Vector2 lastMoveDirection = (player.position - transform.position).normalized;
            agent.SetDestination(player.transform.position);
            anim.SetBool("Move", true);
        }else{
            if (isPushedBack) return;  
            agent.SetDestination(transform.position);
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("Move", false);
            anim.SetInteger("Attack", 2);
            mov = 8.0f;
            agent.speed = mov / 4f; 
            attackflag = false;
            attackTimer = 5f;
        }
    }

    void OnDestroy() {
        SceneManager.LoadSceneAsync(0);
    }
    
    public void Dead(){
        Destroy(gameObject);
    }
    public void ColliderOnOff(){
        boxCollider.enabled = !boxCollider.enabled;
    }
}
