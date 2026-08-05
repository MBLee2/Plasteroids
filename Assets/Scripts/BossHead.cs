using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class BossHead : MonoBehaviour
{

    //Animations
    [Header("Aimations")]
    private Animator animator;
    public bool isPop = false;
    private int hitCount = -1;
    private SpriteRenderer sr;
    public Sprite[] headSprites = new Sprite[5];

    //Boss Head 2 States and Health
    public bool secondState = false;
    public int health = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.layer = 11;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HeadPop()
    {
        isPop = true;
        animator.SetBool("isPop", true);
    }

    public void HeadMove()
    {
         if (isPop)
        {
            SendMessageUpwards("CallActivateTenticles");
            if (secondState == false)
            {
                health += 3;
                secondState = true;
            }
        }
        Debug.Log("MoveHead");

        isPop = !isPop;
        animator.SetBool("IsUp", isPop);

        gameObject.layer = isPop ? 12 : 11;

        

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            HeadLightScript.hitCount += 1;
            SendMessageUpwards("CallUpdateAnim");
            health -= 1;

            if (health < 1)
            {
               
                if (secondState == true)
                {
                    SendMessageUpwards("Outro");
                }
                else
                {
                 HeadMove();
                }
            }
        }
    }

}
