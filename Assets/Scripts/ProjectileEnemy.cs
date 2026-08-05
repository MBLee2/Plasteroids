using UnityEngine;
using System.Collections;
using UnityEngine.Animations;
using Unity.VisualScripting;

public class ProjectileEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    //Get Enemy Script -----------------------------
    [Header("GetMovementScript")]
    private EnemyScript movementScript;   
    private float speed;
    private bool movingLeft;
    Vector3 normalDirection;



    [Header("Attacking")]
    public float startAttackDelay = 3.0f;
    public float betweenAttackDelay = 2.0f;
    public float rotateSpeed = 90.0f;
    public float aimDelay = 2.0f;
    public float shootingTime = 0.5f;
    private enum AttackState { None, Aiming, Attacking};


    private Rigidbody2D rb;
    private GameObject playerObject;


    // Bullet Spawning
    [Header("BulletSpawn")]
    public GameObject bulletObject;
    public float bulletSpeed;
    public float offAimAmount = 1.0f;

    [Header("Animations")]
    private Animator animator;
    private float deathLength = 14/12f;
    private bool trueIsAlive = true;


    // Update is called once per frame
    void Update()
    {
        
    }


    void Start()
    {
        //Getting the Components -------------------------------------
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        //playerObject = GameObject.Find("Player");

        //Get Enemy Movement Scipt
        movementScript = GetComponent<EnemyScript>();


        //Starting Movement and Attack Corourtine
        StartCoroutine(StartAttacking());
    }

    private IEnumerator StartAttacking()
    {
        yield return new WaitForSeconds(startAttackDelay);

        //Get Variabkles from Enemy Movement Script
        movingLeft = movementScript.movingLeft;
        normalDirection = movementScript.normalDirection;
        speed = movementScript.speed;
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(betweenAttackDelay);

        playerObject = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(AimToPlayer());
        yield return new WaitForSeconds(aimDelay);

        //Tell Movement is Attacking
        movementScript.currentState = EnemyScript.AttackState.Attacking;

        float rotation = transform.rotation.eulerAngles.z + (movingLeft ? 90 : -90) + Random.Range(-offAimAmount, offAimAmount);
        Vector3 finalDirection = new Vector3(0, 0, rotation);
        Instantiate(bulletObject, transform.position, Quaternion.Euler(finalDirection));

        yield return new WaitForSeconds(shootingTime);

        rotation = ReclampAngle(transform.rotation.eulerAngles.z);
        while(Mathf.Abs(rotation) >= 0.1)
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
            DeathAnim();
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
            Invoke(nameof(DestroySelf), deathLength);
        }
    }

    void DestroySelf()
    {
        Destroy(this.gameObject);  
        GameManager.Gary.AddScore(300);
        
    }

}
