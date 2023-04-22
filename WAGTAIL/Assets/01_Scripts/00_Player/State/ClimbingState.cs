using System.Linq;
using UnityEngine;

public class ClimbingState : State
{
    private float _playerSpeed;
    private bool _isGrounded;
    private Vector3 _currentVelocity;
    private Vector3 _cVelocity;
    private GameObject _ladder;
    private float _ladderHeight;
    private float _playerHeight;
    
    private bool _isTop;
    private static readonly int Climbing = Animator.StringToHash("climbing");
    private static readonly int Move = Animator.StringToHash("move");

    public ClimbingState(Player _player, StateMachine _stateMachine) : base(_player, _stateMachine)
    {
        player = _player;
        stateMachine = _stateMachine;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void Enter()
    {
        base.Enter();

        _isTop = false;
        // �ִϸ��̼� ����
        player.animator.SetTrigger(Climbing);
        gravityVelocity.y = 0;
        _ladder = player.currentInteractable;
        _ladderHeight = CalcHeight(_ladder);
        _playerHeight = 0f;

        // TODO : 올라가는 속도 Inspector에 빼야함
        _playerSpeed = player.playerSpeed;
        _isGrounded = player.controller.isGrounded;
        //gravityValue = player.gravityValue;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void HandleInput()
    {
        base.HandleInput();

        input = climbingAction.ReadValue<Vector2>();
        velocity = new Vector3(0, input.y, 0);

        if (!(_playerHeight >= _ladderHeight) || _isTop) return;
        _isTop = true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (_isGrounded && input.y < 0) 
        {
            player.isClimbing = false;
            stateMachine.ChangeState(player.idle);
        }

        if (!_isTop || !(input.y > 0)) return;
        
        player.isClimbing = false;
        stateMachine.ChangeState(player.idle);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        _isGrounded = player.controller.isGrounded;

        if (_isGrounded && gravityVelocity.y < 0)
        {
            gravityVelocity.y = 0f;
        }

        _currentVelocity = Vector3.SmoothDamp(_currentVelocity, velocity, ref _cVelocity, player.velocityDampTime);
        player.controller.Move(_currentVelocity * (Time.deltaTime * _playerSpeed) + gravityVelocity * Time.deltaTime);
        _playerHeight = player.transform.position.y;

    }

    public override void Exit()
    {
        base.Exit();

        gravityVelocity.y = 0f;
        player.playerVelocity = new Vector3(input.x, 0, input.y);
        _currentVelocity = new Vector3(0, 0, 0);
        player.animator.SetTrigger(Move);
    }

    private float CalcHeight(GameObject calcTarget)
    {
        MeshFilter mf = calcTarget.GetComponent<MeshFilter>();

        Vector3[] vertices = mf.mesh.vertices;

        return vertices.Select(vertical => _ladder.transform.TransformPoint(vertical)).Select(pos => pos.y).Prepend(0f).Max();
    }
}
