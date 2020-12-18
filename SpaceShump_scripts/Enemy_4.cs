using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part is another serializable data storage clss just like weapon definition
/// </summary>

[System.Serializable]
public class Part
{
    public string name;
    public float health; // the amount of health this part has
    public string[] protectedBy; // the other parts that protect this


    //these two fields are set automatically in start()
    // caching like this makes it faster and easier to find these later

    [HideInInspector]// makes field on next line not appear in inspector

    public GameObject go;


    [HideInInspector]
    public Material mat;
}
public class Enemy_4 : Enemy {
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;
    private Vector3 p0, p1; // the two points to interpolate
    private float timeStart; // birth time
    private float duration = 4;
    public GameObject freeze;


    // Use this for initialization
    void Start () {
        p0 = p1 = pos;
        InitMovement();

        // cahce gamewobject and material of each part in parts
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t!= null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
		
	}

    void InitMovement()
    {
        p0 = p1; // Set p0 to the old p1
        // assign a new on screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        // reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u>=1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2); // apply ease out easing to u
        pos = (1 - u) * p0 + u * p1; //simple linear interpolation
    }

    Part FindPart (string n)
    {
        foreach(Part prt in parts)
        {
            if (prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }
    Part FindPart(GameObject go)
    {
        foreach(Part prt in parts)
        {
            if (prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }

    bool Destroyed(GameObject go)
    {
        return (Destroyed(FindPart(go)));
    }
    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

    bool Destroyed(Part prt)
    {
        if (prt == null)
        {
            return (true);
        }
        return (prt.health <= 0);
    }

    //this changes the color of just one part to red instead of the whole ship/

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    //this will override the oncollisionenter that is part of the enemy.cs

    private void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                ProjectileHero p = other.GetComponent<ProjectileHero>();
                // if the enemy is off screen dont damage it.
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                //hurt this enemy
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                if (prtHit.protectedBy != null)
                {
                    foreach(string s in prtHit.protectedBy)
                    {
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }
                // its not protected so make it take damage
                // get the damage amount from the projectile type and main.w_defs
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                //show damage on part
                ShowLocalizedDamage(prtHit.mat);
                if(prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false);
                }

                bool allDestroyed = true;
                foreach(Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)
                {
                    Main.S.shipDestroyed(this);
                    //Destroy this enemy
                    Destroy(this.gameObject);
                    ScoreManager.SCORE += 500;
                    GameObject frozen = Instantiate<GameObject>(freeze);
                    frozen.transform.position = transform.position;
                    Object.Destroy(frozen, 4.0f);
                    if (ScoreManager.SCORE > HighScore.score)
                    {
                        HighScore.score = ScoreManager.SCORE;
                    }
                }
                Destroy(other);
                break;
        }
    }

}
