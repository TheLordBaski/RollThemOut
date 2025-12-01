using UnityEngine;
using System.Collections.Generic;

namespace ChronoSniper
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class BulletController : MonoBehaviour
    {
        [Header("Bullet Settings")]
        [SerializeField] private float speed = 20f;
        [SerializeField] private float maxLifetime = 30f;
        [SerializeField] private float destroyDelay = 2f;

        [Header("Visual Settings")]
        [SerializeField] private TrailRenderer trailRenderer;

        private Rigidbody rb;
        private bool isActive = true;
        private float lifetime = 0f;
        private int currentBounceIndex = 0;
        private List<BouncePoint> bouncePoints;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public void Initialize(Vector3 direction)
        {
            bouncePoints = BouncePointManager.Instance?.GetBouncePoints();
            if (bouncePoints == null)
                bouncePoints = new List<BouncePoint>();

            rb.linearVelocity = direction.normalized * speed;
            
            // Start recording for replay
            ReplayManager.Instance?.StartRecording(this);
        }

        private void Update()
        {
            if (!isActive) return;

            lifetime += Time.deltaTime;
            if (lifetime >= maxLifetime)
            {
                StopBullet();
            }

            CheckBouncePoints();
        }

        private void CheckBouncePoints()
        {
            if (currentBounceIndex >= bouncePoints.Count) return;

            BouncePoint nextBounce = bouncePoints[currentBounceIndex];
            float distanceToNext = Vector3.Distance(transform.position, nextBounce.Position);

            // When close to bounce point, redirect
            if (distanceToNext < 0.5f)
            {
                PerformBounce(nextBounce);
                currentBounceIndex++;
            }
        }

        private void PerformBounce(BouncePoint bouncePoint)
        {
            Vector3 incomingDir = rb.linearVelocity.normalized;
            Vector3 reflectedDir = Vector3.Reflect(incomingDir, bouncePoint.Normal);
            rb.linearVelocity = reflectedDir * speed;

            // Visual/audio feedback
            bouncePoint.OnBounce();
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Hit enemy
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null && !enemy.IsDead)
                {
                    enemy.Die();
                }
                return; // Don't stop, continue through enemies
            }

            // Hit ricochet surface but no bounce point set
            if (collision.gameObject.CompareTag("Ricochet"))
            {
                // Natural physics bounce
                Vector3 reflectedDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
                rb.linearVelocity = reflectedDir * speed;
            }
            else
            {
                // Hit wall or other non-ricochet surface - bullet stops
                StopBullet();
            }
        }

        private void StopBullet()
        {
            if (!isActive) return;

            isActive = false;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;

            // Stop recording
            ReplayManager.Instance?.StopRecording();

            // Notify game manager
            GameManager.Instance?.OnBulletStopped();

            Destroy(gameObject, destroyDelay);
        }
    }
}
