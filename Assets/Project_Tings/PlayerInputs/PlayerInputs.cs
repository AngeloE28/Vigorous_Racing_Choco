using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    // Inputs
    private PlayerInputActions playerInputActions;

    [SerializeField] private PlayerCamera cameraController;

    // Player colliders and controllers
    [Header("Colliders and Controller")]
    [SerializeField] private Rigidbody carControllerRB;
    [SerializeField] private SphereCollider carControllerCollider;

    [Header("Movement Values")]    
    // Accelerations
    [SerializeField] private float forwardAccel;
    [SerializeField] private float reverseAccel, accelMultiplier;
    [SerializeField] private float gravityForce, gravityMultiplier, dragOnGround, dragInAir; // Controls the added gravity to the car
    [SerializeField] private float turnStrengthOnGround, turnStrengthInAir; // Maximum turn angles
    public float smoothCarRotationVal = 15.0f; // For smooth rotations, when driving on slopes

    [Header("Boost Values")]
    [SerializeField] private float flipBoostAmount = 500.0f;

    // Inputs
    private float speedInput, turnInput;

    // Flip boost
    private float spinTotal;
    private bool allowFlipBoost = false;

    // Is car grounded
    private bool isGrounded = true; 

    [Header("Raycast System")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundRayLength = 0.5f;    
    [SerializeField] private Transform[] groundRayPoint;

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
            wheelSpinDirection = 1;
        }
        // Reversing
        else if (inputAccelerate < 0)
        {
            speedInput = inputAccelerate * reverseAccel * accelMultiplier;
            wheelSpinDirection = -1;
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

        Vector2 stickInput = playerInputActions.Player.Move.ReadValue<Vector2>();
        if (!isGrounded)
        {
            transform.Rotate(Vector3.right * stickInput.y * turnStrengthInAir * Time.deltaTime);

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f,
                                                  turnInput * turnStrengthInAir * Time.deltaTime,
                                                  0.0f));
        }
        
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

    private void FlipBoost(Vector2 stickInput)
    {
        // Provide a boost after rotating a certain amount        
        float boostFlipRequirement = 130.0f;        
        
        // Increment the amount of spin
        if (!isGrounded)                   
            spinTotal += stickInput.magnitude;

        // Check if player cna boost or not
        if (spinTotal > boostFlipRequirement)
            allowFlipBoost = true;
        else
            allowFlipBoost = false;        

        // Execute an action if player can boost or not
        if (allowFlipBoost)
        {                       
            if (isGrounded)
            {                
                StartCoroutine(ResetFlipBoost());  
            }
        }
        else
        {
            if (isGrounded)
            {
                if (spinTotal > 0 && spinTotal < boostFlipRequirement)
                    spinTotal = 0.0f;
            }
        }
    }

    private IEnumerator ResetFlipBoost()
    {
        float resetFlipBoostTime = 0.1f;
        float resetCamTime = 0.3f;        

        yield return new WaitForSeconds(resetFlipBoostTime);

        // Boost and boost effect
        cameraController.SetCamPos(true);
        carControllerRB.AddForce(transform.forward * flipBoostAmount, ForceMode.Impulse);

        // Reset values
        allowFlipBoost = false;
        spinTotal = 0.0f;
        Invoke("ResetCamPos", resetCamTime);
    }

    private void ResetCamPos()
    {
        cameraController.SetCamPos(false);
    }

    public bool GetIsGroundedState()
    {
        // Get the isGrounded boolean
        return isGrounded;
    }
}
