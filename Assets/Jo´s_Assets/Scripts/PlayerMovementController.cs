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


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

}
