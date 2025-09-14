using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class GunMP40 : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform spawnPoint;
    public float fireSpeed = 60f;
    public int maxAmmo = 32;
    private int currentAmmo;

    [Header("Effects")]
    public GameObject muzzleFlash;
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip emptySound;

    [Header("Timings")]
    public float fireRate = 0.12f;   // 500 RPM ≈ 0.12s/shot
    public float reloadTime = 2f;

    [Header("Input")]
    public InputActionProperty reloadAction; // gán nút B ở Inspector

    private XRGrabInteractable grabbable;
    private bool isShooting = false;
    private bool isReloading = false;

    void Start()
    {
        grabbable = GetComponent<XRGrabInteractable>();
        if (grabbable != null)
        {
            grabbable.activated.AddListener(StartFiring);
            grabbable.deactivated.AddListener(StopFiring);
        }

        if (reloadAction != null && reloadAction.action != null)
        {
            reloadAction.action.performed += OnReloadPerformed;
        }

        currentAmmo = maxAmmo;
    }

    void OnDestroy()
    {
        if (grabbable != null)
        {
            grabbable.activated.RemoveListener(StartFiring);
            grabbable.deactivated.RemoveListener(StopFiring);
        }

        if (reloadAction != null && reloadAction.action != null)
        {
            reloadAction.action.performed -= OnReloadPerformed;
        }
    }

    private void OnReloadPerformed(InputAction.CallbackContext ctx)
    {
        Reload();
    }

    private void StartFiring(ActivateEventArgs arg)
    {
        if (!isShooting && !isReloading)
        {
            isShooting = true;
            StartCoroutine(AutoFire());
        }
    }

    private void StopFiring(DeactivateEventArgs arg)
    {
        isShooting = false;
    }

    private IEnumerator AutoFire()
    {
        while (isShooting && !isReloading)
        {
            if (currentAmmo > 0)
            {
                FireBullet();
                currentAmmo--;
            }
            else
            {
                if (audioSource && emptySound) audioSource.PlayOneShot(emptySound);
                isShooting = false;
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    private void FireBullet()
    {
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
    }

    public void Reload()
    {
        if (isReloading || currentAmmo == maxAmmo) return;
        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;

        if (audioSource && reloadSound) audioSource.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
