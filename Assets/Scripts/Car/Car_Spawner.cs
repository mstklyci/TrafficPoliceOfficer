using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_Spawner : MonoBehaviour
{

    [SerializeField] GameObject[] cars;
    [SerializeField] Transform[] spawners;

    //
    private float timer = 0.0f;
    public float spawnTime = 5.0f;
    private int lastCarIndex = -1;
    private int lastSpawnerIndex = -1;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnTime)
        {
            int randomCar = RandomCar();
            int randomSpawner = RandomSpawnPosition();

            Instantiate(cars[randomCar], spawners[randomSpawner].position, spawners[randomSpawner].rotation);

            lastCarIndex = randomCar;
            lastSpawnerIndex = randomSpawner;
            timer = 0.0f;
        }
    }

    int RandomCar()
    {
        int newCar;
        do
        {
            newCar = Random.Range(0, cars.Length);
        } while (newCar == lastCarIndex);

        return newCar;
    }

    int RandomSpawnPosition()
    {
        int newSpawner;
        do
        {
            newSpawner = Random.Range(0, spawners.Length);
        } while (newSpawner == lastSpawnerIndex);

        return newSpawner;
    }
}