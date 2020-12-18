using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy {
    [Header("Set in Inspector: Enemy_1")]

    // # of seconds for a full sin
    public float waveFreq = 2;
    //sin wave width in m
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0; // the inital x value of pos
    private float birthTime;


    //Start works well because its not used by the Enemy superClass


	// Use this for initialization
	void Start () {
        // set x0 to the initial x position of enemy_1
        x0 = pos.x;
        birthTime = Time.time;
	}

    // override the move function on enemy
    public override void Move()
    {
        // because pos is a property, you cant directly set pos.x
        Vector3 tempPos = pos;
        //theta adjusts based on time
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFreq;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // rotate a bit abut y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        // base.Move() still handles the movement down in y 
        base.Move();
        print(bndCheck.isOnScreen);
    }
}
