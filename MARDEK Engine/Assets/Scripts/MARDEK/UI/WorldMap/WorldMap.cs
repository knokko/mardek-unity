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

        void Open(Waypoint waypoint, MoveDirection exitDirection) {
            currentWaypoint.Leave(areaName);
            currentWaypoint = waypoint;
            currentWaypoint.Visit(exitDirection, areaName);
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
            Path bestPath = null;
            float bestScore = 0;

            foreach (var path in allPaths) {
                if (path.HasBeenDiscovered()) {
                    var pathDirection = path.DirectionFrom(currentWaypoint);
                    float score = direction.value.x * pathDirection.x + direction.value.y * pathDirection.y;
                    if (score > bestScore) {
                        bestPath = path;
                        bestScore = score;
                    }
                }
            }

            if (bestPath != null) {
                currentWaypoint.Leave(areaName);
                nextWaypoint = bestPath.GetOther(currentWaypoint);
                startMoveTime = Time.time;
                stopMoveTime = startMoveTime + bestPath.GetDistance() / 100;
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
