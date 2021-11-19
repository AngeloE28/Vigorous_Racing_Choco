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
    [SerializeField] private float maxDustEmission = 25.0f;
    [SerializeField] private ParticleSystem[] dustTrails;   

    [Header("Drift Sparks")]
    [SerializeField] private float maxSparkEmission = 10.0f;    
    [SerializeField] private ParticleSystem[] sparks;
    [SerializeField] private Renderer[] sparksRenderer;
    [ColorUsage(true, true)]
    [SerializeField] private Color minBoostColor;    
    [ColorUsage(true, true)]
    [SerializeField] private Color maxBoostColor;

    [Header("Boost Sign")]    
    [SerializeField] private ParticleSystem boostSign;    
    private bool fxController = false, playFX = false;


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
        SparksManager();
        BoostSignPlayerFlip();
    }    

#region Drift Marks

    private void CheckDrift()
    {        
        if (player != null)
        {
            if (player.GetIsGroundedState())
            {
                if (player.GetIsDrifting())
                    StartTireMarkEmitter();
                else
                    StopTireMarkEmitter();
            }
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
                emissionRate = maxDustEmission;

            if (!player.GetIsGroundedState())
                emissionRate = 0.0f;

            EmitDustTrail(emissionRate);
        }

        if(carAI!= null && carAI.enabled)
        {
            float emissionRate = 0.0f;
            if (carAI.GetIsGroundedState() && carAI.GetAISpeedInput() != 0.0f)
                emissionRate = maxDustEmission;

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

#region Drift Sparks

    private void SparksManager()
    {
        // Controll the colour of the sparks
        SparkColourManager();

        if (player != null && player.enabled)
        {
            float emissionRate = 0.0f;
            if (player.GetIsGroundedState())
            {
                if (player.GetIsDrifting() && player.GetDriftTotal() > player.GetMinDrift())
                    emissionRate = maxSparkEmission;
                else
                    emissionRate = 0.0f;
            }
            else
                emissionRate = 0.0f;

            EmitSparks(emissionRate);
        }
    }

    private void EmitSparks(float emissionRate)
    {
        foreach(ParticleSystem spark in sparks)
        {
            var emissionModule = spark.emission;
            emissionModule.rateOverTime = emissionRate;
        }
    }

    private void SparkColourManager()
    {
        if (player != null)
        {
            if (player.GetDriftTotal() > player.GetMinDrift())            
                SetSparkColour(minBoostColor);                
            
            if (player.GetDriftTotal() >= player.GetMaxDrift())            
                SetSparkColour(maxBoostColor);                
                        
        }
    }

    private void SetSparkColour(Color color)
    {
        foreach (var r in sparksRenderer)
        {
            foreach (var m in r.materials)
                m.SetColor("_Color", color);
        }
    }

#endregion

#region BoostSign

    private void BoostSignPlayerFlip()
    {      
        // Play the effect only once, everytime player can boost from flipping
        if (player != null)
        {
            playFX = player.GetAllowFlipBoost();
            if (playFX)
                fxController = true;            

            if (fxController)
            {
                if (!boostSign.isPlaying)
                    boostSign.Play();
                fxController = false;
            }
        }
    }    

#endregion
}
