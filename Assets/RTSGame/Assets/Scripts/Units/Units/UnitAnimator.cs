using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitAnimator : MonoBehaviour
{
    public Animator animator;
    public UnitAnimator(GameObject gm)
    {
        animator = gm.GetComponentInChildren<Animator>();
    }

    public void SetBool(string name, bool value)
    {
        if (animator)
        {

            if (animator.GetBool(name) != value)
            {
                animator.SetBool(name, value);
            }
        }
    }

    public void SetFloat(string name, float value)
    {
        if (animator)
        {
            animator.SetFloat(name, value);
        }
    }

    public void SetTrigger(string name)
    {
        if (animator)
        {
            animator.SetTrigger(name);
        }
    }

    public IEnumerator DieAnimation(string name, bool value, GameObject gm)
    {
        if (animator)
        {

            animator.SetBool(name, value);

            yield return new WaitForSeconds(3);


            Destroy(gm);
        }
        else
        {
            yield return new WaitForSeconds(3);

            Destroy(gm);
        }
    }

}
