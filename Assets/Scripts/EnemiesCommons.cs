using UnityEngine;
using System.Collections;

public class EnemiesCommons : MonoBehaviour, IStats
{
    [Header("Stats")]
    [SerializeField] protected float detectionRadius = 5f;
    [SerializeField] public Stats stats = new Stats();
    [SerializeField] protected int xp = 10;
    Stats IStats.stats
    {
        get => stats;
        set => stats = value;
    }

    protected Rigidbody2D rb;
    protected Animator anim;
    protected Transform player;
    protected UnityEngine.AI.NavMeshAgent agent;
    protected bool isPushedBack = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    protected IEnumerator PushbackCoroutine(Vector2 direction, float force, float duration)
    {
        isPushedBack = true;
        rb.linearVelocity = direction * force;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isPushedBack = false;
    }

    protected virtual void Animate(float distance, string run, string dirX, string dirY)
    {
        if (isPushedBack) return;
        if (distance < detectionRadius)
        {
            // Move towards the player
            Vector2 lastMoveDirection = (player.position - transform.position).normalized;
            agent.SetDestination(player.position);
            anim.SetBool(run, true);
            if (dirX != null && dirY != null)
            {
                anim.SetFloat(dirX, lastMoveDirection.x);
                anim.SetFloat(dirY, lastMoveDirection.y);
            }
        }
        else
        {
            agent.SetDestination(transform.position);
            anim.SetBool(run, false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    protected void ReceiveAttack(Collider2D other, float pushForce, float pushTime)
    {
        // Get the damage from the attack object
        var attack = other.GetComponent<IDamage>();
        if (attack != null)
        {
            if (attack.type == false)
            { // Physical attack
                stats.vit -= attack.dmg * (1.0f / stats.def);
            }
            else
            { // Magical attack
                stats.vit -= attack.dmg * (1.0f / stats.mdef);
            }
        }
        Debug.Log($"{gameObject.name} took {attack.dmg} damage! Remaining HP: {stats.vit}");
        other.enabled = false;
        // Calculate pushback direction (from enemy to player)    
        Vector2 pushDirection = (transform.position - other.transform.position).normalized;
        StartCoroutine(PushbackCoroutine(pushDirection, pushForce, pushTime));
    }

    protected void XPDrop(){
        PlayerController.instance.XP += xp;
        Debug.Log($"{gameObject.name} dropped {xp} XP! Total XP: {PlayerController.instance.XP}");
    }
    
}