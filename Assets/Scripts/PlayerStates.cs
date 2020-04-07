using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : Singleton<PlayerStates>
{
    protected PlayerStates() { }

    public enum MovementStates
    {
        Idle,
        Jumping,
        DoubleJumping,
        Falling
    }

    public bool IsTalking;
    public bool IsCarrying;

    public MovementStates MovementState;

    private void Awake()
    {
        MovementState = MovementStates.Idle;
        IsTalking = false;
        IsCarrying = false;
    }
}