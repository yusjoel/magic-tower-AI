namespace Gempoll
{
    public class Utility
    {
        public static int GetDamage(Hero hero, Monster monster)
        {
            return GetDamage(hero.Attack, hero.Defense, hero.MagicDefense, monster.HitPoint, monster.Attack,
                monster.Defense, monster.Special);
        }

        /// <summary>
        ///     计算伤害
        /// </summary>
        /// <param name="heroAttack"></param>
        /// <param name="heroDefense"></param>
        /// <param name="heroMagicDefense"></param>
        /// <param name="monsterHitPoint"></param>
        /// <param name="monsterAttack"></param>
        /// <param name="monsterDefense"></param>
        /// <param name="monsterSpecial"></param>
        /// <returns></returns>
        private static int GetDamage(int heroAttack, int heroDefense, int heroMagicDefense,
            int monsterHitPoint, int monsterAttack, int monsterDefense, int monsterSpecial)
        {
            // 魔攻
            if (monsterSpecial == 2) heroDefense = 0;
            // 坚固
            if (monsterSpecial == 3 && monsterDefense < heroAttack - 1) monsterDefense = heroAttack - 1;
            // 模仿
            if (monsterSpecial == 10)
            {
                monsterAttack = heroAttack;
                monsterDefense = heroDefense;
            }
            if (heroAttack <= monsterDefense) return 999999999;

            // 怪物1击造成的伤害
            int damagePerHit = monsterAttack - heroDefense;
            if (damagePerHit < 0) damagePerHit = 0;
            // 2连击 & 3连击

            if (monsterSpecial == 4) damagePerHit *= 2;
            if (monsterSpecial == 5) damagePerHit *= 3;
            if (monsterSpecial == 6) damagePerHit *= 4;
            // 反击
            if (monsterSpecial == 8) damagePerHit += (int) (0.1 * heroAttack);

            // 先攻
            int damage = monsterSpecial == 1 ? damagePerHit : 0;
            // 破甲
            if (monsterSpecial == 7) damage = (int) (0.9 * heroDefense);
            // 净化
            if (monsterSpecial == 9) damage = 3 * heroMagicDefense;

            int totalDamage = damage + (monsterHitPoint - 1) / (heroAttack - monsterDefense) * damagePerHit;
            totalDamage -= heroMagicDefense;

            // 魔防不回血
            return totalDamage <= 0 ? 0 : totalDamage;
        }
    }
}
