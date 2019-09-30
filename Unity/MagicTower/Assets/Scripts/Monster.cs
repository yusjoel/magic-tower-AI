namespace Gempoll
{
    public class Monster
    {
        private readonly int atk;

        private readonly int def;

        private readonly int hp;

        private readonly int id;

        private readonly int money;

        private readonly int special;

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
