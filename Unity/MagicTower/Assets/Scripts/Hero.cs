namespace Gempoll
{
    public class Hero
    {
        private int hp;
        private int atk;
        private int def;
        private int mdef;
        private int money;
        private int yellow;
        private int blue;
        private int red;
        private int v;

        public Hero(int hp, int atk, int def, int mdef, int money, int yellow, int blue, int red, int v)
        {
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.mdef = mdef;
            this.money = money;
            this.yellow = yellow;
            this.blue = blue;
            this.red = red;
            this.v = v;
        }
    }
}