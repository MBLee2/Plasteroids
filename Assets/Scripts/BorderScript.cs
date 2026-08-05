using UnityEngine;

public class BorderScript : MonoBehaviour
{
    public float maxXDist, maxYDist;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxXDist = transform.lossyScale.x / 2;
        maxYDist = transform.lossyScale.y / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Vector3 otherPosition = collision.transform.position;
        if(Mathf.Abs(otherPosition.x) > maxXDist)
        {
            otherPosition.x += otherPosition.x < 0 ? 0.2f : -0.2f;
            otherPosition.x *= -1;
        }
        if(Mathf.Abs(otherPosition.y) > maxYDist)
        {
            otherPosition.y += otherPosition.y < 0 ? 0.2f : -0.2f;
            otherPosition.y *= -1;
        }

        collision.transform.position = otherPosition;
    }
}
