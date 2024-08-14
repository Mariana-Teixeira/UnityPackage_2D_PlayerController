using UnityEngine;
using UnityEngine.InputSystem;

namespace PlatformerController
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        LayerMask _platformMask;

        public PlayerData Data;
        public Vector2 Move;

        PlayerMovementStates _movementStateMachine;
        PlayerAccelerationStates _accelerationStateMachine;
        PlayerCollisions _collisions;
        public Rigidbody2D Body { get; private set; }
        public BoxCollider2D BoxCollider { get; private set; }
        public float RunInput { get; private set; }
        public bool JumpInput { get; private set; }
        public PlayerCollisions Collisions
        {
            get
            {
                return _collisions;
            }
        }
        public PlayerMovementStates Movements
        {
            get
            {
                return _movementStateMachine;
            }
        }
        public LayerMask PlatformerMask
        {
            get
            {
                return _platformMask;
            }
        }
        public bool IsGoingUp
        {
            get
            {
                return Move.y > 0;
            }
        }
        public bool IsGoingDown
        {
            get
            {
                return Move.y < 0;
            }
        }

        private void Awake()
        {
            GetComponents();
            InititializeComponents();
        }

        public void RecalculatePhysics() => Data.Initialize();
        public void SaveData() => Data.SaveFile();

        void InititializeComponents()
        {
            Data.LoadFile();
            Data.Initialize();
            _movementStateMachine = new PlayerMovementStates(this);
            _accelerationStateMachine = new PlayerAccelerationStates(this);
            _collisions = new PlayerCollisions(this);
        }

        void GetComponents()
        {
            Body = GetComponent<Rigidbody2D>();
            BoxCollider = GetComponent<BoxCollider2D>();
        }

        public void OnJumping(InputAction.CallbackContext context)
        {
            if (context.started) JumpInput = true;
            else if (context.canceled) JumpInput = false;
        }

        public void OnRunning(InputAction.CallbackContext context)
        {
            if (context.performed) RunInput = context.ReadValue<float>();
            else if (context.canceled) RunInput = 0;
        }

        private void Update()
        {
            _accelerationStateMachine.UpdateMachine();
            _movementStateMachine.MachineUpdate();

            var position = _collisions.UpdateCollisions(Body.position + Move * Time.deltaTime);
            MovePlayer(position);
        }

        void MovePlayer(Vector2 position)
        {
            Body.position = position;
        }

        private void OnDrawGizmos()
        {
            if (_movementStateMachine == null) return;

            var v1 = new Vector3(BoxCollider.bounds.min.x, BoxCollider.bounds.min.y, 0.0f);
            var v2 = new Vector3(BoxCollider.bounds.max.x, BoxCollider.bounds.min.y, 0.0f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(v1, 0.1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(v2, 0.1f);
        }
    }
}