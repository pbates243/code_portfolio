using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Adjustable/Core")]
    public int maxHealth = 8;
    public int curHealth;

    [Header("References")]
    public GameObject[] playerHearts;
    public GameObject player;
    public PlayerMovement movement;
    public Rigidbody2D rb;
    public bool dead = false;


    [Header("Observe")]
    public bool isStunned = false;
    private float stunTimer;

    private bool godMode = false;

    private void Start()
    {
        curHealth = maxHealth;
        ReRenderHearts();

    }

    public void TakeDamage(int amount)
    {
        if(godMode)
        {
            return;
        }

        curHealth -= amount;

        if (curHealth <= 0)
        {
            // Health can never be a negative
            curHealth = 0;

            Die();
        } else
        {
            // If the player is still alive
            ReRenderHearts();
        }
    }

    public void GainHealth(int amount)
    {
        curHealth += amount;

        // A player can't have more than their maximum health allows
        if (curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }

        ReRenderHearts();
    }

    private void Die()
    {
        Scene currentScene1 = SceneManager.GetActiveScene();
        string sceneName = currentScene1.name;
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        //dead = true;
        if (dead)
        {
            return;
        }


        if (sceneName == "Level2")
        {
            SceneManager.LoadScene(6);
        }
        else
        {
            SceneManager.LoadScene(currentScene);
        }
            // When the player dies, the level should just reset


        // Get the build index of the scene the player is currently in


        // And then tell unity to reload that scene (the one we're on)
        


    }

    private void ReRenderHearts()
    {
        // First disable all hearts
        foreach(GameObject g in playerHearts)
        {
            g.SetActive(false);
        }

        // Then enable as many as the player has health
        for(int i = 0; i < curHealth; i++)
        {
            playerHearts[i].SetActive(true);
        }
    }

    public void ReceiveKnockback (Vector3 from, float amount, float stunTime)
    {
        return;
        movement.beingPushedBack = true;
        //rb.force

        stunTimer = stunTime;
        isStunned = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            godMode = !godMode;
        }

        if(isStunned)
        {
            stunTimer -= Time.deltaTime;
            if(stunTimer <= 0)
            {
                movement.beingPushedBack = false;
                isStunned = false;
            }
        }
    }

}
