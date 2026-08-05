using UnityEngine;

public class BossHand : MonoBehaviour
{
    //Animations
    public Animator animator;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void HandAttack()
    {
        int randomIndex = Random.Range(0,3);
        Debug.Log(randomIndex);
        if (randomIndex == 0)
        {
            animator.SetTrigger("AttackL");
        }
        else if (randomIndex == 1)
        {
            animator.SetTrigger("AttackR");
        }
        else if (randomIndex == 2)
        {
            animator.SetTrigger("AttackL");
            animator.SetTrigger("AttackR");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
