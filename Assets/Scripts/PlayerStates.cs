using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : Singleton<PlayerStates>
{
    protected PlayerStates() { }

    public enum MovementStates
    {
        Idle,
        Walking,
        Jumping,
        DoubleJumping,
        WallRunning,
        Dashing,
        Falling
    }

    public MovementStates MovementState;

    private void Awake()
    {
        MovementState = MovementStates.Idle;
    }
}