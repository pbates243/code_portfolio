using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : MonoBehaviour {
    
    private bool hasCoroutineStarted = false;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Hero" && !hasCoroutineStarted)
        {
            hasCoroutineStarted = true;
            Destroy(gameObject);
            StartCoroutine(FreezePosition());

        }
    }

    IEnumerator FreezePosition()
    {
            Debug.Log("Freeze activated");
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            Debug.Log("Number of enemies: " + enemies.Length); // Check to see if all enemies have been gotten
        foreach (Enemy enemy in enemies)
        {
            enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            enemy.GetComponent<Enemy>().enabled = false;
            Object.Destroy(gameObject, 3.0f);
        }

            yield return new WaitForSeconds(5);
        foreach (Enemy enemy in enemies )
        {
            enemy.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            enemy.GetComponent<Enemy>().enabled = true;
        }

                
     }



    }
    //public void CallAbility(string abilityName)
    //{

    //    StartCoroutine(abilityName);
    //}

    
