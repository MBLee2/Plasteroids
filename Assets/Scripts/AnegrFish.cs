using Unity.VisualScripting;
using UnityEngine;

public class AnegrFish : MonoBehaviour
{

   

[Header("Attacking")]
[SerializeField] private float ChargeDaly;
public Vector3 GroundCheckPosition;
public float insdideRadius;
public float outsideRadius;


public LayerMask groundLayer;

[SerializeField] private float rotateSpeed;
private enum AttackState { None, Aiming, Attacking};
private AttackState currentState;
private Rigidbody2D rb;
private GameObject playerObject;
public float preCD = 2.0f;
public float CD = 5.0f;



[Header("AnimationState")]
private Animator animator;
private bool isDead = false;
private bool trueIsAlive = true;
public float deathAnimTime;

//Get Enemy Script -----------------------------
[Header("GetMovementScript")]
public float speed;


//Death Anima-----------------------------------
public float deathAnimDuration;

public float soundDelay = 1f;


        //Get Player
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
{
   // movementScript.currentState = EnemyScript.AttackState.Attacking;

}
    void Start()
    {
        //Get Components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentState = AttackState.None;

        playerObject = GameObject.FindGameObjectWithTag("Player");
   
   
   

        Invoke("PlaySound", soundDelay);

    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //EnemyScript movementScript = GetComponent<EnemyScript>();
        //bool movingLeft = movementScript.movingLeft;
        //GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
         playerObject = GameObject.FindGameObjectWithTag("Player");

        //Rotate to player
        if (playerObject && trueIsAlive)
        {
         Vector3 playerPosition = playerObject.transform.position;
         Vector3 targetDirection = playerPosition - transform.position;
         float playerAngle = Vector3.Angle(targetDirection, Vector3.right) * (targetDirection.y > 0 ? 1 : -1);



         float currentAngle = transform.rotation.eulerAngles.z; //+ (movingLeft ? 180 : 0);


         float difference = ReclampAngle(playerAngle - currentAngle);

         // Debug.Log(difference);
         transform.Rotate(0, 0, rotateSpeed * Time.deltaTime * (difference > 0 ? 1 : -1));

         //Move Forward

         rb.linearVelocity = transform.right * speed;
        }
    }

    private void PlaySound()
    {
        SoundManager.Sam.playAngler();
        Invoke("PlaySound", soundDelay);
    }

    float ReclampAngle(float angle)
    {
        return ((angle + 180) % 360 + 360) % 360 - 180;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Borders")
        {
            DeathAnim();
        }
    }
    private void DeathAnim()
    {
        if (trueIsAlive)
        {
            gameObject.layer = 11;
            trueIsAlive = false;

            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.linearVelocity = Vector3.zero;
            animator.SetTrigger("Dead");
            
            StopAllCoroutines();
            Invoke(nameof(DestroySelf), deathAnimDuration);
        }
    }

    void DestroySelf()
    {
        Destroy(this.gameObject);  
        GameManager.Gary.AddScore(400);
        
    }


}


