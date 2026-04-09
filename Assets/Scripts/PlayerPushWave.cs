using System.Collections;
using UnityEngine;

public class PlayerPushWave : MonoBehaviour
{
    [Header("Input")]
    public KeyCode key = KeyCode.Space;
    public float cooldown = 2.0f;

    [Header("Wave")]
    public GameObject shockwaveVisualPrefab;
    public float maxRadius = 6f;
    public float expandDuration = 0.6f;

    [Header("Push behavior")]
    public float extraBeyondRadius = 3f;
    public float pushSpeed = 12f;
    public LayerMask enemyLayer;

    [Header("Animation")]
    public string blastTriggerName = "Blast";

    private float nextReadyTime = 0f;

    private Animator animator;
    private readonly System.Collections.Generic.HashSet<Rigidbody2D> pushed = new();

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(key) && Time.time >= nextReadyTime)
        {
            nextReadyTime = Time.time + cooldown;

            // 🔥 trigger blast animation
            if (animator != null)
                animator.SetTrigger(blastTriggerName);

            StartCoroutine(PushWave());
        }
    }

    IEnumerator PushWave()
    {
        pushed.Clear();

        Transform vis = null;
        if (shockwaveVisualPrefab != null)
        {
            GameObject v = Instantiate(shockwaveVisualPrefab, transform.position, Quaternion.identity);
            vis = v.transform;
        }

        float t = 0f;
        float prevRadius = 0f;

        while (t < expandDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / expandDuration);
            float radius = Mathf.Lerp(0f, maxRadius, alpha);

            if (vis != null)
            {
                vis.position = transform.position;
                vis.localScale = new Vector3(radius * 2f, radius * 2f, 1f);
            }

            PushNewlyReached(prevRadius, radius);

            prevRadius = radius;
            yield return null;
        }

        if (vis != null)
            Destroy(vis.gameObject);
    }

    void PushNewlyReached(float innerRadius, float outerRadius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, outerRadius, enemyLayer);

        foreach (var hit in hits)
        {
            float d = Vector2.Distance(transform.position, hit.transform.position);
            if (d <= innerRadius) continue;

            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb == null) continue;
            if (pushed.Contains(rb)) continue;

            pushed.Add(rb);

            Vector2 dir = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = Vector2.zero;

            float targetDist = maxRadius + extraBeyondRadius;
            if (d >= targetDist) continue;

            Enemy ec = hit.GetComponent<Enemy>();
            if (ec != null) ec.Stun(0.25f);

            StartCoroutine(PushOutAtConstantSpeed(rb, dir, targetDist));
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }

    IEnumerator PushOutAtConstantSpeed(Rigidbody2D rb, Vector2 dir, float targetDistFromPlayer)
    {
        while (rb != null)
        {
            float d = Vector2.Distance(transform.position, rb.position);
            if (d >= targetDistFromPlayer)
                yield break;

            rb.MovePosition(rb.position + dir * pushSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
