using UnityEngine;
using System.Collections.Generic;

namespace ChronoSniper
{
    [System.Serializable]
    public class BulletFrame
    {
        public Vector3 position;
        public Quaternion rotation;
        public float timestamp;

        public BulletFrame(Vector3 pos, Quaternion rot, float time)
        {
            position = pos;
            rotation = rot;
            timestamp = time;
        }
    }

    public class ReplayManager : MonoBehaviour
    {
        public static ReplayManager Instance { get; private set; }

        [Header("Replay Settings")]
        [SerializeField] private float replaySpeed = 1f;
        [SerializeField] private Camera replayCamera;
        [SerializeField] private float cameraDistance = 5f;
        [SerializeField] private float cameraHeight = 2f;
        [SerializeField] private float cameraSmoothSpeed = 5f;

        private List<BulletFrame> recordedFrames = new List<BulletFrame>();
        private bool isRecording = false;
        private bool isReplaying = false;
        private float recordingStartTime = 0f;
        private Transform bulletTransform;
        private int currentFrameIndex = 0;
        private Camera mainCamera;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            mainCamera = Camera.main;
            if (replayCamera != null)
            {
                replayCamera.gameObject.SetActive(false);
            }
        }

        public void StartRecording(BulletController bullet)
        {
            recordedFrames.Clear();
            isRecording = true;
            bulletTransform = bullet.transform;
            recordingStartTime = Time.time;
        }

        public void StopRecording()
        {
            isRecording = false;
        }

        private void Update()
        {
            if (isRecording && bulletTransform != null)
            {
                RecordFrame();
            }

            if (isReplaying)
            {
                PlaybackFrame();
            }
        }

        private void RecordFrame()
        {
            float timestamp = Time.time - recordingStartTime;
            BulletFrame frame = new BulletFrame(
                bulletTransform.position,
                bulletTransform.rotation,
                timestamp
            );
            recordedFrames.Add(frame);
        }

        public void StartReplay()
        {
            if (recordedFrames.Count == 0) return;

            isReplaying = true;
            currentFrameIndex = 0;

            // Switch to replay camera
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(false);
            }
            if (replayCamera != null)
            {
                replayCamera.gameObject.SetActive(true);
            }

            // Notify UI
            UIManager.Instance?.ShowReplayUI();
        }

        private void PlaybackFrame()
        {
            if (currentFrameIndex >= recordedFrames.Count)
            {
                EndReplay();
                return;
            }

            BulletFrame currentFrame = recordedFrames[currentFrameIndex];

            // Position replay camera to follow bullet
            if (replayCamera != null)
            {
                Vector3 targetPosition = currentFrame.position - (currentFrame.rotation * Vector3.forward * cameraDistance);
                targetPosition += Vector3.up * cameraHeight;
                
                replayCamera.transform.position = Vector3.Lerp(
                    replayCamera.transform.position,
                    targetPosition,
                    Time.deltaTime * cameraSmoothSpeed
                );

                replayCamera.transform.LookAt(currentFrame.position);
            }

            // Advance playback based on replay speed
            currentFrameIndex += Mathf.Max(1, Mathf.RoundToInt(replaySpeed));
        }

        private void EndReplay()
        {
            isReplaying = false;

            // Switch back to main camera
            if (replayCamera != null)
            {
                replayCamera.gameObject.SetActive(false);
            }
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(true);
            }

            // Show win screen
            GameManager.Instance?.ChangeState(GameState.Win);
        }

        public bool HasRecording()
        {
            return recordedFrames.Count > 0;
        }
    }
}
