using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f; // in m/s
    public float fireRate = 0.3f; // seconds/shot
    public float health = 10;
    public int score = 100; // points earned for destroyinf this
    public float showDamageDuration = 0.1f; // # of seconds to show damage
    public float powerUpDropChance = 0.1f; // chance to drop a powerup


    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials; // all the materials of this and its children
    public bool showingDamage = false;
    public float damageDoneTime; // time to stop showing damage
    public bool notifiedOfDestruction = false;

    protected BoundsCheck bndCheck;



    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();

        // get the materials and colors for this gameobject
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    public Vector3 pos{
        get {
            return (this.transform.position);
        }
        set 
        {
            this.transform.position = value;
        }
    }

	
	// Update is called once per frame
	void Update () {
        Move();

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.offDown)
        {
            // check to make sure its gone off the bottom of the screen
            Destroy(gameObject);
        }
		
	}


    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision coll)
    {
        {
            GameObject otherGO = coll.gameObject;
            switch (otherGO.tag)
            {
                case "ProjectileHero":
                    ShowDamage();
                    ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
                    // if this enemy is off screen dont damage it

                    if (!bndCheck.isOnScreen)
                    {
                        Destroy(otherGO);
                        break;
                    }

                    health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                    if (health <= 0)
                    {
                        if (!notifiedOfDestruction)
                        {
                            Main.S.shipDestroyed(this);
                            ScoreManager.SCORE += 100;
                            if (ScoreManager.SCORE > HighScore.score)
                            {
                                HighScore.score = ScoreManager.SCORE;
                            }
                        }
                        notifiedOfDestruction = true;
                        Destroy(this.gameObject);


                    }



                    Destroy(otherGO);
                    break;

                   


                default:
                    print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                    break;
            }
            //if (otherGO.tag == "ProjectileHero")
            //{
            //    Destroy(otherGO);
            //    Destroy(gameObject);
            //}

            //else{
            //    print("Enemy hit by non-projectile" + otherGO.name);
            //}
        }
    }

        void ShowDamage()
        {
            foreach (Material m in materials)
            {
                m.color = Color.red;
            }
            showingDamage = true;
            damageDoneTime = Time.time + showDamageDuration;
        }

        public void UnShowDamage()
        {
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].color = originalColors[i];
            }
            showingDamage = false;
        }
    }

