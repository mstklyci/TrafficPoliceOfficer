using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollOnOff : MonoBehaviour
{

    [SerializeField] BoxCollider mainCollider;
    [SerializeField] GameObject mainCharacterRig;
    [SerializeField] Animator mainCharacterAnimator;

    void Start()
    {
        GetRagdollBits();
        RagdollModeOff();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Car")
        {
            RagdollModeOn();
        }
    }

    Collider[] ragdollColliders;
    Rigidbody[] limbsRigidbodies;

    void GetRagdollBits()
    {
        ragdollColliders = mainCharacterRig.GetComponentsInChildren<Collider>();
        limbsRigidbodies = mainCharacterRig.GetComponentsInChildren<Rigidbody>();

    }

    void RagdollModeOn()
    {
        mainCharacterAnimator.enabled = false;

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rigid in limbsRigidbodies)
        {
            rigid.isKinematic = false;
        }

        mainCollider.enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void RagdollModeOff()
    {
       foreach(Collider col in ragdollColliders)
       {
            col.enabled = false;
       }
        
       foreach(Rigidbody rigid in limbsRigidbodies)
       {
            rigid.isKinematic = true;
       }

       mainCharacterAnimator.enabled = true;
       mainCollider.enabled = true;
       GetComponent<Rigidbody>().isKinematic = false;

    }
}