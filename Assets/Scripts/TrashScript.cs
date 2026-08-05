using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float minSpeed, maxSpeed, rotateSpeed;
    public int size;
    private float maxX = 11f, maxY = 7f;
    private float direction, speed;

    [Header("Sprites")]
    public Sprite bigSprite;
    public Sprite[] medSprites;
    public Sprite[] smallSprites;
    private SpriteRenderer render;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        render = GetComponentInChildren<SpriteRenderer>();

        ChooseDirection();
        
        float speedMultiplier = Mathf.Pow(1.1f, 3 - size) * (1 + ((GameManager.Gary.stage - 1)/8))* (1 + ((GameManager.Gary.level - 1)/5));
        speed = UnityEngine.Random.Range(minSpeed * speedMultiplier, maxSpeed * speedMultiplier);
        transform.localScale = new Vector3(1, 1, 1) * 0.5f * Mathf.Pow(1.65f, size);

        SetupSpriteAudio();

        if (GameManager.Gary.currentState == GameState.Playing)
        {
            StartMoving();
        }
        
    }

    void ChooseDirection()
    {
        //Setting the randomnized movements
        if(Mathf.Abs(transform.position.x) > GameManager.Gary.sceneWidth / 2)
        {
            direction = UnityEngine.Random.Range(30f, 150f) + (UnityEngine.Random.Range(0, 1) < 0.5f ? 0 : 180);
        } else if (Mathf.Abs(transform.position.y) > GameManager.Gary.sceneHeight / 2)
        {
            direction = UnityEngine.Random.Range(-60f, 60f) + (UnityEngine.Random.Range(0, 1) < 0.5f ? 0 : 180);
        } else
        {
            direction = UnityEngine.Random.Range(0, 360f);
        }
    }

    void SetupSpriteAudio()
    {
        Transform childTransform = gameObject.transform.GetChild(0);
        childTransform.localScale = new Vector3(1 / transform.localScale.x, 1 / transform.localScale.y, 1);
        if(size == 1) { 
            render.sprite = smallSprites[(int) UnityEngine.Random.Range(0f, smallSprites.Length)];
        }
        else if(size == 2) { 
            render.sprite = medSprites[(int) UnityEngine.Random.Range(0f, medSprites.Length)];
        }
        else { 
            render.sprite = bigSprite;
        }
    }

    void FixedUpdate()
    {
        if(Mathf.Abs(transform.position.x) > maxX || Mathf.Abs(transform.position.y) > maxY)
        {
            Destroy(this.gameObject);
        }

    }

    //Set of to start moving
    public void StartMoving()
    {
        rb.linearVelocityX = speed * -Mathf.Sin(direction * Mathf.Deg2Rad);
        rb.linearVelocityY = speed * Mathf.Cos(direction * Mathf.Deg2Rad);

        float rotationMultiplier = Mathf.Pow(3f, 3 - size);
        rb.angularVelocity = Random.Range(-rotateSpeed, rotateSpeed);
        // Debug.Log(rb.angularVelocity);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
	    if(collision.gameObject.tag != "Borders")
	    {
		    DestroyTrash();
	    }
    }

    private void DestroyTrash()
    {
	    if(size > 1)
	    {
            for (int i = 0; i < 2; i++)
            {
                GameObject clone = Instantiate(this.gameObject, transform.position, transform.rotation);
                TrashScript script = clone.GetComponent<TrashScript>();
                script.size = size - 1;
            }
        }
        if(size == 3) { 
            GameManager.Gary.AddScore(20); 
            SoundManager.Sam.playLargeTrash();
        }
        else if(size == 2) { 
            GameManager.Gary.AddScore(50); 
            SoundManager.Sam.playMediumTrash();
        }
        else { 
            GameManager.Gary.AddScore(100); 
            SoundManager.Sam.playSmallTrash();
        }
        Destroy(this.gameObject, 0.01f);
    }

}
