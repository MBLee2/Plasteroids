using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public class SwordFish : MonoBehaviour
{
 

    [Header("Attacking")]
    [SerializeField] private float startDelay;
    [SerializeField] private float dashDelay;
    [SerializeField] private float dashWindup;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashLength;
    private enum AttackState { None, Aiming, Attacking};
    private AttackState currentState;


    private Rigidbody2D rb;
    private GameObject playerObject;


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

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        //Get Components
         rb = GetComponent<Rigidbody2D>();
         animator = GetComponent<Animator>();

        currentState = AttackState.None;
        StartCoroutine(StartAttacking());

    }

    private IEnumerator StartAttacking()
    {
        yield return new WaitForSeconds(startDelay);

        //Get Enemy Movement Scipt
        movementScript = GetComponent<EnemyScript>();

        //Get Variabkles from Enemy Movement Script
        movingLeft = movementScript.movingLeft;
        normalDirection = movementScript.normalDirection;
        speed = movementScript.speed;

        StartCoroutine(AttackPlayer());
    }


    private IEnumerator AttackPlayer()
    { 
        yield return new WaitForSeconds(dashDelay);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(AimToPlayer());
        yield return new WaitForSeconds(dashWindup - 1f);
        SoundManager.Sam.playSwordfish();
        yield return new WaitForSeconds(1f);

        //Tell Movement is Attacking
        movementScript.currentState = EnemyScript.AttackState.Attacking;

        //Dash Start
        animator.SetBool("Dashing", true);
        
        float rotation = transform.rotation.eulerAngles.z;
        float pointDirection = Mathf.Deg2Rad * (rotation + (movingLeft ? 180 : 0));
        Vector3 attackDirection = new Vector3(Mathf.Cos(pointDirection), Mathf.Sin(pointDirection), 0);
        rb.linearVelocity = attackDirection * dashSpeed;
        yield return new WaitForSeconds(dashLength);
        //Dash Ends
        if (isDead == true)
        {
            DeathAnim();
        }
        animator.SetBool("Dashing", false);
        rb.linearVelocity = Vector3.zero;
        rotation = ReclampAngle(transform.rotation.eulerAngles.z);
        while(Mathf.Abs(rotation) > 0)
        {
            if(Mathf.Abs(rotation) < 3)
            {
                transform.Rotate(-transform.rotation.eulerAngles);
            } else
            {
                transform.Rotate(0, 0, 2 * rotateSpeed * Time.deltaTime * (rotation > 0 ? -1 : 1));
            }
            yield return new WaitForSeconds(Time.deltaTime);
            rotation = ReclampAngle(transform.rotation.eulerAngles.z);
        }
        rb.linearVelocity = normalDirection * speed;

        //Tell Movement is not attacking
        movementScript.currentState = EnemyScript.AttackState.None;

        //currentState = AttackState.None;
        movementScript.StartCoroutine(movementScript.ChangeDirection());
        StartCoroutine(AttackPlayer());
    }
  private IEnumerator AimToPlayer()
  {

    //Change State To Aiming
    movementScript.currentState = EnemyScript.AttackState.Aiming;

    //currentState = AttackState.Aiming;
    rb.linearVelocity = Vector3.zero;
    while(movementScript.currentState == EnemyScript.AttackState.Aiming)
    {
        if (playerObject)
        {
            Vector3 playerPosition = playerObject.transform.position;
            Vector3 targetDirection = playerPosition - transform.position;
            float playerAngle = Vector3.Angle(targetDirection, Vector3.right) * (targetDirection.y > 0 ? 1 : -1);
            float currentAngle = transform.rotation.eulerAngles.z + (movingLeft ? 180 : 0);
            float difference = ReclampAngle(playerAngle - currentAngle);
            // Debug.Log(difference);
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime * (difference > 0 ? 1 : -1));
        }
        yield return new WaitForSeconds(Time.deltaTime);
    }
  }

    float ReclampAngle(float angle)
    {
        return ((angle + 180) % 360 + 360) % 360 - 180;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Borders")
        {
            if(currentState != AttackState.Attacking)
            {
                DeathAnim();
            } else if(currentState == AttackState.Attacking && (collision.gameObject.tag == "Trash" || collision.gameObject.tag == "Player"))
            {
                isDead = true;
            } else if (collision.gameObject.tag == "Bullet")
            {
                DeathAnim();
            }
        }
    }
    public void DeathAnim()
    {
        if (trueIsAlive)
        {
            gameObject.layer = 11;
            trueIsAlive = false;

            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.linearVelocity = Vector3.zero;
            animator.SetTrigger("Die");
            
            StopAllCoroutines();
            Invoke(nameof(DestroySelf), deathAnimTime);
        }
    }
    private void DestroySelf()
    {
        GameManager.Gary.AddScore(200);
        Destroy(this.gameObject);
    }
}
