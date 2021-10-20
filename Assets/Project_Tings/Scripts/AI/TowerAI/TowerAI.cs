using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] private float updateFreq = 0.5f;
    [SerializeField] private float towerRange = 15.0f;
    [SerializeField] private float turnSpeed = 8.0f;
    [SerializeField] private float fireRate = 1.0f;
    [SerializeField] string carTag = "Car";
    [SerializeField] private Transform head;

    [Header("Gun Properties")]
    [SerializeField] private bool isBulletHoming = false;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    private float fireCountdown = 0.0f;
    private Transform targetCar;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(UpdateTarget), 0.0f, updateFreq);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCar == null)
            return;

        Vector3 dir = targetCar.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        Vector3 targetRot = Quaternion.Lerp(head.rotation, lookRot, turnSpeed * Time.deltaTime).eulerAngles;
        head.rotation = Quaternion.Euler(0.0f, targetRot.y, 0.0f);

        if(fireCountdown <= 0.0f)
        {
            Shoot();
            fireCountdown = 1.0f / fireRate;
        }
        fireCountdown -= Time.deltaTime;
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (!isBulletHoming)
        {
            PushBackBullet pbb = bullet.GetComponent<PushBackBullet>();
            if (pbb != null)            
                pbb.GetTarget(targetCar);
            
        }
        else
        {
            HomingPushBackBullet hpbb = bullet.GetComponent<HomingPushBackBullet>();
            if(hpbb != null)            
                hpbb.GetTarget(targetCar);
            
        }
    }

    private void UpdateTarget()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag(carTag);
        float shortestDist = Mathf.Infinity;
        GameObject nearestCar = null;

        foreach(GameObject c in cars)
        {
            float distToCar = Vector3.Distance(transform.position, c.transform.position);
            if(distToCar < shortestDist)
            {
                shortestDist = distToCar;
                nearestCar = c;
            }
        }

        if (nearestCar != null && shortestDist <= towerRange)
        {
            targetCar = nearestCar.transform;
        }
        else
            targetCar = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, towerRange);
    }
}
