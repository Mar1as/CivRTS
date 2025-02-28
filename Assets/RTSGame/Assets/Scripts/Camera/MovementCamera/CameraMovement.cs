using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float rychlostWASD = 10f; // Snížená rychlost
    [SerializeField] private float rychlostVysky = 50f;

    [SerializeField] private float sensitivita = 100f;
    [SerializeField] private float delka = 2f;
    private Vector2 rot;

    [SerializeField] private LayerMask terrain;

    private Vector3 movementInput;
    private float scrollInput;

    float lastTargetHeight = float.MinValue;

    [SerializeField] private Vector3[] zoneForMovement = new Vector3[4]; // Definice zóny pro pohyb

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Čtení vstupů v Update
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        scrollInput = -Input.mouseScrollDelta.y * rychlostVysky * Time.deltaTime;

        // Změna výšky
        Height();

        // Rotace kamery
        MouseRotate();
    }

    private void FixedUpdate()
    {
        // Pohyb a změna výšky v FixedUpdate
        Movement();
    }

    void Movement()
    {
        // Výpočet pohybu
        Vector3 move = movementInput * rychlostWASD * ReturnCameraSpeed();
        Vector3 globalMove = transform.TransformDirection(move);

        // Omezení pohybu v zóně (ignorujeme Y)
        Vector3 newPosition = rb.position + globalMove * Time.fixedDeltaTime;
        if (IsPositionInZone(newPosition))
        {
            rb.linearVelocity = new Vector3(globalMove.x, rb.linearVelocity.y, globalMove.z);
        }
        else
        {
            rb.linearVelocity = Vector3.zero; // Zastavení pohybu, pokud je mimo zónu
        }
    }

    void Height()
    {
        // Aplikace změny výšky (bez omezení Y)
        transform.position = new Vector3(transform.position.x, transform.position.y + scrollInput, transform.position.z);

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

        if (Physics.Raycast(ray, out hit, int.MaxValue, terrain))
        {
            // Udržení kamery nad terénem
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

    bool IsPositionInZone(Vector3 position)
    {
        // Kontrola, zda je pozice v definované zóně (ignorujeme Y)
        if (zoneForMovement.Length < 4)
        {
            Debug.LogError("ZoneForMovement must have at least 4 points to define a zone.");
            return true;
        }

        // Zóna je definována jako obdélník v XZ rovině (ignorujeme Y)
        float minX = Mathf.Min(zoneForMovement[0].x, zoneForMovement[1].x, zoneForMovement[2].x, zoneForMovement[3].x);
        float maxX = Mathf.Max(zoneForMovement[0].x, zoneForMovement[1].x, zoneForMovement[2].x, zoneForMovement[3].x);
        float minZ = Mathf.Min(zoneForMovement[0].z, zoneForMovement[1].z, zoneForMovement[2].z, zoneForMovement[3].z);
        float maxZ = Mathf.Max(zoneForMovement[0].z, zoneForMovement[1].z, zoneForMovement[2].z, zoneForMovement[3].z);

        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }
}