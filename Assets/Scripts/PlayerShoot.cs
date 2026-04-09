using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerShoot : MonoBehaviour
{
    public Camera cam;
    public Transform crosshair;
    public GameObject bulletPrefab;

    [Header("Fire")]
    public float fireCooldown = 0.12f;
    private float nextFireTime;

    [Header("Audio")]
    public AudioClip gunshot;
    public float soundCooldown = 0.08f;   // 🔑 tweak this
    private float nextSoundTime;

    private AudioSource audioSource;

    void Start()
    {
        if (cam == null) cam = Camera.main;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireCooldown;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        Vector3 targetWorld;

        if (crosshair != null)
            targetWorld = crosshair.position;
        else
        {
            Vector3 mouse = Input.mousePosition;
            mouse.z = -cam.transform.position.z;
            targetWorld = cam.ScreenToWorldPoint(mouse);
        }

        Vector2 dir = (targetWorld - transform.position).normalized;

        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bullet = b.GetComponent<Bullet>();
        if (bullet != null)
            bullet.Fire(dir);

        PlayGunshot();
    }

    void PlayGunshot()
    {
        if (gunshot == null) return;
        if (Time.time < nextSoundTime) return;

        nextSoundTime = Time.time + soundCooldown;

        // small pitch variation = less repetitive
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(gunshot);
    }
}
