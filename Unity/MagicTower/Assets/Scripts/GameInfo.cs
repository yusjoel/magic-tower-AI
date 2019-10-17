using System.Collections.Generic;

namespace Gempoll
{
    /// <summary>
    ///     游戏信息
    ///     包含各层的地图, 怪物信息, 商店信息, 英雄初始信息, 道具信息等等
    /// </summary>
    public class GameInfo
    {
        /// <summary>
        ///     红宝石加的攻
        /// </summary>
        public readonly int AttackOfRedJewel;

        /// <summary>
        ///     剑加的攻
        /// </summary>
        public readonly int AttackOfSword;

        /// <summary>
        ///     层的列数
        /// </summary>
        public readonly int ColumnCount;

        /// <summary>
        ///     蓝宝石加的防
        /// </summary>
        public readonly int DefenseOfBlueJewel;

        /// <summary>
        ///     盾加的防
        /// </summary>
        public readonly int DefenseOfShield;

        /// <summary>
        ///     总层数
        /// </summary>
        public readonly int FloorCount;

        /// <summary>
        ///     地图
        /// </summary>
        public readonly int[,,] Grid;

        /// <summary>
        ///     蓝药水加的血
        /// </summary>
        public readonly int HitPointOfBluePotion;

        /// <summary>
        ///     绿药水加的血
        /// </summary>
        public readonly int HitPointOfGreenPotion;

        /// <summary>
        ///     红药水加的血
        /// </summary>
        public readonly int HitPointOfRedPotion;

        /// <summary>
        ///     黄药水加的血
        /// </summary>
        public readonly int HitPointOfYellowPotion;

        /// <summary>
        ///     绿宝石加的魔防
        /// </summary>
        public readonly int MagicDefenseOfGreenJewel;

        /// <summary>
        ///     怪物信息
        /// </summary>
        public readonly Dictionary<int, Monster> MonsterMap;

        /// <summary>
        ///     层的行数
        /// </summary>
        public readonly int RowCount;

        /// <summary>
        ///     英雄初始信息
        /// </summary>
        public Hero Hero;

        /// <summary>
        ///     英雄初始楼层
        /// </summary>
        public int HeroFloor;

        /// <summary>
        ///     英雄初始位置X坐标
        /// </summary>
        public int HeroPositionX;

        /// <summary>
        ///     英雄初始位置Y坐标
        /// </summary>
        public int HeroPositionY;

        /// <summary>
        ///     商店信息
        /// </summary>
        public Shop Shop;

        public GameInfo(Scanner scanner)
        {
            FloorCount = scanner.NextInt();
            RowCount = scanner.NextInt();
            ColumnCount = scanner.NextInt();
            Grid = new int[FloorCount, RowCount, ColumnCount];

            for (int i = 0; i < FloorCount; i++)
            for (int j = 0; j < RowCount; j++)
            for (int k = 0; k < ColumnCount; k++)
                Grid[i, j, k] = scanner.NextInt();

            // 读取道具属性
            AttackOfRedJewel = scanner.NextInt();
            DefenseOfBlueJewel = scanner.NextInt();
            MagicDefenseOfGreenJewel = scanner.NextInt();
            HitPointOfRedPotion = scanner.NextInt();
            HitPointOfBluePotion = scanner.NextInt();
            HitPointOfYellowPotion = scanner.NextInt();
            HitPointOfGreenPotion = scanner.NextInt();
            AttackOfSword = scanner.NextInt();
            DefenseOfShield = scanner.NextInt();

            // 读取怪物列表
            MonsterMap = new Dictionary<int, Monster>();
            int monsterCount = scanner.NextInt();
            for (int i = 0; i < monsterCount; i++)
            {
                int id = scanner.NextInt();
                var monster = new Monster(id, scanner.NextInt(), scanner.NextInt(), scanner.NextInt(),
                    scanner.NextInt(), scanner.NextInt());
                MonsterMap.Add(id, monster);
            }

            // 读取商店信息
            Shop = new Shop(scanner.NextInt(), scanner.NextInt(), scanner.NextInt(),
                scanner.NextInt(), scanner.NextInt(), scanner.NextInt());

            // 读取英雄初始状态和位置
            int hitPoint = scanner.NextInt();
            int attack = scanner.NextInt();
            int defense = scanner.NextInt();
            int magicDefense = scanner.NextInt();
            int money = scanner.NextInt();
            int yellowKeyCount = scanner.NextInt();
            int blueKeyCount = scanner.NextInt();
            int redKeyCount = scanner.NextInt();
            HeroFloor = scanner.NextInt();
            HeroPositionX = scanner.NextInt();
            HeroPositionY = scanner.NextInt();

            Hero = new Hero(hitPoint, attack, defense, magicDefense, money, yellowKeyCount, blueKeyCount,
                redKeyCount, 0);
        }
    }
}
