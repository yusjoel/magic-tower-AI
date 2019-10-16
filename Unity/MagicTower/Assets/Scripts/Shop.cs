namespace Gempoll
{
    /// <summary>
    ///     商店
    ///     <para>一个简单的商店模型, 价格随访问次数变化, 出售若干点数的攻, 防, 血, 魔防</para>
    /// </summary>
    public class Shop
    {
        private readonly int attack;

        private readonly int defense;

        private readonly int delta;

        private readonly int hitPoint;

        private readonly int magicDefense;

        private readonly int start;

        public Shop(int start, int delta, int hitPoint, int attack, int defense, int magicDefense)
        {
            this.start = start;
            this.delta = delta;
            this.hitPoint = hitPoint;
            this.attack = attack;
            this.defense = defense;
            this.magicDefense = magicDefense;
        }

        public int GetAttack()
        {
            return attack;
        }

        public int GetDefense()
        {
            return defense;
        }

        public int GetHitPoint()
        {
            return hitPoint;
        }

        public int GetMagicDefense()
        {
            return magicDefense;
        }

        public int GetMoneyNeeded(int visitTime)
        {
            return start + delta * visitTime;
        }

        public bool Buy(Hero hero, int visitTime)
        {
            if (visitTime < 0) return false;
            if (hero.Money < GetMoneyNeeded(visitTime)) return false;

            /**
             * Algorithm to choose HP/ATK/DEF/MDEF
             */

            // Add Attack
            hero.Money -= GetMoneyNeeded(visitTime);
            hero.Attack += GetAttack();
            return true;
        }
    }
}
