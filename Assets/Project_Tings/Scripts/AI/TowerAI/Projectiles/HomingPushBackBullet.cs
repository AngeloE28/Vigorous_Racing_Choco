using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingPushBackBullet : MonoBehaviour
{
    [Header("Bullet Stats")]
    [SerializeField] private float bulletSpeed = 60.0f;
    [SerializeField] private float upwardForce = 30.0f;
    [SerializeField] private float backwardForce = 150.0f;
    [SerializeField] private float forceMult = 1000.0f;
    private Transform target;    
    public void GetTarget(Transform target)
    {
        this.target = target;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        Vector3 dir = target.position - transform.position;

        // Homing bullet
        transform.Translate(dir.normalized * (bulletSpeed * Time.deltaTime), Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        ICakeCar car = other.GetComponent<ICakeCar>();
        if (car != null)
        {
            car.PushBack(backwardForce, upwardForce, forceMult);
            Destroy(gameObject);
        }
    }
}
