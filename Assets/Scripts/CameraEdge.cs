using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Thomas Wang 
[RequireComponent(typeof(BoxCollider))]
public class CameraEdge : MonoBehaviour
{
    [SerializeField] private Vector3 entryDirection;
    [SerializeField] private bool localDirection = false;
    [SerializeField, Range(1.0f, 2.0f)] private float triggerScale = 1.25f;
    [SerializeField] private new BoxCollider collider;

    private BoxCollider collisionCheckTrigger = null;

    private void Awake() {
        collisionCheckTrigger = gameObject.AddComponent<BoxCollider>();
        collisionCheckTrigger.size = collider.size * triggerScale;
        collisionCheckTrigger.center = collider.center;
        collisionCheckTrigger.isTrigger = true;
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, entryDirection);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -entryDirection);
    }

    private void OnTriggerStay(Collider other) {
        // collider: collider of the wall
        // other: object entering wall
        // check if other enters collider
        if (Physics.ComputePenetration(collider, transform.position, transform.rotation,
            other, other.transform.position, other.transform.rotation,
            out Vector3 collisionDirection, out float penetrationDepth
            )) {
            // check which direction other collided from
            float dot = Vector3.Dot(entryDirection, collisionDirection);
            // if dot < 0, it is coming from the entryDirection
            // else, it is coming from the wrong direction
            if (dot < 0) {
                Physics.IgnoreCollision(collider, other, false);
            } else {
                Physics.IgnoreCollision(collider, other, true);

            }


        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
