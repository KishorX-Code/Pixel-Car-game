using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCrash : MonoBehaviour
{
    public AudioClip explosionSound;
    public GameObject explosionPrefab;
    private bool crashed = false;
    public float minimumCrashedSpeed = 15f;
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
            if (explosionSound != null)
            {
                AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            }
        gameObject.SetActive(false);
    }
 }
