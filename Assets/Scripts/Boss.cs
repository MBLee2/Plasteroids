using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
    //Attack CDS
    [Header("ActivationTrack")]
    public float preCD;
    public float AttackCD;
    public float HandAttackCD;
    public float HandAttackStartCD;
    private int targetsToRun;

    //Boss Stages
    public enum BossStage{None, Stage1, Stage2, Stage3};
    public int randomNum = 1;


    //Animations
    Animator animator;
    public float OutroDuration;

    //Tenticle States
    public enum Tenticles{Both, Left, Right, None};
    public static Tenticles currentState = Tenticles.None;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TenticleAttack());
        animator = GetComponent<Animator>();
        Invoke(nameof(CallHandAttackIE), HandAttackStartCD);

    }

    private void CallHandAttackIE()
    {
        StartCoroutine(HandAttackCall());
    }

    public IEnumerator HandAttackCall()
    {

        BroadcastMessage("HandAttack");
        yield return new WaitForSeconds(HandAttackCD);
        StartCoroutine(HandAttackCall());
    }

    // Update is called once per frame
    void Update()
    {
        randomNum = Random.Range(0,2);
    }


    public IEnumerator TenticleAttack()
    {
        yield return new WaitForSeconds(preCD);

        //Start Attack
       List<TenticleScript> childrenTenticles = new List<TenticleScript>();

        foreach(Transform child in transform)
        {
            TenticleScript childScript = child.GetComponent<TenticleScript>();

            if (childScript != null)
            {
                childrenTenticles.Add(childScript);
            }
        }

        targetsToRun = Mathf.Clamp(targetsToRun, 0, childrenTenticles.Count);

        for (int i = 0; i < targetsToRun; i ++)
        {
            TenticleScript tempChildren = childrenTenticles[i];
            int randomIndex = Random.Range(0,childrenTenticles.Count);

            tempChildren = childrenTenticles[randomIndex];
            childrenTenticles[randomIndex] = tempChildren;
       }

        //Cast Attack In Children Class.
      
       
       Debug.Log(randomNum);
      if (childrenTenticles[randomNum].isActive)
      {
         childrenTenticles[randomNum].Attack();
      }
      else if (childrenTenticles[1].isActive)
        {
            childrenTenticles[1].Attack();
        }
        else
        {
            childrenTenticles[0].Attack();
        }


       
       
       yield return new WaitForSeconds (AttackCD);

       StartCoroutine(TenticleAttack());
    }


    public void CallHeadAction()
    {
        BroadcastMessage("HeadMove");
        Debug.Log("CallHead");
    }

    public void CallActivateTenticles()
    {
        BroadcastMessage("ActivateTenticles");
        Debug.Log("ActivateTenticles");
    }

    public void CallUpdateAnim()
    {
        BroadcastMessage("UpdateAnimation");
    }

    public void Outro()
    {
        animator.SetTrigger("Outro");
        Invoke(nameof(EndScene), OutroDuration);
    }

    public void EndScene()
    {
        GameObject[] Trashes = GameObject.FindGameObjectsWithTag("Trash");
        foreach(GameObject Trash in Trashes)
        {
            Destroy(Trash.gameObject);
        }
    }
}
