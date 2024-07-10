using UnityEngine;
using UnityEngine.UI;

namespace MARDEK.UI {
    public class Path : MonoBehaviour {

        [SerializeField] Image image;
        [SerializeField] Waypoint point1;
        [SerializeField] Waypoint point2;

        public bool HasBeenDiscovered() {
            return point1.HasBeenDiscovered() && point2.HasBeenDiscovered();
        }

        void Awake() {
            if (HasBeenDiscovered()) image.color = Color.white;
            else image.color = Color.clear;
        }

        void Start() {
            var position1 = point1.gameObject.transform.position;
            var position2 = point2.gameObject.transform.position;
            var delta = position2 - position1;
            gameObject.transform.position = position1 + delta / 2;
            gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(delta.y, delta.x));
            gameObject.transform.localScale = new Vector3(2 * delta.magnitude, 0.8f, 1);
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
            return (point2.gameObject.transform.position - point1.gameObject.transform.position).magnitude;
        }
    }
}
