using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracking : MonoBehaviour
{
    public float rayLength = 10f; // Length of the ray

    // Update is called once per frame
    void Update()
    {
        // Convert mouse position to world position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Calculate direction from game object to mouse position
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;

        // Cast a ray from the game object in the direction of the mouse
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayLength);

        // Draw the ray in the Scene view for debugging
        Debug.DrawRay(transform.position, direction * rayLength, Color.black);

        // If the ray hit something
        if (hit.collider != null)
        {
            // Do something with the hit object
            Debug.Log("Hit object: " + hit.collider.name);
        }
    }
}