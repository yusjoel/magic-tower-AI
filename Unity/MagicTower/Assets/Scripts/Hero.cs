namespace Gempoll
{
    /// <summary>
    ///     英雄
    /// </summary>
    public class Hero
    {
        public int Attack;

        public int BlueKeyCount;

        public int Defense;

        public int GreenKeyCount;

        public int HitPoint;

        public int MagicDefense;

        public int Money;

        public int RedKeyCount;

        public int YellowKeyCount;

        public Hero(int hitPoint, int attack, int defense, int magicDefense, int money, int yellowKeyCount,
            int blueKeyCount, int redKeyCount, int greenKeyCount)
        {
            HitPoint = hitPoint;
            Attack = attack;
            Defense = defense;
            MagicDefense = magicDefense;
            Money = money;
            YellowKeyCount = yellowKeyCount;
            BlueKeyCount = blueKeyCount;
            RedKeyCount = redKeyCount;
            GreenKeyCount = greenKeyCount;
        }

        public Hero(Hero another) : this(another.HitPoint, another.Attack, another.Defense, another.MagicDefense,
            another.Money, another.YellowKeyCount, another.BlueKeyCount, another.RedKeyCount, another.GreenKeyCount)
        {
        }

        public void GetItem(Item item)
        {
            if (item == null) return;

            HitPoint += item.HitPoint;
            Attack += item.Attack;
            Defense += item.Defense;
            MagicDefense += item.MagicDefense;
            YellowKeyCount += item.YellowKeyCount;
            BlueKeyCount += item.BlueKeyCount;
            RedKeyCount += item.RedKeyCount;
            GreenKeyCount += item.GreenKeyCount;
        }

        public int GetScore()
        {
            return HitPoint;
        }

        /// <summary>
        /// 验证合法性
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            // 打不过的怪
            // 开不了的门
            return HitPoint > 0 && YellowKeyCount >= 0 && BlueKeyCount >= 0 && RedKeyCount >= 0 && GreenKeyCount >= 0;
        }

        public override string ToString()
        {
            return
                $"({HitPoint},{Attack},{Defense},{MagicDefense},{Money},{YellowKeyCount},{BlueKeyCount},{RedKeyCount},{GreenKeyCount})";
        }
    }
}
