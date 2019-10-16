namespace Gempoll
{
    /// <summary>
    ///     怪物
    /// </summary>
    public class Monster
    {
        public readonly int Attack;

        public readonly int Defense;

        public readonly int HitPoint;

        public readonly int Id;

        public readonly int Money;

        public readonly int Special;

        public Monster(int id, int hitPoint, int attack, int defense, int money, int special)
        {
            Id = id;
            HitPoint = hitPoint;
            Attack = attack;
            Defense = defense;
            Money = money;
            Special = special;
        }

        public override string ToString()
        {
            return $"({Id},{HitPoint},{Attack},{Defense},{Money},{Special})";
        }
    }
}
