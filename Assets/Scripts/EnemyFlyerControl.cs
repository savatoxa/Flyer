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

    private int currentWaypointIndex = 0;
    private Rigidbody m_rigidbody;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        if (m_rigidbody == null)
        {
            Debug.LogError("EnemyFlyerControl: Rigidbody component is required on " + gameObject.name);
            return;
        }

        // Auto-collect waypoints from parent if configured
        if (autoCollectWaypoints && waypointsParent != null)
        {
            CollectWaypointsFromParent();
        }

        // Start at the first waypoint
        if (waypoints != null && waypoints.Length > 0)
        {
            currentWaypointIndex = 0;
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

        // Direction towards the target waypoint
        Vector3 directionToTarget = targetWaypoint.position - transform.position;
        directionToTarget.y = 0f; // keep waypoint's altitude

        if (directionToTarget.magnitude <= waypointReachDistance)
        {
            // Waypoint reached — switch to the next one in a loop
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            targetWaypoint = waypoints[currentWaypointIndex];
            directionToTarget = targetWaypoint.position - transform.position;
            directionToTarget.y = 0f;
        }

        // Smooth turn towards the target
        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }

        // Set forward velocity (physics-based movement)
        m_rigidbody.velocity = transform.forward * flySpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}