namespace Gempoll
{
    public class Shop
    {
        private readonly int atk;

        private int def;

        private readonly int delta;

        private int hp;

        private int mdef;

        private readonly int start;

        public Shop(int start, int delta, int hp, int atk, int def, int mdef)
        {
            this.start = start;
            this.delta = delta;
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.mdef = mdef;
        }

        public int moneyNeeded(int visitTime)
        {
            return start + delta * visitTime;
        }

        public bool useShop(Hero hero, int visitTime)
        {
            if (visitTime < 0) return false;
            if (hero.money < moneyNeeded(visitTime)) return false;

            /**
             * Algorithm to choose HP/ATK/DEF/MDEF
             */

            // Add atk
            hero.money -= moneyNeeded(visitTime);
            hero.atk += getAtkPoint();
            return true;
        }

        public int getAtkPoint()
        {
            return atk;
        }
    }
}
