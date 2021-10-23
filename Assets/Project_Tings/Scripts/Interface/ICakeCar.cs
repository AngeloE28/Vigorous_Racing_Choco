using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICakeCar
{
    // Tower effect functions
    void PushBack(float backwardForce, float upwardForce, float forceMult);
    void SlowDown();
    void ReturnToOriginalSpeed();

    // Positioning system

    float GetNextCheckPointDist();
    int GetLapIndex();
    int GetCheckPointIndex();
    void SetDistToNextCheckPoint(float dist);
    void SetLapIndex(int i);
    void SetNextCheckPointIndex(int i);
}
