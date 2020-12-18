using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour 
{
    static public Main S; // singleton
    static public Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies; // array of enemy prefabs
    public float enemySpawnPerSecond = 0.5f; // # of enemies a second
    public float enemyDefualtPadding = 1.5f; // padding for position
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[]
    {WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield, WeaponType.freeze};
    private BoundsCheck bndCheck;

    public void shipDestroyed(Enemy e)
    {
        // potentially generate a powerup
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];
            //spawn a powerup
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // set it to the proper weapontype
            pu.SetType(puType);
             //set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;
        }
    }

    private void Awake()
    {
        S = this;
        //set bndcheck to refernece the boundschk component
        bndCheck = GetComponent<BoundsCheck>();
        //invoke spawnEnemy(
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        //position the enmy above the screen with a random x position
        float enemyPadding = enemyDefualtPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        //set the inital position for the spaned enemy

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //invoke spawnEnemy again
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
      }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);

    }

    public void Restart()
    {
        SceneManager.LoadScene("Scene_1");
        
    }
    /// <summary>
    /// Static funtion that gets a weponddefinition from the weap dict static 
    /// protected field of the main class.
    /// </summary>
    /// <returns>the weapon definitio or if there is no weapondefnition with 
    /// the weapon type passed in, returns a new weapond definition with a 
    /// weapon type of none
    /// </returns>

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        // check to make sure that the key exists 
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        return (new WeaponDefinition());
    }
}

    
    
		

