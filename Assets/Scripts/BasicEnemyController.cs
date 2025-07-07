using UnityEngine;
using System.Collections;

public class BasicEnemyController : EnemiesCommons
{
    
    void OnValidate(){
        if (agent != null){
            agent.speed = stats.mov / 4f; 
        }
    }

    protected override void Awake(){
        base.Awake(); 
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
            Animate(distance, "Run", "X", "Y");
        }
        if (stats.vit <= 0){
            XPDrop();
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        Debug.Log(gameObject.name + " entered trigger with " + other.gameObject.name);

        if (other.CompareTag("PlayerAttack")){
            ReceiveAttack(other, 5.0f, 0.2f);
        }
    }


}
