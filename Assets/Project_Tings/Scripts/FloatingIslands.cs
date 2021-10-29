using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingIslands : MonoBehaviour
{
    [SerializeField] private float maxYPos = 15.0f;
    [SerializeField] private float minYPos = 10.0f;
    [SerializeField] private float movementSpeed = 3.0f;

    
    void LateUpdate()
    {
        if (transform.position.y >= maxYPos)
            movementSpeed *= -1.0f;
        if (transform.position.y <= minYPos)
            movementSpeed *= -1.0f;

        transform.position = new Vector3(transform.position.x,
                                         transform.position.y + movementSpeed * Time.deltaTime, 
                                         transform.position.z);
    }
}
