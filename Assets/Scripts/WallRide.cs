using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRide : MonoBehaviour
{
    public float WallCheckDistance;
    public LayerMask WallLayerMask;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //DetectWalls();
    }

    // RaycastHit DetectWalls()
    // {
    //     RaycastHit hitRight;
    //     RaycastHit hitLeft;
    //     if (Physics.Raycast(transform.position, transform.right, out hitRight, WallCheckDistance, WallLayerMask))
    //     {
    //         return hitRight;
    //     }
    //     Physics.Raycast(transform.position, -transform.right, out hitLeft, WallCheckDistance, WallLayerMask);
    //     Debug.DrawRay(transform.position, transform.right * WallCheckDistance, Color.red);
    //     Debug.DrawRay(transform.position, -transform.right * WallCheckDistance, Color.red);
    // }
}
