using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrip : MonoBehaviour
{
    // Speed of movement
    public float moveSpeed = 5f;

    public float rayLength;
    private LayerMask layersToHit = 8 << 1;

    void Start()
    {
        rayLength = transform.localScale.y / 2;
    }

    void Update()
    {
        // Get input from arrow keys
        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            verticalInput = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            verticalInput = -1f;
        }

        // Move the object up or down
        transform.Translate(Vector3.up * verticalInput * moveSpeed * Time.deltaTime);
    }

    void FixedUpdate()
    {
        // Create a ray pointing upwards from the object's position
        Ray2D ray = new Ray2D(transform.position, Vector2.up);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayLength, layersToHit);

        if (hit.collider != null)
        {
            Debug.Log(hit.collider.gameObject.name + " was hit!");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * rayLength, Color.red);
        }
    }
}
