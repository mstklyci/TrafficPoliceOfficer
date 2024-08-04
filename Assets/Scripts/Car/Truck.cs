using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{

    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider middleRight;
    [SerializeField] WheelCollider middleLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
    [SerializeField] Transform middleRightTransform;
    [SerializeField] Transform middleftTransform;
    [SerializeField] Transform backRightTransform;
    [SerializeField] Transform backLeftTransform;

    [SerializeField] float acceleration;
    [SerializeField] float breakingForce;
    [SerializeField] float maxSteerAngle;
    [SerializeField] float turnSpeed;
    private Rigidbody rb;

    [SerializeField] float maxSpeed;
    [SerializeField] float currentAcceleration = 0f;
    [SerializeField] float currentBreakingForce = 0f;
    [SerializeField] float currentTurnAngle = 0f;

    //Stop
    private bool carsStop;
    [SerializeField] private GameObject carLight;

    //
    [SerializeField] List<Transform> paths;
    private Vector3 centerOfMass;

    [SerializeField] List<List<Transform>> nodes;
    [SerializeField] int currentPathIndex = 0;
    [SerializeField] int currentNode = 0;
    private bool avoiding = false;
    private float targetSteerAngle = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        foreach (GameObject pathObj in GameObject.FindGameObjectsWithTag("Path"))
        {
            paths.Add(pathObj.transform);
        }

        paths.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
        SelectRoad();

        nodes = new List<List<Transform>>();

        foreach (Transform path in paths)
        {
            Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
            List<Transform> pathNodes = new List<Transform>();

            for (int i = 0; i < pathTransforms.Length; i++)
            {
                if (pathTransforms[i] != path.transform)
                {
                    pathNodes.Add(pathTransforms[i]);
                }
            }

            nodes.Add(pathNodes);
        }
    }

    private void FixedUpdate()
    {
        if (carsStop == true)
        {
            currentAcceleration = 0f;
            currentBreakingForce = breakingForce;
        }
        if (carsStop == false)
        {
            currentAcceleration = acceleration;
            currentBreakingForce = 0f;
        }

        float speed = rb.velocity.magnitude * 3.6f;

        if (speed < maxSpeed)
        {
            frontLeft.motorTorque = currentAcceleration;
            frontRight.motorTorque = currentAcceleration;
        }
        else
        {
            frontLeft.motorTorque = 0;
            frontRight.motorTorque = 0;
            currentBreakingForce = breakingForce;
        }

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Car");

        foreach (GameObject obj in taggedObjects)
        {
            if (obj == gameObject)
            {
                continue;
            }

            float gmZ = transform.position.z;
            float objZ = obj.transform.position.z;
            float objX = obj.transform.position.x;

            if (gmZ > objZ)
            {
                if (Mathf.Abs(gmZ - objZ) <= 40 && Mathf.Abs(transform.position.x - objX) <= 3)
                {
                    currentAcceleration = 0f;
                    currentBreakingForce = breakingForce;
                }
            }
        }

        frontRight.brakeTorque = currentBreakingForce;
        frontLeft.brakeTorque = currentBreakingForce;
        middleLeft.brakeTorque = currentBreakingForce;
        middleRight.brakeTorque = currentBreakingForce;
        backRight.brakeTorque = currentBreakingForce;
        backLeft.brakeTorque = currentBreakingForce;

        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(middleLeft, middleftTransform);
        UpdateWheel(middleRight, middleRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);

        ApplySteer();
        CheckWaypointDistnace();
        LerpToSteerAngle();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Destroy(collision.gameObject);
        }
    }

    private void UpdateWheel(WheelCollider col, Transform trans)
    {
        Vector3 position;
        Quaternion rotation;
        col.GetWorldPose(out position, out rotation);

        trans.position = position;
        trans.rotation = rotation;
    }

    private void CheckWaypointDistnace()
    {
        if (Vector3.Distance(transform.position, nodes[currentPathIndex][currentNode].position) < 0.5f)
        {
            if (currentNode == nodes[currentPathIndex].Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }

    private void ApplySteer()
    {
        if (avoiding) return;
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentPathIndex][currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        targetSteerAngle = newSteer;
    }

    private void LerpToSteerAngle()
    {
        frontLeft.steerAngle = Mathf.Lerp(frontLeft.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        frontRight.steerAngle = Mathf.Lerp(frontLeft.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    void SelectRoad()
    {
        float xPosition = gameObject.transform.position.x;

        if (xPosition >= -10 && xPosition < -6)
        {
            currentPathIndex = 0;
        }
        else if (xPosition >= -6 && xPosition < -2)
        {
            currentPathIndex = 3;
        }
        else if (xPosition >= 2 && xPosition < 6)
        {
            currentPathIndex = 4;
        }
        else if (xPosition >= 6 && xPosition <= 10)
        {
            currentPathIndex = 5;
        }
    }
}