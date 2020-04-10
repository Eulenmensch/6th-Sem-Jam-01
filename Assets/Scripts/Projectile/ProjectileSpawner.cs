using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    void SpawnProjectile(GameObject projectile, Vector3 aimDirection, float speed, float killDistance)
    {
        var projectileInstance = Instantiate(projectile, transform.position, Quaternion.identity);
        var projectileBehaviour = projectileInstance.GetComponent<ProjectileBehaviour>();
        if (projectileBehaviour != null)
        {
            projectileBehaviour.Speed = speed;
            projectileBehaviour.Direction = aimDirection;
            projectileBehaviour.KillDistance = killDistance;
            projectileBehaviour.Spawner = gameObject;
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
}
