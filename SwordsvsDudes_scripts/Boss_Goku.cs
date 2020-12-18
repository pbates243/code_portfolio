using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss_Goku : MonoBehaviour
{
    public static float TimeOnGroundWhenProjectile = 2f;
    public static float TimeOnGroundWhenLaser = 3f;

    public static float JumpRate = 0.08f;
    public static Range LandRange = new Range(-2f, 4.5f);

    public static float TimeBetweenProjectiles = 0.4f;
    public GameObject Goku;
    public GameObject player1;
    public EnemyHealth health1;

    [Header("Adjustable")]
    public int projectilesToFire = 3;
    public int damageIfHitByLaser = 3;
    public int damageIfHitByProjectile = 1;
    public float knockbackIfHitByLaser = 100f;
    public float knockbackIfHitByProjectile = 50f;
    

    [Header("Adjustable/Jumping")]
    public Vector3 jumpOffset;
    public Vector3 jumpRange;

    [Header("Prefabs")]
    public GameObject jumpPrefab;
    public GameObject projectilePrefab;
    public GameObject laserPrefab;

    [Header("References")]
    public Transform player;

    [Header("Debugging")]
    public bool showJumpArea = false;

    private float inAirTime;
    private float onGroundTime;

    private float jumpTimer;

    private float timeToNextJump;
    private float landOnGroundTimer; // How long he has to be in the air before landing
    private float goBackToAirTimer; // How long he has to be on ground before going back to air

    private bool onGround = false;

    private void Update()
    {


        if(onGround)
        {
            onGroundTime += Time.deltaTime;
            if(onGroundTime >= goBackToAirTimer)
            {
                onGround = false;
                onGroundTime = 0f;
                landOnGroundTimer = LandRange.GetRandomRange();
            }
        } else
        {
            inAirTime += Time.deltaTime;
            jumpTimer += Time.deltaTime;

            if(jumpTimer >= timeToNextJump)
            {
                transform.position = FindJumpPosition();
                timeToNextJump = JumpRate;
                jumpTimer = 0f;
            }

            if(inAirTime >= landOnGroundTimer)
            {
                transform.position = FindGroundPosition();
                onGround = true;
                inAirTime = 0f;
                jumpTimer = 0f;
                goBackToAirTimer = DoAttack();
            }

            //if (health1.health < 1)
            //{
            //    SceneManager.LoadScene(4);
            //}
        }
    }

    private float DoAttack ()
    {
        float roll = Random.Range(0, 100);

        // 30% chance to do laser attack
        if(roll <= 30)
        {
            Transform laser = Instantiate(laserPrefab, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, null).GetComponent<Transform>();
            Vector3 scale = laser.localScale;

            GokuLaser data = laser.GetComponent<GokuLaser>();
            data.damage = damageIfHitByLaser;
            data.knockback = knockbackIfHitByLaser;

            // If the player is to our left
            if(player.transform.position.x < transform.position.x)
            {
                scale.x *= -1f;
            }

            laser.localScale = scale;

            return TimeOnGroundWhenLaser;
        } else
        {
            // Do projectile attack
            for(int i = 0; i < projectilesToFire; i++)
            {
                float time = i * TimeBetweenProjectiles;
                Invoke("FireProjectile", time);
            }

            return TimeOnGroundWhenProjectile;
        }
    }

    private void FireProjectile ()
    {
        Transform projectile = Instantiate(projectilePrefab, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity, null).GetComponent<Transform>();
        Vector3 scale = projectile.localScale;

        GokuProjectile data = projectile.GetComponent<GokuProjectile>();
        data.damage = damageIfHitByProjectile;
        data.knockback = knockbackIfHitByProjectile;

        // If the player is to our left
        if (player.transform.position.x < transform.position.x)
        {
            scale.x *= -1f;
        }

        projectile.localScale = scale;
    }

    private Vector3 FindGroundPosition ()
    {
        Vector3 groundPos = FindJumpPosition();
        groundPos.y = -7f;
        return groundPos;
    }

    private Vector3 FindJumpPosition ()
    {
        Vector3 jumpPosition = new Vector3();

        jumpPosition = jumpOffset;
        jumpPosition.x = Random.Range(-jumpRange.x / 4.5f, jumpRange.x / 4.5f);
        jumpPosition.y = Random.Range(-jumpRange.y / 4.5f, jumpRange.y / 4.5f);

        return jumpPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if(showJumpArea == false)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawCube(jumpOffset, jumpRange);

    }


}
