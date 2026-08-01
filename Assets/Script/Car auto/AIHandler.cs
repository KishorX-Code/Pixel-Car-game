using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHandler : MonoBehaviour
{
    [SerializeField]
    Carhandler carhandler;
    [SerializeField]
    LayerMask otherCarsLayerMask;
    [SerializeField]
    MeshCollider meshCollider;

    RaycastHit[] raycastHits = new RaycastHit[5];
    bool isCarAhead = false;
    int drivingInLane = 0;
    WaitForSeconds wait = new WaitForSeconds(0.2f);
    private void Awake()
    {
        if (CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }
    void Start()
    {
        StartCoroutine(UpdateLessOftenCO());
    }
    void Update()
    {
        float acclerationInput = 1.0f;
        float steerInput = 0.0f;
        if (isCarAhead)
        
            acclerationInput = -1.0f;
            float desiredPositionX = carlanefixed.Carlanes[drivingInLane];
            float difference = desiredPositionX - transform.position.x;

        if (Mathf.Abs(difference) > 0.05f)
            steerInput = 1.0f * difference;
        steerInput = Mathf.Clamp(steerInput, -1.0f, 1.0f);
        carhandler.SetInput(new Vector2(steerInput, acclerationInput));

    }
    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
          isCarAhead = CheckIfOtherCarsIsAhead();
            yield return wait;
        }
    }
    bool CheckIfOtherCarsIsAhead()
    {
        meshCollider.enabled = false;
        int numberOfHits = Physics.BoxCastNonAlloc(transform.position, Vector3.one * 0.25f, transform.forward, raycastHits, Quaternion.identity, 2, otherCarsLayerMask);
        meshCollider.enabled = true;
        if (numberOfHits > 0)
            return true;
        return false;
    }
    private void OnEnable()
    {
        carhandler.SetMaxSpeed(Random.Range(2f, 4f));
        drivingInLane = Random.Range(0, carlanefixed.Carlanes.Length);
    }
}
