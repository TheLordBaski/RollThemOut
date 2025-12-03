using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace ChronoSniper
{
    /// <summary>
    /// Player controller that handles shooting mechanics and bounce point placement
    /// Now works with FirstPersonController for movement and camera
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private FirstPersonController firstPersonController;

        [Header("Shooting Settings")]
        [SerializeField] private float maxRayDistance = 100f;
        [SerializeField] private LayerMask aimLayerMask;

        [Header("Bounce Point Settings")]
        [SerializeField] private GameObject bouncePointPrefab;
        [SerializeField] private int maxBouncePoints = 10;
        [SerializeField] private Color trajectoryColor = Color.cyan;
        [SerializeField] private float trajectoryLineWidth = 0.05f;

        private List<BouncePoint> bouncePoints = new List<BouncePoint>();
        private LineRenderer trajectoryLine;
        private PlayerInput playerInput;
        private InputAction attackAction;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                attackAction = playerInput.actions["Attack"];
            }

            if (firstPersonController == null)
            {
                firstPersonController = GetComponent<FirstPersonController>();
            }
        }

        private void Start()
        {
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

            HandleInput();
            UpdateTrajectoryVisualization();
        }


        private void HandleInput()
        {
            // Place bounce point with left click or Attack action
            if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                (attackAction != null && attackAction.WasPressedThisFrame()))
            {
                TryPlaceBouncePoint();
            }

            // Remove last bounce point with right click
            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                RemoveLastBouncePoint();
            }

            // Fire bullet with Space (Jump action in Planning state acts as Fire)
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && 
                !GameManager.Instance.BulletFired)
            {
                FireBullet();
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
