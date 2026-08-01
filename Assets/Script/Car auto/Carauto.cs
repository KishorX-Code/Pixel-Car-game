using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carauto : MonoBehaviour
{
    [SerializeField]
    GameObject[] carAIprefabs;

    GameObject[] carAIPool = new GameObject[20];

    WaitForSeconds wait = new WaitForSeconds(2f);

    float timeLastCarSpawnes = 0;

    Transform playerCarTransform;

    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int prefabIndex = 0;

        for (int i = 0; i < carAIPool.Length; i++)
        {
            carAIPool[i] = Instantiate(carAIprefabs[prefabIndex]);
            carAIPool[i].SetActive(false);

            prefabIndex++;

            if (prefabIndex > carAIprefabs.Length - 1)
                prefabIndex = 0;
        }

        StartCoroutine(Updatelessoften());
    }

    IEnumerator Updatelessoften()
    {
        while (true)
        {
            CleanUpCarsBeyondView();
            SpawnNewCars();

            yield return wait;
        }
    }

    void SpawnNewCars()
    {
        if (Time.time - timeLastCarSpawnes < 2)
            return;

        GameObject carTospawn = null;

        foreach (GameObject aiCar in carAIPool)
        {
            if (aiCar.activeInHierarchy)
                continue;

            carTospawn = aiCar;
            break;
        }

        if (carTospawn == null)
            return;

        Vector3 spwanPosition = new Vector3(
            0,
            0,
            playerCarTransform.position.z + 60
        );

        carTospawn.transform.position = spwanPosition;
        carTospawn.SetActive(true);

        timeLastCarSpawnes = Time.time;
    }

    void CleanUpCarsBeyondView()
    {
        foreach (GameObject aiCar in carAIPool)
        {
            if (!aiCar.activeInHierarchy)
                continue;

            if (aiCar.transform.position.z - playerCarTransform.position.z > 200)
                aiCar.SetActive(false);

            if (aiCar.transform.position.z - playerCarTransform.position.z < -50)
                aiCar.SetActive(false);
        }
    }
}