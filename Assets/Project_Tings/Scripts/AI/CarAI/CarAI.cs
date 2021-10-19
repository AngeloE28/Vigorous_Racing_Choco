using System.Collections.Generic;
using UnityEngine;

public class CarAI : MonoBehaviour, IPushBack
{
    [SerializeField] private Rigidbody carAIRb;

    // Path
    [SerializeField] private Transform path;
    private List<Transform> pathNodes;
    private int currentNode = 0;

    [Header("Movement Values")]
    // Acceleration
    [SerializeField] private float forwardAccel;
    [SerializeField] private float accelMultiplier;
    [SerializeField] private float gravityForce, gravityMultiplier, dragOnGround, dragInAir;
    [SerializeField] private float turnAngle;
    [SerializeField] private float smoothCarRotationVal = 15.0f; // For smooth rotations, when driving on slopes
    [SerializeField] private float maxDistFromWaypoint = 7.5f;
    private float aiSpeed;

    // AI Controls
    private float aiTurnInput;
    private bool isGrounded;

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


    // Start is called before the first frame update
    void Start()
    {
        Transform[] wayPoints = path.GetComponentsInChildren<Transform>();
        pathNodes = new List<Transform>();

        // Add waypoints
        for (int i = 0; i < wayPoints.Length; i++)
        {
            if (wayPoints[i] != path.transform)
                pathNodes.Add(wayPoints[i]);
        }

        // Unparent car controller
        carAIRb.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Car follows the car controller
        transform.position = carAIRb.transform.position;
    }

    private void FixedUpdate()
    {
        CarEngine();
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

        if (isGrounded)
        {
            carAIRb.drag = dragOnGround;
            AIDrive();
        }
        else
        {
            carAIRb.drag = dragInAir;

            // Increase gravity
            carAIRb.AddForce(Vector3.up * -gravityForce * gravityMultiplier);
        }

        CheckWayPointDistance();
    }

    private void CheckWayPointDistance()
    {
        // Check ai distance from next waypoint and loop through each waypoint
        if (Vector3.Distance(transform.position, pathNodes[currentNode].position) < maxDistFromWaypoint)
        {
            // Check if its last waypoint
            if (currentNode == pathNodes.Count - 1)
                currentNode = 0;
            else
                currentNode++;
        }
    }

    private void AIDrive()
    {
        Vector3 moveDir = (pathNodes[currentNode].position - transform.position).normalized;

        Vector3 desiredVelocity = moveDir * forwardAccel * accelMultiplier;
        ApplySteer(desiredVelocity);
        aiSpeed = carAIRb.velocity.magnitude;
    }

    private void ApplySteer(Vector3 desiredVelocity)
    {
        // Apply steering forces to the ai        
        Vector3 relativeVector = transform.InverseTransformPoint(pathNodes[currentNode].position);
        aiTurnInput = relativeVector.x / relativeVector.magnitude;

        // Car turns toward waypoint
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, aiTurnInput * turnAngle, 0.0f));

        // Apply wheel turns
        WheelTurns(aiTurnInput, turnAngle);

        // Calculate steering forces
        Vector3 steer = desiredVelocity - carAIRb.velocity;

        // Mass impacts steering
        steer = steer / carAIRb.mass;

        // Clamp steer to a max value
        float maxForce = forwardAccel * accelMultiplier;
        steer = Vector3.ClampMagnitude(steer, maxForce);

        // Add steering
        carAIRb.velocity = Vector3.ClampMagnitude(carAIRb.velocity + steer, maxForce);
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
        frontLeftWheelModel.Rotate(-Vector3.up * carAIRb.velocity.magnitude * turnMultiplier * Time.deltaTime);
        frontRightWheelModel.Rotate(Vector3.up * carAIRb.velocity.magnitude * turnMultiplier * Time.deltaTime);
        foreach (Transform t in backWheels)
        {
            t.Rotate(Vector3.right * carAIRb.velocity.magnitude * turnMultiplier * Time.deltaTime);
        }
    }

    public Rigidbody GetController()
    {
        return carAIRb;
    }

    public void PushBack(float backwardForce, float upwardForce, float forceMult)
    {
        // Push the car backwards
        carAIRb.AddForce(transform.up * (upwardForce * forceMult));
        carAIRb.AddForce(-transform.forward * (backwardForce * forceMult));
    }
}
