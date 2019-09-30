using System.Collections.Generic;

namespace Gempoll
{
    public class Node
    {
        private readonly List<int> doors;

        /// <summary>
        ///     楼层
        /// </summary>
        public int f;

        private Hero hero;

        /// <summary>
        ///     节点的ID
        /// </summary>
        private int id;

        private Item item;

        /// <summary>
        ///     邻接表记录所有相邻节点
        /// </summary>
        private HashSet<Node> linked;

        private readonly List<Monster> monsters;

        /// <summary>
        ///     节点的类型
        /// </summary>
        public int type;

        /// <summary>
        ///     x
        /// </summary>
        public int x;

        /// <summary>
        ///     y
        /// </summary>
        public int y;

        public Node(int type, int floor, int x, int y)
        {
            this.type = type;
            f = floor;
            this.x = x;
            this.y = y;
            hero = null;
            item = null;
            monsters = new List<Monster>();
            doors = new List<int>();
            linked = new HashSet<Node>();
        }

        public void setId(int id)
        {
            this.id = id;
        }

        public Node setHero(Hero hero)
        {
            this.hero = hero;
            return this;
        }

        public Node setItem(Item item)
        {
            this.item = item;
            return this;
        }

        public Node setDoor(int door)
        {
            doors.Add(door);
            return this;
        }

        public Node setMonster(Monster monster)
        {
            monsters.Add(monster);
            return this;
        }

        public void addNode(Node another)
        {
            linked.Add(another);
        }
    }
}
