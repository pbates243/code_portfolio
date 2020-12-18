using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyHealth : MonoBehaviour
{
    [Header("Adjustable")]
    public int health;
    public int maxHealth;
    public Color colorToFlash = Color.red;

    [Header("References")]
    public SpriteRenderer sr;
    public GameObject[] specialHealthBar;


    private Color originalColor;
    private bool flash = false;
    private float flashSpeed = 5f;
    private float flashProgress;

    private void Start()
    {
        if(sr == null)
        {
            Debug.LogWarning("Failed to assign sprite renderer to EnemyHealth.cs on enemy. Attempting to automatically grab reference", gameObject);
            sr = GetComponent<SpriteRenderer>();
            if(sr == null)
            {
                Debug.LogError("Failed to automatically grab sprite renderer reference on enemy, does enemy not have a sprite renderer at all? Enemy cannot function without one - Destroying enemy");
                Destroy(gameObject);
                return;
            }
        }

        originalColor = sr.color;
    }

    public void TakeDamage (int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            health = 0; // Health can't be negative

            Die();
        } else
        {
            // If we took damage but it wasn't lethal
            DamageIndicator();

            if(specialHealthBar != null && specialHealthBar.Length > 0)
            {
                UpdateHealthBar();
            }
        }
    }

    private void Die ()
    {
        Destroy(gameObject);
        if (gameObject.name == "Goku")
        {
            SceneManager.LoadScene(4);
        }
    }

    private void DamageIndicator()
    {
        sr.color = colorToFlash;
        flashProgress = 0f;
        flash = true;
    }

    private void Update()
    {

        if(flash)
        {
            flashProgress += Time.deltaTime * flashSpeed;
            sr.color = Color.Lerp(colorToFlash, originalColor, flashProgress);

            if(sr.color == originalColor)
            {
                flash = false;
            }
        }
    }

    private void UpdateHealthBar ()
    {
        // Disable all hearts for now
        foreach(GameObject g in specialHealthBar)
        {
            g.SetActive(false);
        }

        // And only enable as many as there is health
        for(int i = 0; i < health; i++)
        {
            specialHealthBar[i].SetActive(true);
        }

        if (specialHealthBar.Length <2)
        {
            SceneManager.LoadScene(0);
        }
    }

}
