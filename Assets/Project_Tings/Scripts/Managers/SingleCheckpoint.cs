using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCheckpoint : MonoBehaviour
{
    private CheckpointSystem cps;

    private void OnTriggerEnter(Collider other)
    {
        // Check for player
        var p = other.GetComponent<PlayerInputs>();
        if(p != null)
        {
            cps.WentThroughCheckpoint(this);
        }
    }

    public void SetTrackCheckpiont(CheckpointSystem cps)
    {
        this.cps = cps;
    }
}
