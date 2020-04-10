using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    private InputActions inputActions; //default controls is just the Csharp code you generate from the action maps asset
    private Vector2 LookDelta;

    private void Awake() => inputActions = new InputActions();

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Update()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    public float GetAxisCustom(string axisName)
    {
        LookDelta = inputActions.Player.Look.ReadValue<Vector2>(); // reads theavailable camera values and uses them.
        LookDelta.Normalize();

        if (axisName == "Mouse X")
        {
            return LookDelta.x;
        }
        else if (axisName == "Mouse Y")
        {
            return LookDelta.y;
        }
        return 0;
    }
}