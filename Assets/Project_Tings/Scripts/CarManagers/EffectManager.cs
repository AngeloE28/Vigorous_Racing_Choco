using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private TrailRenderer[] tireMarks;
    private bool isTireMarks = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckDrift();        
    }

    private void CheckDrift()
    {
        var player = car.GetComponent<PlayerInputs>();
        if (player != null)
        {
            if (player.GetIsDrifting())
                StartTireMarkEmitter();
            else
                StopTireMarkEmitter();
        }
    }

    private void StartTireMarkEmitter()
    {
        if (isTireMarks)
            return;
        foreach(var t in tireMarks)
        {
            t.emitting = true;
        }

        isTireMarks = true;
    }

    private void StopTireMarkEmitter()
    {
        if (!isTireMarks)
            return;

        foreach(var t in tireMarks)
        {
            t.emitting = false;
        }
        isTireMarks = false;
    }
}
