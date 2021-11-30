using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCheckpoint : MonoBehaviour
{
    private CheckpointSystem cps;

    private void OnTriggerEnter(Collider other)
    {
        // Check for player
        var car = other.GetComponent<ICakeCar>();
        if(car != null)
        {
            cps.WentThroughCheckpoint(this, other.transform);
        }
    }

    public void SetTrackCheckpiont(CheckpointSystem cps)
    {
        this.cps = cps;
    }
}
