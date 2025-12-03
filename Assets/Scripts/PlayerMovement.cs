using System;
using UnityEngine;

namespace DragesStudio
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float moveSpeed = 5f;
        
        private Rigidbody _rigidbody;
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Move(Vector2 direction)
        {
            Vector3 moveDirection = new(direction.x, 0, direction.y);
            _rigidbody.MovePosition(transform.position + moveDirection * Time.deltaTime * moveSpeed );
        }
        
        public void Look(Vector2 lookDelta)
        {
            if (lookDelta.sqrMagnitude < 0.01f) return;
            Vector3 lookDir3D = new(lookDelta.x, 0, lookDelta.y);
            Quaternion targetRotation = Quaternion.LookRotation(lookDir3D, Vector3.up);
            _rigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f));
        }
    }
}