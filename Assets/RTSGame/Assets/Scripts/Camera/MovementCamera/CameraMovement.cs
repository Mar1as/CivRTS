using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] private int rychlost = 10000;
    [SerializeField] private int sensitivita = 100;
    [SerializeField] float delka = 2f;
    private Vector2 rot;

    [SerializeField] LayerMask terrain;
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

        Vector3 move = new Vector3(horizMove, 0, vertikMove) * rychlost * ReturnCameraSpeed();
        var globalMove = transform.TransformDirection(move);

        rb.linearVelocity = new Vector3(globalMove.x, rb.linearVelocity.y, globalMove.z);
    }

    void Height()
    {
        Vector3 move = new Vector3(0, -Input.mouseScrollDelta.y, 0) * rychlost;
        if (move.y > delka)
        {
            //Debug.Log("Move.y " + move.y);
        }
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, move.y, rb.linearVelocity.z);
        CheckGround();
    }

    void MouseRotate()
    {
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.LeftAlt))
        {
            rot.x += Input.GetAxis("Mouse X") * sensitivita;
            rot.y += Input.GetAxis("Mouse Y") * sensitivita;
            if (rot.y > 0)
            {
                rot.y = 0;
            }
            if (rot.y < -90)
            {
                rot.y = -90;
            }
            transform.rotation = Quaternion.Euler(-rot.y, rot.x, 0);
        }

    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, delka, terrain))
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, hit.point.y + delka, gameObject.transform.position.z);
        }
    }

    int ReturnCameraSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }
}
