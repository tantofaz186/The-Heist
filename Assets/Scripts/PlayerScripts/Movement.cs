using System.Collections;
using CombatReportScripts;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(CombatReportBehaviour))]
public class Movement : NetworkBehaviour, IUseAction
{
    public PlayerStats playerStats;
    public bool pausado = false;
    public InputAction movement;

    private OwnerNetworkAnimator corpo_FSM;

    [SerializeField]
    Animator headFSM;

    private Rigidbody corpo_fisico;

    public float walkSpeed;
    public float runSpeed;
    public float crouchSpeed;
    public float crouchRun;

    public float jumpForce = 6;
    public bool running = false;
    public bool isCrouching = false;
    public float vel;

    public LayerMask Ground;
    public static Movement Instance;
    public CombatReportBehaviour playerCombatReport;

    public AudioListPlay jumpSound;
    public AudioPlay walkSound;
    public AudioPlay runSound;
    bool isPlayingRun = false;
    bool isPlayingWalk = false;

    protected void Start()
    {
        corpo_fisico = transform.GetComponent<Rigidbody>();
        corpo_fisico.isKinematic = false;
        corpo_FSM = transform.GetComponent<OwnerNetworkAnimator>();
        vel = walkSpeed;
        lastPosition = new Vector2(transform.position.x, transform.position.z);
    }

    private void FixedUpdate()
    {
        if (!pausado)
        {
            Movimentar();
        }
    }

    void Movimentar()
    {
        if (!IsOwner) return;
        if (running && isCrouching == false)
        {
            vel = runSpeed;
            corpo_FSM.Animator.SetFloat("mover", 1f);
            if (!isPlayingRun)
            {
                runSound.PlayAudioClientRpc();
                isPlayingRun = true;
                isPlayingWalk = false;
            }
        }

        if (isCrouching && running == false)
        {
            vel = crouchSpeed;
            corpo_FSM.Animator.SetFloat("mover", 0f);
            isPlayingWalk = false;
            isPlayingRun = false;
        }
        else if (isCrouching && running)
        {
            vel = crouchRun;
            corpo_FSM.Animator.SetFloat("mover", 0f);
            isPlayingWalk = false;
            isPlayingRun = false;
        }
        else if (running == false && isCrouching == false)
        {
            vel = walkSpeed;
            corpo_FSM.Animator.SetFloat("mover", 0f);
            if (!isPlayingWalk)
            {
                walkSound.PlayAudioClientRpc();
                isPlayingWalk = true;
                isPlayingRun = false;
            }
        }

        if (!isPlayingRun)
        {
            runSound.PauseAudioClientRpc();
        }

        if (!isPlayingWalk)
        {
            walkSound.PauseAudioClientRpc();
        }

        Vector3 velocity = Vector3.zero;
        Vector2 valueRead = movement.ReadValue<Vector2>();
        velocity += transform.forward * (valueRead.y * vel * Time.fixedDeltaTime);
        velocity += transform.right * (valueRead.x * vel * Time.fixedDeltaTime);


        velocity.y = (corpo_fisico.velocity.y < 0) ? corpo_fisico.velocity.y * 1.03f : corpo_fisico.velocity.y;
        corpo_fisico.velocity = velocity;
        Vector2 current2dPos = new Vector2(transform.position.x, transform.position.z);
        playerCombatReport.combatReportData.distanciaPercorrida += Vector2.Distance(current2dPos, lastPosition);
        lastPosition = current2dPos;
        if (valueRead.y != 0 || valueRead.x != 0)
        {
            corpo_FSM.Animator.SetBool("movimentando", true);
        }
        else
        {
            corpo_FSM.Animator.SetBool("movimentando", false);
        }
    }

    private Vector2 lastPosition;

    public void Jump(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        bool can_jump = Physics.CheckSphere(transform.GetChild(0).transform.position, 0.2f, Ground,
            QueryTriggerInteraction.Ignore);
        if (can_jump)
        {
            corpo_fisico.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            corpo_FSM.SetTrigger("pular");
            jumpSound.PlayAudioClientRpc();
            playerCombatReport.combatReportData.pulos++;
        }
    }

    public void freeze()
    {
        //corpo_FSM.Animator.speed = .2f;
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (isCrouching)
        {
            isCrouching = false;
            corpo_FSM.Animator.SetBool("crouch", false);
            headFSM.SetBool("crouch", false);
        }
        else
        {
            isCrouching = true;
            corpo_FSM.Animator.SetBool("crouch", true);
            headFSM.SetBool("crouch", true);
        }

        running = false;
    }

    public void Run(InputAction.CallbackContext context)
    {
        running = !running;
    }

    public void setActions()
    {
        movement = PlayerActions.Instance.PlayerInputActions.Player.Move;
        PlayerActions.Instance.PlayerInputActions.Player.Run.performed += Run;
        PlayerActions.Instance.PlayerInputActions.Player.Jump.performed += Jump;
        PlayerActions.Instance.PlayerInputActions.Player.Crouch.performed += Crouch;
    }

    public void unsetActions()
    {
        PlayerActions.Instance.PlayerInputActions.Player.Run.performed -= Run;
        PlayerActions.Instance.PlayerInputActions.Player.Jump.performed -= Jump;
        PlayerActions.Instance.PlayerInputActions.Player.Crouch.performed -= Crouch;
    }

    public void Slow()
    {
        StartCoroutine(nameof(SlowCoroutine));
    }

    public void Stun(Vector3 direction)
    {
        StartCoroutine(StunCoroutine(direction));
    }

    public IEnumerator StunCoroutine(Vector3 direction)
    {
        if (IsOwner)
        {
            pausado = true;
                    corpo_fisico.AddForce(direction, ForceMode.Impulse);
                    yield return new WaitForSeconds(.8f);
                    pausado = false;
        }
        
    }
    IEnumerator SlowCoroutine()
    {
        Debug.Log("Slowed");
        runSpeed /= 3f;
        crouchSpeed /= 3;
        crouchRun /= 3;
        walkSpeed /= 3;
        yield return new WaitForSeconds(3.5f);
        runSpeed *= 3f;
        crouchSpeed *= 3f;
        walkSpeed *= 3f;
        crouchRun *= 3f;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        Instance = this;
    }
}