using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Carhandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform gamemodel;
    [SerializeField] AudioSource engineAudio;
    [SerializeField] float idlePitch = 1.0f;
    [SerializeField] float maxPitch = 2.2f;
    [SerializeField]
    AudioSource skidAudio;
    [SerializeField]
    AudioSource explosionAudio;

    float skidSpeed = 10f;
    float maxstreevelocity = 2;
    float maxForwardvelocity = 50;
    float accelerationMultipler = 6f;
    float breakMultiple = 15;
    float steeringMultipler = 5;
    float maxReverseVelocity = 10f;
    float currentAccleration = 0f;
    float reverseTimer = 0f;
    float reverseDelay = 2f;

    Vector2 input = Vector2.zero;
    float carStartPositionZ;
    float distanceTravelled = 0;
    public AudioClip explosionSound;
    public GameObject explosionPrefab;
    private bool crashed = false;
    public float minimumCrashedSpeed = 15f;
    public float DistanceTravelled => distanceTravelled;

    public event Action<Carhandler> OnPlayerCrashed;

    void Start()
    {
        if (engineAudio == null)
            engineAudio = GetComponent<AudioSource>();
        if (engineAudio != null)
        {
            engineAudio.loop = true;
            engineAudio.playOnAwake = false;
            carStartPositionZ = transform.position.z;
        }
    }
    void Update()
    {
        gamemodel.transform.rotation = Quaternion.Euler(0, rb.velocity.x * 5, 0);
        EngineSound();
        SkidSound();

        distanceTravelled = transform.position.z - carStartPositionZ;
    }
    private void FixedUpdate()
    {
        if (input.y > 0)
            Accelerate();
        else
            rb.drag = 0.2f;
        if (input.y < 0)
            Brake();
        Steer();

        if (Mathf.Abs(rb.velocity.z) < 0.1f)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
        }

    }
    void Accelerate()

    {
        rb.drag = 0;
        currentAccleration = Mathf.MoveTowards(
            currentAccleration, accelerationMultipler, Time.fixedDeltaTime * 3f);
        if (rb.velocity.z >= maxForwardvelocity)
            return;
        rb.AddForce(rb.transform.forward * accelerationMultipler * input.y);
    }
    void Brake()
    {
        rb.drag = 0;
        if (rb.velocity.z > 0.2f)
        {
            reverseTimer = 0f;
            rb.AddForce(rb.transform.forward * breakMultiple * input.y);
            return;
        }
        reverseTimer += Time.fixedDeltaTime;
        if (reverseTimer >= reverseDelay)
        {
            rb.AddForce(rb.transform.forward * accelerationMultipler * input.y);
            if (rb.velocity.z < -maxReverseVelocity)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -maxReverseVelocity);
            }
        }





    }


    void Steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            float speedBaseSteerLimit = rb.velocity.z / 5.0f;
            speedBaseSteerLimit = Mathf.Clamp01(speedBaseSteerLimit);

            rb.AddForce(rb.transform.right * steeringMultipler * input.x * speedBaseSteerLimit);

            float normalizedx = rb.velocity.x / maxstreevelocity;

            normalizedx = Mathf.Clamp(normalizedx, -1.0f, 1.0f);
            rb.velocity = new Vector3(normalizedx * maxstreevelocity, 0, rb.velocity.z);
        }
        else
        {
            rb.velocity = Vector3.Lerp(
    rb.velocity,
    new Vector3(0, rb.velocity.y, rb.velocity.z),
    Time.fixedDeltaTime * 3
);
        }
    }
    public void SetInput(Vector2 inputvector)
    {
        inputvector.Normalize();
        input = inputvector;
    }
    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxForwardvelocity = newMaxSpeed;
    }
    void EngineSound()
    {
        if (engineAudio == null)
            return;

        if (Mathf.Abs(rb.velocity.z) > 0.5f)
        {
            if (!engineAudio.isPlaying)
                engineAudio.Play();
        }
        else
        {
            if (engineAudio.isPlaying)
                engineAudio.Pause();
        }
        float speedPercent = Mathf.Clamp01(rb.velocity.magnitude / maxForwardvelocity);
        engineAudio.pitch = Mathf.Lerp(idlePitch, maxPitch, speedPercent);
        engineAudio.volume = Mathf.Lerp(0.7f, 1.0f, speedPercent);
    }
    void SkidSound()
    {
        if (skidAudio == null)
        {
            return;
        }
        bool braking = input.y < 0;
        bool turning = Mathf.Abs(input.x) > 0.3f;
        bool movingFast = rb.velocity.magnitude > skidSpeed;
        if (braking && turning && movingFast)
        {
            if (!skidAudio.isPlaying)
            {
                skidAudio.Play();
            }
        }
        else
        {
            if (skidAudio.isPlaying)
            {
                skidAudio.Stop();
            }
        }
    }
    IEnumerator SlowDownTimeCo()
    {
        while (Time.timeScale > 0.2f)
        {
            Time.timeScale -= Time.deltaTime * 2f;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0.2f, 1f);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(0.5f);
        while (Time.timeScale < 1.0f)
        {
            Time.timeScale += Time.unscaledDeltaTime * 2f;
            Time.timeScale = Mathf.Clamp(Time.timeScale, 0.2f, 1f);
            yield return null;
        }
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (crashed)
            return;
        if (collision.relativeVelocity.magnitude < minimumCrashedSpeed)
            return;
        crashed = true;
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        if (explosionSound != null && explosionSound != null)
        {
            explosionAudio.clip = explosionSound;
            explosionAudio.loop = true;
            explosionAudio.Play();
        }
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        OnPlayerCrashed?.Invoke(this);
        StartCoroutine(SlowDownTimeCo());
        enabled = false;
    }
    public void StopExplosionSound()
    {
        if(explosionAudio != null)
        {
            explosionAudio.Stop();
        }
    }
}




