using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float str = 3.0f;
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
    GameObject attack;
    private bool attackFlag = false;
    
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
        attackSpawn = GameObject.Find("Attack_Spawn");
        if (attackSpawn == null){
            Debug.Log("PlayerController: Error, no se ha encontrado el objeto Attack_Spawn");
        }
        joystick = GameObject.Find("Fixed Joystick").GetComponent<Joystick>();
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
        
        Animate(moveInput, h + joystick.Direction.x, v + joystick.Direction.y);

        Vector2 targetPosition = rb.position + moveInput * speed * Time.fixedDeltaTime;
        
        // Check for collision at the target position
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        filter.useTriggers = false;

        RaycastHit2D[] results = new RaycastHit2D[1];
        int hitCount = rb.Cast(moveInput.normalized, filter, results, (moveInput * speed * Time.fixedDeltaTime).magnitude);

        if (hitCount == 0) {
            rb.MovePosition(targetPosition);
        } 

        //rb.linearVelocity = moveInput * speed;
        if (Input.GetKeyDown(KeyCode.LeftShift)){
            Dash();
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            Roll();
        }
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
            var attack = other.GetComponent<IDamage>();
            if (attack != null)
                health -= attack.str;
            Debug.Log($"{gameObject.name} took {attack.str} damage! Remaining HP: {health}");

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

    public void Dash(){
        StartCoroutine(DashCoroutine());
    }

    public void Roll(){
        StartCoroutine(RollCoroutine());
    }

    public void Attack(){
        if(!attackFlag){
            anim.SetBool("Attack", true);
            attackFlag=true;
            Debug.Log("PlayerController: Attack");
            Transform spawnTransform = attackSpawn.transform;

            // Snap lastMoveDirection to the nearest cardinal direction
            Vector2 dir = lastMoveDirection;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) {
                dir = new Vector2(Mathf.Sign(dir.x), 0); // Left or Right
            } else {
                dir = new Vector2(0, Mathf.Sign(dir.y)); // Up or Down
            }
                
            Vector3 offset = (Vector3)dir * attackOffset;
            Vector3 spawnPosition = spawnTransform.position + offset;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            attack = Instantiate<GameObject>(prefabAttack, spawnPosition, rotation);

            if (dir.y != 0 || dir.x < 0) {
                Vector3 scale = attack.transform.localScale;
                attack.transform.localScale = new Vector3(scale.x, -Mathf.Abs(scale.y), scale.z);
            }

            var follow = attack.GetComponent<SwordAttackController>();
            if (follow != null){
                follow.player = this;
                follow.offset = offset;
                follow.dmg = str;
                //Destroy(attack, time);
            }
        }
    }

    public void endAttack(){
        anim.SetBool("Attack", false);
        attackFlag = false;
        Destroy(attack);
    }

    IEnumerator DashCoroutine(){
        isAction = true;
        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(lastMoveDirection.normalized * dashspeed * dashDuration);
        float elapsed = 0f;
        while (elapsed < dashDuration){
            rb.MovePosition(Vector3.Lerp(start, end, elapsed / dashDuration));//transform.position = Vector3.Lerp(start, end, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        //transform.position = end;
        /*rb.linearVelocity = Vector2.zero;
        rb.AddForce(lastMoveDirection * dashspeed, ForceMode2D.Impulse);
        Debug.Log("PlayerController: Dash");

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero; // Stop movement after dash*/
        isAction = false;
    }

    IEnumerator RollCoroutine(){
        isAction = true;
        anim.SetBool("Roll", true);
        Vector3 start = transform.position;
        Vector3 end = start + (Vector3)(lastMoveDirection.normalized * rollspeed * rollDuration);

        // Store original scale
        //Vector3 originalScale = transform.localScale;
        // Reduce height (Y axis)
        //transform.localScale = new Vector3(originalScale.x, originalScale.y * 0.5f, originalScale.z);

        float elapsed = 0f;
        while (elapsed < rollDuration){
            rb.MovePosition(Vector3.Lerp(start, end, elapsed / rollDuration));//transform.position = Vector3.Lerp(start, end, elapsed / rollDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;

        //transform.localScale = originalScale; // Restore original scale
        anim.SetBool("Roll", false);
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
            float minFill = 0.12f;
            float maxFill = 0.9f;
            float t = Mathf.Clamp01(health / vit);
            healthBarFill.fillAmount = Mathf.Lerp(minFill, maxFill, t);
        }
    }

    public void Animate(Vector2 moveInput, float h, float v){        
        if (moveInput.sqrMagnitude > 0.01f){
            lastMoveDirection = moveInput.normalized;
            anim.SetBool("Run", true);//anim.SetInteger("Run", animDirection(moveInput.normalized));
            anim.SetFloat("X", h);
            anim.SetFloat("Y", v);
        }else{
            anim.SetBool("Run", false);//anim.SetInteger("Run", 0);
        }
    }

    public void OnAttackEnded() {
        anim.SetBool("Attack", false);
    }
    void OnDestroy() {
        SceneManager.LoadSceneAsync(0);
    }
}
