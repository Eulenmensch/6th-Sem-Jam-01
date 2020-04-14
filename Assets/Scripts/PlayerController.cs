using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private CharacterController CharacterControllerRef;

    public GameObject Camera;
    //public Transform GroundCheckSphere;
    //public float GroundCheckRadius;
    //public LayerMask GroundLayerMask;

    public float Speed;
    public float AirControlFactor;
    public float GravitationalAcceleration;
    public float Damping;
    public float JumpHeight;
    public float DoubleJumpHeight;
    public float FallModifier;
    public float ShortJumpModifier;
    public float Deadzone;

    private float HorizontalInput;
    private float VerticalInput;
    private bool IsReceivingJumpInput;
    private bool HasStoppedReceivingJumpInput;
    private bool IsReceivingDoubleJumpInput;

    float cameraAngle;
    Vector2 inputVector;
    Vector2 rotatedInputVector;

    public Vector3 MoveDirection;
    public Vector3 VelocityGravitational;
    public Vector3 CurrentImpact;

    private int JumpCount;


    private void Start()
    {
        CharacterControllerRef = GetComponent<CharacterController>();
        HasStoppedReceivingJumpInput = true; // set the start value to true to start to not eat the first jump input
        IsReceivingJumpInput = false;
    }

    private void Update()
    {
        RotateMoveInputToCameraForward();
    }

    void FixedUpdate()
    {
        if (IsGrounded())
        {
            JumpCount = 0;
        }
        if (PlayerStates.Instance.MovementState != PlayerStates.MovementStates.Dashing)
        {
            HandleMoveInput();
            ApplyMoveInput();
            HandleJumpInput();
            ApplyJumpInput();
            HandleDoubleJumpInput();
            ApplyDoubleJumpInput();
            ApplyGravity();
        }
        HandleImpact();
        ApplyImpact();
        SetRotationToMoveDirection();
        Debug.DrawRay(transform.position, MoveDirection, Color.magenta); //TODO: Remove this after debugging
        CharacterControllerRef.Move(MoveDirection * Time.deltaTime);

        Debug.Log(PlayerStates.Instance.MovementState);
    }

    void HandleMoveInput()
    {
        Vector3 inputVector = new Vector3(HorizontalInput, 0.0f, VerticalInput);
        if (IsGrounded())
        {
            if (inputVector.magnitude >= Deadzone)
            {
                PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Walking;
            }
            else
            {
                PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Idle;
            }
        }
    }

    void ApplyMoveInput()
    {
        Vector3 inputVector = new Vector3(HorizontalInput, 0.0f, VerticalInput);
        if (IsGrounded()) // Grounded Movement
        {
            MoveDirection = inputVector;
            MoveDirection = Vector3.ClampMagnitude(MoveDirection, 1);
            MoveDirection *= Speed;
        }
        else if (!IsGrounded()) // Aerial Movement
        {
            MoveDirection = new Vector3(MoveDirection.x, 0f, MoveDirection.z); // reset the move vector to ground plane
            MoveDirection += inputVector * AirControlFactor; // gives aircontrol dependent on AirControlFactor
            MoveDirection = Vector3.ClampMagnitude(MoveDirection, Speed); // limits the movedirection's speed to the defined movespeed
        }
    }

    void HandleJumpInput()
    {
        if (IsGrounded())
        {
            if (IsReceivingJumpInput && PlayerStates.Instance.MovementState != PlayerStates.MovementStates.Jumping && HasStoppedReceivingJumpInput)
            {
                HasStoppedReceivingJumpInput = false;
                JumpCount++;
                PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Jumping;
            }
            else if (PlayerStates.Instance.MovementState == PlayerStates.MovementStates.Jumping)
            {
                PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Idle;
            }
        }
    }

    void ApplyJumpInput()
    {
        if (PlayerStates.Instance.MovementState == PlayerStates.MovementStates.Jumping)
        {
            MoveDirection.y = Mathf.Sqrt(JumpHeight * -2f * -GravitationalAcceleration);
        }
    }

    void HandleDoubleJumpInput()
    {
        if (IsReceivingDoubleJumpInput && !IsGrounded())
        {
            HasStoppedReceivingJumpInput = false; //to prevent an unintended jump when keeping the jump button held after a double jump
            PlayerStates.Instance.MovementState = PlayerStates.MovementStates.DoubleJumping;
            VelocityGravitational.y = 0.0f;
            IsReceivingDoubleJumpInput = false;
            JumpCount += 2;
        }
        else if (IsGrounded() && PlayerStates.Instance.MovementState == PlayerStates.MovementStates.DoubleJumping)
        {
            PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Idle;
        }
    }

    void ApplyDoubleJumpInput()
    {
        if (PlayerStates.Instance.MovementState == PlayerStates.MovementStates.DoubleJumping)
        {
            MoveDirection.y = Mathf.Sqrt(DoubleJumpHeight * -2f * -GravitationalAcceleration);
        }
    }

    void ApplyGravity()
    {
        float currentVelocityY = MoveDirection.y + VelocityGravitational.y;
        if (CharacterControllerRef.isGrounded && VelocityGravitational.y <= 0.0f) // reset gravitational velocity to 0 if grounded
        {
            VelocityGravitational.y = 0.0f;
        }
        else if (!IsGrounded() && !IsReceivingJumpInput && currentVelocityY >= 0) // make the jump shorter if the jump button was released before reaching the apex
        {
            VelocityGravitational.y -= GravitationalAcceleration * ShortJumpModifier * Time.deltaTime;
        }
        else if (!IsGrounded() && currentVelocityY < 0) // make the descent faster than the ascend
        {
            VelocityGravitational.y -= GravitationalAcceleration * FallModifier * Time.deltaTime;
        }
        else // if nothing else, apply normal gravity
        {
            VelocityGravitational.y -= GravitationalAcceleration * Time.deltaTime;
        }
        MoveDirection.y += VelocityGravitational.y;
    }

    void HandleImpact()
    {
        CurrentImpact = Vector3.Lerp(CurrentImpact, Vector3.zero, Damping * Time.deltaTime);
    }
    void ApplyImpact()
    {
        MoveDirection += CurrentImpact;
    }

    void SetRotationToMoveDirection()
    {
        Vector3 lookDirection = new Vector3(HorizontalInput, 0.0f, VerticalInput);
        if (lookDirection.magnitude >= 0.1)
        {
            transform.DORotateQuaternion(Quaternion.LookRotation(lookDirection, Vector3.up), 0.4f).SetEase(Ease.OutCirc);
            //transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }

    bool IsGrounded()
    {
        //return Physics.CheckSphere(GroundCheckSphere.position, GroundCheckRadius, GroundLayerMask); //TODO: Delete this if the code below is less errorprone
        return (CharacterControllerRef.collisionFlags & CollisionFlags.Below) != 0;
        //return (CharacterControllerRef.isGrounded);
    }

    public void AddImpact(Vector3 direction, float magnitude)
    {
        CurrentImpact += direction * magnitude;
    }

    public void ResetImpact()
    {
        CurrentImpact = Vector3.zero;
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    void RotateMoveInputToCameraForward()
    {
        cameraAngle = (Camera.transform.rotation.eulerAngles.y) * Mathf.Deg2Rad;

        rotatedInputVector = new Vector2(
            inputVector.x * Mathf.Cos(-cameraAngle) - inputVector.y * Mathf.Sin(-cameraAngle),
            inputVector.x * Mathf.Sin(-cameraAngle) + inputVector.y * Mathf.Cos(-cameraAngle)
        );


        HorizontalInput = rotatedInputVector.x;
        VerticalInput = rotatedInputVector.y;
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && !IsReceivingJumpInput)
        {
            IsReceivingJumpInput = true;
        }
        else if (context.performed && IsReceivingJumpInput)
        {
            IsReceivingJumpInput = false;
            HasStoppedReceivingJumpInput = true;
        }
    }

    public void GetDoubleJumpInput(InputAction.CallbackContext context)
    {
        if (context.started && (PlayerStates.Instance.MovementState == PlayerStates.MovementStates.Jumping || PlayerStates.Instance.MovementState == PlayerStates.MovementStates.Falling))
        {
            IsReceivingDoubleJumpInput = true;
            gameObject.SendMessage("DoubleJump");
        }
    }
}