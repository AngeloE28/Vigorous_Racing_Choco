using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    // Inputs
    private PlayerInputActions playerInputActions;

    // Player colliders and controllers
    [SerializeField] private Rigidbody carControllerRB;
    [SerializeField] private SphereCollider carControllerCollider;

    [Header("Movement Values")]    
    // Accelerations
    [SerializeField] private float forwardAccel;
    [SerializeField] private float reverseAccel, accelMultiplier;
    [SerializeField] private float gravityForce, gravityMultiplier, dragOnGround, dragInAir; // Controls the added gravity to the car
    [SerializeField] private float turnStrengthOnGround, turnStrengthInAir; // Maximum turn angles
    public float smoothCarRotationVal = 15.0f; // For smooth rotations, when driving on slopes

    // Inputs
    private float speedInput, turnInput;

    // Is car grounded
    private bool isGrounded = true; 

    [Header("Raycast System")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRayLength = 0.5f;    
    [SerializeField] private Transform[] groundRayPoint;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();    
    }


    // Start is called before the first frame update
    void Start()
    {
        // Unparent the car controller that will be followed
        carControllerRB.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        CarEngine();       
    }


    private void PlayerInput()
    {
        // Speed is 0.0f when not moving, which is when there is no inputs
        speedInput = 0.0f;        

        float inputAccelerate = playerInputActions.Player.Accelerate.ReadValue<float>();
        
        // Going forward
        if (inputAccelerate > 0)
        {
            speedInput = inputAccelerate * forwardAccel * accelMultiplier;
        }
        // Reversing
        else if (inputAccelerate < 0)
        {
            speedInput = inputAccelerate * reverseAccel * accelMultiplier;
        }

        // Turn input
        turnInput = playerInputActions.Player.Turning.ReadValue<float>();

        // Can only turn on the grounded if player is not moving
        if (isGrounded && speedInput != 0.0f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f,
                                                  turnInput * turnStrengthOnGround * inputAccelerate * Time.deltaTime,
                                                  0.0f));
        }

        if (!isGrounded)
        {
            Vector2 stickInput = playerInputActions.Player.Move.ReadValue<Vector2>();
            transform.Rotate(Vector3.right * stickInput.y * turnStrengthInAir * Time.deltaTime);

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f,
                                                  turnInput * turnStrengthInAir * Time.deltaTime,
                                                  0.0f));
        }

        // Car follows the carController thats moving
        transform.position = carControllerRB.transform.position;        
    }

    private void CarEngine()
    {
        isGrounded = false;
        // Check if the ray is colliding with any object with the Ground layermask
        Vector3[] normals = new Vector3[groundRayPoint.Length];
        RaycastHit[] hits = new RaycastHit[groundRayPoint.Length];
        for (int i = 0; i < groundRayPoint.Length; i++)
        {
            if (Physics.Raycast(groundRayPoint[i].position, -transform.up, out hits[i], groundRayLength, groundMask))
            {
                isGrounded = true;
                normals[i] = hits[i].normal;
            }
        }
        // Average the normals
        Vector3 avgNormal = (normals[0] + normals[1] + normals[2] + normals[3]) / groundRayPoint.Length;

        // Rotate the car
        Quaternion targetRotation;
        targetRotation = Quaternion.FromToRotation(transform.up, avgNormal) * transform.rotation;

        // Smooth out rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothCarRotationVal);

        // Check if car is grounded
        if (isGrounded)
        {
            // Update the drage when on the ground
            carControllerRB.drag = dragOnGround;

            // Check if input is received
            if (Mathf.Abs(speedInput) > 0)
            {
                carControllerRB.AddForce(transform.forward * speedInput);
            }
        }
        else
        {
            // Car not grounded
            // Increase the gravity applied to the car
            carControllerRB.drag = dragInAir;
            carControllerRB.AddForce(Vector3.up * -gravityForce * gravityMultiplier);
        }
    }

    public bool GetIsGroundedState()
    {
        // Get the isGrounded boolean
        return isGrounded;
    }
}
