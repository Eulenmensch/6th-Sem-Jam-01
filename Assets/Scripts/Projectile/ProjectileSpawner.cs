using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpawner : MonoBehaviour
{
    private ProjectileBehaviour ActiveProjectileBehaviour;
    void SpawnProjectile(GameObject projectile, Vector3 aimDirection, float speed, float killDistance)
    {
        var projectileInstance = Instantiate(projectile, transform.position, Quaternion.identity);
        ActiveProjectileBehaviour = projectileInstance.GetComponent<ProjectileBehaviour>();
        if (ActiveProjectileBehaviour != null)
        {
            ActiveProjectileBehaviour.Speed = speed;
            ActiveProjectileBehaviour.Direction = aimDirection;
            ActiveProjectileBehaviour.KillDistance = killDistance;
            ActiveProjectileBehaviour.Spawner = gameObject;
        }
    }

    private void OnEnable()
    {
        InputEvents.Instance.OnShootProjectile += SpawnProjectile;
    }
    private void OnDisable()
    {
        InputEvents.Instance.OnShootProjectile -= SpawnProjectile;
    }

    public void GetDashInput(InputAction.CallbackContext context)
    {
        if (context.canceled && ActiveProjectileBehaviour != null)
        {
            ActiveProjectileBehaviour.Speed = 0.0f;
        }
    }
}
