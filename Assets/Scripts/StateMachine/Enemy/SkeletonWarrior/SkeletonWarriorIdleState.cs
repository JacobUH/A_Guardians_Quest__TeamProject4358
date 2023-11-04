using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkeletonWarriorIdleState : EnemyIdleState
{
    private SkeletonWarriorStateMachine skeletonWarriorStateMachine;

    public SkeletonWarriorIdleState(SkeletonWarriorStateMachine skeletonWarriorStateMachine) : base(skeletonWarriorStateMachine)
    {
        this.skeletonWarriorStateMachine = skeletonWarriorStateMachine;
    }

    private int idleHash = Animator.StringToHash("Idle");
    private int aggroHash = Animator.StringToHash("Aggro");
    private int idleBlendHash = Animator.StringToHash("IdleBlendSpeed");
    private float crossFixedDuration = 0.3f;
    private int patrolIndex;
    private GameObject destination;
    private float idleTime;
    private float randomIdleTime;
    private bool playingAnimation;

    public override void Enter()
    {
        if (skeletonWarriorStateMachine.patrolPath.Count() != 0)
        {
            patrolIndex = 0;
        }
        PlayAnimation(idleHash, crossFixedDuration);
        skeletonWarriorStateMachine.animator.SetFloat(idleBlendHash, 0f);
        randomIdleTime = Random.Range(0.5f, 1.0f);
    }

    public override void Exit()
    {

    }

    public override void Tick()
    {
        if (idleTime < randomIdleTime)
        {
            idleTime += Time.deltaTime;
            return;
        }

        if (GetCurrentTarget() != null)
        {
            FaceTarget(2f);

            if (skeletonWarriorStateMachine.isAggro)
            {
                if (playingAnimation)
                {
                    if (GetNormalizedTime(skeletonWarriorStateMachine.animator, aggroHash) >= 1f)
                    {
                        skeletonWarriorStateMachine.SwitchState(new SkeletonWarriorChasingState(skeletonWarriorStateMachine));
                    }
                }
                else skeletonWarriorStateMachine.SwitchState(new SkeletonWarriorChasingState(skeletonWarriorStateMachine));
                return;
            }
            else
            {
                skeletonWarriorStateMachine.isAggro = true;
                PlayAnimation(aggroHash, crossFixedDuration);
                playingAnimation = true;
            }
            return;
        }
        else skeletonWarriorStateMachine.isAggro = false;

        if (skeletonWarriorStateMachine.patrolPath.Count() != 0)
        {
            if (idleTime < 5f)
            {
                idleTime += Time.deltaTime;
            }
            else
            {
                destination = skeletonWarriorStateMachine.patrolPath[patrolIndex];
                if (Vector3.Distance(destination.transform.position, skeletonWarriorStateMachine.transform.position) <= 0.01f)
                {
                    patrolIndex++;
                    skeletonWarriorStateMachine.animator.SetFloat(idleBlendHash, 0f);
                    if (patrolIndex == skeletonWarriorStateMachine.patrolPath.Count()) patrolIndex = 0;
                    idleTime = 0f; 
                    return;
                }
                else
                {
                    FaceTargetInstantly(destination);
                    Move(skeletonWarriorStateMachine.transform.forward * skeletonWarriorStateMachine.walkSpeed);
                    skeletonWarriorStateMachine.animator.SetFloat(idleBlendHash, 1f, 0.1f, Time.deltaTime);
                }
            }
        }
    }
}
