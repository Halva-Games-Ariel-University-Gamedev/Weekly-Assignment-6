using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // This handles triggers
    void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    // This handles physical bumps
    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    void HandleHit(GameObject hitObject)
    {
        Health health = hitObject.GetComponent<Health>();

        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log("Hit " + hitObject.name + " for " + damage + " damage!");
        }
        else
        {
            // If the script is on a child object, try to find it on the parent
            health = hitObject.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("Hit parent of " + hitObject.name);
            }
        }

        Destroy(gameObject);
    }
}