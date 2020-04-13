using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public GameObject Spawner;
    public float Speed;
    public Vector3 Direction;
    public float KillDistance;

    void Update()
    {
        Move();
        Die();
    }

    void Move()
    {
        transform.position += Direction * Speed * Time.deltaTime;
    }

    void Die()
    {
        if ((transform.position - Spawner.transform.position).magnitude >= KillDistance)
        {
            Destroy(gameObject);
            if (PlayerStates.Instance.MovementState == PlayerStates.MovementStates.Dashing)
            {
                PlayerStates.Instance.MovementState = PlayerStates.MovementStates.Falling;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Direction = Vector3.Reflect(Direction, other.GetContact(0).normal);
    }

}
