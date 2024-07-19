using System;
using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI {
    public class Path : MonoBehaviour {

        public static float GetDistance(Waypoint point1, Waypoint point2) {
            return (point2.gameObject.transform.position - point1.gameObject.transform.position).magnitude;
        }

        [SerializeField] Image image;
        [SerializeField] Waypoint point1;
        [SerializeField] Waypoint point2;

        public bool HasBeenDiscovered() {
            if (point1 == null || point2 == null) return false;
            return point1.HasBeenDiscovered() && point2.HasBeenDiscovered();
        }

        void Awake() {
            if (HasBeenDiscovered()) image.color = Color.white;
            else image.color = Color.clear;
        }

        bool IsReachable(Waypoint point, Waypoint other) {
            return point.upWaypoint == other || (point.rightWaypoint == other) || 
                point.downWaypoint == other || point.leftWaypoint == other;
        }

        public void CheckReachable() {
            if (point1 == null || point2 == null) return;
            if (!IsReachable(point1, point2)) Debug.Log(point1 + " can't reach " + point2);
            if (!IsReachable(point2, point1)) Debug.Log(point2 + " can't reach " + point1);
        }

        void OnValidate() {
            if (point1 == null || point2 == null) {
                if (gameObject.activeInHierarchy) Debug.Log("Path " + name + " is missing at least 1 waypoint");
                return;
            }

            var position1 = point1.gameObject.transform.position;
            var position2 = point2.gameObject.transform.position;
            var delta = position2 - position1;
            gameObject.transform.position = position1 + delta / 2;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x));

            float extraScale;
            var localScale = gameObject.transform.localScale;
            if (Math.Abs(localScale.x) > Math.Abs(localScale.y)) {
                extraScale = localScale.x / gameObject.transform.lossyScale.x;
            } else {
                extraScale = localScale.y / gameObject.transform.lossyScale.y;
            }
            
            if (float.IsFinite(extraScale)) {
                gameObject.transform.localScale = new Vector3(extraScale * delta.magnitude, 0.8f, 1);
            }
        }

        public Vector2 DirectionFrom(Waypoint waypoint) {
            if (waypoint == point1) return point2.gameObject.transform.position - point1.gameObject.transform.position;
            if (waypoint == point2) return point1.gameObject.transform.position - point2.gameObject.transform.position;
            return Vector2.zero;
        }

        public Waypoint GetOther(Waypoint waypoint) {
            if (waypoint == point1) return point2;
            if (waypoint == point2) return point1;
            return null;
        }

        public float GetDistance() {
            return GetDistance(point1, point2);
        }
    }
}
