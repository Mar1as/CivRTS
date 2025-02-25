using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Cameros : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float rychlostWASD = 10f;
    [SerializeField] private float rychlostVysky = 50f;
    [SerializeField] private float sensitivita = 100f;
    [SerializeField] private float delka = 2f;
    private Vector2 rot;

    [SerializeField] private LayerMask terrain;

    private PlayerInput inputActions;
    private InputAction moveAction;
    private InputAction scrollAction;
    private InputAction lookAction;
    private InputAction rotateAction;

    private void Awake()
    {
        inputActions = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        // Inicializace akcí
        moveAction = inputActions.actions["Move"];
        scrollAction = inputActions.actions["Scroll"];
        lookAction = inputActions.actions["Look"];
        //rotateAction = inputActions.actions["Rotate"];
    }

    private void OnEnable()
    {
        moveAction.Enable();
        scrollAction.Enable();
        lookAction.Enable();
        //rotateAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        scrollAction.Disable();
        lookAction.Disable();
        //rotateAction.Disable();
    }

    private void Update()
    {
        Height();
        Movement();
        MouseRotate();
    }

    void Movement()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        float horizMove = moveInput.x;
        float vertikMove = moveInput.y;

        Vector3 move = new Vector3(horizMove, 0, vertikMove) * rychlostWASD * ReturnCameraSpeed();
        Vector3 globalMove = transform.TransformDirection(move);

        rb.linearVelocity = new Vector3(globalMove.x, rb.linearVelocity.y, globalMove.z);
    }

    void Height()
    {
        float scroll = scrollAction.ReadValue<float>() * rychlostVysky * Time.deltaTime;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, scroll, rb.linearVelocity.z);

        CheckGround();
    }

    void MouseRotate()
    {
        if (Keyboard.current.leftAltKey.isPressed)
        {
            Vector2 lookInput = lookAction.ReadValue<Vector2>();
            rot.x += lookInput.x * sensitivita * Time.deltaTime;
            rot.y += lookInput.y * sensitivita * Time.deltaTime;

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
            float targetHeight = hit.point.y + delka;
            if (transform.position.y < targetHeight)
            {
                transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            }
        }
    }

    int ReturnCameraSpeed()
    {
        return Keyboard.current.leftShiftKey.isPressed ? 2 : 1;
    }
}