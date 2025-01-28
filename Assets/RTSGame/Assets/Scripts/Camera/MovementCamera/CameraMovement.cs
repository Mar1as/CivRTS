using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float rychlostWASD = 10000f;
    [SerializeField] private float rychlostVysky = 50f;

    [SerializeField] private float sensitivita = 100f;
    [SerializeField] private float delka = 2f;
    private Vector2 rot;

    [SerializeField] private LayerMask terrain;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Movement();
        MouseRotate();
        Height();
    }

    void Movement()
    {
        float horizMove = Input.GetAxis("Horizontal");
        float vertikMove = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizMove, 0, vertikMove) * rychlostWASD * ReturnCameraSpeed();
        Vector3 globalMove = transform.TransformDirection(move);

        rb.linearVelocity = new Vector3(globalMove.x, rb.linearVelocity.y, globalMove.z);
    }

    void Height()
    {
        // Získání vstupu koleèka myši pro zmìnu výšky
        float scroll = -Input.mouseScrollDelta.y * rychlostVysky * Time.deltaTime;

        // Aplikace zmìny výšky
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, scroll, rb.linearVelocity.z);

        // Kontrola, zda kamera není pod terénem
        CheckGround();
    }

    void MouseRotate()
    {
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.LeftAlt))
        {
            rot.x += Input.GetAxis("Mouse X") * sensitivita * Time.deltaTime;
            rot.y += Input.GetAxis("Mouse Y") * sensitivita * Time.deltaTime;

            // Omezení rotace na vertikální ose
            rot.y = Mathf.Clamp(rot.y, -90f, 0f);

            transform.rotation = Quaternion.Euler(-rot.y, rot.x, 0);
        }
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, delka, terrain))
        {
            // Udržení kamery nad terénem
            float targetHeight = hit.point.y + delka;
            if (transform.position.y < targetHeight)
            {
                transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            }
        }
    }

    int ReturnCameraSpeed()
    {
        return Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
    }
}
