using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an enum of the various possible weapon types.
/// It also includes a shield type to allow a shield power-up
/// Items marked[NI] below are not implemented in the IGDPD book.
/// </summary>

public enum WeaponType
{
    none, // the defualt / no weapon
    blaster, // the simple blaster
    spread, // two shots
    phaser, // [NI] shots that move in waves
    misslie, // [NI] homing missiles
    laser, //[NI] Damage over time
    shield, // raise shield level
    freeze 
}

/// <summary>
/// The WeapondDefinition class allows you to set the properties 
/// of a specific weapon in the Inspector. The Main class has
/// an Array of WeaponDefinitions that makes this possible.
/// </summary>

[System.Serializable]

public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter; // to show the powerup
    public Color color = Color.white; // color of collar & power-up
    public GameObject ProjectileHeroPrefab; // prefab for projectiles
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float continuousDamage = 0;
    public float delayBetweenShots = 0;
    public float veloctiy = 20; // speed of prjectiles
}

public class Weapon : MonoBehaviour {
    static public Transform Projectile_Anchor;
    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime;
    private Renderer collarRend;


	// Use this for initialization
	void Start () {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // call setType() for the default type of weapontype.none
        SetType(_type);

        if (Projectile_Anchor == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            Projectile_Anchor = go.transform;
        }

        // find the firedlegate of the root GO
        GameObject rootGO = transform.root.gameObject;

        if (rootGO.GetComponent<Hero>()!= null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
		
	}

    public WeaponType type{
        get{
            return (_type);

        }
        set{
            SetType(value);
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
            else
            {
                this.gameObject.SetActive(true);
            }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0; // can fire immediately after
        }
    public void Fire()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;

        }
        if (Time.time - lastShotTime < def.delayBetweenShots)
        {
            return;
        }
        ProjectileHero p;
        Vector3 vel = Vector3.up * def.veloctiy;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;

            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
        }
    }

    public ProjectileHero MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.ProjectileHeroPrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = ("ProjectileHero");
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(Projectile_Anchor, true);
        ProjectileHero p = go.GetComponent<ProjectileHero>();
        p.type = type;
        lastShotTime = Time.time;
        return (p);
            
        }

    }


   

