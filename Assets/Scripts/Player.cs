using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private VariableJoystick variableJoystick;
    [SerializeField] private Rigidbody player;
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float jumpHeight = 6f;
    [SerializeField] private float runSpeed = 4f;

    public int score = 0;

    private const float JUMP_THRESHOLD = 0.3f;
    private const int COIN_LAYER = 7;
    private bool isGrounded;
    private float superJumpTimer;
    private bool canSuperJump;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundLayers, QueryTriggerInteraction.Ignore);
        CalculateSuperJumpTiming();
    }

    private void CalculateSuperJumpTiming()
    {
        if (isGrounded)
        {
            superJumpTimer = 0f;
            canSuperJump = false;
        }
        else if (superJumpTimer > 0 && JumpIsPressed() && !canSuperJump)
            superJumpTimer -= Time.deltaTime;
        else if (superJumpTimer > 0 && !JumpIsPressed() && !canSuperJump)
        {
            superJumpTimer = 0f;
            canSuperJump = true;
        }
    }

    // Update called every physics update
    void FixedUpdate()
    {
        Jump();
        SuperJump();
        Move(); 
    }

    private void Move()
    {
        player.velocity = new Vector3(variableJoystick.Horizontal * runSpeed, player.velocity.y, 0);
    }

    private void Jump()
    {
        if (isGrounded && JumpIsPressed())
        {
            superJumpTimer = 1f;
            player.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
        }

    }

    private void SuperJump()
    {
        if (isGrounded || !canSuperJump || !JumpIsPressed()) 
            return;

        player.velocity = new Vector3(player.velocity.x, jumpHeight, 0); // more precise than AddForce while jumping
        canSuperJump = false;
    }

    private bool JumpIsPressed()
    {
        return variableJoystick.Vertical > JUMP_THRESHOLD;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == COIN_LAYER)
        {
            Destroy(other.gameObject);
            score++;
        }
    }
}
