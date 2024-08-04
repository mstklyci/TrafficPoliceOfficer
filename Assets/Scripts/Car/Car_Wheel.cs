using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Wheel : MonoBehaviour
{
    [SerializeField] WheelCollider targetWheel;
    private Vector3 wheelPosition = new Vector3();
    private Quaternion wheelRotation = new Quaternion();

    private void Update()
    {
        targetWheel.GetWorldPose(out wheelPosition, out wheelRotation);
        transform.position = wheelPosition;
        transform.rotation = wheelRotation;
    }
}