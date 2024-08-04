using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsManager : MonoBehaviour
{

    public bool transition;

    [SerializeField] GameObject greenLight;
    [SerializeField] GameObject redLight;
    [SerializeField] GameObject barrier;

    public Animator barrierAnim;

    void Start()
    {
        StartCoroutine(LightControl());
        transition = false;
        barrierAnim.SetBool("BarrierOpening", false);
    }

    private void Update()
    {
        if (transition == true)
        {
            greenLight.SetActive(true);
            redLight.SetActive(false);
            barrier.transform.rotation = Quaternion.Euler(-135, 0, 0);           
        }
        else if (transition == false)
        {
            greenLight.SetActive(false);
            redLight.SetActive(true);
            barrier.transform.rotation = Quaternion.Euler(-90, 0, 0);          
        }
    }

    IEnumerator LightControl()
    {
        while (true)
        {
            transition = true;
            barrierAnim.SetBool("BarrierOpening", true);
            yield return new WaitForSeconds(10);

            transition = false;
            barrierAnim.SetBool("BarrierOpening", false);
            yield return new WaitForSeconds(10);
        }
    }
}