using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private PlayerInputs player;

    [Header("Sounds")]
    [SerializeField] private AudioSource carSFX;
    [SerializeField] private AudioClip engine;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if(player.GetIsDrifting() && player.GetIsGroundedState())
        {
            
        }   
        
    }
}
