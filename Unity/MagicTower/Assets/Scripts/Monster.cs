namespace Gempoll
{
    public class Monster
    {
        public readonly int atk;

        public readonly int def;

        public readonly int hp;

        public readonly int id;

        public readonly int money;

        public readonly int special;

        public Monster(int id, int hp, int atk, int def, int money, int special)
        {
            this.id = id;
            this.hp = hp;
            this.atk = atk;
            this.def = def;
            this.money = money;
            this.special = special;
        }

        public override string ToString()
        {
            return $"({id},{hp},{atk},{def},{money},{special})";
        }
    }
}
