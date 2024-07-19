using MARDEK.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MARDEK.UI {
    public class WorldMap : MonoBehaviour {

        static Vector3 OFFSET = new(0, 20, 0);

        [SerializeField] CharacterImageAnimation player;
        [SerializeField] Image areaName;

        [SerializeField] MoveDirection up;
        [SerializeField] MoveDirection down;
        [SerializeField] MoveDirection left;
        [SerializeField] MoveDirection right;

        Path[] allPaths;
        Waypoint[] allWaypoints;

        Waypoint currentWaypoint;
        Waypoint nextWaypoint;
        float startMoveTime;
        float stopMoveTime;


        void Start() {
            allPaths = GetComponentsInChildren<Path>();
            allWaypoints = GetComponentsInChildren<Waypoint>();

            if (currentWaypoint == null) {
                currentWaypoint = allWaypoints[0];
                currentWaypoint.Visit(up, areaName);
            }
        }

        Waypoint FindBestWaypointForDirection(Waypoint sourceWaypoint, MoveDirection direction) {
            Waypoint bestDestination = null;
            float bestScore = 0.0f;
            float secondScore = bestScore;

            foreach (var path in allPaths) {
                var destination = path.GetOther(sourceWaypoint);
                if (destination == null) continue;

                var pathDirection = path.DirectionFrom(sourceWaypoint);
                if (pathDirection != Vector2.zero) pathDirection.Normalize();
                float score = direction.value.x * pathDirection.x + direction.value.y * pathDirection.y;
                if (score > bestScore) {
                    secondScore = bestScore;
                    bestDestination = destination;
                    bestScore = score;
                }
            }

            // Don't choose a winner if the difference is too small to see
            if (bestScore - secondScore < 0.05f) return null;

            return bestDestination;
        }

        void OnValidate() {
            if (!gameObject.activeInHierarchy) return;

            allPaths = GetComponentsInChildren<Path>();
            allWaypoints = GetComponentsInChildren<Waypoint>();

            bool allWaypointsHaveSamePosition = true;
            if (allWaypoints.Length > 0) {
                var firstPosition = allWaypoints[0].gameObject.transform.position;
                foreach (var waypoint in allWaypoints) {
                    if (waypoint.gameObject.transform.position != firstPosition) allWaypointsHaveSamePosition = false;
                }
            }

            // When the waypoint positions haven't been initialized yet, the rest of the code is useless
            if (allWaypointsHaveSamePosition) return;

            foreach (var sourceWaypoint in allWaypoints) {
                sourceWaypoint.upWaypoint = FindBestWaypointForDirection(sourceWaypoint, up);
                sourceWaypoint.rightWaypoint = FindBestWaypointForDirection(sourceWaypoint, right);
                sourceWaypoint.downWaypoint = FindBestWaypointForDirection(sourceWaypoint, down);
                sourceWaypoint.leftWaypoint = FindBestWaypointForDirection(sourceWaypoint, left);
            }

            foreach (var path in allPaths) {
                path.CheckReachable();
            }
        }

        void Update() {
            if (nextWaypoint == null) {
                player.transform.position = currentWaypoint.transform.position + OFFSET;
                player.movementSpriteAnimationDirection = down;
            } else {
                float progress = Mathf.Min(1, (Time.time - startMoveTime) / (stopMoveTime - startMoveTime));
                player.transform.position = OFFSET + (1 - progress) * currentWaypoint.transform.position + progress * nextWaypoint.transform.position;
                player.movementSpriteAnimationDirection = ToMoveDirection(nextWaypoint.transform.position - currentWaypoint.transform.position);
            }
        }

        void FixedUpdate() {
            if (nextWaypoint != null && Time.fixedTime >= stopMoveTime) {
                nextWaypoint.Visit(ToMoveDirection(currentWaypoint.transform.position - nextWaypoint.transform.position), areaName);
                currentWaypoint = nextWaypoint;
                nextWaypoint = null;
            }
        }

        public void EnterCurrentMap()
        {
            if (nextWaypoint == null) currentWaypoint.EnterRegion();
        }
        
        public void OnMovementInput(InputAction.CallbackContext ctx)
        {
            var direction = ToMoveDirection(ctx.ReadValue<Vector2>());
            Waypoint destination = null;
            if (direction == up) destination = currentWaypoint.upWaypoint;
            if (direction == right) destination = currentWaypoint.rightWaypoint;
            if (direction == down) destination = currentWaypoint.downWaypoint;
            if (direction == left) destination = currentWaypoint.leftWaypoint;

            if (destination != null && destination.HasBeenDiscovered()) {
                currentWaypoint.Leave(areaName);
                nextWaypoint = destination;
                startMoveTime = Time.time;
                stopMoveTime = startMoveTime + Path.GetDistance(currentWaypoint, nextWaypoint) / 100;
            }
        }

        private MoveDirection ToMoveDirection(Vector2 direction) {
            if (direction == Vector2.zero) return null;

            if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x)) {
                if (direction.y > 0) return up;
                else return down;
            } else {
                if (direction.x > 0) return right;
                else return left;
            }
        }
    }
}
