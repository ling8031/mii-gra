using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    private int currentWaypoint = 0;

    [SerializeField] private float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.currentGameState != GameManager.GameState.GAME)
        {
            return;
        }
        float distance = Vector2.Distance(transform.position, waypoints[currentWaypoint].transform.position);

        if (distance < 0.1f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

        //Debug.Log("Wchodzi w movetowards" + currentWaypoint);
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].transform.position, speed * Time.deltaTime);
    }
}
