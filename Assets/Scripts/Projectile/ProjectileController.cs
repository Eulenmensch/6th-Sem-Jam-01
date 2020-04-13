using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileController : MonoBehaviour
{
    public float ProjectileSpeed;
    public GameObject Projectile;
    public float KillDistance;

    bool IsReceivingShootInput;

    Vector3 GetAimDirection() //FIXME: This needs to be more intricate with a raycast from camera and then building a direction from spawner position to hit position
    {
        return Camera.main.transform.forward;
    }

    void Shoot()
    {
        Destroy(GameObject.FindGameObjectWithTag("Projectile"));
        InputEvents.Instance.ShootProjectile(Projectile, GetAimDirection(), ProjectileSpeed, KillDistance);
    }

    public void GetShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shoot();
        }
    }
}
