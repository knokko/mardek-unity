using UnityEngine;
using MARDEK.Core;
using MARDEK.CharacterSystem;
using UnityEngine.UI;

namespace MARDEK.UI
{
    public class CharacterImageAnimation : MonoBehaviour
    {
        [SerializeField] CharacterSelectable characterSelectable;
        [SerializeField] CharacterProfile fixedCharacter;
        [SerializeField] Image characterImage;
        public MoveDirection movementSpriteAnimationDirection;
        [SerializeField] Sprite disabledSprite;

        public void Update()
        {
            var characterInfo = fixedCharacter;
            if (characterInfo == null) {
                var character = characterSelectable.Character;
                if (character != null) characterInfo = character.Profile;
            }

            if (characterInfo == null)
            {
                characterImage.sprite = disabledSprite;
                return;
            }
                
            var clip = characterInfo.WalkSprites.GetClipByReference(movementSpriteAnimationDirection);
            var animRatio = Time.time % 1;
            characterImage.sprite = clip.GetSprite(animRatio);
        }
    }
}