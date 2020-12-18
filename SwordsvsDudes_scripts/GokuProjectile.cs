using UnityEngine;

public class GokuProjectile : MonoBehaviour
{
    public static float MoveSpeed = 0.6f;

    [Header("Assigned via script")]
    public int damage;
    public float knockback;

    private float existTime = 20f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            col.GetComponent<PlayerHealth>().TakeDamage(damage);
            col.GetComponent<PlayerHealth>().ReceiveKnockback(transform.position, knockback, 0.5f);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Move the projectile
        if(transform.localScale.x > 0)
        {
            // Move right
            transform.position = Vector3.Lerp(transform.position, transform.position + (Vector3.right * MoveSpeed), 0.4f);
        } else
        {
            // Move left
            transform.position = Vector3.Lerp(transform.position, transform.position + (Vector3.left * MoveSpeed), 0.4f);
        }

        // Destroy the projectile (after an amount of time)
        existTime -= Time.deltaTime;
        if(existTime <= 0)
        {
            Destroy(gameObject);
        }
    }

}
