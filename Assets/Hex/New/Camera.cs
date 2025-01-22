using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float decelerationFactor = 0.9f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minHeight = 5f;  // Minimální výška kamery
    [SerializeField] private float maxHeight = 50f; // Maximální výška kamery

    private Camera cam;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 inputDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputDirection += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDirection += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDirection += Vector3.right;
        }

        if (inputDirection != Vector3.zero)
        {
            rb.linearVelocity = inputDirection.normalized * speed;
        }
        else
        {
            rb.linearVelocity *= decelerationFactor;

            if (rb.linearVelocity.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector3.zero;
            }
        }
    }

    private void HandleZoom()
    {
        if (cam == null) return;

        float scroll = Input.mouseScrollDelta.y * decelerationFactor;
        if (scroll != 0f)
        {
            Vector3 position = transform.position;
            position.y -= scroll * zoomSpeed;
            position.y = Mathf.Clamp(position.y, minHeight, maxHeight);
            transform.position = position;
        }
    }
}
