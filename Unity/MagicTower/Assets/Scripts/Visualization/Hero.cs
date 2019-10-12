using UnityEngine;
using UnityEngine.UI;

namespace Gempoll.Visualization
{
    /// <summary>
    ///     英雄
    ///     有4个方向, 移动的时候有4帧动画
    /// </summary>
    public class Hero : MonoBehaviour
    {
        /// <summary>
        ///     英雄的朝向
        /// </summary>
        public enum Direction
        {
            Down,

            Left,

            Right,

            Up
        }

        public Sprite[] DownSprites;

        public Image Image;

        public Sprite[] LeftSprites;

        public Sprite[] RightSprites;

        public Sprite[] UpSprites;

        private Direction direction;

        public void SetDirection(Direction direction)
        {
            this.direction = direction;
            SetImage();
        }

        private void SetImage()
        {
            if (direction == Direction.Down)
                Image.sprite = DownSprites[0];
            else if (direction == Direction.Left)
                Image.sprite = LeftSprites[0];
            else if (direction == Direction.Right)
                Image.sprite = RightSprites[0];
            else if (direction == Direction.Up)
                Image.sprite = UpSprites[0];
        }
    }
}
