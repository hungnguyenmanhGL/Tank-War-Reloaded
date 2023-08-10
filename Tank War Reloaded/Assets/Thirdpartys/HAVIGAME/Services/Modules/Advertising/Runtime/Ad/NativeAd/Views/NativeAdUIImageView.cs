using UnityEngine;
using UnityEngine.UI;

namespace HAVIGAME.Services.Advertisings {
    public class NativeAdUIImageView : NativeAdTextureElementView {
        [SerializeField] private Image imgContent;

        public override GameObject RegisterGameObject => imgContent.gameObject;

        public override void UpdateElement() {
            if (HasElement && Element.HasData) {
                Texture2D texture = Element.GetData();
                if (texture != null) {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
                    imgContent.sprite = sprite;
                    imgContent.preserveAspect = true;
                }
                else {
                    Hide();
                }
            }
            else {
                Hide();
            }
        }
    }
}
