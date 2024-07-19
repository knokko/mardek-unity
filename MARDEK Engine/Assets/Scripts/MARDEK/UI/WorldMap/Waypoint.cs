using MARDEK.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MARDEK.UI {
    public class Waypoint : MonoBehaviour {

        [SerializeField] Image currentIcon;
        [SerializeField] SceneReference scene;
        [SerializeField] Sprite nameSprite;

        [SerializeField] Sprite defaultIcon;
        [SerializeField] Sprite activeIcon;
        [SerializeField] Sprite disabledIcon;

        [SerializeField] bool discovered;

        [Header("These 4 are automatically chosen, so you should NOT change them!")]
        public Waypoint upWaypoint;
        public Waypoint rightWaypoint;
        public Waypoint downWaypoint;
        public Waypoint leftWaypoint;
        
        MoveDirection approachDirection;

        public bool HasBeenDiscovered() {
            return discovered;
        }

        public void Discover() {
            discovered = true;
        }

        public void Visit(MoveDirection approachDirection, Image areaName) {
            currentIcon.sprite = activeIcon;
            this.approachDirection = approachDirection;
            areaName.color = Color.white;
            areaName.sprite = nameSprite;
        }

        public void Leave(Image areaName) {
            currentIcon.sprite = defaultIcon;
            areaName.color = Color.clear;
        }

        public void EnterRegion() {
            Debug.Log("Enter scene " + scene + " from " + approachDirection);
            SceneManager.LoadScene(scene);
        }

        void Awake() {
            if (discovered) {
                currentIcon.sprite = defaultIcon;
                currentIcon.color = Color.white;
            } else currentIcon.color = Color.clear;
        }
    }
}
