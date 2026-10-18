using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public float score = 100f;
    public virtual void TakeDamage(float damage)
    {
        score -= damage;
        if (score <= 0) Die();
    }
    protected abstract void Die();
}
