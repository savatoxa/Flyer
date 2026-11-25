using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyerControl : MonoBehaviour
{
    [Header("Path Setup")]
    [Tooltip("Parent object containing waypoint children. If set and autoCollectWaypoints is true, waypoints array will be auto-filled.")]
    public Transform waypointsParent;
    
    [Tooltip("Auto-collect child transforms of waypointsParent as waypoints on Start.")]
    public bool autoCollectWaypoints = true;
    
    [Tooltip("Array of waypoints. The object will fly through them in a loop.")]
    public Transform[] waypoints;

    [Header("Flight Settings")]
    [Tooltip("Flight speed (m/s)")]
    public float flySpeed = 15.0f;

    [Tooltip("Rotation speed towards the next waypoint (degrees/s)")]
    public float turnSpeed = 60.0f;

    [Tooltip("Distance at which waypoint is considered reached")]
    public float waypointReachDistance = 2.0f;

    [Tooltip("Use gravity? If false, plane will maintain altitude automatically.")]
    public bool useGravity = false;

    [Header("Bank Settings")]
    [Tooltip("Maximum bank angle during turns (degrees)")]
    public float maxBankAngle = 45.0f;

    [Tooltip("Bank rotation speed (degrees/s)")]
    public float bankSpeed = 90.0f;

    [Tooltip("Bank direction sign (1 = standard, -1 = inverted). Adjust if plane banks outward instead of inward.")]
    public float bankSignMultiplier = -1f;

    [Header("Pitch Settings")]
    [Tooltip("Maximum pitch angle (up/down) in degrees")]
    public float maxPitchAngle = 30.0f;

    [Tooltip("Pitch adjustment speed (degrees/s)")]
    public float pitchSpeed = 60.0f;

    [Header("Debug")]
    [Tooltip("Enable debug logging")]
    public bool debugMode = false;

    private int currentWaypointIndex = 0;
    private Rigidbody m_rigidbody;
    private float currentBankAngle = 0f;
    private float currentPitchAngle = 0f;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        if (m_rigidbody == null)
        {
            Debug.LogError("EnemyFlyerControl: Rigidbody component is required on " + gameObject.name);
            return;
        }

        // Configure Rigidbody
        m_rigidbody.useGravity = useGravity;
        m_rigidbody.drag = 0f;
        m_rigidbody.angularDrag = 0.5f;

        // Auto-collect waypoints from parent if configured
        if (autoCollectWaypoints && waypointsParent != null)
        {
            CollectWaypointsFromParent();
        }

        // Start at the first waypoint
        if (waypoints != null && waypoints.Length > 0)
        {
            currentWaypointIndex = 0;
            if (debugMode)
            {
                Debug.Log($"EnemyFlyerControl: Starting at waypoint {currentWaypointIndex} at position {waypoints[currentWaypointIndex].position}");
            }
        }
        else
        {
            Debug.LogWarning("EnemyFlyerControl: No waypoints assigned. Object will not move.");
        }
    }
    
    /// <summary>
    /// Collects child transforms from waypointsParent and populates the waypoints array.
    /// Filters children whose names start with "waypoint" (case-insensitive).
    /// </summary>
    private void CollectWaypointsFromParent()
    {
        List<Transform> collected = new List<Transform>();
        
        foreach (Transform child in waypointsParent)
        {
            // Optional: filter by name prefix "waypoint"
            if (child.name.StartsWith("waypoint", System.StringComparison.OrdinalIgnoreCase))
            {
                collected.Add(child);
            }
        }
        
        // Sort by name so waypoint00, waypoint01, ... maintain order
        collected.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.OrdinalIgnoreCase));
        
        waypoints = collected.ToArray();
        Debug.Log($"EnemyFlyerControl: Collected {waypoints.Length} waypoints from parent '{waypointsParent.name}'");
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0 || m_rigidbody == null)
            return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Full direction including vertical component
        Vector3 directionToTarget = targetWaypoint.position - transform.position;
        
        // Horizontal distance for waypoint checking (ignore vertical)
        Vector3 horizontalDirection = directionToTarget;
        horizontalDirection.y = 0f;

        // Check if reached waypoint (horizontal distance only)
        if (horizontalDirection.magnitude <= waypointReachDistance)
        {
            if (debugMode)
            {
                Debug.Log($"Waypoint {currentWaypointIndex} reached! Horizontal distance: {horizontalDirection.magnitude:F2}m. Switching to next waypoint.");
            }
            
            // Waypoint reached — switch to the next one in a loop
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            targetWaypoint = waypoints[currentWaypointIndex];
            directionToTarget = targetWaypoint.position - transform.position;
            horizontalDirection = directionToTarget;
            horizontalDirection.y = 0f;
        }
        else if (debugMode && Time.frameCount % 60 == 0) // Log every 60 frames to avoid spam
        {
            Debug.Log($"Distance to waypoint {currentWaypointIndex}: horizontal {horizontalDirection.magnitude:F2}m, vertical {directionToTarget.y:F2}m");
        }

        // Calculate desired pitch based on vertical offset
        float targetPitchAngle = 0f;
        if (Mathf.Abs(directionToTarget.y) > 0.5f) // Dead zone for vertical adjustment
        {
            // Normalize vertical difference to pitch angle (-maxPitchAngle to +maxPitchAngle)
            // Positive y difference means waypoint is above -> pitch up (negative pitch in Unity)
            // We'll invert sign: if waypoint is above, we need to pitch up (negative x rotation in Unity)
            float verticalRatio = Mathf.Clamp(directionToTarget.y / 10f, -1f, 1f); // Full pitch at 10m difference
            targetPitchAngle = -verticalRatio * maxPitchAngle; // Negative because Unity's positive X rotation is nose down
        }
        
        // Smoothly adjust pitch
        currentPitchAngle = Mathf.MoveTowards(currentPitchAngle, targetPitchAngle, pitchSpeed * Time.deltaTime);

        // Calculate horizontal turn
        float targetBankAngle = 0f;
        if (horizontalDirection.sqrMagnitude > 0.01f)
        {
            // Get forward direction WITHOUT bank and pitch (only yaw)
            Vector3 euler = transform.eulerAngles;
            Quaternion rotationFlat = Quaternion.Euler(0f, euler.y, 0f);
            Vector3 forwardFlat = rotationFlat * Vector3.forward;
            
            // Get angle between horizontal forward and horizontal target direction
            float turnAngle = Vector3.SignedAngle(forwardFlat, horizontalDirection.normalized, Vector3.up);
            
            // Dead zone for small angles (no banking on straight flight)
            float minTurnAngleForBank = 5f;
            if (Mathf.Abs(turnAngle) > minTurnAngleForBank)
            {
                // Normalize turn angle to bank angle range (-maxBankAngle to +maxBankAngle)
                // Divide by 45 degrees to get full bank at 45-degree turn
                float normalizedTurn = Mathf.Clamp(turnAngle / 45f, -1f, 1f);
                targetBankAngle = bankSignMultiplier * normalizedTurn * maxBankAngle;
                
                if (debugMode && Time.frameCount % 60 == 0)
                {
                    Debug.Log($"TurnAngle: {turnAngle:F1}°, TargetBank: {targetBankAngle:F1}°, TargetPitch: {targetPitchAngle:F1}°");
                }
            }
        }
        
        // Smoothly adjust bank angle
        currentBankAngle = Mathf.MoveTowards(currentBankAngle, targetBankAngle, bankSpeed * Time.deltaTime);
        
        // Apply rotation with pitch, yaw, and bank
        // First, rotate towards target horizontally (yaw)
        Quaternion targetYawRotation = Quaternion.LookRotation(horizontalDirection.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetYawRotation, turnSpeed * Time.deltaTime);
        
        // Then apply pitch and bank while preserving yaw
        Vector3 finalEuler = transform.eulerAngles;
        finalEuler.x = currentPitchAngle;
        finalEuler.z = currentBankAngle;
        transform.eulerAngles = finalEuler;

        // Set forward velocity (physics-based movement)
        // Use the actual forward after applying all rotations
        m_rigidbody.velocity = transform.forward * flySpeed;
        
        // Optional: if not using gravity, add slight upward force to maintain altitude
        if (!useGravity && Mathf.Abs(directionToTarget.y) < 0.5f)
        {
            // Small upward velocity to counteract any downward drift
            Vector3 vel = m_rigidbody.velocity;
            vel.y = Mathf.MoveTowards(vel.y, 0f, 5f * Time.deltaTime);
            m_rigidbody.velocity = vel;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}