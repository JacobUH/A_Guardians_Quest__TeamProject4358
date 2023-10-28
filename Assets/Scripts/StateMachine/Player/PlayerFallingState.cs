using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerFallingState : PlayerState
{
    public PlayerFallingState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    private readonly int fallHash = Animator.StringToHash("Fall");
    private const float crossFadeDuration = 0.1f;

    public override void Enter()
    {
        PlayAnimation(fallHash, crossFadeDuration);
    }

    public override void Exit()
    {
    }

    public override void Tick()
    {
        HandlePlayerMovement();
        HandleCameraMovement();
        if (playerStateMachine.controller.isGrounded)
        {
            playerStateMachine.SwitchState(new PlayerFreeLookState(playerStateMachine));
        }
    }
}
