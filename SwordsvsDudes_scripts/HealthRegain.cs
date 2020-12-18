using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegain : MonoBehaviour {

    public int amount = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            PlayerHealth thehealth = other.gameObject.GetComponent<PlayerHealth>();
            thehealth.GainHealth(amount);
            Destroy(gameObject);
        }
    }
}
