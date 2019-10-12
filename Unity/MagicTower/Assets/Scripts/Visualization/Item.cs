using UnityEngine;
using UnityEngine.UI;

namespace Gempoll.Visualization
{
    /// <summary>
    /// 道具
    /// 无动画e
    /// </summary>
    public class Item : MonoBehaviour
    {
        public Image Image;

        public Sprite[] Sprites;

        public void SetItem(int itemId)
        {
            int index = Helper.GetItemSpriteIndex(itemId);
            Image.sprite = Sprites[index];
        }
    }
}
