using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IStats
{
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 lastMoveDirection = Vector2.right;
    public static PlayerController instance;
    private bool isAction = false;
    public Image healthBarFill;

    [Header("Stats")]
    [SerializeField] protected float detectionRadius = 5f;
    [SerializeField] public Stats stats = new Stats();
    Stats IStats.stats => stats;

    [Header("Prefabs")]
    public GameObject prefabAttack;

    [Header("Others")]
    [SerializeField] private float attackOffset = 0.5f;
    [SerializeField] private float dashspeed = 15.0f;
    [SerializeField] private float rollspeed = 8.0f;
    [SerializeField] private float dashDuration = 0.3f; // Duration in seconds
    [SerializeField] private float rollDuration = 0.3f;

    private float speed;
    private char attackType = 0;
    public float health;
    public Joystick joystick;
    GameObject attack;
    private bool attackFlag = false;
    private bool isPushedBack = false;
    private float collisionDamageTimer = 0f;
    
    void OnValidate(){
        speed = stats.mov / 4f; // Or whatever logic you want
        health = stats.vit;
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
        SetHealth(20.0f);
        joystick = GameObject.Find("Fixed Joystick").GetComponent<Joystick>();
        UpdateHealthUI();
    }
    
    void FixedUpdate(){        
        if (isAction) return;
        if (health <= 0){
            Destroy(gameObject);
        }
        float h = Input.GetAxis("Horizontal"); //maybe max input joytick to deide movement
        float v = Input.GetAxis("Vertical");
        Vector2 moveInput = new Vector2(h + joystick.Direction.x, v + joystick.Direction.y);
        
        Animate(moveInput, h + joystick.Direction.x, v + joystick.Direction.y);

        if (Input.GetKeyDown(KeyCode.LeftShift)){
            Dash();
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            Roll();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl)){
            attackType = (attackType+1)%4;
            if(attackType == 0){
                prefabAttack = Resources.Load<GameObject>("SwordAttack");
            }
            if(attackType == 1){
                prefabAttack = Resources.Load<GameObject>("MagicAttack");
            }
            if(attackType == 2){
                prefabAttack = Resources.Load<GameObject>("DaggerAttack");
            }
            if(attackType == 3){
                prefabAttack = Resources.Load<GameObject>("BowAttack");
            }
        }
    }

    void Update(){   
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

    void OnCollisionEnter2D(Collision2D collision){
        Debug.Log(gameObject.name + " collided with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy")) {
            var attack = collision.gameObject.GetComponent<IStats>();
            if (attack != null)
                health -= attack.stats.str;
            Debug.Log($"{gameObject.name} took {attack.stats.str} damage! Remaining HP: {health}");

            // Calculate pushback direction (from enemy to player)
            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
            float pushForce = 5.0f;
            StartCoroutine(PushbackCoroutine(pushDirection, pushForce, 0.2f));
        }
    }
    void OnCollisionStay2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Enemy")) {
            collisionDamageTimer += Time.fixedDeltaTime;
            if (collisionDamageTimer >= 0.5f) {
                var attack = collision.gameObject.GetComponent<IDamage>();
                if (attack != null) {
                    health -= attack.str;
                    Debug.Log($"{gameObject.name} is taking periodic damage! Remaining HP: {health}");
                }
                collisionDamageTimer = 0f; // Reset timer after applying damage
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision){
        if (collision.gameObject.CompareTag("Enemy")) {
            collisionDamageTimer = 0f;
        }
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
            // Snap lastMoveDirection to the nearest cardinal direction
            Vector2 dir = lastMoveDirection;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y)) {
                dir = new Vector2(Mathf.Sign(dir.x), 0); // Left or Right
            } else {
                dir = new Vector2(0, Mathf.Sign(dir.y)); // Up or Down
            }
                
            Vector3 offset = (Vector3)dir * attackOffset;
            Vector3 spawnPosition = transform.position + offset;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            attack = Instantiate<GameObject>(prefabAttack, spawnPosition, rotation);

            if (dir.y != 0 || dir.x < 0) {
                Vector3 scale = attack.transform.localScale;
                attack.transform.localScale = new Vector3(scale.x, -Mathf.Abs(scale.y), scale.z);
            }

            var follow = attack.GetComponent<IDamage>();
            if (follow != null){
                //switchcase de ataques y cambiar stats de player acorde al ataque (quizas cambiar stats arriba en el ctrlizquierdo)
                follow.player = this;
                follow.offset = offset;
                follow.dmg = stats.str;
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
        rb.linearVelocity = lastMoveDirection.normalized * dashspeed;
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = Vector2.zero;
        isAction = false;
    }

    IEnumerator RollCoroutine(){
        isAction = true;
        anim.SetBool("Roll", true);
        rb.linearVelocity = lastMoveDirection.normalized * rollspeed;
        yield return new WaitForSeconds(rollDuration);
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Roll", false);
        isAction = false;
    }

    IEnumerator PushbackCoroutine(Vector2 direction, float force, float duration){
        isPushedBack = true;
        rb.linearVelocity = direction * force;
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isPushedBack = false;
    }

    public void SetMov(float m){
        stats.mov = m;
        speed = stats.mov / 4f;
    }

    public void SetHealth(float h){
        stats.vit = h;
        health = stats.vit;
    }

    void UpdateHealthUI(){
        // Update the fill amount of the health bar
        if (healthBarFill != null){
            float minFill = 0.12f;
            float maxFill = 0.9f;
            float t = Mathf.Clamp01(health / stats.vit);
            healthBarFill.fillAmount = Mathf.Lerp(minFill, maxFill, t);
        }
    }

    public void Animate(Vector2 moveInput, float h, float v){     
        if (isPushedBack) return;     
        if (moveInput.sqrMagnitude > 0.01f){
            lastMoveDirection = moveInput.normalized;
            anim.SetBool("Run", true);//anim.SetInteger("Run", animDirection(moveInput.normalized));
            anim.SetFloat("X", h);
            anim.SetFloat("Y", v);
            rb.linearVelocity = moveInput * speed;
        }else{
            anim.SetBool("Run", false);//anim.SetInteger("Run", 0);
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void OnAttackEnded() {
        anim.SetBool("Attack", false);
    }
    void OnDestroy() {
        SceneManager.LoadSceneAsync(0);
    }
}
