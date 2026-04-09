using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    public int damage = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerHealth hp = collision.collider.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(damage);

            Destroy(gameObject); // enemy disappears after hitting player
        }
    }
}
