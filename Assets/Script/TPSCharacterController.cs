using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCharacterController : MonoBehaviour
{
    private Transform characterBody;
    struct CharacterPosition
    {
        public float x;
        public float y;
        public float z;
        public float angle_y;
        public PlayerMove playerMove;
    }

    public enum PlayerMove
    {
        stop = 0,
        turn_left = 1,
        turn_right = 2,
        moveFront = 3,
        moveBack = 4,
        moveFrontLeft,
        moveFrontRight,
        moveBackLeft,
        moveBackRight
    };

    public float speed = 3.0f;
    public float speedSmoothTime = 0.1f;
    public float turnSmoothTime = 0.1f;
    private float speedSmoothVelocity;
    private float turnSmoothVelocity;
    public float jumpVelocity = 20f;
    private float currentVelocityY;
    public float rotationSpeed = 180f;
    [Range(0.01f, 1f)] public float airControlPercent;

    CharacterPosition charaPos;
    private Vector3 moveDirection;
    Animator animator;
    CharacterController characterController;
    private Camera followCam;
    public PlayerMove before_move = PlayerMove.stop;
    public PlayerMove after_move = PlayerMove.stop;
    private NetworkManager networkManager;

    public float currentSpeed =>
        new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;

    void Start()
    {
        characterBody = transform.GetChild(0).transform;
        animator = characterBody.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        followCam = Camera.main;
        StartCoroutine(CheckMove());
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0)) Rotate();
        MoveCharacter();
        if (Input.GetKey("space")) Jump();
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    private void MoveCharacter()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 move = new Vector3(0, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime);
        move = this.transform.TransformDirection(move);
        characterController.Move(speed * move);
        characterController.Move(new Vector3(0, -1, 0));
        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime, 0));

        if(moveInput.x == 1 && moveInput.y == 0)
        {
            charaPos.playerMove = PlayerMove.turn_right;
            before_move = PlayerMove.turn_right;
        }
        else if(moveInput.x == -1 && moveInput.y == 0)
        {
            charaPos.playerMove = PlayerMove.turn_left;
            before_move = PlayerMove.turn_left;
        }
        else if (moveInput.x == 0 && moveInput.y == 1)
        {
            charaPos.playerMove = PlayerMove.moveFront;
            before_move = PlayerMove.moveFront;
        }
        else if (moveInput.x == 0 && moveInput.y == -1)
        {
            charaPos.playerMove = PlayerMove.moveBack;
            before_move = PlayerMove.moveBack;
        }
        else if (moveInput.x == -1 && moveInput.y == 1)
        {
            charaPos.playerMove = PlayerMove.moveFrontLeft;
            before_move = PlayerMove.moveFrontLeft;
        }
        else if (moveInput.x == -1 && moveInput.y == -1)
        {
            charaPos.playerMove = PlayerMove.moveBackLeft;
            before_move = PlayerMove.moveBackLeft;
        }
        else if (moveInput.x == 1 && moveInput.y == 1)
        {
            charaPos.playerMove = PlayerMove.moveFrontRight;
            before_move = PlayerMove.moveFrontRight;
        }
        else if (moveInput.x == 1 && moveInput.y == -1)
        {
            charaPos.playerMove = PlayerMove.moveBackRight;
            before_move = PlayerMove.moveBackRight;
        }
        else
        {
            charaPos.playerMove = PlayerMove.stop;
            before_move = PlayerMove.stop;
        }

        bool isMove = moveInput.magnitude != 0;
        animator.SetBool("isRun", isMove);
    }

    public void Rotate()
    {
        if(followCam == null) followCam = Camera.main;
        var targetRotation = followCam.transform.eulerAngles.y; //다음은 여기고치기!!

        transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation,
                                    ref turnSmoothVelocity, turnSmoothTime);
    }

    public void Jump()
    {
        if (!characterController.isGrounded) return;
        currentVelocityY = jumpVelocity;
    }

    IEnumerator CheckMove()
    {
        while (true)
        {
            if (before_move != after_move)
            {
                charaPos.x = transform.position.x;
                charaPos.y = transform.position.y;
                charaPos.z = transform.position.z;
                charaPos.angle_y = transform.eulerAngles.y;
                //sendPacket
                var data = networkManager.WritePacketManager.WRITE_PU_C2S_MOVE(networkManager.MyNick, transform.position, charaPos.angle_y, (int)before_move);
                networkManager.SendPacket(data);
            }

            //if (before_action != after_action)
            //{
                //action
            //}
            after_move = before_move;
            //after_action = before_action;

            //if (ConnectionTimer > 1f)
            //{
            //    NetworkManager.ConnectPacket();
            //    ConnectionTimer = 0f;
            //}

            //ConnectionTimer += Time.deltaTime;

            yield return null;
        }
    }
}
