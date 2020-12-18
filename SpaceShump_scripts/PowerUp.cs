using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {
    [Header("Set in Inspector")]
    //this is an unusual but handy use of vector2s. x holds a min value an
    // and y a max value for a random range that will be called later

    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f; // seconds the powerup exists
    public float fadeTime = 4f;

    [Header("Set Dynamically")]
    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

	// Use this for initialization
	void Awake () {
        cube = transform.Find("Cube").gameObject;

        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = GetComponent<Renderer>();

        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;
        //set the rotation of this gameobject to r:[0, 0, 0]
        transform.rotation = Quaternion.identity;
        // quaternion.identity is equal to no rotation.

        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));
        birthTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        // fade put the powerup over time
        //Given the default values, a powerup will exist for 10 seconds
        // then fade out over 4 seconds
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        //for lifetime seconds, u will be <=0. Then it will transistion to
        // 1 over the course destroy this powerup

        if (u>=1)
        {
            Destroy(this.gameObject);
            return;
        }

        // use u to determine the alpha value of the cube
        if (u>0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;

            c = letter.color;
            c.a = 1f - (u * .5f);
            letter.color = c;
        }

        if (!bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
		
	}

    public void SetType(WeaponType wt)
    {
        // grab the weapon definition from main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        cubeRend.material.color = def.color;
        letter.text = def.letter; // set the letter that is shown
        type = wt; // finally actually set the type
    }

    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }
}
