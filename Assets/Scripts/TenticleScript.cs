using UnityEngine;
public class TenticleScript : MonoBehaviour
{
    //Attacking states and health
    public bool isDown = false;
    public int health = 13;
    public bool isActive = true;


    //Animations
    Animator animator;
    public Sprite[] tenticleSprites = new Sprite[13];
    private SpriteRenderer sr;

    //DeadState
    private bool dead = false;

    //Wiggle Anim
    private Rigidbody2D rb;
    private bool isUp = false;
    public float wiggleSec = 2f;
    public float MoveSpeed = 1.0f;

    //Sound

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       // StartCoroutine(Wiggle());
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        animator.SetBool("IsActive", true);

        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
        //float Speed = isUp ? MoveSpeed : -MoveSpeed;
        //rb.linearVelocityY = Speed;
    }//

   // private IEnumerator Wiggle()
   // {
   //    // Vector3 newPos = new Vector3 (transform.position.x, transform.position.y + MoveSpeed, transform.position.z);
   //    // transform.position = newPos;
////
   //    // yield return new WaitForSeconds(wiggleSec);
////
   //    //   Vector3 nPos = new Vector3 (transform.position.x, transform.position.y - MoveSpeed, transform.position.z);
   //    //  transform.position = nPos;
////
   //    //   yield return new WaitForSeconds(wiggleSec);
////
   //    //  StartCoroutine(Wiggle());
   // }

    public void Attack()
    {
        if (isActive)
        {
            isDown = !isDown;
            animator.SetBool("IsDown", isDown);
            SoundManager.Sam.PlayTenticleMove();
        }
       // else
       // {
       //     SendMessageUpwards("AllTenticleAttack");
       // }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (dead == false)
        {
        
             if (collision.gameObject.tag == "Bullet")
             {
                 health -= 1;
                 UpdateSprite();
             }

             if (health < 1)
             {
                 dead = true;
                 UpdateIsActive();

                 UpdateSprite();
                 health = 13;
                 gameObject.layer = 11;
                 isActive = false;
                 SendMessageUpwards("CallHeadAction");
                 Debug.Log("Activate Head");

             }
        }
    }

    public void UpdateSprite()
    {
        if (health > -1)
        {
          sr.sprite = tenticleSprites[13-health];
        }
    }


// De activate tenticles when head is up
    public void HeadMove()
    {
        isActive = false;


        sr.color = Color.blue;
        gameObject.layer = 11;
    }
//Activate tenticles when head is down
    public void ActivateTenticles()
    {
        if (dead == false)
        {
        isActive = true;
        
        
        sr.color = Color.white;
        gameObject.layer = 12;
        }
    }

    private void UpdateIsActive()
    {
       

    }
}
