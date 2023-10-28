using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [Header("Movement Parameters")]
    public float movementSpeed = 1.5f;
    public float walkSpeed = 0.7f;
    public float chaseSpeed = 2f;
    public float dodgeSpeed = 2f;
    public float changeDirectionSpeed = 15;
    public float jumpForce = 0.6f;
    public bool walkMode;
    public float groundCheckDistance = 0.2f;
    public float slideSpeed = 2.0f;
    public bool isGrounded;
    public Vector3 slideDirection;

    [Header("Combat Parameters")]
    public float attackRange = 2f;

    [Header("Required Components")]
    public CharacterController controller;
    public Animator animator;
    public ForceReceiver forceReceiver;
    public ComboManager comboManager;
    public Character character;
    public TargetManager targetManager;

    private State currentState;
    private Vector3 groundNormal;

    private void Update()
    {
        GroundCheck();
        currentState?.Tick();
    }

    public virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        forceReceiver = GetComponent<ForceReceiver>();
        comboManager = GetComponent<ComboManager>();
        character = GetComponent<Character>();
        targetManager = GetComponentInChildren<TargetManager>();
        character.DamageEvent += OnDamage;
        character.DieEvent += OnDie;
    }

    public void SwitchState(State newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public virtual void OnDamage()
    { }

    public virtual void OnDie(GameObject dieCharacter)
    { }

    public void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance);

        groundNormal = hit.normal;
        float groundAngle = Vector3.Angle(Vector3.up, groundNormal);
        if (groundAngle > 45)
            ApplySlide();
        else slideDirection = Vector3.zero;
    }

    private void ApplySlide()
    {
        slideDirection = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized * slideSpeed;
    }
}
