using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float fireSpeed = 50f;
    public int maxAmmo = 10;
    private int currentAmmo;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    private XRGrabInteractable grabbable;

    void Start()
    {
        grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);
        currentAmmo = maxAmmo;
    }

    void OnDestroy()
    {
        grabbable.activated.RemoveListener(FireBullet);
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;

        // Spawn đạn
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = spawnPoint.forward * fireSpeed;
        Destroy(bullet, 5f);

        // Hiệu ứng
        if (muzzleFlash) muzzleFlash.Play();
        if (audioSource && fireSound) audioSource.PlayOneShot(fireSound);
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
        if (audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);
    }
}
