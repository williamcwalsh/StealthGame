using System.Collections.Generic;
using UnityEngine;

public class EnemyWaypoints : MonoBehaviour
{
    public List<Transform> waypoints;
    public float speed = 5f;
    public float turnSpeed = 360f;

    private Transform currentWaypoint;
    private EnemyVision2D enemyVision;

    private float closeEnouth = 0.5f;
    private int point = 0;

    void Awake()
    {
        enemyVision = GetComponent<EnemyVision2D>();
    }

    void Start()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        currentWaypoint = waypoints[point];
        SnapToCurrentWaypointDirection();
        UpdateVisionDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        currentWaypoint = waypoints[point];
        RotateTowardsWaypoint();
        UpdateVisionDirection();

        transform.position = Vector3.MoveTowards(
            transform.position,
            currentWaypoint.position,
            Time.deltaTime * speed
        );

        if (Vector3.Distance(transform.position, currentWaypoint.position) < closeEnouth)
        {
            if (point + 1 < waypoints.Count)
                point++;
            else
                point = 0;
        }
    }

    void RotateTowardsWaypoint()
    {
        if (currentWaypoint == null)
            return;

        Vector2 moveDirection = currentWaypoint.position - transform.position;
        if (moveDirection.sqrMagnitude <= 0.001f)
            return;

        Quaternion targetRotation = GetTargetRotation(moveDirection);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );
    }

    void SnapToCurrentWaypointDirection()
    {
        if (currentWaypoint == null)
            return;

        Vector2 moveDirection = currentWaypoint.position - transform.position;
        if (moveDirection.sqrMagnitude <= 0.001f)
            return;

        transform.rotation = GetTargetRotation(moveDirection);
    }

    Quaternion GetTargetRotation(Vector2 moveDirection)
    {
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg + 90f;
        return Quaternion.Euler(0f, 0f, angle);
    }

    void UpdateVisionDirection()
    {
        if (enemyVision == null)
            return;

        enemyVision.SetFacingDirection(-transform.up);
    }
}
