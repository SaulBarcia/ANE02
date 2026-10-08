using UnityEngine;
using UnityEngine.InputSystem; // <-- Necesario para el nuevo sistema

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerNewInput : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;

    [Header("Salto")]
    public float jumpForce = 5f;
    public float groundCheckDistance = 1.1f;
    public LayerMask groundLayer;

    [Header("Input System")]
    [Tooltip("Arrastra aquí tu asset PlayerInputActions")]
    public InputActionAsset playerActionsAsset;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction pauseAction;

    [Header("Menú de pausa")]
    public GameObject MenuPauseUI;
    public bool isPaused = false;

    private Rigidbody rb;
    private bool isGrounded;
    private Vector2 inputDirection;
    private bool jumpRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Configurar las acciones desde el Asset
        var gameplayMap = playerActionsAsset.FindActionMap("Player");
        moveAction = gameplayMap.FindAction("Move");
        jumpAction = gameplayMap.FindAction("Jump");
        pauseAction = gameplayMap.FindAction("Pause");
        MenuPauseUI.SetActive(false);
    }

    void OnEnable()
    {
        // Activar el mapa de acciones cuando el objeto esté activo
        playerActionsAsset.Enable();
    }

    void OnDisable()
    {
        // Desactivar para evitar fugas o lectura cuando el juego está pausado
        playerActionsAsset.Disable();
    }

    void Update()
    {
        // 1. Leer el Vector2 del joystick o del teclado (WASD)
        inputDirection = moveAction.ReadValue<Vector2>();

        // 2. Comprobar el suelo
        CheckGroundStatus();

        // 3. Detectar si se presionó el botón de salto
        if (jumpAction.triggered && isGrounded)
        {
            jumpRequested = true;
        }
        if (pauseAction.triggered)
        {
            TogglePause();
        }
        if (!isPaused)
        {
            inputDirection = moveAction.ReadValue<Vector2>();
            CheckGroundStatus();
            if (jumpAction.triggered)
            {
                jumpRequested = true;
            }
        }
        RaycastHit hit;
        int layerMask = 5;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }

    }
    void FixedUpdate()
    {
        // 4. Aplicar física
        MovePlayer();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }
    }

    void MovePlayer()
    {
        Vector3 targetVelocity = new Vector3(inputDirection.x, 0f, inputDirection.y) * moveSpeed;
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = (targetVelocity - new Vector3(currentVelocity.x, 0f, currentVelocity.z));

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }
    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused == true)
        {
            Time.timeScale = 0f;
            MenuPauseUI.SetActive(true);
        }
        else if (isPaused == false)
        {
            Time.timeScale = 1f;
            MenuPauseUI.SetActive(false);
        }
    }
}