using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GunController : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float fireSpeed = 100f;
    public int maxAmmo = 10;
    private int currentAmmo;

    [Header("Effects")]
    public GameObject muzzleFlash;
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip emptySound; // Âm thanh khi bắn hết đạn

    [Header("Timings")]
    public float fireCooldown = 2.5f;  // Delay giữa các phát
    public float reloadTime = 4f;      // Thời gian nạp đạn

    [Header("Input")]
    public InputActionProperty reloadAction; // Gán nút B ở Inspector

    private XRGrabInteractable grabbable;
    private bool canShoot = true;
    private bool isReloading = false;

    void Start()
    {
        grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);

        reloadAction.action.performed += ctx => Reload();
        currentAmmo = maxAmmo;
    }

    void OnDestroy()
    {
        grabbable.activated.RemoveListener(FireBullet);
        reloadAction.action.performed -= ctx => Reload();
    }

    public void FireBullet(ActivateEventArgs arg)
    {
        if (!canShoot || isReloading) return;

        if (currentAmmo > 0)
        {
            currentAmmo--;

            // Spawn bullet
            GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = spawnPoint.forward * fireSpeed;
            Destroy(bullet, 3f);

            // Muzzle flash
            if (muzzleFlash != null)
            {
                GameObject flash = Instantiate(muzzleFlash, spawnPoint.position, spawnPoint.rotation);
                Destroy(flash, 0.1f);
            }

            // Sound
            if (audioSource && fireSound) audioSource.PlayOneShot(fireSound);
            var haptic = GetComponent<HapticFeedback>();
            if (haptic != null) haptic.PlayHaptic(grabbable);

            // Bắt đầu cooldown
            StartCoroutine(FireDelay());
        }
        else
        {
            // Nếu hết đạn, phát âm thanh "cạch cạch"
            if (audioSource && emptySound) audioSource.PlayOneShot(emptySound);
        }
    }

    private IEnumerator FireDelay()
    {
        canShoot = false;
        yield return new WaitForSeconds(fireCooldown);
        canShoot = true;
    }

    public void Reload()
    {
        if (isReloading || currentAmmo == maxAmmo) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        // Play reload sound
        if (audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
