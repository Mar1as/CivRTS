using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float rychlostWASD = 10f; // Sn�en� rychlost
    [SerializeField] private float rychlostVysky = 50f;

    [SerializeField] private float sensitivita = 100f;
    [SerializeField] private float delka = 2f;
    private Vector2 rot;

    [SerializeField] private LayerMask terrain;

    private Vector3 movementInput;
    private float scrollInput;

    float lastTargetHeight = float.MinValue;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // �ten� vstup� v Update
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        scrollInput = -Input.mouseScrollDelta.y * rychlostVysky * Time.deltaTime;

        //transform.position = new Vector3(transform.position.x, transform.position.y + scrollInput, transform.position.z);
        Height();

        // Rotace kamery
        MouseRotate();
    }

    private void FixedUpdate()
    {
        // Pohyb a zm�na v��ky v FixedUpdate
        Movement();
    }

    void Movement()
    {
        // V�po�et pohybu
        Vector3 move = movementInput * rychlostWASD * ReturnCameraSpeed();
        Vector3 globalMove = transform.TransformDirection(move);

        // Aplikace pohybu
        rb.linearVelocity = new Vector3(globalMove.x, rb.linearVelocity.y, globalMove.z);
    }

    void Height()
    {
        // Aplikace zm�ny v��ky
        transform.position = new Vector3(transform.position.x, transform.position.y + scrollInput, transform.position.z);

        // Kontrola, zda kamera nen� pod ter�nem
        CheckGround();
    }

    void MouseRotate()
    {
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.LeftAlt))
        {
            rot.x += Input.GetAxis("Mouse X") * sensitivita * Time.deltaTime;
            rot.y += Input.GetAxis("Mouse Y") * sensitivita * Time.deltaTime;

            // Omezen� rotace na vertik�ln� ose
            rot.y = Mathf.Clamp(rot.y, -90f, 0f);

            transform.rotation = Quaternion.Euler(-rot.y, rot.x, 0);
        }
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, int.MaxValue, terrain))
        {
            // Udr�en� kamery nad ter�nem
            float targetHeight = hit.point.y + delka;
            if (transform.position.y < targetHeight)
            {
                transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            }

            lastTargetHeight = targetHeight;
        }
        else
        {
            if (lastTargetHeight != float.MinValue)
            {
                if (transform.position.y < lastTargetHeight)
                {
                    transform.position = new Vector3(transform.position.x, lastTargetHeight, transform.position.z);
                }
            }
        }

    }

    int ReturnCameraSpeed()
    {
        return Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
    }
}