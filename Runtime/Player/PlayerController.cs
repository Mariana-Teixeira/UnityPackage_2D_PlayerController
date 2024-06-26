using UnityEngine;

namespace marianateixeira.PlayerController
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        LayerMask _platformMask;

        public PlayerData Data;
        public Vector2 Move;

        PlayerMovementStates _movementStateMachine;
        PlayerAccelerationStates _accelerationStateMachine;

        public PlayerCollisions Collisions { get; private set; }

        public Rigidbody2D Body { get; private set; }
        public BoxCollider2D BoxCollider { get; private set; }

        PlayerControls _playerInputMap;
        public float RunInput { get; private set; }
        public bool JumpInput { get; private set; }
        public LayerMask PlatformerMask
        {
            get { return _platformMask; }
        }

        private void Awake()
        {
            GetComponents();
            InititializeComponents();
        }

        void Start()
        {
            EnableInputs();
        }

        public void RecalculatePhysics() => Data.Initialize();
        public void SaveData() => Data.SaveFile();

        void InititializeComponents()
        {
            Data.LoadFile();
            Data.Initialize();
            _movementStateMachine = new PlayerMovementStates(this);
            _accelerationStateMachine = new PlayerAccelerationStates(this);
            Collisions = new PlayerCollisions(this);
        }

        void GetComponents()
        {
            Body = GetComponent<Rigidbody2D>();
            BoxCollider = GetComponent<BoxCollider2D>();
        }

        void EnableInputs()
        {
            _playerInputMap = new PlayerControls();
            _playerInputMap.Enable();
            
            _playerInputMap.Player.Jumping.started += context => JumpInput = true;
            _playerInputMap.Player.Jumping.canceled += context => JumpInput = false;

            _playerInputMap.Player.Running.performed += context => RunInput = context.ReadValue<float>();
            _playerInputMap.Player.Running.canceled += context => RunInput = 0;
        }

        private void FixedUpdate()
        {
            _accelerationStateMachine.UpdateMachine();
            _movementStateMachine.UpdateMachine();

            Vector2 position = Body.position + Move * Time.fixedDeltaTime;
            Collisions.UpdateCollisions(ref position);

            MovePlayer(position);
        }

        void MovePlayer(Vector2 position)
        {
            Body.MovePosition(position);
        }
    }
}