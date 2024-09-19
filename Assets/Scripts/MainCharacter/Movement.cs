using UnityEngine;

public class MainCharacterMovement : MonoBehaviour
{
    public float walkSpeed = 2;
    public float jumpSpeed = 2;
    public float damageDealt = 1;
    public float currentHealth = 2;
    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Attack = Animator.StringToHash("attack");
    private static readonly int Die = Animator.StringToHash("die");
    private static readonly int Hurt = Animator.StringToHash("hurt");
    private static readonly int Casting = Animator.StringToHash("casting");
    private static readonly int Victory = Animator.StringToHash("victory");
    private static readonly int Idle = Animator.StringToHash("idle");
    private static readonly int Jump = Animator.StringToHash("jump");
    private Rigidbody2D _characterBody;
    private BoxCollider2D _characterBoxCollider;
    private Animator _animator;
    private float _horizontalInput;
    private GameObject _enemy;
    private void Start()
    {
        _characterBody = GetComponent<Rigidbody2D>();
        _characterBoxCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
    }
    // TODO: Must handle movement on Simulator
    private void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal"); 
        if (_horizontalInput == 0 && IsGrounded())
        {
            _animator.SetTrigger(Idle);
            _characterBody.velocity = Vector2.zero;
        }
        if (_horizontalInput != 0)
        {
            if (_horizontalInput < 0) TurnLeft(); else TurnRight();
            if (IsGrounded())
            {
                _animator.SetTrigger(Walk);
                _characterBody.velocity = _horizontalInput switch
                {
                    > 0 => new Vector2(walkSpeed, _characterBody.velocity.y),
                    < 0 => new Vector2(-walkSpeed, _characterBody.velocity.y),
                    _ => _characterBody.velocity
                };
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || !IsGrounded())
        {
            _animator.SetTrigger(Jump);
            _characterBody.velocity = new Vector2(_characterBody.velocity.x, IsGrounded() ? jumpSpeed : _characterBody.velocity.y);
        }

        // TODO: Handle attack logic
        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger(Attack);
        }
        // TODO: Handle hurt logic
        if (Input.GetKeyDown(KeyCode.H))
        {
            currentHealth -= 1;
            _animator.SetTrigger(currentHealth <= 0 ? Die : Hurt);
        }
        // TODO: Handle casting logic
        if (Input.GetKeyDown(KeyCode.C))
        {
            _animator.SetTrigger(Casting);
        }
        // TODO: Handle victory logic
        if (Input.GetKeyDown(KeyCode.V))
        {
            _animator.SetTrigger(Victory);
        }

    }
    private void TurnLeft()
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void TurnRight()
    {
        transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
    private bool IsGrounded()
    {
        var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
        return raycastHit.collider is not null;
    }
    private bool CanDealDamage()
    {
        var raycastHit = Physics2D.BoxCast(_characterBoxCollider.bounds.center, _characterBoxCollider.bounds.size, 0, new Vector2(Mathf.Sign(transform.localScale.x), 0), 1f, LayerMask.GetMask("Enemy"));
        _enemy = raycastHit.collider.gameObject;
        return _enemy is not null;
    }
}
