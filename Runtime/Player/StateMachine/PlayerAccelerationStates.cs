using UnityEngine;

namespace marianateixeira.PlatformerController
{
    public class PlayerAccelerationStates
    {
        PlayerController _controller;

        public enum State { STOP, ACCELERATE, MAXSPEED, DECELETARE, NULL };
        State currentState;

        public PlayerAccelerationStates(PlayerController controller)
        {
            this._controller = controller;

            currentState = State.STOP;
        }

        public void UpdateMachine()
        {
            TickState();
            if (CheckingStateConditions() != State.NULL) ChangeState(CheckingStateConditions());
        }

        public State CheckingStateConditions()
        {
            switch (currentState)
            {
                case State.STOP:
                    if (_controller.Collisions.IsLeftColliding && _controller.RunInput == 1
                        || _controller.Collisions.IsRightColliding && _controller.RunInput == -1
                        || (!_controller.Collisions.AreLeftOrRightColliding)
                        && _controller.RunInput != 0) return State.ACCELERATE;
                    break;
                case State.ACCELERATE:
                    if (_controller.Collisions.AreLeftOrRightColliding) return State.STOP;
                    if (_controller.RunInput != _controller.Data.AccelerationDirection) return State.DECELETARE;
                    else if (_controller.Data.RunSpeed >= _controller.Data.MaxSpeed) return State.MAXSPEED;
                    break;

                case State.MAXSPEED:
                    if (_controller.Collisions.AreLeftOrRightColliding) return State.STOP;
                    else if (_controller.RunInput == 0.0f && _controller.Move.x == 0.0f) return State.STOP;
                    else if (_controller.RunInput != _controller.Data.AccelerationDirection) return State.DECELETARE;
                    break;

                case State.DECELETARE:
                    if (_controller.Collisions.AreLeftOrRightColliding) return State.STOP;
                    else if (_controller.RunInput == _controller.Data.AccelerationDirection) return State.ACCELERATE;
                    else if (_controller.RunInput == -_controller.Data.AccelerationDirection && _controller.Data.RunSpeed <= 0.05f) return State.ACCELERATE;
                    else if (_controller.RunInput == 0.0f && _controller.Data.RunSpeed <= 0.05f) return State.STOP;
                    break;
            }

            return State.NULL;
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
                case State.ACCELERATE:
                    if (_controller.RunInput != 0) _controller.Data.AccelerationDirection = _controller.RunInput;
                    break;
            }
        }

        public void TickState()
        {
            switch (currentState)
            {
                case State.STOP:
                    _controller.Data.RunSpeed = 0f;
                    break;

                case State.ACCELERATE:
                    _controller.Data.RunSpeed += _controller.Data.Acceleration * Time.deltaTime;
                    break;

                case State.MAXSPEED:
                    _controller.Data.RunSpeed = _controller.Data.MaxSpeed;
                    break;

                case State.DECELETARE:
                    _controller.Data.RunSpeed -= _controller.Data.Deceleration * Time.deltaTime;
                    break;
            }


        }
    }
}
