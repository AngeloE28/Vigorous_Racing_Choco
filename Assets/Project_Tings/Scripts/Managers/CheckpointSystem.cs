using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private List<SingleCheckpoint> checkPoints;
    [SerializeField] private int nextCheckPointIndex;
    [SerializeField] private int lapCounter;

    private void Awake()
    {
        this.checkPoints = new List<SingleCheckpoint>();

        Transform checkPoints = transform.Find("CheckPoint");
        foreach(Transform cp in checkPoints)
        {
            SingleCheckpoint checkPoint = cp.GetComponent<SingleCheckpoint>();
            checkPoint.SetTrackCheckpiont(this);
            this.checkPoints.Add(checkPoint);
        }

        nextCheckPointIndex = 0;
        lapCounter = 0;
    }

    public void WentThroughCheckpoint(SingleCheckpoint cp)
    {
        if (checkPoints.IndexOf(cp) == nextCheckPointIndex)
        {
            if (nextCheckPointIndex == checkPoints.Count - 1)
            {
                nextCheckPointIndex = 0;
                lapCounter++;
            }
            else
                nextCheckPointIndex++;
        }
    }
}
