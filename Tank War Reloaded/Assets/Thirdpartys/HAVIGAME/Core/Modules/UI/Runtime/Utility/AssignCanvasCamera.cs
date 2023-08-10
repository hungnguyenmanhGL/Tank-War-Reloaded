using UnityEngine;

namespace HAVIGAME.UI {
    [AddComponentMenu("HAVIGAME/UI/Assign Canvas Camera")]
    [RequireComponent(typeof(Canvas))]
    public class AssignCanvasCamera : MonoBehaviour {

        private void Start() {
            Assign();
        }

        public void Assign() {
            Canvas canvas = GetComponent<Canvas>();

            if (canvas) {
                canvas.worldCamera = Camera.main;
            }
        }
    }
}
