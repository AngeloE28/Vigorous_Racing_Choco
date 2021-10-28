using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PositioningSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] cars;
    private float[] carPositions;    
    private ICakeCar[] cakeCars;    
    [SerializeField] private int playerCurrentPos;
    private float playerPos;    
    
    // Controllers
    private PlayerInputs player;    

    // Placements
    private int firstPlace = 1;
    private int secondPlace = 2;
    private int thirdPlace = 3;

    private void Awake()
    {
        
        carPositions = new float[cars.Length];        
        cakeCars = new ICakeCar[cars.Length];        

        // Assign the cake cars
        for (int i = 0; i < cars.Length; i++)
        {
            var cakeCar = cars[i].GetComponent<ICakeCar>();
            if (cakeCar != null)
            {
                cakeCars[i] = cakeCar;
            }
        }

        player = cars[0].GetComponent<PlayerInputs>();        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        CarPositions();
    }

    private void CalcCarRankings()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            var carDist = cakeCars[i].GetNextCheckPointDist();
            carPositions[i] = carDist;            
        }
        playerPos = player.GetComponent<ICakeCar>().GetNextCheckPointDist();
        Array.Sort(carPositions);
        playerCurrentPos = CalcPlayerRank(playerPos);
    }

    private void CarPositions()
    {
        // Check if player has the most, least or equal amounts of laps
        if (cakeCars[0].GetLapIndex() > cakeCars[1].GetLapIndex() &&
            cakeCars[0].GetLapIndex() > cakeCars[2].GetLapIndex())
        {
            playerCurrentPos = firstPlace;
        }
        if (cakeCars[0].GetLapIndex() < cakeCars[1].GetLapIndex() &&
            cakeCars[0].GetLapIndex() < cakeCars[2].GetLapIndex())
        {
            playerCurrentPos = thirdPlace;
        }
        if(cakeCars[0].GetLapIndex() == cakeCars[1].GetLapIndex() &&
           cakeCars[0].GetLapIndex() == cakeCars[2].GetLapIndex())
        {
            // Check if player has the most, least or equal amounts of checkpoints
            if (cakeCars[0].GetCheckPointIndex() > cakeCars[1].GetCheckPointIndex() &&
                cakeCars[0].GetCheckPointIndex() > cakeCars[2].GetCheckPointIndex())
            {
                playerCurrentPos = firstPlace;
            }

            if (cakeCars[0].GetCheckPointIndex() < cakeCars[1].GetCheckPointIndex() &&
                cakeCars[0].GetCheckPointIndex() < cakeCars[2].GetCheckPointIndex())
            {
                playerCurrentPos = thirdPlace;
            }

            if (cakeCars[0].GetCheckPointIndex() == cakeCars[1].GetCheckPointIndex() &&
                cakeCars[0].GetCheckPointIndex() == cakeCars[2].GetCheckPointIndex())
            {                
                // Check players current position since player has equal number of laps and checkpoints
                CalcCarRankings();
            }
        }
    }

    private int CalcPlayerRank(float carPos)
    {
        int carRank = Array.IndexOf(carPositions, carPos);
        
        switch(carRank)
        {
            case 0:
                carRank = firstPlace;
                break;
            case 1:
                carRank = secondPlace;
                break;
            case 2:
                carRank = thirdPlace;
                break;
            case 3:
                carRank = 4;
                break;
        }
        return carRank;
    }
}