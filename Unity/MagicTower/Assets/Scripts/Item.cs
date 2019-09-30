namespace Gempoll
{
    public class Item
    {
        private int atk;

        private int blue;

        private int def;

        private int green;

        private int hp;

        private int mdef;

        private int red;

        private int special;

        private int yellow;

        public Item setHp(int hp)
        {
            this.hp = hp;
            return this;
        }

        public Item setAtk(int atk)
        {
            this.atk = atk;
            return this;
        }

        public Item setDef(int def)
        {
            this.def = def;
            return this;
        }

        public Item setMdef(int mdef)
        {
            this.mdef = mdef;
            return this;
        }

        public Item setYellow(int yellow)
        {
            this.yellow = yellow;
            return this;
        }

        public Item setBlue(int blue)
        {
            this.blue = blue;
            return this;
        }

        public Item setRed(int red)
        {
            this.red = red;
            return this;
        }

        public Item setGreen(int green)
        {
            this.green = green;
            return this;
        }

        public Item setSpecial(int special)
        {
            this.special = special;
            return this;
        }
    }
}
