using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Diagnostics.Contracts;
using System.Collections;
using UnityEngine.Animations;


public class PlayerScript : MonoBehaviour
{
   
    //Rotation Input --------------------
    [Header("Rotaiton")]
    private float RotateDirection;
    public InputActionReference RotateInput;
    public float rotateSpeed;
    private Vector3 originalScale, flippedScale;


    //Thrust Force ---------------------
    [Header("Thrust")]
    private Rigidbody2D rb;
    bool isThrusting;
    public InputActionReference ThrustInput;
    public float ForceAmount;


    //Bullet Shooting -------------------
    [Header("Shooting")]
    public float distanceFromOrigin;
    public InputActionReference ShootInput;
    public GameObject BulletObject;
    private bool shootCD;
    public float shootCDTime;

    [Header("Animation")]
    public RuntimeAnimatorController[] animators;
    private Animator animator;
    public float respawnTime = 2.0f;

    [Header("Sound")]
    public AudioSource moveStart;
    public AudioSource moveMiddle;
    public AudioSource  moveEnd;
    public AudioSource  shoot;

    [Header("Debuffs")]
    public float stunDuration;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootCD = false;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Debug.Log(GameManager.Gary.level - 1);
        animator.runtimeAnimatorController = animators[GameManager.Gary.level - 1];

        //Flip the Scale when upside down
       //originalScale = transform.localScale;
       //flippedScale = new Vector3(originalScale.x, -originalScale.y, originalScale.z);

        rotateSpeed = 3.0f;
        ActionEnabled();
        StartCoroutine(RespawnInvincibility());
    }

    void SetIndestructible()
    {
        gameObject.layer = LayerMask.NameToLayer("Indestructable");
        Debug.Log(gameObject.layer);
    }

    void SetHittable()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        Debug.Log(gameObject.layer);
    }

    private IEnumerator RespawnInvincibility()
    {
        SetIndestructible();
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.25f);
            sprite.enabled = false;
            yield return new WaitForSeconds(0.25f);
            sprite.enabled = true;
        }
        SetHittable();
    }


    void ActionEnabled()
    {
        RotateInput.action.Enable();
        ThrustInput.action.Enable();
        ShootInput.action.Enable();
    }

    void ActionDisabled()
    {
        RotateInput.action.Disable();
        ThrustInput.action.Disable();
        ShootInput.action.Disable();
    }


    // Update is called once per frame
    void Update()
    {
        if(GameManager.Gary.currentState == GameState.Playing){
        // Shoot Bullet
        if (ShootInput.action.triggered)
        {
            ShootBullet();
        }

        float ForceApplied = rb.linearVelocity.magnitude;
        // animator.SetFloat("Speed",ForceApplied);

        // Debug.Log(ForceApplied);
        //Thrust Key Press down detection
        isThrusting = ThrustInput.action.ReadValue<float>() > 0;
        if (ThrustInput.action.triggered)
        {
            animator.SetBool("IsThrust", true);
            SoundManager.Sam.playMoveStart();
        } else if (ThrustInput.action.WasReleasedThisFrame())
        {
            animator.SetBool("IsThrust", false);
            SoundManager.Sam.playMoveEnd();
        }

        //Rotation Key Detection
        RotateDirection = RotateInput.action.ReadValue<float>();
        }
    }

    float ReclampAngle(float angle)
    {
        return ((angle + 180) % 360 + 360) % 360 - 180;
    }

    void FixedUpdate()
    {
        if(GameManager.Gary.currentState == GameState.Playing){
        //Rotate Ship
        transform.Rotate(0.0f, 0.0f, rotateSpeed * -RotateDirection);
        // float direction = ReclampAngle(transform.rotation.eulerAngles.z);
        // if(direction > 90 || direction < -90)
        // {
            // transform.localScale = flippedScale;
        // } else
        // {
            // transform.localScale = originalScale;
        // }

        //Add Thrust to Ship
        if (isThrusting)
        {
            rb.AddForce(transform.right * ForceAmount);
        }
        }
    }
    
    public IEnumerator ReturnToMiddle()
    {
        float rotation = ReclampAngle(transform.rotation.eulerAngles.z);
        Vector3 currPosition = transform.position;
        rb.linearVelocity = Vector3.zero;
        while(Mathf.Abs(rotation) > 0.05 || currPosition.magnitude > 0.05)
        {
            if(Mathf.Abs(rotation) < 3)
            {
                transform.Rotate(-transform.rotation.eulerAngles);
            } else
            {
                transform.Rotate(0, 0, 130f * Time.deltaTime * (rotation > 0 ? -1 : 1));
            }

            Vector3 returnDirection = -currPosition;
            transform.position = currPosition + (Vector3.Normalize(returnDirection) * 6f * Time.deltaTime);

            yield return new WaitForSeconds(Time.deltaTime);
            rotation = ReclampAngle(transform.rotation.eulerAngles.z);
            currPosition = transform.position;
        }

    }

    public IEnumerator GameEnd()
    {
        SetIndestructible();
        float rotation = ReclampAngle(transform.rotation.eulerAngles.z);
        Vector3 currPosition = transform.position - new Vector3(0, 15, 0);
        Debug.Log(rotation + ", " + currPosition);
        rb.linearVelocity = Vector3.zero;
        while(Mathf.Abs(90.0f - rotation) > 0.05 || currPosition.magnitude > 0.05)
        {
            if(Mathf.Abs(90.0f - rotation) < 3)
            {
                transform.Rotate(0, 0, 90.0f - transform.rotation.eulerAngles.z);
            } else
            {
                transform.Rotate(0, 0, 130f * Time.deltaTime * ((90.0f - rotation) > 0 ? 1 : -1));
            }

            Vector3 returnDirection = -currPosition;
            transform.position = currPosition + (Vector3.Normalize(returnDirection) * 6f * Time.deltaTime);
            // Debug.Log(returnDirection);

            yield return new WaitForSeconds(Time.deltaTime);
            rotation = ReclampAngle(transform.rotation.eulerAngles.z);
            currPosition = transform.position - new Vector3(0, 10, 0);
        }

    }


    void ShootBullet()
    {
       if (shootCD == false)
        {
        Vector3 spawnPoint = transform.position + transform.right * distanceFromOrigin;
        Instantiate(BulletObject, spawnPoint, transform.rotation * Quaternion.Euler(new Vector3(0, 0, -90f)));
        SoundManager.Sam.playShoot();
        shootCD = true;
        Invoke(nameof(ShootCoolDownReset), shootCDTime);


       }
    }


    void ShootCoolDownReset()
    {
        shootCD = false;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Trash" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Enemy Bullet")
        {
            CallDie();
        }
    }

    public void CallDie()
    {
         GameManager.Gary.SetRespawnTimer();
         SoundManager.Sam.StopMovingSounds();
         animator.SetTrigger("Break");
         SoundManager.Sam.playPlayerDeath();

         gameObject.layer = 11;
         ActionDisabled();
         Invoke(nameof(DestroySelf), respawnTime);
    }

    private void DestroySelf()
    {
         Destroy(this.gameObject);
    }

    public void CallSlowDown()
    {
        StartCoroutine(Slowed());
    }

    private IEnumerator Slowed()
    {
        //Slow down by decreasing force
        ActionDisabled();
        // GetComponent<SpriteRenderer>().color = Color.blue;
        animator.SetBool("Electrocuted", true);

        yield return new WaitForSeconds(stunDuration);

        //Turn Force Back to normal.
        ActionEnabled();
        // GetComponent<SpriteRenderer>().color = Color.white;
        animator.SetBool("Electrocuted", false);

    }



}