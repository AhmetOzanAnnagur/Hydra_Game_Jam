using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;

    public TMP_Text healthText;

    [Header("Death")]
    public float restartDelay = 1.0f;   // seconds before reload
    public AudioClip deathSound;        // drag death sound here

    private bool isDead = false;
    private AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateUI();

        if (currentHealth == 0)
        {
            isDead = true;
            Debug.Log("Player died!");

            PlayDeathSound();
            Invoke(nameof(RestartGame), restartDelay);
        }
    }

    void PlayDeathSound()
    {
        if (deathSound == null) return;

        audioSource.Stop(); // stop any other sound on player
        audioSource.PlayOneShot(deathSound);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = currentHealth.ToString();
    }
}
