using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SC_Car : MonoBehaviour
{

    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] Transform frontRightTransform;
    [SerializeField] Transform frontLeftTransform;
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
    private float currentTurnAngle = 0f;

    //Stop
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject trafficPolice;
    private bool carsStop;
    private bool carWaitOver = false;
    [SerializeField] private GameObject carLight;

    //
    [SerializeField] List<Transform> paths;
    private Vector3 centerOfMass;

    [SerializeField] List<List<Transform>> nodes;
    [SerializeField] int currentPathIndex = 0;
    private static bool Park2Full = false;
    private static bool Park3Full = false;
    private static bool Park4Full = false;
    [SerializeField] int currentNode = 0;
    private bool avoiding = false;
    private float targetSteerAngle = 0f;

    private void Start()
    {
        controller = GameObject.FindWithTag("Police").GetComponent<CharacterController>();
        trafficPolice = GameObject.FindWithTag("Police");

        rb = GetComponent<Rigidbody>();
        //
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        foreach (GameObject pathObj in GameObject.FindGameObjectsWithTag("Path"))
        {
            paths.Add(pathObj.transform);
        }

        paths.Sort((a, b) => a.GetSiblingIndex().CompareTo(b.GetSiblingIndex()));
        SelectRoad();
        StartCoroutine(WaitTime());

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
        if (controller.carStop == false)
        {
            carsStop = false;
        }

        float gmS = transform.position.z;
        float trafficZ = trafficPolice.transform.position.z;
        float trafficX = trafficPolice.transform.position.x;

        if (gmS > trafficZ)
        {
            if (Mathf.Abs(gmS - trafficZ) <= 40 && Mathf.Abs(transform.position.x - trafficX) <= 1.5 && maxSpeed >= 70 ||
                Mathf.Abs(gmS - trafficZ) <= 20 && Mathf.Abs(transform.position.x - trafficX) <= 1.5 && maxSpeed == 50 ||
                Mathf.Abs(gmS - trafficZ) <= 10 && Mathf.Abs(transform.position.x - trafficX) <= 1.5 && maxSpeed <= 20)
            {
                if(controller.carStop == true)
                {
                    carsStop = true;
                }               
            }
        }
        
        if(carsStop == true)
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
                if (Mathf.Abs(gmZ - objZ) <= 40 && Mathf.Abs(transform.position.x - objX) <= 3 && maxSpeed >= 70||
                    Mathf.Abs(gmZ - objZ) <= 30 && Mathf.Abs(transform.position.x - objX) <= 3 && maxSpeed == 50||
                    Mathf.Abs(gmZ - objZ) <= 10 && Mathf.Abs(transform.position.x - objX) <= 3 && maxSpeed <= 20)
                {
                    currentAcceleration = 0f;
                    currentBreakingForce = breakingForce;
                    carsStop = true;
                }
            }
        }

        GameObject[] taggedStop = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject stopObj in taggedStop)
        {
            float gmZ = transform.position.z;
            float objZ = stopObj.transform.position.z;
            float objX = stopObj.transform.position.x;

            if (gmZ > objZ && carWaitOver == false)
            {
                if (Mathf.Abs(gmZ - objZ) <= 40 && Mathf.Abs(transform.position.x - objX) <= 1.5 && maxSpeed >= 70 ||
                    Mathf.Abs(gmZ - objZ) <= 20 && Mathf.Abs(transform.position.x - objX) <= 1.5 && maxSpeed == 50 ||
                    Mathf.Abs(gmZ - objZ) <= 10 && Mathf.Abs(transform.position.x - objX) <= 1.5 && maxSpeed <= 20)
                {
                    currentAcceleration = 0f;
                    currentBreakingForce = breakingForce;
                    carsStop = true;
                }
            }
        }

        frontRight.brakeTorque = currentBreakingForce;
        frontLeft.brakeTorque = currentBreakingForce;
        backRight.brakeTorque = currentBreakingForce;
        backLeft.brakeTorque = currentBreakingForce;

        frontLeft.steerAngle = currentTurnAngle;
        frontRight.steerAngle = currentTurnAngle;

        UpdateWheel(frontLeft, frontLeftTransform);
        UpdateWheel(frontRight, frontRightTransform);
        UpdateWheel(backLeft, backLeftTransform);
        UpdateWheel(backRight, backRightTransform);

        ApplySteer();
        CheckWaypointDistnace();
        LerpToSteerAngle();
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

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LowSpeed" && (currentPathIndex == 1 || currentPathIndex == 2 || currentPathIndex == 6 || currentPathIndex == 7 || currentPathIndex == 8 || currentPathIndex == 9))
        {
            maxSpeed = 15;
        }
        if (other.gameObject.tag == "Break" && (currentPathIndex == 1 || currentPathIndex == 2 || currentPathIndex == 6 || currentPathIndex == 8 || currentPathIndex == 9))
        {
            maxSpeed = 0;
        }
        if (other.gameObject.tag == "HighSpeed")
        {
            maxSpeed = 50;
        }      
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ParkArea")
        {
            carLight.SetActive(false);
            gameObject.tag = "Empty";
        }
        if (other.gameObject.tag == "BusStop" && (currentPathIndex == 7))
        {
            maxSpeed = 0;
            StartCoroutine(BusStop());
        }

        if(other.gameObject.tag == "Destroy")
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Obstacle")
        {
            Destroy(collision.gameObject);
        }
    }
    IEnumerator BusStop()
    {
        yield return new WaitForSeconds(3);

        maxSpeed = 50;
    }

    IEnumerator WaitTime()
    {   
        while (true)
        {
            if (carsStop == true)
            {
                yield return new WaitForSeconds(20);

                if (carsStop == true)
                {
                    maxSpeed = 70;
                    carWaitOver = true;
                }
            }
            yield return null;
        }
    }

    void SelectRoad()
    {
        float xPosition = gameObject.transform.position.x;

        if (xPosition >= -10 && xPosition < -6)
        {
            float randomValue = Random.Range(0f, 1f);

            if (randomValue < 0.60f)
            {
                currentPathIndex = 0;
            }
            else if (randomValue < 0.80f && gameObject.name != "Car_Bus(Clone)")
            {
                currentPathIndex = 1;
            }
            else if (randomValue >= 0.80f && gameObject.name != "Car_Bus(Clone)")
            {
                if (!Park2Full)
                {
                    currentPathIndex = 2;
                    Park2Full = true;
                }
                else if (!Park3Full)
                {
                    currentPathIndex = 8;
                    Park3Full = true;
                }
                else if (!Park4Full)
                {
                    currentPathIndex = 9;
                    Park4Full = true;
                }
                else
                {
                    currentPathIndex = 0;
                }
            }
            else
            {
                currentPathIndex = 0;
            }
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
            float randomValue = Random.Range(0f, 1f);

            if (randomValue < 0.7f)
            {
                currentPathIndex = 5;
            }
            else if (randomValue >= 0.7f && gameObject.name != "Car_Bus(Clone)")
            {
                currentPathIndex = 6;
            }
            else if (randomValue >= 0.7f && gameObject.name == "Car_Bus(Clone)")
            {
                currentPathIndex = 7;
            }
        }
    }
}