using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterController : MonoBehaviour
{

    [SerializeField] private float speed;
    public bool policeDead;
    Animator policeAnimator;

    //HumanStopController
    public bool humanStop;
    public bool carStop;  

    void Start()
    {
        policeAnimator = GetComponent<Animator>();

        policeDead = false;
        carStop = false;
    }

    void Update()
    {    
        //Lock pos
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -9, 9);
        pos.z = Mathf.Clamp(pos.z, 1.1f, 1.15f);
        transform.position = pos;

        if (policeDead == false && Time.timeScale != 0)
        {
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                transform.Translate(0, 0, +speed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, -90, 0);
                policeAnimator.SetBool("IsRunning", true);
                policeAnimator.SetBool("stopMove", false);
            }
            else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                transform.Translate(0, 0, +speed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0, +90, 0);
                policeAnimator.SetBool("IsRunning", true);
                policeAnimator.SetBool("stopMove", false);
            }
            else
            {
                transform.Translate(0, 0, 0);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                policeAnimator.SetBool("IsRunning", false);
            }

            //StopSign
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W))
            {
                humanStop = true;

                policeAnimator.SetBool("stopMove", true);
                transform.rotation = Quaternion.Euler(0, +90, 0);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                humanStop = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                policeAnimator.SetBool("stopMove", false);
                policeAnimator.SetBool("IsRunning", false);
            }

            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S))
            {
                carStop = true;

                policeAnimator.SetBool("stopMove", true);
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                carStop = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                policeAnimator.SetBool("stopMove", false);
                policeAnimator.SetBool("IsRunning", false);
            }

            //Obstacles
            if (Input.GetKey(KeyCode.C) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
            {
                GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

                foreach (GameObject obstacle in obstacles)
                {
                    if (Mathf.Abs(obstacle.transform.position.x - gameObject.transform.position.x) <= 0.5f)
                    {
                        Vector3 newPosition = obstacle.transform.position;
                        newPosition.x = gameObject.transform.position.x;
                        obstacle.transform.position = newPosition;
                    }
                }
            }
        }

        if (Mathf.Abs(pos.x) == 9 || Mathf.Abs(pos.x) == -9)
        {
             policeAnimator.SetBool("IsRunning", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Car")
        {
            policeDead = true;
        }
    }
}