using UnityEngine;

public class CubeMover : MonoBehaviour
{
    public float speed = 3f;      // Speed of movement
    public float distance = 5f;   // Distance to move from the starting point

    private Vector3 startPos;

    void Start()
    {
        // Record the starting position
        startPos = transform.position;
    }

    void Update()
    {
        // Calculate the new x position using Mathf.PingPong for smooth back-and-forth motion
        float x = Mathf.PingPong(Time.time * speed, distance * 2) - distance;

        // Apply the new position, keeping y and z the same
        transform.position = new Vector3(startPos.x + x, startPos.y, startPos.z);
    }
}