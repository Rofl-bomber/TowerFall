using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Splines;

public class GameManager : MonoBehaviour
{
    public Dictionary<AircraftBase, AircraftInfo> aircraftEnemies=new();
    public GameObject targetShip;
    public AttackEntryPoint[] AttackEntryPoints;
    public GameObject player;
    
    public class AircraftInfo
    {
        public Transform endPoint;
        public Transform startPoint;
        public float landingTime;
        public AircraftInfo(Transform _startPoint, Transform _endPoint, float _landingTime){endPoint=_endPoint; startPoint=_startPoint; landingTime=_landingTime;}
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AttackEntryPoints = targetShip.GetComponentsInChildren<AttackEntryPoint>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void RegistryAircraft(AircraftBase aircraft)
    {
        AircraftHealth aircraftHealth = aircraft.GetComponent<AircraftHealth>();
        aircraftHealth.OnDeath += AicraftDeathHandler;
        Transform startPoint = aircraft.transform;
        Transform endPoint = AttackEntryPoints[Random.Range(0, AttackEntryPoints.Length)].transform;
        Spline path = aircraft.CreateApproachPath(
            new Pose(startPoint.position, startPoint.rotation),
            new Pose(endPoint.position, endPoint.rotation),
            aircraft.approachSpeed
        );
        float landingTime = path.GetLength() / aircraft.approachSpeed;
        aircraft.path = path;
        aircraft.target = targetShip.transform;
        aircraft.gameManager = this;
        aircraftEnemies[aircraft] = new AircraftInfo(
            startPoint,
            endPoint,
            landingTime
        );
    }
    void AircraftChangePathHandler(AircraftBase aircraft)
    {
        
    }
    void AicraftDeathHandler(AircraftBase aircraft)
    {
        aircraftEnemies.Remove(aircraft);
        Destroy(aircraft.gameObject);
    }
    public int GetEnemyCount() {return aircraftEnemies.Count;} 
    
}
