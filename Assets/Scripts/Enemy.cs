using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 3f;
    public float spawnDistance = 20f;

    [Header("Damage")]
    public int damage = 1;

    [Header("Map Bounds")]
    public Vector2 mapMin = new Vector2(-8f, -11f);
    public Vector2 mapMax = new Vector2(43f, 19f);

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float stunTimer = 0f;

    [System.NonSerialized] public bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        FindPlayer();
        gameObject.tag = "Enemy";
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (player == null || isDead)
        {
            rb.linearVelocity = Vector2.zero;
            if (animator != null)
                animator.SetBool("IsRunning", false);
            return;
        }

        if (stunTimer > 0f)
        {
            stunTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            if (animator != null)
                animator.SetBool("IsRunning", false);
            return;
        }

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * speed;

        if (animator != null)
            animator.SetBool("IsRunning", rb.linearVelocity.sqrMagnitude > 0.01f);

        if (direction.x != 0)
            spriteRenderer.flipX = direction.x < 0;
    }

    public void Stun(float time)
    {
        stunTimer = time;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth hp = collision.collider.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(damage);

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        if (other.CompareTag("Bullet"))
        {
            isDead = true;

            NotifyScoreDisplay();
            CreateClones();

            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void NotifyScoreDisplay()
    {
        ScoreDisplay scoreDisplay = FindObjectOfType<ScoreDisplay>();
        if (scoreDisplay != null)
            scoreDisplay.AddEnemyKill(transform.position);
    }

    void CreateClones()
    {
        if (player == null) return;

        for (int i = 0; i < 2; i++)
        {
            Vector2 spawnPos = GetSpawnPosition();
            GameObject newEnemy = Instantiate(gameObject, spawnPos, Quaternion.identity);

            Enemy e = newEnemy.GetComponent<Enemy>();
            if (e != null)
                e.isDead = false;
        }
    }

    Vector2 GetSpawnPosition()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return ClampToMap((Vector2)player.position + Random.insideUnitCircle * 5f);

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        float distance = Mathf.Max(camWidth, camHeight) + spawnDistance;

        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(0f, 360f);
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 pos = (Vector2)player.position + dir * distance;

            if (IsInsideMap(pos))
                return pos;
        }

        return ClampToMap((Vector2)player.position);
    }

    bool IsInsideMap(Vector2 pos)
    {
        return pos.x >= mapMin.x && pos.x <= mapMax.x &&
               pos.y >= mapMin.y && pos.y <= mapMax.y;
    }

    Vector2 ClampToMap(Vector2 pos)
    {
        return new Vector2(
            Mathf.Clamp(pos.x, mapMin.x, mapMax.x),
            Mathf.Clamp(pos.y, mapMin.y, mapMax.y)
        );
    }
}
