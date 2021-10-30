using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBackBullet : MonoBehaviour
{
    [Header("Bullet Stats")]
    [SerializeField] private float bulletSpeed = 60.0f;
    [SerializeField] private float upwardForce = 30.0f;
    [SerializeField] private float backwardForce = 150.0f;
    [SerializeField] private float forceMult = 1000.0f;
    private Transform target;
    private Rigidbody rb;

    private void Start()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
                
        Vector3 dir = (target.position - transform.position).normalized * bulletSpeed;        
        rb = GetComponent<Rigidbody>();
        rb.velocity = dir;
        Destroy(gameObject, 5.0f);
    }

    public void GetTarget(Transform target)
    {
        this.target = target;
    }
        
    private void OnTriggerEnter(Collider other)
    {
        ICakeCar car = other.GetComponent<ICakeCar>();
        if(car != null)
        {
            car.PushBack(backwardForce, upwardForce, forceMult);
            Destroy(gameObject);
        }
    }
}
