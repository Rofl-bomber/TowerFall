using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] Barrel[] barrels;
    [SerializeField] Transform turretPivot;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] public float convergenceDistance = 500f;
    public float fireRate = 200f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnValidate()
    {
        barrels = GetComponentsInChildren<Barrel>();
        foreach (Barrel barrel in barrels)
        {
            barrel.fireRate = fireRate;
            barrel.projectilePrefab = projectilePrefab;
            barrel.turretPivot = turretPivot;
            barrel.UpdateConvergence(convergenceDistance);
        }
    }
    public void Shoot()
    {
        foreach (Barrel barrel in barrels)
        {
            barrel.MakeShoot();
        }
    }
}
