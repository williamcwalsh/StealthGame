using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyVision2D : MonoBehaviour
{
    static bool sceneReloadRequested;

    public Transform player;
    public float viewDistance = 6f;

    [Range(0f, 360f)]
    public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public bool canSeePlayer;

    Vector2 facingDirection = Vector2.down;

    void Awake()
    {
        sceneReloadRequested = false;
    }

    void Update()
    {
        canSeePlayer = PlayerInSight();

        if (canSeePlayer)
            ReloadCurrentScene();
    }

    public void SetFacingDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.01f)
            facingDirection = dir.normalized;
    }

    bool PlayerInSight()
    {
        if (player == null)
            return false;

        Vector2 origin = transform.position;
        Vector2 toPlayer = (Vector2)player.position - origin;

        if (toPlayer.magnitude > viewDistance)
            return false;

        float angleToPlayer = Vector2.Angle(facingDirection, toPlayer);
        if (angleToPlayer > viewAngle * 0.5f)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            toPlayer.normalized,
            viewDistance,
            obstacleMask
        );
        if (hit.collider != null)
        {
            float wallDist = hit.distance;
            float playerDist = toPlayer.magnitude;
            if (wallDist < playerDist)
                return false;
        }

        return true;
    }

    void ReloadCurrentScene()
    {
        // Prevent multiple enemies from requesting the same reload in one frame.
        if (sceneReloadRequested)
            return;

        sceneReloadRequested = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 pos = transform.position;
        Vector3 left = DirFromAngle(-viewAngle * 0.5f);
        Vector3 right = DirFromAngle(viewAngle * 0.5f);

        Gizmos.DrawLine(pos, pos + left * viewDistance);
        Gizmos.DrawLine(pos, pos + right * viewDistance);
        Gizmos.DrawWireSphere(pos, viewDistance);
    }

    Vector3 DirFromAngle(float angleOffset)
    {
        float baseAngle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
        float angle = baseAngle + angleOffset;
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
    }
}
