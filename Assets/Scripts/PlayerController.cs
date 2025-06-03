using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 lastMoveDirection = Vector2.right;
    public static PlayerController instance;
    private bool isAction = false;
    public Image healthBarFill;

    [Header("Stats")]
    [SerializeField] private float mov;
    [SerializeField] private float str;
    [SerializeField] private float dex;
    [SerializeField] private float mind;
    [SerializeField] private float def;
    [SerializeField] private float mdef;
    [SerializeField] private float vit;
    [SerializeField] private float res;

    [Header("Prefabs")]
    public GameObject prefabAttack;
    public GameObject attackSpawn;

    [Header("Others")]
    [SerializeField] private float time = 0.5f;
    [SerializeField] private float attackOffset = 0.5f;
    [SerializeField] private float dashspeed;
    [SerializeField] private float rollspeed;
    [SerializeField] private float dashDuration = 0.3f; // Duration in seconds
    [SerializeField] private float rollDuration = 0.3f;

    private float speed;
    public float health;
    public Joystick joystick;
    
    void OnValidate(){
        speed = mov / 4f; // Or whatever logic you want
        health = vit;
    }

    void Awake(){
        if (instance != null){
            Debug.Log("PlayerController: Error, ya existe una instancia de player");
            return;
        }
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        instance = this;
    }

    void Start(){
        SetMov(8.0f);
        SetHealth(10.0f);
        dashspeed = 15.0f;
        rollspeed = 8.0f;
        str = 1f;
        attackSpawn = GameObject.Find("Attack_Spawn");
        if (attackSpawn == null){
            Debug.Log("PlayerController: Error, no se ha encontrado el objeto Attack_Spawn");
        }
        joystick = GameObject.Find("Floating Joystick").GetComponent<Joystick>();
        UpdateHealthUI();
    }

    void Update(){   
        if (isAction) return;
        if (health <= 0){
            Destroy(gameObject);
        }
        float h = Input.GetAxis("Horizontal"); //maybe max input joytick to deide movement
        float v = Input.GetAxis("Vertical");
        Vector2 moveInput = new Vector2(h + joystick.Direction.x, v + joystick.Direction.y);
        
        
        if (moveInput.sqrMagnitude > 0.01f){
            lastMoveDirection = moveInput.normalized;
            anim.SetInteger("Run", animDirection(moveInput.normalized));
        }else{
            anim.SetInteger("Run", 0);
        }

        rb.linearVelocity = moveInput * speed;
        Dash();
        Roll();
        Attack(h, v);
        UpdateHealthUI();

        if (health <= 0){            
            Transform mainCamera = transform.Find("Main Camera");
            if (mainCamera != null){
                mainCamera.parent = null;
                // Optionally move the camera to a safe position or show a UI
                Camera cam = mainCamera.GetComponent<Camera>();
                if (cam != null) cam.enabled = true;
            }
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        Debug.Log(gameObject.name + " entered trigger with " + other.gameObject.name);
        // You can add your game logic here, e.g., collect item, activate switch
        if (other.CompareTag("Enemy")){            
            // Get the damage from the attack object
            float dmg = 1f;
            var attack = other.GetComponent<SwordAttackController>();
            if (attack != null)
                dmg = attack.dmg;

            health -= dmg;
            Debug.Log($"{gameObject.name} took {dmg} damage! Remaining HP: {health}");

            // Calculate pushback direction (from enemy to player)    
            Vector2 pushDirection = (transform.position - other.transform.position).normalized;
            float pushForce = 1.0f; // Adjust this value as needed
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

    void Dash(){
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            StartCoroutine(DashCoroutine());
        }
    }

    void Roll(){
        if (Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(RollCoroutine());
        }
    }

    void Attack(float h, float v){
        if (Input.GetMouseButtonDown(0)){
            anim.SetBool("Attack", true);
            Debug.Log("PlayerController: Attack");
            Transform spawnTransform = attackSpawn.transform;
            
            Vector3 offset = (Vector3)lastMoveDirection * attackOffset;
            Vector3 spawnPosition = spawnTransform.position + offset;

            float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject attack = Instantiate<GameObject>(prefabAttack, spawnPosition, rotation);

            var follow = attack.GetComponent<SwordAttackController>();
            if (follow != null){
                follow.player = this;
                follow.offset = offset;
                follow.dmg = str;
                Destroy(attack, time);
            }
        }
    }

    IEnumerator DashCoroutine(){
        isAction = true;
        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(lastMoveDirection.normalized * dashspeed * dashDuration);
        float elapsed = 0f;
        while (elapsed < dashDuration){
            transform.position = Vector3.Lerp(start, end, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        /*rb.linearVelocity = Vector2.zero;
        rb.AddForce(lastMoveDirection * dashspeed, ForceMode2D.Impulse);
        Debug.Log("PlayerController: Dash");

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero; // Stop movement after dash*/
        isAction = false;
    }

    IEnumerator RollCoroutine(){
        isAction = true;
        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(lastMoveDirection.normalized * rollspeed * rollDuration);

        // Store original scale
        Vector3 originalScale = transform.localScale;
        // Reduce height (Y axis)
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);

        float elapsed = 0f;
        while (elapsed < rollDuration){
            transform.position = Vector3.Lerp(start, end, elapsed / rollDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        transform.localScale = originalScale; // Restore original scale
        isAction = false;
        /*isAction = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(lastMoveDirection * rollspeed, ForceMode2D.Impulse);
        Debug.Log("PlayerController: Roll");

        // Store original scale
        Vector3 originalScale = transform.localScale;
        // Reduce height (Y axis)
        transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);

        yield return new WaitForSeconds(rollDuration);

        transform.localScale = originalScale;// Restore original scale
        rb.linearVelocity = Vector2.zero; // Stop movement after roll
        isAction = false;*/
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

    public void SetMov(float m){
        mov = m;
        speed = mov / 4f;
    }

    public void SetHealth(float h){
        vit = h;
        health = vit;
    }

    void UpdateHealthUI(){
        // Update the fill amount of the health bar
        if (healthBarFill != null){
            healthBarFill.fillAmount = health / vit;
        }
    }

    int animDirection(Vector2 dir){
        // Example: 0 = right, 1 = up, 2 = left, 3 = down    
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? 1 : 2; // 1: 2
        else
            return dir.y > 0 ? 3 : 4; // 3 : 4
    }
    public void OnAttackEnded() {
        anim.SetBool("Attack", false);
    }
}
