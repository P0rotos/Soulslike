using UnityEngine;

public class SwordAttackController : MonoBehaviour{
    /*public float rotspeed = 180f;
    public float maxAngle = -180f;*/
    public Transform player;
    public Vector3 offset;
    public float time = 0.2f;
    public float dmg = 1f;
    
    void Start(){
        Destroy(gameObject, time); // Destroy after 3 seconds
    }

    void Update(){
        if (player != null){
            transform.position = player.position + offset;
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
