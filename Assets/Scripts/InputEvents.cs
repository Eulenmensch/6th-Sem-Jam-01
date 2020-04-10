using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputEvents : Singleton<InputEvents>
{
    protected InputEvents() { }

    public event Action<GameObject, Vector3, float, float> OnShootProjectile;
    public void ShootProjectile(GameObject projectile, Vector3 aimDirection, float speed, float killDistance)
    {
        if (OnShootProjectile != null)
        {
            OnShootProjectile(projectile, aimDirection, speed, killDistance);
        }
    }
}
