using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Bullet chỉ sống vài giây để tránh rác
        Destroy(gameObject, 0.1f);
    }
}
