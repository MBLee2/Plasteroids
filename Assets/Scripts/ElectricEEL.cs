using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;

public class ElectricEEL : MonoBehaviour
{
    public float health = 2f;

    [Header("Attacking")]
[SerializeField] private float ChargeDaly;
public Vector3 GroundCheckPosition;
public float insdideRadius;
public float outsideRadius;

public LayerMask groundLayer;
/*
[SerializeField] private float dashWindup;
[SerializeField] private float rotateSpeed;
[SerializeField] private float dashSpeed;
[SerializeField] private float dashLength;
*/
private enum AttackState { None, Aiming, Attacking};
private AttackState currentState;
private Rigidbody2D rb;
private GameObject playerObject;
public float preCD = 2.0f;
public float CD = 5.0f;

//Sprite Flix
private Vector3 originalScale;
private Vector3 flippedScaleX;
//Animations------------------------------------
[Header("AnimationState")]
private Animator animator;
private bool isDead = false;
private bool trueIsAlive = true;
public float deathAnimTime;
//Get Enemy Script -----------------------------
[Header("GetMovementScript")]
private EnemyScript movementScript;   
//Varaibels Derive From Movement Script 
private bool movingLeft;
private Vector3 normalDirection;
private float speed;

//Death Anima-----------------------------------
public float deathAnimDuration;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
          //Get Components
   rb = GetComponent<Rigidbody2D>();
   animator = GetComponent<Animator>();
   currentState = AttackState.None;
   StartCoroutine(StartAttacking());

  //Get Enemy Movement Scipt
   movementScript = GetComponent<EnemyScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator StartAttacking()
    {
        //PreCD
        yield return new WaitForSeconds(preCD);

        //Get Variabkles from Enemy Movement Script
        movingLeft = movementScript.movingLeft;
        normalDirection = movementScript.normalDirection;
        speed = movementScript.speed;
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator AttackPlayer()
    {
        rb.linearVelocity = Vector3.zero;

        //Show Charging Circle--- Start Charging
        BroadcastMessage("StartCharge");
        animator.SetTrigger("Charge");
        movementScript.currentState = EnemyScript.AttackState.Aiming;
        Debug.Log(movementScript.currentState);

        SoundManager.Sam.playEel();
        yield return new WaitForSeconds(ChargeDaly);

        movementScript.currentState = EnemyScript.AttackState.Attacking;
        Debug.Log(movementScript.currentState);

        playerObject = GameObject.FindGameObjectWithTag("Player");
      
        //Start Attack
        BroadcastMessage("Attack");

        //Inside Circle Charge-------------
        Collider2D[] insideColliders = Physics2D.OverlapCircleAll(transform.position + GroundCheckPosition, insdideRadius, groundLayer);
        foreach (Collider2D collider in insideColliders)
        {
            if (collider.gameObject.tag == "Player")
            {
                playerObject.GetComponent<PlayerScript>().CallDie();
            }
        }

        yield return new WaitForSeconds(0.05f);
        
        //Check OUtside Circle-------------
         Collider2D[] outsideColliders = Physics2D.OverlapCircleAll(transform.position + GroundCheckPosition, outsideRadius, groundLayer);
         foreach (Collider2D collider in outsideColliders)
        {
            if (collider.gameObject.tag == "Player")
            {
                playerObject.GetComponent<PlayerScript>().CallSlowDown();
            }
        }

        //Set Everyingthing Back To Normal
        rb.linearVelocity = normalDirection * speed;
        movementScript.currentState = EnemyScript.AttackState.None;
        movementScript.StartCoroutine(movementScript.ChangeDirection());

        //Post CD
        yield return new WaitForSeconds(CD);


        //Call Attack for looping
        StartCoroutine(AttackPlayer());

        

    }
   void OnDrawGizmos()
 {
     
    //set colore 
    // Gizmos.color = Color.red;
    
     
    //draw sphere

    Gizmos.DrawWireSphere(transform.position + GroundCheckPosition, insdideRadius);
    Gizmos.DrawWireSphere(transform.position + GroundCheckPosition, outsideRadius);

    

 }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Borders")
        {
            health--;
            if(health <= 0)
            {
                DeathAnim(); 
            }
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
            animator.SetTrigger("Death");
            
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
