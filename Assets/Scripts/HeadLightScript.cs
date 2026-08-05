using Unity.VisualScripting;
using UnityEngine;

public class HeadLightScript : MonoBehaviour
{
private SpriteRenderer sr;
public Sprite[] headSprites = new Sprite[5];
public static int hitCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAnimation()
{
    if (hitCount < 6)
    {
    sr.sprite = headSprites[hitCount];
    }
}
}
