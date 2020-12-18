using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; // singleton

    [Header("Set in Inspector")]
    // these fields control the movement of the ship

    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;
    public GameObject ProjectileHeroPrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    public float freezeTime;

    [Header("Set Dynamically")]
    [SerializeField]
    public float _shieldLevel = 1;
    private GameObject lastTriggerGo = null;
    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    private void Start()
    {
        if (S == null)
        {
            S = this; // set the singleton
            ClearWeapons();
            weapons[0].SetType(WeaponType.blaster);

        }

        else
        {
            Debug.LogError("Hero.Awake() - Attemptd to assign second Hero.S!");
        }

        //fireDelegate += TempFire;
    }


    // Update is called once per frame
    void Update()
    {
        // pull in information from the input 
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        // allow the ship to fire
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TempFire();
        //}

        // use the fireDelegate to fire weaponds 
        //first make sure the button is pressed: Axis("Jump")
        // then ensure that fireDelegate isnt null to avoid an error
        if (Input.GetKeyDown(KeyCode.Space) && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    void TempFire()
    {
        GameObject projGO = Instantiate<GameObject>(ProjectileHeroPrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigid = projGO.GetComponent<Rigidbody>();
        //rigid.velocity = Vector3.up * projectileSpeed;
        ProjectileHero proj = projGO.GetComponent<ProjectileHero>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).veloctiy;
        rigid.velocity = Vector3.up * tSpeed;;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        print("Triggered: " + other.gameObject.name);

        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "Powerup")
        {
            AbsorbPowerUp(go);
        }
        //else if (go.tag == "Freeze")
        //{
        //    Destroy(go);
        //}

        else
        {
            print("Triggere by non-Enemy: " + go.name);
        }


    }

    public void AbsorbPowerUp (GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch(pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;

            case WeaponType.freeze:


            default:
                if(pu.type == weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                }
                else{
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
            //leave empty for now
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {

        get
        {
            return (_shieldLevel);
        }

        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }

        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}

