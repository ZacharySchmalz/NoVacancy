using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skulk : Enemy
{
    public float attackDistance;    // distance to attack
    public float runTime;           // length of animation, used for calculating animation time length with speed multipliers
    public float attackTime;

    private bool isAttacking;
    private bool isRunning;

    protected override  void Start()
    {
        base.Start();
        isAttacking = false;
        isRunning = false;
    }

    protected override void Update()
    {
        base.Update();

        // If within distance, move towards target
        if (distance < detectionRange && distance > attackDistance)
        {
            if (!isRunning)
                startRoutine(1);
            // Move twards target
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
        }

        // Attack Mode
        else if (distance < attackDistance)
        {
            if (!isAttacking)
                startRoutine(2);
        }
    }

    private void startRoutine(int routineIndex)
    {
        switch(routineIndex)
        {
            case 1:
            {
                    StartCoroutine(runState());
                    break;
            }
            case 2:
            {
                    StartCoroutine(attackState());
                    break;
            }
        }
    }

    private IEnumerator attackState()
    {
        isAttacking = true;
        anim.SetTrigger("attack1");
        yield return new WaitForSeconds((attackTime / anim.GetCurrentAnimatorStateInfo(0).speed));
        isAttacking = false;
    }

    private IEnumerator runState()
    {
        isRunning = true;
        anim.SetTrigger("run1");
        yield return new WaitForSeconds(runTime / anim.GetCurrentAnimatorStateInfo(0).speed);
        isRunning = false;
    }
}
