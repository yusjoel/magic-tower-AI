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
        ///     包含的怪物列表
        /// </summary>
        public readonly List<Monster> monsters;

        /// <summary>
        ///     楼层
        /// </summary>
        public int f;

        public Hero hero;

        /// <summary>
        ///     节点的ID
        /// </summary>
        public int id;

        /// <summary>
        ///     包含的道具
        /// </summary>
        public Item item;

        // HashSet本身是无序的, 不同的实现会造成遍历时的结果不同
        // JDK的不同都会造成遍历结果不同, 更何况是Java和C#的区别
        // 所以在测试的时候不应该依赖HashSet的顺序
        // 但是PriorityQueue的操作是完全依赖HashSet遍历的顺序的
        // 最终会造成中间步骤不一致, 或者最优解有多个的情况下, 选择的解不一致
        // 所以这里改成LinkedHashSet, 遍历结果会和加入顺序保持一致

        /// <summary>
        ///     邻接表记录所有相邻节点
        /// </summary>
        public LinkedHashSet<Node> linked;

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
            linked = new LinkedHashSet<Node>();
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
            builder.Append(" -- ").Append(linked.Count).Append(" Linked ");
            foreach (var node in linked)
                builder.Append(node.id).Append(",");
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

        public Node merge(Node another, bool[] visited)
        {
            var node = new Node(type, another.f, another.x, another.y).setHero(new Hero(hero));
            node.linked = new LinkedHashSet<Node>(linked);

            // get item
            if (another.item != null)
                node.hero.getItem(another.item);

            // open doors...
            foreach (int v in another.doors)
            {
                if (v == 1) node.hero.yellow--;
                if (v == 2) node.hero.blue--;
                if (v == 3) node.hero.red--;
            }

            // beat monsters...
            foreach (var monster in another.monsters) node.hero.hp -= Util.getDamage(node.hero, monster);

            if (!node.hero.isValid())
                return null;

            // merge linked nodes
            foreach (var to in another.linked)
                if (!visited[to.id])
                    node.addNode(to);

            return node;
        }

        public bool shouldEat(Hero hero)
        {
            if (item != null) return true;
            if (hero == null) return false;

            // 无伤怪物直接干掉
            if (doors.Count > 0) return false;
            if (monsters.Count == 0) return false;

            foreach (var monster in monsters)
                if (Util.getDamage(hero, monster) != 0)
                    return false;

            return true;
        }

        public int getScore()
        {
            return hero == null ? 0 : hero.getScore();
        }

        public override int GetHashCode()
        {
            return 1000000 * f + 1000 * x + y;
        }
    }
}
