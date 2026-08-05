using UnityEngine;
using UnityEngine.UI;

public class BackgroundScript : MonoBehaviour
{
    [Header("BackgroundScroll")]
    public RawImage rawImage;
    public float xSpeed;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + new Vector2(xSpeed, 0) * Time.deltaTime, rawImage.uvRect.size);
    }
}
