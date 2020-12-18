using UnityEngine;

public class Enemy_Skeleton : MonoBehaviour
{
    public static float PlayerDetectDistance = 12f;
    public static float StoppingDistance = 3f;
    public static float AttackDistance = 3f;
    public static float DashDistance = 6f;
    public static float DashSpeed = 9f;

    [Header("Adjustable")]
    public float moveSpeed = 0.05f;
    public int damage = 3;
    public float attackRate = 3f;
    public float knockback = 50f;
    public float stunTime = 1.5f;

    [Header("References")]
    public Animator anim;
    public BoxCollider2D boxCollider;
    public Rigidbody2D rb;

    private Transform player;

    private Vector3 playerPos;
    private Vector3 curPos;

    private Vector2 bottomLeft;
    private Vector2 bottomRight;

    private float cooldown;
    private bool isDashing = false;

    private Vector3 startPosWhenDash;
    private Vector3 playerPosWhenDash;
    private float startTimeWhenDash;
    private float dashLength;

    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerHealth>().transform;
    }

    private void Update()
    {
        CalculatePositionCaches();

        if (Vector3.Distance(curPos, playerPos) > PlayerDetectDistance)
        {
            return;
        }

        // If the skeleton is dashing it shouldn't be changing which direction
        // it's looking because it shouldn't be able to look the other way
        // until the dash is finished
        if (!isDashing)
        {
            CalculateDirectionToFace();
        }

        CalculateCombat();

        // If the skeleton is dashing, it shouldn't be moving because
        // moving would override the dashing
        if (!isDashing)
        {
            CalculateMovement();
        }

        if(isDashing)
        {
            float distCovered = (Time.time - startTimeWhenDash) * DashSpeed;
            float fracJourney = distCovered / dashLength;

            transform.position = Vector3.Lerp(startPosWhenDash, playerPosWhenDash, fracJourney);

            // If the dash has finished
            if(transform.position == playerPosWhenDash)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isDashing = false;
            }
        }
    }

    private void CalculatePositionCaches ()
    {
        playerPos = player.position;
        curPos = transform.position;

        // Bottom left
        bottomLeft = new Vector2(transform.position.x, transform.position.y)
            + new Vector2(-(boxCollider.size.x / 2f), 0.1f);

        // Bottom right
        bottomRight = new Vector2(transform.position.x, transform.position.y)
            + new Vector2(boxCollider.size.x / 2f, 0.1f);
    }

    private void CalculateMovement ()
    {
        // If the player is close by, the knight should stop moving
        if(Vector3.Distance(curPos, playerPos) <= StoppingDistance)
        {
            anim.SetBool("isMoving", false);
            return;
        }

        if(playerPos.x < curPos.x)
        {
            // Left
            // If the enemy can't move any further left
            if(IsGround_Left() == false)
            {
                anim.SetBool("isMoving", false);
                Debug.Log("Cant move left!");
                return;
            } else
            {
                // If the enemy CAN move further left
                Move(false);
            }
        } else if(playerPos.x > curPos.x)
        {
            // Right
            // If the enemy can't move any further right
            if (IsGround_Right() == false)
            {
                anim.SetBool("isMoving", false);
                return;
            }
            else
            {
                // If the enemy CAN move further right
                Move(true);
            }
        }
    }

    private void CalculateDirectionToFace ()
    {
        if (playerPos.x < curPos.x)
        {
            transform.localScale = new Vector3(-6f, 10f, 6f);
        }
        else if (playerPos.x > curPos.x)
        {
            transform.localScale = new Vector3(6f, 10f, 6f);
        }
    }

    private void CalculateCombat ()
    {
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        float roll = Random.Range(0, 100);

        // 30% chance to do a dash attack
        if(roll <= 30f)
        {
            if (Vector3.Distance(curPos, playerPos) <= DashDistance)
            {
                anim.SetTrigger("attack2");

                player.GetComponent<PlayerHealth>().TakeDamage(damage);
                player.GetComponent<PlayerHealth>().ReceiveKnockback(transform.position, knockback, stunTime);

                // Dash toward the player
                startPosWhenDash = curPos;
                playerPosWhenDash = playerPos;
                playerPosWhenDash.y = curPos.y;
                isDashing = true;
                startTimeWhenDash = Time.time;
                dashLength = Vector3.Distance(curPos, playerPos);
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        } else
        {
            // If not doing a dash attack, do a normal one
            if (Vector3.Distance(curPos, playerPos) <= AttackDistance)
            {
                anim.SetTrigger("attack1");

                player.GetComponent<PlayerHealth>().TakeDamage(damage);
                player.GetComponent<PlayerHealth>().ReceiveKnockback(transform.position, knockback, stunTime);
            }
        }

        cooldown = attackRate;
    }

    private void Move (bool right)
    {
        anim.SetBool("isMoving", true);
        if (right)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + (transform.right * moveSpeed), 0.4f);
        } else
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + (-transform.right * moveSpeed), 0.4f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "SideDetection")
        {
            Destroy(gameObject);
            transform.position = Vector3.Lerp(transform.position, transform.position + (-transform.right * moveSpeed), 0.4f);
        }
    }

    #region Ground Detection
    private bool IsGround_Left()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(bottomLeft, Vector2.down);

        bool isOnValidGround = false;

        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.tag == "Ground" && hit.distance < 0.14f)
            {
                isOnValidGround = true;
            }
        }

        return isOnValidGround;
    }

    private bool IsGround_Right()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(bottomRight, Vector2.down);

        bool isOnValidGround = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "Ground" && hit.distance < 0.14f)
            {
                isOnValidGround = true;
            }
        }

        return isOnValidGround;
    }
    #endregion

}
