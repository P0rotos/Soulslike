using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SlimeBossController : EnemiesCommons
{
    [Header("Stats")]
    [SerializeField] private float attackTimer = 5f; 
    private bool attackflag = false;

    CapsuleCollider2D boxCollider;
    
    void OnValidate(){
        if (agent != null){
            agent.speed = stats.mov / 4f; 
        }
    }

    protected override void Awake(){
        base.Awake(); 
        boxCollider = GetComponent<CapsuleCollider2D>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start(){   
        agent.speed = stats.mov / 4f;   
        base.Start();
        //rb.linearDamping = 0f;
    }

    // Update is called once per frame
    void Update(){
        if (player != null){
            float distance = Vector2.Distance(transform.position, player.position);
            Animate(distance, "Move", null, null);
        }
        if (stats.vit <= 0){
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
            ReceiveAttack(other, 4.0f, 0.2f);
        }
    }

    protected override void Animate(float distance, string run, string dirX, string dirY){   
        base.Animate(distance, run, dirX, dirY);
        if (distance < detectionRadius){
            attackTimer -= Time.deltaTime;
            if (isPushedBack) return;  
            if (!attackflag){
                if (attackTimer <= 0f){
                    anim.SetInteger("Attack", 1);
                    stats.mov = 10.0f;
                    agent.speed = stats.mov / 4f; 
                    attackflag = true;
                    attackTimer = 3f; 
                }
            }else{                    
                if (attackTimer <= 0f){
                    anim.SetInteger("Attack", 2);
                    stats.mov = 8.0f;
                    agent.speed = stats.mov / 4f; 
                    attackflag = false;
                    attackTimer = 5f; 
                }
            }
        }else{
            if (isPushedBack) return;  
            anim.SetInteger("Attack", 2);
            stats.mov = 8.0f;
            agent.speed = stats.mov / 4f; 
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
