using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashController : MonoBehaviour
{
    public float DashSpeed;
    public float ProjectileReachedDistance;

    private GameObject Projectile;
    private CharacterController Controller;
    private PlayerController Player;

    bool IsReceivingDashInput;

    private void Start()
    {
        Controller = GetComponent<CharacterController>();
        Player = GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        Projectile = GameObject.FindGameObjectWithTag("Projectile");
        if (Projectile != null)
        {
            ApplyDashInput();
            HandleDashState();
        }
    }

    void HandleDashInput()
    {
        Player.ResetImpact();

        if (Projectile != null)
        {

            Player.VelocityGravitational.y = 0.0f;
            Player.MoveDirection.y = 0.0f;
            PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Dashing;
        }
    }

    void ApplyDashInput()
    {
        if (PlayerStates.Instance.MovementState == PlayerStates.MovementStates.Dashing)
        {
            Vector3 moveDirection = Projectile.transform.position - transform.position;
            //Controller.Move(moveDirection.normalized * DashSpeed * Time.deltaTime);
            Player.AddImpact(moveDirection, DashSpeed);
        }
    }

    void HandleDashState()
    {
        Vector3 projectileDistance = Projectile.transform.position - transform.position;

        if (projectileDistance.magnitude <= ProjectileReachedDistance && PlayerStates.Instance.MovementState == PlayerStates.MovementStates.Dashing)
        {
            PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Falling;
        }
    }

    // public void GetDashInput(InputAction.CallbackContext context)
    // {
    //     if (context.canceled)
    //     {
    //         HandleDashInput();
    //     }
    // }
    public void GetDashInput(InputAction.CallbackContext context)
    {
        if (context.performed && !IsReceivingDashInput)
        {
            IsReceivingDashInput = true;
        }
        else if (context.performed && IsReceivingDashInput)
        {
            IsReceivingDashInput = false;
            HandleDashInput();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (PlayerStates.Instance.MovementState == PlayerStates.MovementStates.Dashing)
        {
            PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Falling;
        }
    }
}
