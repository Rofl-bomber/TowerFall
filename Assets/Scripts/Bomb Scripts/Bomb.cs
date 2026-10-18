using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float damage = 100f;
    float lifeTime = 10f;
    float lifeTimer = 0f;

    void Update()
    {
        lifeTime += Time.deltaTime;
        if (lifeTimer > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ShipHealth ship = other.GetComponentInParent<ShipHealth>();

        if (ship != null)
        {
            ship.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
