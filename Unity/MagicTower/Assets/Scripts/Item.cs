using System.Text;

namespace Gempoll
{
    public class Item
    {
        public int atk;

        public int blue;

        public int def;

        public int green;

        public int hp;

        public int mdef;

        public int red;

        public int special;

        public int yellow;

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

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (hp > 0) builder.Append("hp+").Append(hp).Append(';');
            if (atk > 0) builder.Append("atk+").Append(atk).Append(';');
            if (def > 0) builder.Append("def+").Append(def).Append(';');
            if (mdef > 0) builder.Append("mdef+").Append(mdef).Append(';');
            if (yellow > 0) builder.Append("yellow+").Append(yellow).Append(';');
            if (blue > 0) builder.Append("blue+").Append(blue).Append(';');
            if (red > 0) builder.Append("red+").Append(red).Append(';');
            if (green > 0) builder.Append("green+").Append(green).Append(';');
            return builder.ToString();
        }

        public void merge(Item another)
        {
            if (another == null) return;

            hp += another.hp;
            atk += another.atk;
            def += another.def;
            mdef += another.mdef;
            yellow += another.yellow;
            blue += another.blue;
            red += another.red;
            green += another.green;
            special += another.special;
        }
    }
}
