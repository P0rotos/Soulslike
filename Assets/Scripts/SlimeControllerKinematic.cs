using UnityEngine;
using System.Collections;

public class SlimeControllerKinematic : MonoBehaviour, IDamage
{
    [Header("Stats")]
    [SerializeField] private float vit;
    [SerializeField] private float _str = 1f;
    [SerializeField] private float mind;
    [SerializeField] private float def;
    [SerializeField] private float mdef;
    [SerializeField] private float mov;
    public float str => _str;

    private Rigidbody2D rb;
    private Animator anim;
    public float speed;
    public float detectionRadius = 5f; // Only chase if player is within this distance
    private Transform player;
    
    void OnValidate(){
        speed = mov / 4f; // Or whatever logic you want
    }

    public void SetMov(float m){
        mov = m;
        speed = mov / 4f;
    }

    void Awake(){
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){   
        SetMov(8.0f);     
        vit = 10.0f;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        //rb.linearDamping = 0f;
    }

    // Update is called once per frame
    void Update(){
        if (player != null){
            float distance = Vector2.Distance(transform.position, player.position);
            //if (distance < detectionRadius){
                // Move towards the player
            //    Vector2 lastMoveDirection = (player.position - transform.position).normalized;
            //    transform.position = Vector2.MoveTowards(
            //        transform.position,
            //        player.position,
            //        speed * Time.deltaTime
            //    );
            //    anim.SetInteger("Run", animDirection(lastMoveDirection));
            //}else{
            //    anim.SetInteger("Run", 0);
            //}
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
            float pushForce = 2.0f; // Adjust this value as needed
            StartCoroutine(PushbackCoroutine(pushDirection, pushForce, 0.1f));
        }
        if (other.CompareTag("Enemy")){
            // Calculate pushback direction (from enemy to player)    
            Vector2 pushDirection = (transform.position - other.transform.position).normalized;
            float pushForce = 0.1f; // Adjust this value as needed
            StartCoroutine(PushbackCoroutine(pushDirection, pushForce, 0.1f));
        }
    }

    void OnTriggerExit2D(Collider2D other){
        Debug.Log(gameObject.name + " exited trigger with " + other.gameObject.name);
    }

    // Optional: Called every frame while the trigger is overlapping
    void OnTriggerStay2D(Collider2D other){
        // Debug.Log(gameObject.name + " is staying in trigger with " + other.gameObject.name);
    }

    IEnumerator PushbackCoroutine(Vector2 direction, float distance, float duration){
        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(direction * distance);
        float elapsed = 0f;
        while (elapsed < duration){
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }

    public void Animate(float distance){        
        if (distance < detectionRadius){
            // Move towards the player
            Vector2 lastMoveDirection = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
            anim.SetBool("Run", true);//anim.SetInteger("Run", animDirection(moveInput.normalized));
            anim.SetFloat("X", lastMoveDirection.x);
            anim.SetFloat("Y", lastMoveDirection.y);
        }else{
            anim.SetBool("Run", false);
        }
    }
}
