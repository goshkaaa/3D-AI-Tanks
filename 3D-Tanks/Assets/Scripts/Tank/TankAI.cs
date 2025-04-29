using UnityEngine;

public class TankAI : MonoBehaviour
{
    private TankMovement m_Movement;
    private TankShooting m_Shooting;
    private Transform m_PlayerTank;
    private float m_UpdatePathInterval = 0.5f;
    private float m_NextPathUpdate;
    private float m_ShootRange = 20f;
    private float m_ChaseRange = 30f;
    private float m_CurrentLaunchForce;
    private bool m_Fired;

    private void Awake()
    {
        m_Movement = GetComponent<TankMovement>();
        m_Shooting = GetComponent<TankShooting>();
        
        // Make AI tank twice as slow
        m_Movement.m_Speed = m_Movement.m_Speed / 2f;
        m_Movement.m_TurnSpeed = m_Movement.m_TurnSpeed / 2f;
    }

    private void Start()
    {
        // Find the player tank
        GameObject playerTank = GameObject.FindGameObjectWithTag("Player");
        if (playerTank != null)
        {
            m_PlayerTank = playerTank.transform;
        }
    }

    private void Update()
    {
        if (m_PlayerTank == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, m_PlayerTank.position);

        // Update path to player periodically
        if (Time.time >= m_NextPathUpdate)
        {
            m_NextPathUpdate = Time.time + m_UpdatePathInterval;
            UpdatePathToPlayer();
        }

        // Handle shooting
        if (distanceToPlayer <= m_ShootRange)
        {
            // Aim at player
            Vector3 targetDirection = (m_PlayerTank.position - transform.position).normalized;
            float targetAngle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
            
            // Rotate towards player
            if (Mathf.Abs(targetAngle) > 5f)
            {
                m_Movement.m_TurnInputValue = Mathf.Sign(targetAngle);
            }
            else
            {
                m_Movement.m_TurnInputValue = 0f;
                // Shoot when facing player
                if (!m_Fired)
                {
                    m_CurrentLaunchForce = m_Shooting.m_MaxLaunchForce;
                    m_Shooting.Fire();
                    m_Fired = true;
                    Invoke("ResetFire", 2f);
                }
            }
        }
    }

    private void UpdatePathToPlayer()
    {
        if (m_PlayerTank == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, m_PlayerTank.position);

        if (distanceToPlayer > m_ChaseRange)
        {
            // Stop moving if too far
            m_Movement.m_MovementInputValue = 0f;
            m_Movement.m_TurnInputValue = 0f;
            return;
        }

        // Calculate direction to player
        Vector3 directionToPlayer = (m_PlayerTank.position - transform.position).normalized;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);

        // Set movement values
        m_Movement.m_MovementInputValue = 1f; // Always move forward
        m_Movement.m_TurnInputValue = Mathf.Clamp(angleToPlayer / 45f, -1f, 1f); // Smooth turning
    }

    private void ResetFire()
    {
        m_Fired = false;
    }
} 