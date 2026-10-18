using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float lifeTimer = 0f;
    [SerializeField] float lifeTime = 7f;
    public float velocity = 300f;
    // Update is called once per frame
    void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifeTime) Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Попал по {other.name}");
        Health health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(20);
            Destroy(gameObject);
        }
    }
}
