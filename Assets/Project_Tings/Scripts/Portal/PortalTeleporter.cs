using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct CakeCar
{
    public Transform transform;
    public bool overLapped;
}

public class PortalTeleporter : MonoBehaviour
{
    [SerializeField] private Transform portalExit;
    private List<CakeCar> cakeCarDetails; // The details of what goes through the portal    
    private List<GameObject> cakeCars;// What goes through the portal    

    // Start is called before the first frame update
    void Start()
    {
        cakeCars = new List<GameObject>();
        cakeCarDetails = new List<CakeCar>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        for(int i = 0; i<cakeCars.Count; i++)
        {
            if(cakeCarDetails[i].overLapped)
            {
                CakeCar currentCar;
                currentCar.transform = cakeCarDetails[i].transform;
                Vector3 portalToCar = currentCar.transform.position - transform.position;
                float dotProduct = Vector3.Dot(transform.up, portalToCar);

                // Player has moved across the portal
                if (dotProduct < 0.0f)
                {
                    // Teleport player
                    float rotationDiff = -Quaternion.Angle(transform.rotation, portalExit.rotation);
                    rotationDiff += 180;
                    currentCar.transform.Rotate(Vector3.up, rotationDiff);

                    Vector3 posOffset = Quaternion.Euler(0.0f, rotationDiff, 0.0f) * portalToCar;
                    currentCar.transform.position = portalExit.position + posOffset;

                    currentCar.overLapped = false;
                    cakeCarDetails[i] = currentCar;
                }
            }
            //cakeCarDetails.Remove(cakeCarDetails[i]);
        }  
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car")
        {
            CakeCar car;
            car.transform = other.transform;
            car.overLapped = true;
            cakeCarDetails.Add(car);
            cakeCars.Add(other.gameObject);
        }
    }
}
