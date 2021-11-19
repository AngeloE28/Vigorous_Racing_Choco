using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour
{
    [SerializeField] private SphereCollider carControllerCollider;
    [SerializeField] private BoxCollider carCollider;
    [SerializeField] private BoxCollider carBodyCollider;    

    // Update is called once per frame
    void Update()
    {
        // Colliders that will ignore each other
        Physics.IgnoreCollision(carControllerCollider, carCollider, true);
        Physics.IgnoreCollision(carControllerCollider, carBodyCollider, true);
        Physics.IgnoreCollision(carCollider, carBodyCollider, true);
        carBodyCollider.transform.rotation = this.transform.rotation;
        carBodyCollider.transform.position = carCollider.transform.position;
    }
}
