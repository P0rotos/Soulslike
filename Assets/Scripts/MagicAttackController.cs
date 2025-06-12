using UnityEngine;

public class SwordAttackController : MonoBehaviour{
    /*public float rotspeed = 180f;
    public float maxAngle = -180f;*/
    public PlayerController player;
    public Vector3 offset;
    public float dmg = 1f;
    
    void Start(){
        Debug.Log("SwordAttackController created at: " + Time.time);
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
            Destroy(gameObject);
        }
    }*/
}
