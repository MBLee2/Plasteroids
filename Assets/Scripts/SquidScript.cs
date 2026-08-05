using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SquidScript : MonoBehaviour
{
    private EnemyScript movementScript;  
    private float speed;
    private bool movingLeft;
    Vector3 normalDirection;

    public float health = 3f;


    [Header("Attacking")]
    public float startAttackDelay = 3.0f;
    public float betweenAttackDelay = 4.0f;
    public float attackLength = 1.0f;
    public float timeBetweenShots = 0.2f;
    public float inkSpread = 0.5f;

    private Rigidbody2D rb;


    // Bullet Spawning
    [Header("InkSpawn")]
    public GameObject inkObject;

    [UnitHeaderInspectable("Animations")]
    private Animator animator;
    private float deathLength = 0.55f;
    private bool trueIsAlive = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Getting the Components -------------------------------------
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        //Tell Movement is Attacking
        movementScript.currentState = EnemyScript.AttackState.Attacking;
        rb.linearVelocity = normalDirection * speed;

        StartCoroutine(ShootInk());
        yield return new WaitForSeconds(attackLength);

        //Tell Movement is not attacking
        movementScript.currentState = EnemyScript.AttackState.None;

        movementScript.StartCoroutine(movementScript.ChangeDirection());
        StartCoroutine(AttackPlayer());
    }

    private IEnumerator ShootInk()
    {
        while(movementScript.currentState == EnemyScript.AttackState.Attacking)
        {
            Vector3 finalDirection = new Vector3(0, 0, movingLeft ? -90 : 90);
            Vector3 spawnPosition = transform.position;
            spawnPosition.y += Random.Range(-inkSpread, inkSpread);
            spawnPosition.z += 1;
            Instantiate(inkObject, spawnPosition, Quaternion.Euler(finalDirection));
            SoundManager.Sam.playSquid();
            yield return new WaitForSeconds(timeBetweenShots);
        }
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
            animator.SetTrigger("Die");
            
            StopAllCoroutines();
            Invoke(nameof(DestroySelf), deathLength);
        }
    }

    void DestroySelf()
    {
        Destroy(this.gameObject);  
        GameManager.Gary.AddScore(400);
    }
}
