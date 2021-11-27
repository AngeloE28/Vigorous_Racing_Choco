using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ChocolateSlow : MonoBehaviour
{
    [SerializeField] private GameObject vfxChocolateGameobject;
    [SerializeField] private float updateFreq = 0.15f;
    [SerializeField] private string carTag = "Car";
    private VisualEffect vfxChocolate;
    private bool activate = false;
    
    [Header("Sounds")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private float targetPitch = 0.5f;
    [SerializeField] private float targetVol = 1.0f;

    private void Awake()
    {
        vfxChocolate = GetComponentInChildren<VisualEffect>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(FireChocolate), 0.0f, updateFreq);
    }

    private void Update()
    {
        // VFX
        float chocolateVfxRate = 0.0f;
        float targetVFXRate = 128.0f;

        // Volume
        float audioVol = 0.0f;
        float audioPitch = 0.0f;

        // Lerping vals
        float smoothActivate = 100.0f;
        float smoothDeactivate = 10.0f;

        // Activate vfx and audio
        if (activate)
        {
            chocolateVfxRate = Mathf.Lerp(chocolateVfxRate, targetVFXRate, smoothActivate);
            audioVol = Mathf.Lerp(audioVol, targetVol, smoothActivate);
            audioPitch = Mathf.Lerp(audioPitch, targetPitch, smoothActivate);
        }
        else
        {
            chocolateVfxRate = Mathf.Lerp(chocolateVfxRate, 0.0f, smoothDeactivate);
            audioVol = Mathf.Lerp(audioVol, 0.0f, smoothDeactivate);
            audioPitch = Mathf.Lerp(audioPitch, 0.0f, smoothDeactivate);
        }

        sfx.volume = audioVol;
        sfx.pitch = audioPitch;
        vfxChocolate.SetFloat("Rate", chocolateVfxRate);
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (activate)
        {
            ICakeCar car = other.GetComponent<ICakeCar>();
            if (car != null)                            
                car.SlowDown();
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ICakeCar car = other.GetComponent<ICakeCar>();
        if (car != null)        
            car.ReturnToOriginalSpeed();        
    }

    private void FireChocolate()
    {
        GameObject[] cars = GameObject.FindGameObjectsWithTag(carTag);
        List<GameObject> cs = new List<GameObject>();

        foreach(GameObject c in cars)
        {
            float distToCar = Vector3.Distance(transform.position, c.transform.position);
            if (distToCar < 50.0f)            
                cs.Add(c);            
            else
                cs.Remove(c);
        }

        if (cs.Count > 0)
            activate = true;
        if (cs.Count == 0)
            activate = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 50.0f);
    }
}
