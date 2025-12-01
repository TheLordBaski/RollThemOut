using UnityEngine;

namespace ChronoSniper
{
    public class Enemy : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Color aliveColor = Color.red;
        [SerializeField] private Color deadColor = Color.gray;
        [SerializeField] private GameObject deathEffectPrefab;

        [Header("Death Settings")]
        [SerializeField] private float ragdollForce = 5f;

        private Renderer enemyRenderer;
        private bool isDead = false;

        public bool IsDead => isDead;

        private void Awake()
        {
            enemyRenderer = GetComponentInChildren<Renderer>();
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = aliveColor;
            }
        }

        public void Die()
        {
            if (isDead) return;

            isDead = true;

            // Visual feedback
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = deadColor;
            }

            // Spawn death effect
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            // Apply physics ragdoll effect
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.AddForce(Vector3.up * ragdollForce, ForceMode.Impulse);
            }

            // Notify game manager
            GameManager.Instance?.OnEnemyKilled();

            // Disable collider to prevent multiple hits
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}
