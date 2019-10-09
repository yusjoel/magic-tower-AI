namespace Gempoll
{
    public class Hero
    {
        public int atk;

        public int blue;

        public int def;

        public int green;

        public int hp;

        public int mdef;

        public int money;

        public int red;

        public int yellow;

        public Hero(int hp, int atk, int def, int mdef, int money, int yellow, int blue, int red, int green)
        {
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.mdef = mdef;
            this.money = money;
            this.yellow = yellow;
            this.blue = blue;
            this.red = red;
            this.green = green;
        }

        public Hero(Hero another) : this(another.hp, another.atk, another.def, another.mdef, another.money,
            another.yellow, another.blue, another.red, another.green)
        {
        }

        public void getItem(Item item)
        {
            if (item == null) return;

            hp += item.hp;
            atk += item.atk;
            def += item.def;
            mdef += item.mdef;
            yellow += item.yellow;
            blue += item.blue;
            red += item.red;
            green += item.green;
        }

        public int getScore()
        {
            return hp;
        }

        public bool isValid()
        {
            return hp > 0 && yellow >= 0 && blue >= 0 && red >= 0 && green >= 0;
        }

        public override string ToString()
        {
            return $"({hp},{atk},{def},{mdef},{money},{yellow},{blue},{red},{green})";
        }
    }
}
