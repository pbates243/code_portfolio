using UnityEngine;

public class GokuLaser : MonoBehaviour
{
    public static float GhostTime = 0.75f;
    public static float ExistTime = 1.25f;

    [Header("Assigned via script")]
    public int damage;
    public float knockback;

    [Header("References")]
    public GameObject ghostForm;
    public GameObject realForm;

    private bool ghost = true;
    private float timer;

    private bool hasHitPlayer = false;

    private void Start()
    {
        UpdateVisual();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= GhostTime)
        {
            ghost = false;
            UpdateVisual();
        }
        if(timer >= ExistTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        // If the laser is in its ghost form it can't hurt or effect anything/anyone
        if(ghost || hasHitPlayer)
        {
            return;
        }

        if(col.tag == "Player")
        {
            col.GetComponent<PlayerHealth>().TakeDamage(damage);
            col.GetComponent<PlayerHealth>().ReceiveKnockback(transform.position, knockback, 1f);
            hasHitPlayer = true;
        }
    }

    private void UpdateVisual()
    {
        if(ghost)
        {
            ghostForm.SetActive(true);
            realForm.SetActive(false);
        } else
        {
            ghostForm.SetActive(false);
            realForm.SetActive(true);
        }
    }

}
