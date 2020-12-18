using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Death : MonoBehaviour {
    public PlayerHealth playerHealth;

	// Use this for initialization

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //Destroy(gameObject);
            //playerHealth.curHealth = playerHealth.curHealth - playerHealth.curHealth;
            playerHealth.TakeDamage(8);
            //GameController1.instance.lastCheckPointPos = transform.position;
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
        }
    }
}
