using UnityEngine;

public class Spear : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerSFX playerSFX;

    public GameObject SpearPrefab;
    public GameObject SpearObject;
    public SpearCollision spearCollision;

    public float throwForce = 100;
    public float gravity = 9.8f;
    public float GrabDistance = 0.5f;

    public bool hasSpear = true;
    public bool isThrowing = false;
    public bool isThrown = false;
    public bool isRecalling = false;
    public bool collided = false;

    private Rigidbody2D rb;
    public Vector3 throwDirection;
    public GameObject collidedObject;


    void Update()
    {
        if (hasSpear)
        {
            if (Input.GetMouseButtonDown(1) && !isThrown && !isRecalling)
            {
                Throw();
            }
            else if (Input.GetMouseButtonDown(1) && isThrown && !isRecalling)
            {
                Recall();
            }
        }
    }

    void FixedUpdate()
    {
        if (isThrowing)
        {
            Throwing();
        }

        else if (isRecalling)
        {
            Recalling();
        }
    }

    public void Throw()
    {
        Instantiate(SpearPrefab, transform.position, Quaternion.identity);
        SpearObject = GameObject.FindWithTag("Spear");
        rb = SpearObject.GetComponent<Rigidbody2D>();
        spearCollision = SpearObject.GetComponentInChildren<SpearCollision>();
        PlatformEffector2D platformEffector2D = SpearObject.GetComponentInChildren<PlatformEffector2D>();
        Collider2D collider = SpearObject.GetComponent<Collider2D>();

        // Set the direction towards the mouse
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        throwDirection = (mousePos - transform.position).normalized;
        throwDirection *= playerMovement.Speed/60;

        Debug.DrawLine(transform.position, transform.position + throwDirection * 6f, Color.red, 2.0f);

        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        SpearObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Apply force to the spear in the direction of the mouse
        rb.velocity = throwDirection * throwForce;
        rb.gravityScale = gravity;
        isThrowing = true;
        isThrown = true;
        spearCollision.collided = false;
        platformEffector2D.enabled = true;
        collider.enabled = false;

        int RandomClip = Random.Range(0,2);
        playerSFX.Spear.clip = playerSFX.ThrowSpear[RandomClip];
        playerSFX.Spear.Play();
    }

    public void Recall()
    {
        SpearObject = GameObject.FindWithTag("Spear");
        rb = SpearObject.GetComponent<Rigidbody2D>();
        Collider2D collider = SpearObject.GetComponent<Collider2D>();

        GrabDistance = 0.5f;
        
        isThrowing = false;
        isRecalling = true;
        rb.isKinematic = false;
        rb.freezeRotation = false;
        rb.gravityScale = 0;
        collider.enabled = false;

        int RandomClip = Random.Range(0,3);
        playerSFX.Spear.clip = playerSFX.RecallSpear[RandomClip];
        playerSFX.Spear.Play();
    }

    public void Throwing()
    {
        if(!spearCollision.collided)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            SpearObject.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void Recalling()
    {
        rb.velocity += new Vector2 ( (((SpearObject.transform.position.x - transform.position.x) * (throwForce/100)) *-1 ),
                                     (((SpearObject.transform.position.y - transform.position.y) * (throwForce/100)) *-1 ));
        Vector3 directionToPlayer = (transform.position - SpearObject.transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg + 180;
        SpearObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        GrabDistance += 0.05f;

        if (Vector2.Distance(transform.position, SpearObject.transform.position) < GrabDistance && isRecalling)
        {
            // Spear reached the player, destroy it
            Destroy(SpearObject);
            isThrown = false;
            isRecalling = false;

            int RandomClip = Random.Range(0,2);
            playerSFX.Impale.clip = playerSFX.CatchSpear[RandomClip];
            playerSFX.Impale.Play();
        }
    }

    public void CollideGround()
    {
        Collider2D collider = SpearObject.GetComponent<Collider2D>();
        PlatformEffector2D platformEffector2D = SpearObject.GetComponentInChildren<PlatformEffector2D>();
        platformEffector2D.enabled = true;

        playerSFX.Impale.clip = playerSFX.ImpaleGround[0];
        playerSFX.Impale.Play();

        rb.isKinematic = true;
        rb.velocity = new Vector2(0,0);
        rb.freezeRotation = true;
        spearCollision.collided = true;
        collider.enabled = true;
    }

    public void CollideEnemy()
    {
        Collider2D collider = SpearObject.GetComponent<Collider2D>();

        SpearObject.transform.parent = spearCollision.CollidedObject.transform;

        playerSFX.Impale.clip = playerSFX.ImpaleEnemy[0];
        playerSFX.Impale.Play();

        rb.isKinematic = true;
        rb.velocity = new Vector2(0,0);
        rb.freezeRotation = true;
        spearCollision.collided = true;
        collider.enabled = true;
    }

}
