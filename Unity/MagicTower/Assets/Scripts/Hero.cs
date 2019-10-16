﻿namespace Gempoll
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

        public bool IsValid()
        {
            // TODO: 为什么钥匙没了就结束了?
            return HitPoint > 0 && YellowKeyCount >= 0 && BlueKeyCount >= 0 && RedKeyCount >= 0 && GreenKeyCount >= 0;
        }

        public override string ToString()
        {
            return
                $"({HitPoint},{Attack},{Defense},{MagicDefense},{Money},{YellowKeyCount},{BlueKeyCount},{RedKeyCount},{GreenKeyCount})";
        }
    }
}
