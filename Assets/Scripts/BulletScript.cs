using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float moveSpeed;
    public float TimeToDespawn;
    private Rigidbody2D rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        if (collision.gameObject.tag == "Trash" || collision.gameObject.tag == "Enemy")
        {
            Destroy(this.gameObject);
        }
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
