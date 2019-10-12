using UnityEngine;
using UnityEngine.UI;

namespace Gempoll.Visualization
{
    /// <summary>
    ///     怪物
    ///     包含两个精灵, 来回切换
    ///     包含两个数字显示位置, 一般显示临界和损失血量
    /// </summary>
    public class Monster : MonoBehaviour
    {
        public Image Image;

        public Text LeftBottomText;

        public Text LeftTopText;

        public Sprite[] Sprites;

        public void SetMonsterId(int monsterId)
        {
            Image.sprite = Sprites[Helper.GetMonsterSpriteIndex(monsterId)];
            LeftBottomText.text = "";
            LeftTopText.text = "";
        }
    }
}
