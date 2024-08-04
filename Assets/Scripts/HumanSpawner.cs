using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{

    [SerializeField] GameObject[] humans;

    private float timer = 0.0f;
    [SerializeField] float spawnTime = 5.0f;
    private int lastHumanIndex = -1;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnTime)
        {
            int randomHuman = RandomHuman();

            Instantiate(humans[randomHuman], gameObject.transform.position, gameObject.transform.rotation);

            lastHumanIndex = randomHuman;
            timer = 0.0f;
        }
    }
    int RandomHuman()
    {
        if (humans.Length == 1)
        {
            return 0;
        }

        int newHuman;
        do
        {
            newHuman = Random.Range(0, humans.Length);
        } while (newHuman == lastHumanIndex);

        return newHuman;
    }
}