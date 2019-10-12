using UnityEngine;
using UnityEngine.UI;

namespace Gempoll.Visualization
{
    /// <summary>
    ///     门
    ///     有开门动画
    ///     开门后消失
    /// </summary>
    public class Door : MonoBehaviour
    {
        public Sprite[] BlueDoorSprites;

        public Image Image;

        public Sprite[] RedDoorSprites;

        public Sprite[] YellowDoorSprites;

        private DoorType doorType;

        public void SetDoorType(DoorType doorType)
        {
            this.doorType = doorType;
            SetImage();
        }

        private void SetImage()
        {
            if (doorType == DoorType.Yellow)
                Image.sprite = YellowDoorSprites[0];
            else if (doorType == DoorType.Blue)
                Image.sprite = BlueDoorSprites[0];
            else if (doorType == DoorType.Red)
                Image.sprite = RedDoorSprites[0];
        }
    }
}
