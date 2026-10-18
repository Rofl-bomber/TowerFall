using System;
using UnityEngine;
using UnityEngine.Splines; 

public abstract class AircraftBase : MonoBehaviour
{
   [SerializeField] public float approachSpeed = 10f;
   [SerializeField] protected float attackSpeed = 10f;
   [SerializeField] protected float escapeSpeed = 10f;
   [SerializeField] protected float rollSpeed = 120f;
   [SerializeField] protected float turnSpeed = 60f;
   public AimReticle aimReticle;
   public GameManager gameManager;
   public float moveSpeed;
   float currentRoll = 0f;
   float targetRoll = 0f;
   Vector3 debugPosition;
   float freeRadius = 10f;
   protected float currentT = 0f;
   public Spline path;
   public Transform target;
   protected AirCraftState currentAircraftState;
   protected enum AirCraftState
    {
        Approach,
        Attack,
        Escape
    }
    void Awake()
    {
        aimReticle = GetComponentInChildren<AimReticle>();
        aimReticle.transform.SetParent(null);
        aimReticle.Hide();
    }
    protected abstract void Approach();
    protected abstract void Attack();
    protected abstract void Escape();
    public Vector3 GetLeadPoint(
        Vector3 shooterPos,
        Vector3 targetPos,
        Vector3 targetVelocity,
        float projectileSpeed)
    {
        Vector3 D = targetPos - shooterPos;

        float a = Vector3.Dot(targetVelocity, targetVelocity) 
                - projectileSpeed * projectileSpeed;

        float b = 2f * Vector3.Dot(D, targetVelocity);

        float c = Vector3.Dot(D, D);

        float discriminant = b * b - 4f * a * c;

        if (discriminant < 0)
            return targetPos;

        float sqrt = Mathf.Sqrt(discriminant);

        float t1 = (-b - sqrt) / (2f * a);
        float t2 = (-b + sqrt) / (2f * a);

        float t = Mathf.Min(
            t1 > 0 ? t1 : float.MaxValue,
            t2 > 0 ? t2 : float.MaxValue
        );

        if (t == float.MaxValue)
            return targetPos;

        return targetPos + targetVelocity * t;
    }
    protected bool Move(Spline path, float moveSpeed)
    {
        if (currentT >= 1f) return false;
        transform.position = path.GetPointAtLinearDistance(currentT, moveSpeed * Time.deltaTime, out currentT);
        Vector3 desiredPosition = path.GetPointAtLinearDistance(currentT, 2 * moveSpeed * Time.deltaTime, out _);
        Vector3 desiredDirection = (currentT < 0.95f) ? desiredPosition - transform.position :  path.EvaluateTangent(currentT);
        //Vector3 desiredDirection = desiredPosition - transform.position;
        Turn(desiredDirection, moveSpeed);
        return true;
    }
    float CalculateRoll(Vector3 currentDirection, Vector3 targetDirection, Vector3 vectorUp, float moveSpeed, float dt)
    {
        float angularSpeed = Vector3.Angle(currentDirection, targetDirection) * Mathf.Deg2Rad / dt;
        Vector3 turnAxis = Vector3.Cross(
            currentDirection,
            targetDirection
        );
        float sign = (-1f) *Mathf.Sign(
            Vector3.Dot(turnAxis, vectorUp)
        );
        // return Mathf.Clamp(
        //     sign * Mathf.Atan(moveSpeed * angularSpeed / Physics.gravity.magnitude) * Mathf.Rad2Deg,
        //     -90f,
        //     90f
        // );
        return sign * Mathf.Atan(moveSpeed * angularSpeed / Physics.gravity.magnitude) * Mathf.Rad2Deg;
    }
    protected void Turn(Vector3 desiredDirection, float moveSpeed)
    {
        desiredDirection.Normalize();

        Quaternion baseRotation =
            Quaternion.LookRotation(desiredDirection, Vector3.up);

        targetRoll = CalculateRoll(
            transform.forward,
            desiredDirection,
            transform.up,
            moveSpeed,
            Time.deltaTime
        );

        currentRoll = Mathf.MoveTowards(
            currentRoll,
            targetRoll,
            rollSpeed * Time.deltaTime
        );

        Quaternion roll = Quaternion.AngleAxis(
            currentRoll,
            baseRotation * Vector3.forward
        );

        Quaternion desiredRotation = roll * baseRotation;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            desiredRotation,
            turnSpeed * Time.deltaTime
        );
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(debugPosition, freeRadius);

        for (int i = 0; i < path.Count; i++)
        {
            BezierKnot knot = path[i];
            Gizmos.DrawSphere(knot.Position, 2f);
        }

        Gizmos.color = Color.yellow;

        Vector3 prev = path.EvaluatePosition(0);

        int steps = 50;

        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;

            Vector3 current = path.EvaluatePosition(t);

            Gizmos.DrawLine(prev, current);
            prev = current;
        }
    }
    public virtual Spline CreateApproachPath(Pose startPoint, Pose endPoint, float moveSpeed) {return null;}
}
