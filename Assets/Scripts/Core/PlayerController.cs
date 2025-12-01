using UnityEngine;
using System.Collections.Generic;

namespace ChronoSniper
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float maxVerticalAngle = 80f;
        [SerializeField] private Transform cameraTransform;

        [Header("Shooting Settings")]
        [SerializeField] private float maxRayDistance = 100f;
        [SerializeField] private LayerMask aimLayerMask;

        [Header("Bounce Point Settings")]
        [SerializeField] private GameObject bouncePointPrefab;
        [SerializeField] private int maxBouncePoints = 10;
        [SerializeField] private Color trajectoryColor = Color.cyan;
        [SerializeField] private float trajectoryLineWidth = 0.05f;

        private float verticalRotation = 0f;
        private List<BouncePoint> bouncePoints = new List<BouncePoint>();
        private LineRenderer trajectoryLine;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SetupTrajectoryLine();
        }

        private void SetupTrajectoryLine()
        {
            GameObject lineObj = new GameObject("TrajectoryLine");
            lineObj.transform.SetParent(transform);
            trajectoryLine = lineObj.AddComponent<LineRenderer>();
            trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
            trajectoryLine.startColor = trajectoryColor;
            trajectoryLine.endColor = trajectoryColor;
            trajectoryLine.startWidth = trajectoryLineWidth;
            trajectoryLine.endWidth = trajectoryLineWidth;
            trajectoryLine.positionCount = 0;
        }

        private void Update()
        {
            if (GameManager.Instance.CurrentState != GameState.Planning) 
            {
                trajectoryLine.positionCount = 0;
                return;
            }

            HandleCameraRotation();
            HandleInput();
            UpdateTrajectoryVisualization();
        }

        private void HandleCameraRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }

        private void HandleInput()
        {
            // Place bounce point
            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceBouncePoint();
            }

            // Remove last bounce point
            if (Input.GetMouseButtonDown(1))
            {
                RemoveLastBouncePoint();
            }

            // Fire bullet
            if (Input.GetKeyDown(KeyCode.Space) && !GameManager.Instance.BulletFired)
            {
                FireBullet();
            }

            // Escape to unlock cursor
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void TryPlaceBouncePoint()
        {
            if (bouncePoints.Count >= maxBouncePoints) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, aimLayerMask))
            {
                // Only place on ricochet surfaces
                if (hit.collider.CompareTag("Ricochet"))
                {
                    GameObject bounceObj = Instantiate(bouncePointPrefab, hit.point, Quaternion.identity);
                    BouncePoint bouncePoint = bounceObj.GetComponent<BouncePoint>();
                    if (bouncePoint == null)
                    {
                        bouncePoint = bounceObj.AddComponent<BouncePoint>();
                    }
                    bouncePoint.Initialize(hit.point, hit.normal);
                    bouncePoints.Add(bouncePoint);
                }
            }
        }

        private void RemoveLastBouncePoint()
        {
            if (bouncePoints.Count > 0)
            {
                BouncePoint lastPoint = bouncePoints[bouncePoints.Count - 1];
                bouncePoints.RemoveAt(bouncePoints.Count - 1);
                Destroy(lastPoint.gameObject);
            }
        }

        private void FireBullet()
        {
            Vector3 direction = cameraTransform.forward;
            
            // Pass bounce points to bullet via GameManager
            BouncePointManager.Instance?.SetBouncePoints(bouncePoints);
            GameManager.Instance.FireBullet(direction);

            // Hide trajectory line when firing
            trajectoryLine.positionCount = 0;
        }

        private void UpdateTrajectoryVisualization()
        {
            List<Vector3> trajectoryPoints = new List<Vector3>();
            trajectoryPoints.Add(cameraTransform.position);

            Vector3 currentPos = cameraTransform.position;
            Vector3 currentDir = cameraTransform.forward;

            // Add bounce points to trajectory
            foreach (BouncePoint bp in bouncePoints)
            {
                trajectoryPoints.Add(bp.Position);
                
                // Calculate reflection direction
                Vector3 toNextPoint = (bp.Position - currentPos).normalized;
                currentDir = Vector3.Reflect(toNextPoint, bp.Normal);
                currentPos = bp.Position;
            }

            // Extend final trajectory segment
            trajectoryPoints.Add(currentPos + currentDir * 10f);

            trajectoryLine.positionCount = trajectoryPoints.Count;
            trajectoryLine.SetPositions(trajectoryPoints.ToArray());
        }

        public List<BouncePoint> GetBouncePoints()
        {
            return new List<BouncePoint>(bouncePoints);
        }
    }
}
