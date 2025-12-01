using UnityEngine;
using System.Collections.Generic;

namespace ChronoSniper
{
    public class BouncePointManager : MonoBehaviour
    {
        public static BouncePointManager Instance { get; private set; }

        private List<BouncePoint> activeBouncePoints = new List<BouncePoint>();

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

        public void SetBouncePoints(List<BouncePoint> points)
        {
            activeBouncePoints = new List<BouncePoint>(points);
        }

        public List<BouncePoint> GetBouncePoints()
        {
            return new List<BouncePoint>(activeBouncePoints);
        }

        public void ClearBouncePoints()
        {
            foreach (BouncePoint bp in activeBouncePoints)
            {
                if (bp != null)
                {
                    Destroy(bp.gameObject);
                }
            }
            activeBouncePoints.Clear();
        }
    }
}
