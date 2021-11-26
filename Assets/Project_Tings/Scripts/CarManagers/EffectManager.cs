using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EffectManager : MonoBehaviour
{
    [Header("Car GameObject")]
    [SerializeField] private GameObject car;    

    [Header("Drift Trails")]
    [SerializeField] private ParticleSystem[] driftTrails;
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


    [Header("Wind Boost")]
    [SerializeField] private ParticleSystem windBoost;
    [SerializeField] private float maxWindboostEmission = 40.0f;

    [Header("Sounds")]
    [SerializeField] private AudioSource playerSFX;
    [SerializeField] private AudioClip driftSFX;
    [SerializeField] private AudioClip airBoostSignSFX;

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
        WindBoost();
    }    

#region Dust Trails

    // Drift Dust Trail
    private void CheckDrift()
    {
        if (player != null)
        {
            float emissionRate = 0.0f;
            float driftVol = 0.8f;
            if (player.GetIsDrifting() && player.GetIsGroundedState())
            {
                emissionRate = maxDustEmission;

                // Play sound
                playerSFX.loop = true;
                playerSFX.volume = driftVol;
                if (playerSFX.clip == null)
                    playerSFX.clip = driftSFX;
                if (!playerSFX.isPlaying)
                    playerSFX.Play();
            }
            else
            {
                emissionRate = 0.0f;

                // Reset Audiosource
                playerSFX.volume = 1.0f;
                playerSFX.loop = false;
                playerSFX.clip = null;
                playerSFX.Stop();
            }

            EmitDustTrail(emissionRate, driftTrails);
        }
    }

    private void DustTrail()
    {        
        if(player != null && player.enabled)
        {
            float emissionRate = 0.0f;
            if (player.GetIsGroundedState() && player.GetSpeedInput() != 0.0f)
                emissionRate = maxDustEmission;

            if (!player.GetIsGroundedState())
                emissionRate = 0.0f;

            EmitDustTrail(emissionRate, dustTrails);
        }

        if(carAI!= null && carAI.enabled)
        {
            float emissionRate = 0.0f;
            if (carAI.GetIsGroundedState() && carAI.GetAISpeedInput() != 0.0f)
                emissionRate = maxDustEmission;

            if (!carAI.GetIsGroundedState())
                emissionRate = 0.0f;

            EmitDustTrail(emissionRate, dustTrails);
        }               
    }

    private void EmitDustTrail(float emissionRate, ParticleSystem[] system)
    {
        foreach (ParticleSystem trail in system)
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
                // Play sfx
                // Make sure its not looping
                if (playerSFX.loop)
                    playerSFX.loop = false;

                if (!boostSign.isPlaying)
                {
                    boostSign.Play();
                    playerSFX.clip = airBoostSignSFX;
                    playerSFX.Play();
                }                

                fxController = false;
            }
        }
    }

#endregion

#region WindBoost
    private void WindBoost()
    {
        if(player != null)
        {
            float emissionRate = 0.0f;
            var emissionModule = windBoost.emission;
            if (player.GetIsBoosting())
                emissionRate = maxWindboostEmission;
            else
                emissionRate = 0.0f;

            emissionModule.rateOverTime = emissionRate;
        }
    }    

#endregion
}
