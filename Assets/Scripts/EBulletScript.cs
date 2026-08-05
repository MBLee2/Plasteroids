using Unity.VisualScripting;
using UnityEngine;

public class EBulletScript : MonoBehaviour
{
    public float moveSpeed;
    public float TimeToDespawn;
    private Rigidbody2D rb;

    public Sprite[] sprites;
    private SpriteRenderer render;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        render = GetComponentInChildren<SpriteRenderer>();
        render.sprite = sprites[Random.Range(0, sprites.Length)];
        Invoke(nameof(DestroySelf), TimeToDespawn);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.up * moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Trash")
        {
            Destroy(this.gameObject);
        }
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
