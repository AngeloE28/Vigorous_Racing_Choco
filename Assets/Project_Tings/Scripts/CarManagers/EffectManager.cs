using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("Car GameObject")]
    [SerializeField] private GameObject car;    

    [Header("Tire marks")]
    [SerializeField] private TrailRenderer[] tireMarks;
    private bool isTireMarks = false;

    [Header("Dust Trails")]
    [SerializeField] private float maxEmission = 25.0f;
    [SerializeField] private ParticleSystem[] dustTrails;    

    // Player or ai scripts
    private PlayerInputs player;
    private CarAI carAI;


    private void Awake()
    {
        player = car.GetComponent<PlayerInputs>();
        carAI = car.GetComponent<CarAI>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDrift();
        DustTrail();
    }

    #region Drift Marks

    private void CheckDrift()
    {        
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

    #endregion

    #region Dust Trail

    private void DustTrail()
    {        
        if(player != null && player.enabled)
        {
            float emissionRate = 0.0f;
            if (player.GetIsGroundedState() && player.GetSpeedInput() != 0.0f)
                emissionRate = maxEmission;

            if (!player.GetIsGroundedState())
                emissionRate = 0.0f;

            EmitDustTrail(emissionRate);
        }

        if(carAI!= null && carAI.enabled)
        {
            float emissionRate = 0.0f;
            if (carAI.GetIsGroundedState() && carAI.GetAISpeedInput() != 0.0f)
                emissionRate = maxEmission;

            if (!carAI.GetIsGroundedState())
                emissionRate = 0.0f;

            EmitDustTrail(emissionRate);
        }               
    }

    private void EmitDustTrail(float emissionRate)
    {
        foreach (ParticleSystem trail in dustTrails)
        {
            var emissionModule = trail.emission;
            emissionModule.rateOverTime = emissionRate;
        }
    }

    #endregion
}
