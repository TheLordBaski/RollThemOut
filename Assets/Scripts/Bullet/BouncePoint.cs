using UnityEngine;

namespace ChronoSniper
{
    public class BouncePoint : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Color normalColor = Color.cyan;
        [SerializeField] private Color bounceColor = Color.yellow;
        [SerializeField] private float bounceEffectDuration = 0.3f;

        private Vector3 position;
        private Vector3 normal;
        private Renderer bounceRenderer;
        private float bounceTimer = 0f;

        public Vector3 Position => position;
        public Vector3 Normal => normal;

        private void Awake()
        {
            bounceRenderer = GetComponent<Renderer>();
            if (bounceRenderer != null)
            {
                bounceRenderer.material.color = normalColor;
            }
        }

        public void Initialize(Vector3 pos, Vector3 surfaceNormal)
        {
            position = pos;
            normal = surfaceNormal.normalized;
            transform.position = pos;

            // Orient the marker to face the normal
            if (normal != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(normal);
            }
        }

        public void OnBounce()
        {
            bounceTimer = bounceEffectDuration;
            if (bounceRenderer != null)
            {
                bounceRenderer.material.color = bounceColor;
            }
        }

        private void Update()
        {
            if (bounceTimer > 0f)
            {
                bounceTimer -= Time.deltaTime;
                if (bounceTimer <= 0f && bounceRenderer != null)
                {
                    bounceRenderer.material.color = normalColor;
                }
            }
        }
    }
}
