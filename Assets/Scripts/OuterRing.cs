using UnityEngine;
using UnityEngine.Rendering;

public class OuterRing : MonoBehaviour
{
    Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCharge()
    {
        animator.SetTrigger("Charge");
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
