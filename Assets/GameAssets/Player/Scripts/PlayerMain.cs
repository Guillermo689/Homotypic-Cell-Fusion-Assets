using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMain : MonoBehaviour
{
    public float oxygen;
    [SerializeField] private TMP_Text oxygenText;
    public Rigidbody2D rb;
    public float oxygenConsumed;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float grabRadius;
    [SerializeField] private LayerMask cellLayer;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private bool isGrabbing = false;
    [SerializeField] private GameObject cellGrabbed;
    [SerializeField] private GameObject guardianCell;

    internal PlayerInput playerInput;
    internal InputAction moveAction;
    internal InputAction grabAction;
    internal InputAction turnCell;
    public InputAction pause;

    private bool facingLeft;
    public bool guardianCreated;
    public GameObject cellTograb;

    [SerializeField] Collider2D[] cells;

    [SerializeField] private GameObject guardianParticles;
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] footSteps;
    [SerializeField] private AudioClip cellLetGo;
    [SerializeField] private AudioClip cellPickUp;
    [SerializeField] private AudioClip createGuardian;

    [SerializeField] private FixedJoystick joyStick;

    private Animator animator;
    private int walkID;
   
    private int idleID;
    private int carryID;
    private int gameOverID;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
       
        moveAction = playerInput.actions["Movement"];
        grabAction = playerInput.actions["Grab"];
        turnCell = playerInput.actions["TurnCell"];
        pause = playerInput.actions["Pause"];

        walkID = Animator.StringToHash("Walk");
        gameOverID = Animator.StringToHash("GameOver");
        idleID = Animator.StringToHash("Idle");
        carryID = Animator.StringToHash("Carry");
    }
    void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<CellFusion>().audioSource;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        oxygenText.text = "Oxygen: " + Mathf.RoundToInt(oxygen).ToString(); 
        if (oxygen <= 100)
        {
            oxygenText.color = Color.red;
        }
        else
        {
            oxygenText.color = Color.white;
        }
        
        oxygen -= oxygenConsumed * Time.deltaTime;
        if (oxygen <= 0)
        {
            animator.Play(gameOverID);
            oxygen = 0;
        }
        if (grabAction.WasPressedThisFrame() || Mouse.current.leftButton.wasPressedThisFrame)
        {
            GrabCell();
        }
        if (turnCell.WasPerformedThisFrame() || Mouse.current.rightButton.wasPressedThisFrame)
        {
            CreateGuardianCell();
        }
    }
    private void FixedUpdate()
    {
        if (oxygen > 0)
        {
            Movement();
        }
             
    }
    public void GrabCell()
    {
        
            if (!isGrabbing)
            {
                if (cellTograb != null)
                {
                    if (cellTograb.CompareTag("Cell"))
                    {
                        CellMain cell = cellTograb.GetComponent<CellMain>();
                        if (!cell.isGrabbed)
                        {
                            cell.isGrabbed = true;
                            isGrabbing = true;
                            cell.grabPoint = grabPoint;
                            cellGrabbed = cellTograb.gameObject;
                            audioSource.PlayOneShot(cellPickUp);
                        }
                    }
                    if (cellTograb.CompareTag("GuardianCell"))
                    {
                        GuardianCell cell = cellTograb.GetComponent<GuardianCell>();
                        if (!cell.isGrabbed)
                        {
                            cell.isGrabbed = true;
                            cell.grabPoint = grabPoint;
                            isGrabbing = true;
                            cellGrabbed = cellTograb.gameObject;
                            audioSource.PlayOneShot(cellPickUp);
                        }
                    }
                }
                
            }
            else
            {
                LetGoCell();
            }
        
       
        animator.SetBool(carryID, isGrabbing);
       
    }
    public void FootSteps()
    {
        int footstep = Random.Range(0, footSteps.Length);
        audioSource.PlayOneShot(footSteps[footstep]);
    }
    private void LetGoCell()
    {       
        if (cellGrabbed != null)
        {
            if (cellGrabbed.CompareTag("Cell"))
            {
                CellMain cell = cellGrabbed.GetComponent<CellMain>();
                cell.isGrabbed = false;
                cell.StartMovement();
                cellGrabbed = null;
                isGrabbing = false;
                audioSource.PlayOneShot(cellLetGo);
                return;
            }
            if (cellGrabbed.CompareTag("GuardianCell"))
            {
                GuardianCell guardianCell = cellGrabbed.GetComponent<GuardianCell>();
                guardianCell.isGrabbed = false;
                guardianCell.StartMovement();
                cellGrabbed = null;
                isGrabbing = false;
                audioSource.PlayOneShot(cellLetGo);
            }
        }
       
       
    }

    public void CreateGuardianCell()
    {
        if (isGrabbing && !guardianCreated)
        {
            
                Destroy(cellGrabbed);
                cellGrabbed = null;
                isGrabbing = false;
                GameObject cell = Instantiate(guardianCell, grabPoint.position, Quaternion.identity);
                cell.GetComponent<GuardianCell>().grabPoint = grabPoint;
                Instantiate(guardianParticles, grabPoint.position, Quaternion.identity);
                guardianCreated = true;
                audioSource.PlayOneShot(createGuardian);
            
        }
    }
   
    private void Movement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector2 joystickinput = new Vector2(joyStick.Horizontal, joyStick.Vertical);
        if (joystickinput.x != 0 || joystickinput.y != 0)
        {
            rb.velocity = joystickinput * movementSpeed;
        }
        else
        {
            rb.velocity = input * movementSpeed;
        }
        if (rb.velocity.x != 0)
        {
            if (rb.velocity.x < 0)
            {
                facingLeft = true;
                transform.localScale = new Vector2(-1, 1);

            }
            if (rb.velocity.x > 0)
            {
                facingLeft = false;
                transform.localScale = new Vector2(1, 1);

            }
            animator.SetBool(walkID, true);
        }
       
       

        if (rb.velocity.y == 0 && rb.velocity.x == 0)
        {
            animator.SetBool(walkID, false);
        }
        else
        {
            animator.SetBool(walkID, true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(grabPoint.position, grabRadius);
    }
}
