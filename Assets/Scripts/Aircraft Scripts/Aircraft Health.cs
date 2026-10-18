using System;
using UnityEngine;

public class AircraftHealth : Health
{
    public event Action<AircraftBase> OnDeath;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Die()
    {
        AircraftBase aircraft = GetComponent<AircraftBase>();
        Destroy(aircraft.aimReticle.gameObject);
        OnDeath?.Invoke(aircraft);
    }
}
