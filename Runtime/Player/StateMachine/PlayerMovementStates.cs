using UnityEngine;

namespace PlatformerController
{

    public class PlayerMovementStates
    {
        PlayerController _controller;
        public enum State { IDLE, RUN, JUMP, FALL, NULL };
        public State currentState { get; private set; }

        public PlayerMovementStates(PlayerController controller)
        {
            this._controller = controller;
            currentState = State.IDLE;
        }

        public void MachineUpdate()
        {
            TickState();

            var newState = CheckingStateConditions();
            if (newState != currentState)
            {
                ChangeState(newState);
            }
        }

        public State CheckingStateConditions()
        {
            switch (currentState)
            {
                case State.IDLE:
                    if (!_controller.Collisions.IsGrounded) return State.FALL;
                    else if (_controller.JumpInput && _controller.Data.ReadyToJump) return State.JUMP;
                    else if (_controller.Data.RunSpeed != 0) return State.RUN;
                    break;

                case State.RUN:
                    if (!_controller.Collisions.IsGrounded) return State.FALL;
                    else if (_controller.JumpInput && _controller.Data.ReadyToJump) return State.JUMP;
                    else if (_controller.Data.RunSpeed == 0 && _controller.RunInput == 0) return State.IDLE;
                    break;

                case State.JUMP:
                    if (_controller.IsGoingDown || _controller.Collisions.IsTopColliding) return State.FALL;
                    else if (_controller.Data.VariableJump && !_controller.JumpInput) return State.FALL;
                    break;

                case State.FALL:
                    if (_controller.Collisions.IsGrounded && _controller.Move.x != 0) return State.RUN;
                    else if (_controller.Collisions.IsBottomColliding) return State.IDLE;
                    break;
            }

            return currentState;
        }

        public void ChangeState(State state)
        {
            currentState = state;
            EnterState();
        }

        public void EnterState()
        {
            switch (currentState)
            {
                case State.JUMP:
                    _controller.Data.ReadyToJump = false;
                    _controller.Move.y = _controller.Data.InitialJumpVelocity;
                    break;
                case State.FALL:
                    break;
            }
        }

        public void TickState()
        {
            switch (currentState)
            {
                case State.IDLE:
                    if (!_controller.JumpInput) _controller.Data.ReadyToJump = true;
                    break;

                case State.RUN:
                    if (!_controller.JumpInput) _controller.Data.ReadyToJump = true;
                    _controller.Move.x = _controller.Data.AccelerationDirection * _controller.Data.RunSpeed;
                    break;

                case State.JUMP:
                    _controller.Move.x = _controller.Data.AccelerationDirection * _controller.Data.RunSpeed;
                    _controller.Move.y += _controller.Data.JumpGravity * Time.deltaTime;
                    break;

                case State.FALL:
                    if (!_controller.JumpInput) _controller.Data.ReadyToJump = true;
                    _controller.Move.x = _controller.Data.AccelerationDirection * _controller.Data.RunSpeed;
                    if (_controller.Move.y > _controller.Data.MaximumFallSpeed * -1.0f) _controller.Move.y += _controller.Data.FallGravity * Time.deltaTime;
                    else _controller.Move.y = _controller.Data.MaximumFallSpeed * -1.0f;
                    break;
            }
        }
    }
}
