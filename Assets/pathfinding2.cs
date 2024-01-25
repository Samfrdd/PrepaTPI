using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathfinding2 : MonoBehaviour
{
    [SerializeField]
    private Transform[] _waypoints;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private int currentWaypointIndex = 0;
    [SerializeField]
    private Transform _to;

    public Transform[] Waypoints { get => _waypoints;private set => _waypoints = value; }
    public Transform To { get => _to; private set => _to = value; }

    void Update()
    {
        if (currentWaypointIndex < Waypoints.Length)
        {
            MoveTowardsWaypoint();
        }
        else
        {
           // Debug.Log("Path completed!");
        }
    }

    void MoveTowardsWaypoint()
    {
        Transform targetWaypoint = Waypoints[currentWaypointIndex];
        float step = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, step);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex++;
        }
    }

    public void AddWaypoint(Transform newWaypoint)
    {
        // Ajoutez le nouveau waypoint à la liste
        System.Array.Resize(ref _waypoints, _waypoints.Length + 1);
        _waypoints[_waypoints.Length - 1] = newWaypoint;
        _to = newWaypoint;

        // Vous pouvez ajuster le comportement en fonction de vos besoins
    }
}
