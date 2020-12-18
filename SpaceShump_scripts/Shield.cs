using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    [Header("Set in Inspector")]
    public float rotationsPerSecond = .1f;
    [Header("Set Dynamically")]
    public int levelShown = 0;

    Material mat; // this nonpublic variable will not appear in the Inspector



	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material; 
		
	}
	
	// Update is called once per frame
	void Update () {
        // read the current sheild level from the hero singleton
        int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
        // if this is different from level Shown
        if (levelShown != currLevel)
        {
            levelShown = currLevel;
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }

        float rZ = -(rotationsPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rZ);
		
	}
}
