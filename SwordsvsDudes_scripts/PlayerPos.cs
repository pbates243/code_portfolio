using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPos : MonoBehaviour {
    private GameController1 gm;
    public PlayerHealth playerHealth;


	// Use this for initialization
	void Start () {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameController1>();
        transform.position = gm.lastCheckPointPos;
		
	}

	
	// Update is called once per frame
	void Update () {
        if (playerHealth.dead == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
		
	}
}
