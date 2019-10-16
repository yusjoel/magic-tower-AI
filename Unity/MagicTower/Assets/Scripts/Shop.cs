namespace Gempoll
{
    public class Shop
    {
        private readonly int atk;

        private readonly int delta;

        private readonly int start;

        private readonly int def;

        private readonly int hp;

        private readonly int mdef;

        public Shop(int start, int delta, int hp, int atk, int def, int mdef)
        {
            this.start = start;
            this.delta = delta;
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.mdef = mdef;
        }

        public int getAtkPoint()
        {
            return atk;
        }

        public int getDefPoint()
        {
            return def;
        }

        public int getHpPoint()
        {
            return hp;
        }

        public int getMDefPoint()
        {
            return mdef;
        }

        public int moneyNeeded(int visitTime)
        {
            return start + delta * visitTime;
        }

        public bool useShop(Hero hero, int visitTime)
        {
            if (visitTime < 0) return false;
            if (hero.Money < moneyNeeded(visitTime)) return false;

            /**
             * Algorithm to choose HP/ATK/DEF/MDEF
             */

            // Add Attack
            hero.Money -= moneyNeeded(visitTime);
            hero.Attack += getAtkPoint();
            return true;
        }
    }
}
