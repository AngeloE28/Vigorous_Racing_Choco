using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] private Renderer[] renders;
    [SerializeField] private float minHeight = 2.0f;
    [SerializeField] private float maxHeight = 25.0f;    
    private float currentHeight = 8.0f;
    private bool dissolve = false;

    private void Start()
    {
        currentHeight = maxHeight;
        foreach (var r in renders)
        {
            foreach (var m in r.materials)
            {
                m.SetFloat("_Height", currentHeight);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeDissolve();
        }
        if (dissolve)
        {            
            currentHeight = Mathf.Lerp(currentHeight, minHeight, Time.deltaTime);
        }
        else
        {         
            currentHeight = Mathf.Lerp(currentHeight, maxHeight, Time.deltaTime/2);
        }

        foreach (var r in renders)
        {
            foreach (var m in r.materials)
            {
                m.SetFloat("_Height", currentHeight);
            }
        }
    }

    private void ChangeDissolve()
    {
        if (dissolve)
            dissolve = false;
        else
            dissolve = true;
    }
}
