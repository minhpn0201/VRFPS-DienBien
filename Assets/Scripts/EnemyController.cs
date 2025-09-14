using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform player;               // Drag XR Origin hoặc Main Camera vào
    public GameObject bulletPrefab;        // Prefab viên đạn
    public GameObject muzzleFlashPrefab;   // Prefab flash
    public AudioSource audioSource;        // Audio source gắn vào enemy
    public AudioClip gunSound;             // Âm thanh bắn
    public Transform firePoint;            // Vị trí đầu nòng súng

    [Header("Stats")]
    public float fireRate = 1.5f;          // Thời gian giữa mỗi phát bắn
    public float fireSpeed = 25f;          // Tốc độ viên đạn
    public float detectionRange = 25f;     // Khoảng phát hiện player
    public float shootRange = 15f;         // Trong tầm này thì dừng bắn
    public float moveSpeed = 3.5f;         // Tốc độ chạy
    public float rotationSpeed = 5f;       // Tốc độ xoay mặt về player

    private float nextFireTime;
    private Animator anim;
    private NavMeshAgent agent;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = shootRange * 0.8f; // dừng lại cách player 80% tầm bắn
            agent.updateRotation = false;               // để tự mình xoay bằng script
        }
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Nếu player ngoài tầm detection → idle
        if (distance > detectionRange)
        {
            agent.isStopped = true;
            anim.SetBool("isMoving", false);
            return;
        }

        // Nếu player trong tầm bắn
        if (distance <= shootRange)
        {
            agent.isStopped = true;
            anim.SetBool("isMoving", false);

            // Xoay mượt về phía player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
            }

            // Bắn nếu cooldown xong
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            // Chạy lại gần player
            agent.isStopped = false;
            agent.SetDestination(player.position);
            anim.SetBool("isMoving", true);
        }
    }

    void Shoot()
    {
        anim.SetTrigger("isShooting");

        // Spawn bullet
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * fireSpeed;
            }
            Destroy(bullet, 2f);
        }

        // Spawn muzzle flash
        if (muzzleFlashPrefab != null && firePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint);
            Destroy(flash, 0.2f);
        }

        // Play gun sound
        if (audioSource != null && gunSound != null)
        {
            audioSource.PlayOneShot(gunSound);
        }
    }
}
