using System.Collections.Generic;
using System.Text;

namespace Gempoll
{
    public class Node
    {
        /// <summary>
        ///     包含的门列表
        /// </summary>
        public readonly List<int> doors;

        /// <summary>
        ///     邻接表记录所有相邻节点
        /// </summary>
        public readonly HashSet<Node> linked;

        /// <summary>
        ///     包含的怪物列表
        /// </summary>
        public readonly List<Monster> monsters;

        /// <summary>
        ///     楼层
        /// </summary>
        public int f;

        private Hero hero;

        /// <summary>
        ///     节点的ID
        /// </summary>
        private int id;

        /// <summary>
        ///     包含的道具
        /// </summary>
        public Item item;

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

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (id != 0) builder.Append("Id=").Append(id).Append(": ");
            builder.Append($"({f},{x},{y})");
            if (hero != null) builder.Append(" -- Hero: ").Append(hero);
            if (item != null) builder.Append(" -- Items: ").Append(item);
            if (doors.Count > 0) builder.Append(" -- Doors: ").Append(Serialize(doors));
            if (monsters.Count > 0) builder.Append(" -- Monsters: ").Append(Serialize(monsters));
            builder.Append(" -- ").Append(linked.Count).Append(" Linked");
            return builder.ToString();
        }

        private string Serialize<T>(List<T> list)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                stringBuilder.Append(item);
                if (i < list.Count - 1)
                    stringBuilder.Append(", ");
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        public void merge(Node another)
        {
            // merge items...
            if (item != null) item.merge(another.item);

            // merge doors...
            doors.AddRange(another.doors);
            // merge monsters...
            monsters.AddRange(another.monsters);

            // merge nodes
            foreach (var to in another.linked)
            {
                if (to != this)
                {
                    linked.Add(to);
                    to.addNode(this);
                }
                to.linked.Remove(another);
            }
        }
    }
}
