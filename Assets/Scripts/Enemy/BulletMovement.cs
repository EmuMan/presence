using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision _collision)
    {
        if (_collision.gameObject.layer != LayerMask.NameToLayer("PlayerAttacks") || _collision.gameObject.layer != LayerMask.NameToLayer("EnemyAttacks"))
        {
            Destroy(gameObject);
        }
    }
    
}
