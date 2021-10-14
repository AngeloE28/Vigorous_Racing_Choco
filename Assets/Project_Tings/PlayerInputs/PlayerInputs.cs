using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [SerializeField] private PlayerCamera cameraController;

    // Player controllers
    [Header("Car Controller")]
    [SerializeField] private Rigidbody carControllerRB;    

    [Header("Movement Values")]    
    // Accelerations
    [SerializeField] private float forwardAccel;
    [SerializeField] private float reverseAccel, accelMultiplier;
    [SerializeField] private float gravityForce, gravityMultiplier, dragOnGround, dragInAir; // Controls the added gravity to the car
    [SerializeField] private float turnStrengthOnGround, turnStrengthInAir; // Maximum turn angles
    private float speedController;
    private float defaultSpeedControllerVal = 1.0f;
    [SerializeField] private float smoothCarRotationVal = 15.0f; // For smooth rotations, when driving on slopes

    // Inputs
    private PlayerInputActions playerInputActions;    
    private float speedInput, turnInput;

    [Header("Boost Values")]
    // Drift boost
    [SerializeField] private float speedReduction = 0.8f;
    [SerializeField] private float maxDriftBoostTime = 2.0f;
    [SerializeField] private float minDriftBoostTime = 0.75f;
    [SerializeField] private float driftBoostSpeedVal = 4.0f;
    private bool allowBoost = false;
    private float defaultTurnControllerVal = 1.0f;
    private float driftTurnMultiplier = 1.5f;
    private float driftTotal;
    private float turnController;
    private bool isDrifiting = false;
    private bool isBoosting = false;

    // Flip boost
    [SerializeField] private float flipBoostAmount = 6.0f;
    private float spinTotal;
    private bool allowFlipBoost = false;

    // Is car grounded
    private bool isGrounded = true; 

    [Header("Raycast System")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRayLength = 0.5f;    
    [SerializeField] private Transform[] groundRayPoints;

    [Header("Wheels")]
    [SerializeField] private float maxWheelTurnAngle;
    [SerializeField] private Transform frontLeftWheel, frontRightWheel; // Turning the wheel left and right
    [SerializeField] private Transform frontLeftWheelModel;
    [SerializeField] private Transform frontRightWheelModel;
    [SerializeField] private Transform[] backWheels;
    private float wheelSpinDirection;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }


    // Start is called before the first frame update
    void Start()
    {
        turnController = defaultTurnControllerVal; // Default to 1
        speedController = defaultSpeedControllerVal; // Default to 1

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
        print(driftTotal);
    }


    private void PlayerInput()
    {
        // Speed is 0.0f when not moving, which is when there is no inputs
        speedInput = 0.0f;        

        float inputAccelerate = playerInputActions.Player.Accelerate.ReadValue<float>();
        
        // Going forward
        if (inputAccelerate > 0)
        {
            speedInput = inputAccelerate * forwardAccel * accelMultiplier * speedController;
            wheelSpinDirection = 1;
        }
        // Reversing
        else if (inputAccelerate < 0)
        {
            speedInput = inputAccelerate * reverseAccel * accelMultiplier * speedController;
            wheelSpinDirection = -1;
        }

        // Turn input
        turnInput = playerInputActions.Player.Turning.ReadValue<float>();



        // Can only turn on the grounded if player is not moving
        if (isGrounded && speedInput != 0.0f)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f,
                                                  turnInput * turnStrengthOnGround * inputAccelerate * turnController *Time.deltaTime,
                                                  0.0f));
        }

        Vector2 stickInput = playerInputActions.Player.Move.ReadValue<Vector2>();
        if (!isGrounded)
        {
            transform.Rotate(Vector3.right * stickInput.y * turnStrengthInAir * Time.deltaTime);

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f,
                                                  turnInput * turnStrengthInAir * Time.deltaTime,
                                                  0.0f));
        }
        
        // Apply a boost for drifting
        DriftBoost(turnInput);

        // Apply a boost after spinning around a certain amount
        FlipBoost(stickInput);

        // Procedurally Animate the wheels
        WheelTurns(turnInput, maxWheelTurnAngle);

        // Car follows the carController thats moving
        transform.position = carControllerRB.transform.position;        
    }
      
    private void CarEngine()
    {
        isGrounded = false;
        // Check if the ray is colliding with any object with the Ground layermask
        Vector3[] normals = new Vector3[groundRayPoints.Length];
        RaycastHit[] hits = new RaycastHit[groundRayPoints.Length];
        for (int i = 0; i < groundRayPoints.Length; i++)
        {
            if (Physics.Raycast(groundRayPoints[i].position, -transform.up, out hits[i], groundRayLength, groundMask))
            {
                isGrounded = true;
                normals[i] = hits[i].normal;
            }
        }
        // Average the normals
        Vector3 avgNormal = (normals[0] + normals[1] + normals[2] + normals[3]) / groundRayPoints.Length;

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

    private void WheelTurns(float turnInput, float turnAngle)
    {
        // Turn both wheels in the direction the car is turning
        frontLeftWheel.localRotation = Quaternion.Euler(frontLeftWheel.localRotation.eulerAngles.x,
                                                        (turnInput * turnAngle),
                                                        frontLeftWheel.localRotation.eulerAngles.z);

        frontRightWheel.localRotation = Quaternion.Euler(frontRightWheel.localRotation.eulerAngles.x,
                                                        (turnInput * turnAngle),
                                                        frontRightWheel.localRotation.eulerAngles.z);

        float turnMultiplier = 100.0f;
        // Spin the wheel back or forward
        frontLeftWheelModel.Rotate(-Vector3.up * carControllerRB.velocity.magnitude * turnMultiplier * wheelSpinDirection * Time.deltaTime);
        frontRightWheelModel.Rotate(Vector3.up * carControllerRB.velocity.magnitude * turnMultiplier * wheelSpinDirection * Time.deltaTime);
        foreach (Transform t in backWheels)
        {
            t.Rotate(Vector3.right * carControllerRB.velocity.magnitude * turnMultiplier * wheelSpinDirection * Time.deltaTime);
        }
    }

    private void DriftBoost(float turnInput)
    {
        float driftButton = playerInputActions.Player.Drift.ReadValue<float>();       

        isDrifiting = (driftButton > 0) ? true : false;
        
        if (isGrounded)
        {
            if (isDrifiting)
            {
                
                driftTotal += Mathf.Abs(turnInput * Time.deltaTime);               

                if (driftTotal > maxDriftBoostTime)
                    driftTotal = maxDriftBoostTime;

                speedController = speedReduction;
                turnController = driftTurnMultiplier;                         
            }
            else
            {
                // Reset turncontroller
                turnController = defaultTurnControllerVal;

                if (driftTotal > minDriftBoostTime)
                {                    
                    allowBoost = true;
                }
                if (allowBoost && driftTotal > 0)
                {
                    // Start countdown
                    driftTotal -= 1 * Time.deltaTime;
                }

                if (driftTotal <= 0)
                {
                    // Can't boost anymore
                    allowBoost = false;
                    driftTotal = 0.0f;
                }
                if(driftTotal < minDriftBoostTime)
                {
                    driftTotal = 0.0f;
                }
            }
            
            // Apply boost
            Boost(allowBoost, driftBoostSpeedVal, driftTotal);
        }
    }

    private void FlipBoost(Vector2 stickInput)
    {
        // Provide a boost after rotating a certain amount        
        float boostFlipRequirement = 300.0f;

        float resetFlipBoostTime = 1.0f;

        // Increment the amount of spin
        if (!isGrounded)                   
            spinTotal += stickInput.magnitude * turnStrengthInAir *Time.deltaTime;        

        // Check if player can boost or not
        allowFlipBoost = (spinTotal > boostFlipRequirement) ? true : false;        
        
        // Execute an action if player can boost or not
        if (isGrounded)
        {
            if (allowFlipBoost)
            {
                // Boost
                Boost(allowFlipBoost, flipBoostAmount, resetFlipBoostTime);                
                Invoke("ResetFlipBoost", resetFlipBoostTime);            
            }
            else
            {                
                // Reset the spin total
                if (spinTotal > 0 && spinTotal < boostFlipRequirement)
                    spinTotal = 0.0f;
            }
        }
    }

    private void Boost(bool isBoosting, float boostAmount, float boostTime)
    {
        if(isBoosting)
        {
            this.isBoosting = isBoosting;

            // Apply boost 
            speedController = driftBoostSpeedVal;

            // Make sure player is constantly moving
            if (Mathf.Abs(speedInput) != 1)
                speedInput = wheelSpinDirection * forwardAccel * accelMultiplier * speedController;            
            
            // Camera effect
            cameraController.SetCamPos(true);            
            Invoke("ResetBoost", boostTime);
        }        
    }

    private void ResetBoost()
    {
        // Reset values
        speedController = defaultSpeedControllerVal;
        isBoosting = false;
        cameraController.SetCamPos(false);
    }

    private void ResetFlipBoost()
    {    
        // Reset values
        spinTotal = 0.0f;                
        allowFlipBoost = false;
    }

    public bool GetIsGroundedState()
    {
        // Get the isGrounded boolean
        return isGrounded;
    }

    public bool GetIsBoosting()
    {
        return isBoosting;
    }

    public bool GetIsDrifting()
    {
        return isDrifiting;
    }

    public PlayerInputActions GetPlayerInputActions()
    {
        // Returns the player input actions
        return playerInputActions;
    }
        
    public float GetSpeedInput()
    {
        return speedInput;
    }

    public Rigidbody GetCarController()
    {
        return carControllerRB;
    }
}
