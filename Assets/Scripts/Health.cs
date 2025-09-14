using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    [Header("FX")]
    public AudioSource audioSource;
    public AudioClip hurtSound;   // tiếng "ựa"
    public bool destroyOnDeath = true; // enemy thì true, player có thể false

    void Start()
    {
        currentHealth = maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(20); // mỗi viên bullet trừ 20 HP
            Destroy(collision.gameObject); // xóa đạn sau va chạm
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Phát tiếng "ựa" mỗi lần dính đạn
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                Destroy(gameObject, 0.3f);
            }
            else
            {
                Debug.Log("Player died!");
                // TODO: gọi GameOver hoặc reset scene
            }
        }
    }
}
