using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float verticalMoveDelay;
    public float speed;
    public bool movingLeft;
    public Vector3 normalDirection;
    private enum VerticalDirection { None, Up, Down };
    private VerticalDirection vertDir;
    private float maxX = 11f, maxY = 7f;

    public enum AttackState { None, Aiming, Attacking};
    [Header("Attacking")]
    public AttackState currentState;

    private Rigidbody2D rb;

    //Sprite Flix
    private Vector3 originalScale;
    private Vector3 flippedScaleX;

    //Animations------------------------------------

    [Header("AnimationState")]
    private Animator animator;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        movingLeft = Random.Range(-1, 1) < 0;
        originalScale = transform.localScale;
        flippedScaleX = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        transform.localScale = movingLeft ? flippedScaleX : originalScale;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        //Set Left or Right Direction
        speed = Random.Range(minSpeed, maxSpeed);
        //movingLeft = Random.Range(-1, 1) < 0;
        normalDirection = movingLeft ? Vector3.left : Vector3.right;

        //Flip Sprite
       // originalScale = transform.localScale;
       // flippedScaleX = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
       // transform.localScale = movingLeft ? originalScale : flippedScaleX;


        //Movements
        rb.linearVelocity = normalDirection * speed;
        vertDir = VerticalDirection.None;


        StartCoroutine(ChangeDirection());

        //Attacking

        //currentState = AttackState.None;
        //tartCoroutine(AttackPlayer());
    }

    void FixedUpdate()
    {
        if(Mathf.Abs(transform.position.x) > maxX || Mathf.Abs(transform.position.y) > maxY)
        {
            Destroy(this.gameObject);
        }

    }

    public IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(verticalMoveDelay);
        if(currentState == AttackState.None)
        {
            VerticalDirection newDirection = (VerticalDirection) Random.Range(-1, 3);
            if(newDirection != vertDir)
            {
                if(newDirection == VerticalDirection.None) 
                { 
                    rb.linearVelocityY = 0;
                }
                else
                {
                    rb.linearVelocityY = Random.Range(minSpeed, maxSpeed) * (newDirection == VerticalDirection.Up ? 1 : -1);
                }

                vertDir = newDirection;
            }
            StartCoroutine(ChangeDirection());
        }
    }
/*
    private IEnumerator AttackPlayer()
    {

        yield return new WaitForSeconds(dashDelay);
        playerObject = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(AimToPlayer());
        yield return new WaitForSeconds(dashWindup - 1f);
        SoundManager.Sam.playSwordfish();
        yield return new WaitForSeconds(1f);
        currentState = AttackState.Attacking;
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


        currentState = AttackState.None;
        StartCoroutine(ChangeDirection());
        StartCoroutine(AttackPlayer());
    }


    private IEnumerator AimToPlayer()
    {
        currentState = AttackState.Aiming;
        rb.linearVelocity = Vector3.zero;

        while(currentState == AttackState.Aiming)
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


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Borders")
        {
            if(!(currentState == AttackState.Attacking && collision.gameObject.tag == "Trash"))
            {
                isDead = true;
                GameManager.Gary.AddScore(200);
            }
            if (collision.gameObject.tag == "Bullet")
            {
                DeathAnim();
            }
        }
    }

    float ReclampAngle(float angle)
{
    return ((angle + 180) % 360 + 360) % 360 - 180;
}
    public void DeathAnim()
    {
         gameObject.layer = 11;
         rb.constraints = RigidbodyConstraints2D.FreezeAll;
         animator.SetTrigger("SwordFishDie");
         Invoke(nameof(DestroySelf), 1.8f);
    }
    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
    */
}


