using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace marianateixeira.PlatformerController
{
    public class PlayerCollisions
    {
        PlayerController _controller;
        public RaycastHit2D[] AllCollisions
        {
            get
            {
                RaycastHit2D[] hits = Physics2D.BoxCastAll(
                        _controller.Body.position,
                        _controller.BoxCollider.size,
                        0.0f,
                        _controller.Move.normalized,
                        _controller.Move.magnitude * Time.deltaTime,
                        _controller.PlatformerMask);

                return hits;
            }
        }

        public bool IsItColliding
        {
            get
            {
                return AllCollisions.Length > 0;
            }
        }

        public bool IsBottomColliding
        {
            get
            {
                return IsSideColliding(Vector2.down);
            }
        }

        public bool IsTopColliding
        {
            get
            {
                return IsSideColliding(Vector2.up);
            }
        }

        public bool IsLeftColliding
        {
            get
            {
                return IsSideColliding(Vector2.left);
            }
        }

        public bool IsRightColliding
        {
            get
            {
                return IsSideColliding(Vector2.right);
            }
        }

        public bool AreLeftOrRightColliding
        {
            get
            {
                return IsLeftColliding || IsRightColliding;
            }
        }

        public bool IsGrounded
        {
            get
            {
                var leftHit = Physics2D.Raycast(
                    new Vector2(_controller.BoxCollider.bounds.min.x, _controller.BoxCollider.bounds.min.y),
                    Vector2.down,
                    0.05f,
                    _controller.PlatformerMask);

                var rightHit = Physics2D.Raycast(
                    new Vector2(_controller.BoxCollider.bounds.max.x, _controller.BoxCollider.bounds.min.y),
                    Vector2.down,
                    0.05f,
                    _controller.PlatformerMask);

                if (leftHit.collider != null || rightHit.collider != null) return true;
                else return false;
            }
        }

        public PlayerCollisions(PlayerController controller)
        {
            this._controller = controller;
        }

        public Vector2 UpdateCollisions(Vector2 position)
        {
            if (IsItColliding == true)
            {
                position = MovePositionToCompensateCollision(position);
            }

            return position;
        }

        bool IsSideColliding(Vector2 side)
        {
            // TODO: Check is Vector 2 side is normalized.

            // The normal is the opposite force to the movement direction.
            side *= -1.0f;
            foreach (var col in AllCollisions)
            {
                if (col.normal == side)
                {
                    return true;
                }
            }

            return false;
        }

        Vector2 MovePositionToCompensateCollision(Vector2 playerPosition)
        {
            float boundsMinY = playerPosition.y - (_controller.BoxCollider.size.y / 2);
            float boundsMaxY = playerPosition.y + (_controller.BoxCollider.size.y / 2);
            float boundsMaxX = playerPosition.x + (_controller.BoxCollider.size.x / 2);
            float boundsMinX = playerPosition.x - (_controller.BoxCollider.size.x / 2);
            float difference;

            foreach (var col in AllCollisions)
            {
                if (col.normal == Vector2.up)
                {
                    _controller.Move = Vector2.zero;
                    difference = boundsMinY - col.point.y;
                }
                else if (col.normal == Vector2.down)
                {
                    _controller.Move.y = 0.0f;
                    difference = boundsMaxY - col.point.y;
                }
                else if (col.normal == Vector2.left)
                {
                    _controller.Move.x = 0.0f;
                    difference = boundsMaxX - col.point.x;
                }
                else // Vector2.right
                {
                    _controller.Move.x = 0.0f;
                    difference = boundsMinX - col.point.x;
                }

                difference = Mathf.Abs(difference);
                playerPosition += col.normal * difference;
            }

            return playerPosition;
        }
    }
}