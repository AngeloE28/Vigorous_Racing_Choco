using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [SerializeField] private List<Transform> cars;
    [SerializeField] private List<int> lapCounters;
    private List<SingleCheckpoint> checkPoints;
    private List<int> nextCheckPointIndexes;    

    private void Awake()
    {
        this.checkPoints = new List<SingleCheckpoint>();
        nextCheckPointIndexes = new List<int>();
        lapCounters = new List<int>();
        
        foreach(var t in cars)
        {
            nextCheckPointIndexes.Add(0);
            lapCounters.Add(1); // Cars start with a lap index of 1
        }

        Transform checkPoints = transform.Find("CheckPoint");
        foreach(Transform cp in checkPoints)
        {
            SingleCheckpoint checkPoint = cp.GetComponent<SingleCheckpoint>();
            checkPoint.SetTrackCheckpiont(this);
            this.checkPoints.Add(checkPoint);            
        }
        
        CalcCarCheckpointDist();        
    }    

    private void FixedUpdate()
    {
        CalcCarCheckpointDist();
    }

    private void CalcCarCheckpointDist()
    {        
        // Calculate the distance of each car to the next checkpoint
        CarDistToCheckPoint(0);
        CarDistToCheckPoint(1);
        CarDistToCheckPoint(2);        
    }

    public void WentThroughCheckpoint(SingleCheckpoint cp, Transform car)
    {
        int nextCPIndex = this.nextCheckPointIndexes[cars.IndexOf(car)];
        int lapCounter = this.lapCounters[cars.IndexOf(car)];
        if (checkPoints.IndexOf(cp) == nextCPIndex)
        {
            if (nextCPIndex == checkPoints.Count - 1)
            {
                nextCPIndex = 0;
                lapCounter++;
            }
            else
                nextCPIndex++;

            this.nextCheckPointIndexes[cars.IndexOf(car)] = nextCPIndex;
            this.lapCounters[cars.IndexOf(car)] = lapCounter;

            cars[cars.IndexOf(car)].gameObject.GetComponent<ICakeCar>().SetLapIndex(lapCounter);
            cars[cars.IndexOf(car)].gameObject.GetComponent<ICakeCar>().SetNextCheckPointIndex(nextCPIndex);
        }
    }    

    private void CarDistToCheckPoint(int carIndex)
    {
        var car = cars[carIndex].GetComponent<ICakeCar>();
        float carDist = Vector3.Distance(cars[carIndex].position, checkPoints[nextCheckPointIndexes[carIndex]].transform.position);
        if (car != null)
        {
            car.SetDistToNextCheckPoint(carDist);
        }
    }
}
