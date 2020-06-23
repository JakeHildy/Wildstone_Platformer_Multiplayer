using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CoinPickup : MonoBehaviour
{

    [SerializeField] AudioClip coinPickUpSFX;
    [SerializeField] int value = 1;


    private void OnTriggerEnter2D(Collider2D other) 
    {        
        if (!(other is CapsuleCollider2D)) return; // Check if other is Capsule collider so that the box collider doesn't double trigger coin pickup.
        AudioSource.PlayClipAtPoint(coinPickUpSFX, Camera.main.transform.position);
        FindObjectOfType<GameSession>().AddToScore(value);
        Destroy(gameObject);           
    }


}

