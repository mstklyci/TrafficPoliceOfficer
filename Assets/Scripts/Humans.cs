using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class Humans : MonoBehaviour
{

    // HumanWalk
    [SerializeField] private float walkSpeed;
    [SerializeField] private float newWalkSpeed;
    private bool humanWait;

    // Police
    private CharacterController controller;
    private LightsManager lightsManager;
    private bool humanStop;
    private bool lightWait;
    private GameObject BarrierArea;

    private Animator humanAnim;
    [SerializeField] private Transform policeTarget;
    [SerializeField] private Transform humanTarget;

    //Audio
    private AudioSource soundSource;
    [SerializeField] private AudioClip deadsoundClip;

    void Start()
    {
        humanAnim = GetComponent<Animator>();
        controller = GameObject.FindWithTag("Police").GetComponent<CharacterController>();
        lightsManager = GameObject.FindWithTag("TrafficLight").GetComponent<LightsManager>();
        BarrierArea = GameObject.FindWithTag("Barrier").GetComponent<GameObject>();
        policeTarget = GameObject.FindWithTag("Police").GetComponent<Transform>();
        soundSource = GameObject.FindWithTag("Audio").GetComponent<AudioSource>();
        humanStop = false;
        humanWait = false;
        lightWait = false;

        UpdateHumanTarget();
        StartCoroutine(WaitTime());
    }

    void Update()
    {
        UpdateHumanTarget();

        float distance = Mathf.Abs(gameObject.transform.position.x - policeTarget.position.x);
        float humanDistance = humanTarget != null ? Mathf.Abs(gameObject.transform.position.x - humanTarget.position.x) : Mathf.Infinity;

        if (controller != null && controller.humanStop == true && distance <= 1.0f)
        {
            humanStop = true;
        }
        else if (controller != null && controller.humanStop == false || distance >= 1.0f)
        {
            humanStop = false;
        }

        if (humanStop == false && humanWait == false && lightWait == false && gameObject.tag != "Dead")
        {
            transform.Translate(0, 0, walkSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, -90, 0);
            humanAnim.SetBool("IsWalking", true);
        }
        else if (humanStop == true || humanDistance <= 1.0f || lightWait == true)
        {
            humanAnim.SetBool("IsWalking", false);
            humanWait = true;
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        if (humanDistance <= 1.0f)
        {
            humanAnim.SetBool("IsWalking", false);
            humanWait = true;
        }
        else if (humanDistance > 1.2f)
        {
            humanWait = false;           
        }

        //
        if (humanTarget != null && humanDistance <= 1.4f)
        {
            Humans targetHuman = humanTarget.GetComponent<Humans>();
            if (targetHuman != null && targetHuman.walkSpeed < walkSpeed)
            {
                walkSpeed = targetHuman.walkSpeed;
            }
        }
        else if (humanTarget != null && humanDistance > 1.4f || humanTarget == null)
        {
            walkSpeed = newWalkSpeed;
        }
    }

    private void UpdateHumanTarget()
    {
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");

        if (humans.Length > 0)
        {
            humanTarget = humans
                .Where(human => human != gameObject && human.transform.position.x < gameObject.transform.position.x)
                .Select(human => human.transform)
                .OrderBy(humanTransform => Vector3.Distance(gameObject.transform.position, humanTransform.position))
                .FirstOrDefault();
        }
    }

    IEnumerator WaitTime()
    {
        while (true)
        {
            if (walkSpeed == 0)
            {
                yield return new WaitForSeconds(0.1f);

                if (walkSpeed == 0 && gameObject.tag != "Dead")
                {
                    walkSpeed = newWalkSpeed;
                }
            }
            yield return null;
        }
    }

    IEnumerator BarrierWaitTime()
    {
        while (true)
        {
            if (walkSpeed == 0)
            {
                yield return new WaitForSeconds(15f);

                if (walkSpeed == 0 && gameObject.tag != "Dead")
                {
                    walkSpeed = newWalkSpeed;
                }
            }
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Score" && DeadCounter.deadCounter > 0 && controller.policeDead == false)
        {
            DeadCounter.scoreCounter++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Barrier")
        {
            if (lightsManager.transition == true)
            {
                lightWait = false;
                StartCoroutine(BarrierWaitTime());
            }
            else if (lightsManager.transition == false)
            {
                lightWait = true;
                humanAnim.SetBool("IsWalking", false);
            }
            else
            {
                transform.Translate(0, 0, walkSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, -90, 0);
                humanAnim.SetBool("IsWalking", true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Barrier")
        {
            transform.Translate(0, 0, walkSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, -90, 0);
            humanAnim.SetBool("IsWalking", true);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Car")
        {
            walkSpeed = 0;            
            gameObject.tag = "Dead";
            StartCoroutine(DestroyBody());

            if(controller.policeDead == false && DeadCounter.deadCounter >= 0)
            {
                DeadCounter.deadCounter--;
                soundSource.PlayOneShot(deadsoundClip);
            }
        }      
    }

    private IEnumerator DestroyBody()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}   