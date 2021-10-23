using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ChocolateSlow : MonoBehaviour
{
    [SerializeField] private GameObject vfxChocolateGameobject;    
    private VisualEffect vfxChocolate;
    private bool activate = false;

    private void Awake()
    {
        vfxChocolate = GetComponentInChildren<VisualEffect>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(FireChocolate), 0.0f, 0.5f);
    }

    private void Update()
    {
        float val = 0.0f;
        if (activate)
            val = Mathf.Lerp(val, 128.0f, 100.0f);
        else
            val = Mathf.Lerp(val, 0.0f, 10.0f);

        vfxChocolate.SetFloat("Rate", val);        
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
        GameObject[] cars = GameObject.FindGameObjectsWithTag("Car");
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
