using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Material skyboxMaterial;

    void Start()
    {
        skyboxMaterial = RenderSettings.skybox;
    }

    void Update()
    {
        float currentRotation = skyboxMaterial.GetFloat("_Rotation");
        currentRotation += rotationSpeed * Time.deltaTime;
        skyboxMaterial.SetFloat("_Rotation", currentRotation);
    }
}