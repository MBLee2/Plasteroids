using UnityEngine;

public class FlipSprite : MonoBehaviour
{

     private Vector3 originalScale, flippedScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
          originalScale = transform.localScale;
          flippedScale = new Vector3(originalScale.x, -originalScale.y, originalScale.z);
    }

    // Update is called once per frame
    void Update()
    {
         
         float direction = ReclampAngle(transform.rotation.eulerAngles.z);

          if(direction > 90 || direction < -90)
        {
            transform.localScale = flippedScale;
        } else
        {
            transform.localScale = originalScale;
        }
    }

    float ReclampAngle(float angle)
{
    return ((angle + 180) % 360 + 360) % 360 - 180;
}
}
