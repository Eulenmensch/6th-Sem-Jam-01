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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Shoot();
    }

    Vector3 GetAimDirection() //FIXME: This needs to be more intricate with a raycast from camera and then building a direction from spawner position to hit position
    {
        return Camera.main.transform.forward;
    }

    void Shoot()
    {
        Debug.Log("pew");
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
