using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy {

    [Header("Set in Inspector: Enemy_2")]
    //determines how much the sin wave will affect movement
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;

    [Header("Set Dynamically: Enemy_2")]
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

	// Use this for initialization
	void Start () {
        // pick any point on the left sde of screen
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //pick any point on the right side

        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        //possibly swap sides
        if(Random.value > 0.5f)
        {
            // setting the .x of each point to its negative
            p0.x *= -1;
            p1.x *= -1;

        }

        // set the birth time to the current time
        birthTime = Time.time;
		
	}

    public override void Move()
    {
        float u = (Time.time - birthTime) / lifeTime;
        // if u>1 then it has been longer than lifetime since birth

        if (u >1)
        {
            Destroy(this.gameObject);
            return;
        }

        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));
        // interpolate the two linear points

        pos = (1 - u) * p0 + u * p1;
       
    }
}
