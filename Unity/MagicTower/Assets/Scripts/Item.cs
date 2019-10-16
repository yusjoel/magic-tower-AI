using System.Text;

namespace Gempoll
{
    /// <summary>
    ///     道具
    /// </summary>
    public class Item
    {
        public int Attack;

        public int BlueKeyCount;

        public int Defense;

        public int GreenKeyCount;

        public int HitPoint;

        public int MagicDefense;

        public int RedKeyCount;

        // TODO: 以后删除
        /// <summary>
        ///     特殊节点
        ///     <para>0x01: 访问了商店</para>
        /// </summary>
        public int Special;

        public int YellowKeyCount;

        public void Merge(Item another)
        {
            if (another == null) return;

            HitPoint += another.HitPoint;
            Attack += another.Attack;
            Defense += another.Defense;
            MagicDefense += another.MagicDefense;
            YellowKeyCount += another.YellowKeyCount;
            BlueKeyCount += another.BlueKeyCount;
            RedKeyCount += another.RedKeyCount;
            GreenKeyCount += another.GreenKeyCount;
            Special |= another.Special;
        }

        public Item SetAttack(int attack)
        {
            Attack = attack;
            return this;
        }

        public Item SetBlueKeyCount(int blueKeyCount)
        {
            BlueKeyCount = blueKeyCount;
            return this;
        }

        public Item SetDefense(int defense)
        {
            Defense = defense;
            return this;
        }

        public Item SetGreenKeyCount(int greenKeyCount)
        {
            GreenKeyCount = greenKeyCount;
            return this;
        }

        public Item SetHitPoint(int hitPoint)
        {
            HitPoint = hitPoint;
            return this;
        }

        public Item SetMagicDefense(int magicDefense)
        {
            MagicDefense = magicDefense;
            return this;
        }

        public Item SetRedKeyCount(int redKeyCount)
        {
            RedKeyCount = redKeyCount;
            return this;
        }

        public Item SetSpecial(int special)
        {
            Special = special;
            return this;
        }

        public Item SetYellowKeyCount(int yellowKeyCount)
        {
            YellowKeyCount = yellowKeyCount;
            return this;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (HitPoint > 0) builder.Append("hp+").Append(HitPoint).Append(';');
            if (Attack > 0) builder.Append("atk+").Append(Attack).Append(';');
            if (Defense > 0) builder.Append("def+").Append(Defense).Append(';');
            if (MagicDefense > 0) builder.Append("mdef+").Append(MagicDefense).Append(';');
            if (YellowKeyCount > 0) builder.Append("yellow+").Append(YellowKeyCount).Append(';');
            if (BlueKeyCount > 0) builder.Append("blue+").Append(BlueKeyCount).Append(';');
            if (RedKeyCount > 0) builder.Append("red+").Append(RedKeyCount).Append(';');
            if (GreenKeyCount > 0) builder.Append("green+").Append(GreenKeyCount).Append(';');
            return builder.ToString();
        }
    }
}
