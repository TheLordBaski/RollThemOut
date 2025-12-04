using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DragesStudio
{
    public class CameraController : MonoBehaviour
    {
        public Transform player;

        void Update() {
            transform.position = player.transform.position;
        }
    }
}