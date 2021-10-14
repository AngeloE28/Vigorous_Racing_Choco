using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct CakeCar
{
    public GameObject car;
    public bool overLapped;
}

public class PortalTeleporter : MonoBehaviour
{
    [SerializeField] private Transform portalParent;
    [SerializeField] private Transform portalExitParent;
    [SerializeField] private Transform portalExitCollider;
    [SerializeField] private bool turnCarAround = false;
    [SerializeField] private GameObject[] allCars;
    private Dictionary<int, CakeCar> cakeCars; // Controls which car to teleport    

    private void Awake()
    {
        cakeCars = new Dictionary<int, CakeCar>();

        // Add all the cars
        for(int i = 0; i < allCars.Length; i++)
        {           
            CakeCar aiCar;
            aiCar.car = allCars[i];
            aiCar.overLapped = false;
            cakeCars.Add(i, aiCar);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Loop through all the cars
        for(int i = 0; i < allCars.Length; i++)
        {
            CakeCar currentCar = cakeCars[i];
            if(currentCar.overLapped)
            {
                Vector3 portalToCar = currentCar.car.transform.position - transform.position;
                float dotProduct = Vector3.Dot(transform.forward, portalToCar);

                // Car has moved across the portal
                if (dotProduct < 0.0f)
                {
                    // Teleport car
                    var m = portalExitParent.transform.localToWorldMatrix * portalParent.worldToLocalMatrix * currentCar.car.transform.localToWorldMatrix;
                    float rotationDiff = -Quaternion.Angle(transform.rotation, portalExitParent.rotation);
                    
                    // Check if this portal should turn the car around
                    if (turnCarAround)
                        rotationDiff += 180;                    
                    
                    // Get the player and ai scripts
                    var playerController = currentCar.car.gameObject.GetComponent<PlayerInputs>();
                    var aiController = currentCar.car.gameObject.GetComponent<CarAI>();

                    // Check for player
                    if (playerController != null && aiController == null)
                    {
                        // Rotate the player
                        currentCar.car.transform.Rotate(Vector3.up, rotationDiff);

                        // Teleport the player
                        Vector3 posOffset = Quaternion.Euler(0.0f, rotationDiff, 0.0f) * portalToCar;
                        playerController.GetCarController().gameObject.transform.position = portalExitCollider.position + posOffset;
                        currentCar.overLapped = false;
                    }
                    // Check for ai
                    else if (aiController != null && playerController == null)
                    {
                        // Rotate the player
                        currentCar.car.transform.Rotate(Vector3.up, rotationDiff);

                        // Teleport the player
                        Vector3 posOffset = Quaternion.Euler(0.0f, rotationDiff, 0.0f) * portalToCar;
                        aiController.GetController().gameObject.transform.position = portalExitCollider.position + posOffset;
                        currentCar.overLapped = false;
                    }
                    else
                        continue; // Unknown gameObject, skip this iteration
                }
            }
            cakeCars[i] = currentCar; // Update the value of the car
        }        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Car")
        {
            for (int i = 0; i < allCars.Length; i++)
            {
                CakeCar currentCar = cakeCars[i];

                // Get player and ai script
                var playerController = currentCar.car.gameObject.GetComponent<PlayerInputs>();
                var aiController = currentCar.car.gameObject.GetComponent<CarAI>();
                // Check for player
                if (playerController != null && aiController == null)
                {
                    if (playerController.GetCarController().gameObject == other.gameObject)
                    {
                        currentCar.overLapped = true;
                        cakeCars[i] = currentCar;
                        continue; // Its the player, skip this iteration
                    }
                }
                // Check for ai
                else if (aiController != null && playerController == null)
                {
                    if (aiController.GetController().gameObject == other.gameObject)
                    {
                        currentCar.overLapped = true;
                        cakeCars[i] = currentCar;
                        continue;
                    }
                }
                else
                    continue; // Unknown gameObject, skip this iteration
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Car")
        {
            for (int i = 0; i < allCars.Length; i++)
            {
                CakeCar currentCar = cakeCars[i];
                // Get player and ai script
                var playerController = currentCar.car.gameObject.GetComponent<PlayerInputs>();
                var aiController = currentCar.car.gameObject.GetComponent<CarAI>();
                // Check for player
                if (playerController != null && aiController == null)
                {
                    if (playerController.GetCarController().gameObject == other.gameObject)
                    {
                        currentCar.overLapped = false;
                        cakeCars[i] = currentCar;
                        continue; // Its the player, skip this iteration
                    }
                }
                // Check for ai
                else if (aiController != null && playerController == null)
                {
                    if (aiController.GetController().gameObject == other.gameObject)
                    {
                        cakeCars[i] = currentCar;
                        currentCar.overLapped = true;
                        continue;
                    }
                }
                else
                    continue; // Unknown gameObject, skip this iteration
            }
        }        
    }
}
