using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public PlayerMovement playerMovement;

    [Header("Adjustable")]
    public float attackRange = 2f;
    public float downAttackRange = 4f;
    public float swingRate = 1f;
    public float downswingRate = .5f;
    public int damage = 1;
    public bool isSwinging = false;
    public bool canSwing = true;

    private float cooldown;
    private float downCoolDown;
    public Rigidbody2D rb;
    Vector2 jump = new Vector2(0, 12);



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        if (downCoolDown > 0)
        {
            downCoolDown -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Swing();
            cooldown = swingRate;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && canSwing)
        {
            DownSwing();
            downCoolDown = downswingRate;
            isSwinging = true;
            canSwing = false;
        }
    }

    private void Swing ()
    {

        // Animation
        anim.SetTrigger("attack1");

        // Actual gameplay stuff

        RaycastHit2D[] hits = null;
        // Check left if facing left
        if (transform.localScale.x < 0)
        {
            hits = Physics2D.RaycastAll(transform.position, Vector2.left);
        }

        // Check right if facing right
        if (transform.localScale.x > 0)
        {
            hits = Physics2D.RaycastAll(transform.position, Vector2.right);
        }

        // If we are near nothing
        if (hits == null)
        {
            return;
        }

        for(int i = 0; i < hits.Length; i++ )
        {
            // If what we hit is within range
            if(hits[i].distance > attackRange)
            {
                return;
            }

            // If we hit ourself
            if(hits[i].collider.gameObject == gameObject)
            {
                continue;
            }

            // If we hit an enemy
            if(hits[i].collider.tag == "Enemy")
            {
                hits[i].collider.GetComponent<EnemyHealth>().TakeDamage(damage);
                return;
            }
        }

    }

    private void DownSwing ()
    {

        // Animation
        anim.Play("DownSwing");

        // Actual gameplay stuff

        //RaycastHit2D[] hits = null;
        //// Check left if facing left
        //if (transform.localScale.x < 0)
        //{
        //    if (playerMovement.isGrounded == true)
        //    {
        //        hits = Physics2D.RaycastAll(transform.position, Vector2.left);
        //    }
        //    else
        //    {
        //        hits = Physics2D.RaycastAll(transform.position, Vector2.down);
        //    }

        //}

        //// Check right if facing right
        //if (transform.localScale.x > 0)
        //{
        //    if (playerMovement.isGrounded == true)
        //    {
        //        hits = Physics2D.RaycastAll(transform.position, Vector2.right);
        //    }
        //    else
        //    {
        //        hits = Physics2D.RaycastAll(transform.position, Vector2.down);
        //    }

        //}

        //// If we are near nothing
        //if (hits == null)
        //{
        //    return;
        //}

        //for (int i = 0; i < hits.Length; i++)
        //{
        //    // If what we hit is within range
        //    if (hits[i].distance > downAttackRange)
        //    {
        //        return;
        //    }

        //    // If we hit ourself
        //    if (hits[i].collider.gameObject == gameObject)
        //    {
        //        continue;
        //    }

        //    //// If we hit an enemy
        //    //if (hits[i].collider.tag == "Enemy")
        //    //{
        //    //    hits[i].collider.GetComponent<EnemyHealth>().TakeDamage(damage);
        //    //    return;
        //    //}

        //    //if (hits[i].collider.tag == "Enemy")
        //    //{

        //    //    hits[i].collider.GetComponent<EnemyHealth>().TakeDamage(damage);
        //    //    return;
        //    //}


        //}

    }



    void OnTriggerEnter2D(Collider2D other)
    {
                 // Current velocity
        if (other.gameObject.tag == "EnemyHead" && isSwinging)
        {
            other.GetComponentInParent<EnemyHealth>().TakeDamage(damage);

            anim.SetBool("jumping", true);
            Vector3 vel = rb.velocity;
            vel.y = jump.y;
            rb.velocity = vel;
            //rb.AddForce(jump);
            isSwinging = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isSwinging = false;
            canSwing = true;
        }

    }

}
