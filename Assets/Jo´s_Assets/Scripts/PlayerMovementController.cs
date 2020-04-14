using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovementController : MonoBehaviour
{
    private Animator _animator;

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        var inputVector = context.ReadValue<Vector2>();
        _animator.SetFloat("velXFloat", inputVector.x);
        _animator.SetFloat("velYFloat", inputVector.y);
    }

    // Idle = 0
    // Walking = 1
    // Jumping = 2
    // DoubleJumping = 3
    // Dashing = 4
    // Falling = 5

    private void Update()
    {
        _animator.SetInteger("MovementState", (int)PlayerStates.Instance.MovementState);
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

}
