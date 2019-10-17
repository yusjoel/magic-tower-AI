using System.Collections.Generic;
using System.Text;

namespace Gempoll
{
    /// <summary>
    ///     节点
    /// </summary>
    public class Node
    {
        /// <summary>
        ///     包含的门列表
        /// </summary>
        public readonly List<int> Doors;

        /// <summary>
        ///     起始节点所在楼层
        /// </summary>
        public readonly int Floor;

        /// <summary>
        ///     包含的怪物列表
        /// </summary>
        public readonly List<Monster> Monsters;

        /// <summary>
        ///     起始节点所在位置X坐标
        /// </summary>
        public readonly int X;

        /// <summary>
        ///     起始节点所在位置Y坐标
        /// </summary>
        public readonly int Y;

        /// <summary>
        ///     英雄
        /// </summary>
        public Hero Hero;

        /// <summary>
        ///     节点的ID
        /// </summary>
        public int Id;

        /// <summary>
        ///     包含的道具, null表示没有
        /// </summary>
        public Item Item;

        // HashSet本身是无序的, 不同的实现会造成遍历时的结果不同
        // JDK的不同都会造成遍历结果不同, 更何况是Java和C#的区别
        // 所以在测试的时候不应该依赖HashSet的顺序
        // 但是PriorityQueue的操作是完全依赖HashSet遍历的顺序的
        // 最终会造成中间步骤不一致, 或者最优解有多个的情况下, 选择的解不一致
        // 所以这里改成LinkedHashSet, 遍历结果会和加入顺序保持一致

        /// <summary>
        ///     邻接表记录所有相邻节点
        /// </summary>
        public LinkedHashSet<Node> LinkedNodes;

        /// <summary>
        ///     合并掉的节点
        /// </summary>
        public List<Node> MergedNodes;

        /// <summary>
        ///     起始节点的物件编号(如道具编号, 怪物编号)
        /// </summary>
        public int ObjectId;

        public Node(int objectId, int floor, int x, int y)
        {
            ObjectId = objectId;
            Floor = floor;
            X = x;
            Y = y;
            Hero = null;
            Item = null;
            Monsters = new List<Monster>();
            Doors = new List<int>();
            LinkedNodes = new LinkedHashSet<Node>();
            MergedNodes = new List<Node>();
        }

        /// <summary>
        ///     连接新的节点
        /// </summary>
        /// <param name="another"></param>
        public void Link(Node another)
        {
            LinkedNodes.Add(another);
        }

        public override int GetHashCode()
        {
            return 1000000 * Floor + 1000 * X + Y;
        }

        /// <summary>
        ///     评估函数
        /// </summary>
        /// <returns></returns>
        public int GetScore()
        {
            return Hero?.GetScore() ?? 0;
        }

        /// <summary>
        ///     用于简化图结构的合并节点
        /// </summary>
        /// <param name="another"></param>
        public void Merge(Node another)
        {
            // 合并节点中的物件
            // 可以合并的节点只会出现两边Item都不为空和都为空两种情况
            Item?.Merge(another.Item);
            Doors.AddRange(another.Doors);
            Monsters.AddRange(another.Monsters);

            // 处理连接关系
            foreach (var to in another.LinkedNodes)
            {
                if (to != this)
                {
                    LinkedNodes.Add(to);
                    to.Link(this);
                }
                to.LinkedNodes.Remove(another);
            }

            // 加入到已合并节点列表
            MergedNodes.Add(another);
        }

        /// <summary>
        ///     访问节点
        /// </summary>
        /// <param name="another"></param>
        /// <param name="visitedNodes"></param>
        /// <returns></returns>
        public Node TryVisit(Node another, bool[] visitedNodes)
        {
            // 先克隆当前节点
            var node = new Node(ObjectId, another.Floor, another.X, another.Y).SetHero(new Hero(Hero));
            node.LinkedNodes = new LinkedHashSet<Node>(LinkedNodes);

            // 获取目标节点道具
            if (another.Item != null)
                node.Hero.GetItem(another.Item);

            // 开门扣钥匙
            foreach (int door in another.Doors)
            {
                if (door == 1) node.Hero.YellowKeyCount--;
                if (door == 2) node.Hero.BlueKeyCount--;
                if (door == 3) node.Hero.RedKeyCount--;
            }

            // 杀怪
            foreach (var monster in another.Monsters)
                node.Hero.HitPoint -= Utility.GetDamage(node.Hero, monster);

            // 检查有效性
            if (!node.Hero.IsValid())
                return null;

            // 有效的话进行连接
            foreach (var to in another.LinkedNodes)
                if (!visitedNodes[to.Id])
                    node.Link(to);

            node.LinkedNodes.Remove(another);

            // 加钱
            foreach (var monster in another.Monsters)
                node.Hero.Money += monster.Money;

            // 设访问
            visitedNodes[another.Id] = true;

            return node;
        }

        /// <summary>
        ///     模拟java的list.toString()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
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

        public Node SetDoor(int door)
        {
            Doors.Add(door);
            return this;
        }

        public Node SetHero(Hero hero)
        {
            Hero = hero;
            return this;
        }

        public void SetId(int id)
        {
            Id = id;
        }

        public Node SetItem(Item item)
        {
            Item = item;
            return this;
        }

        public Node SetMonster(Monster monster)
        {
            Monsters.Add(monster);
            return this;
        }

        /// <summary>
        ///     是否可以直接吃掉
        /// </summary>
        /// <param name="hero"></param>
        /// <returns></returns>
        public bool ShouldEat(Hero hero)
        {
            // 道具, 总是吃掉
            if (Item != null) return true;

            // hero == null 相当于Graph.ShouldEat设置为false
            if (hero == null) return false;

            // 有门, 不吃
            if (Doors.Count > 0) return false;

            // 没有怪物, 那么就是英雄节点, 不吃
            if (Monsters.Count == 0) return false;

            // 所有的怪物, 如果有伤害, 不吃
            foreach (var monster in Monsters)
                if (Utility.GetDamage(hero, monster) != 0)
                    return false;

            // 反之吃掉
            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (Id != 0) builder.Append("Id=").Append(Id).Append(": ");
            builder.Append($"({Floor},{X},{Y})");
            if (Hero != null) builder.Append(" -- Hero: ").Append(Hero);
            if (Item != null) builder.Append(" -- Items: ").Append(Item);
            if (Doors.Count > 0) builder.Append(" -- Doors: ").Append(Serialize(Doors));
            if (Monsters.Count > 0) builder.Append(" -- Monsters: ").Append(Serialize(Monsters));
            builder.Append(" -- ").Append(LinkedNodes.Count).Append(" Linked ");
            foreach (var node in LinkedNodes)
                builder.Append(node.Id).Append(",");
            return builder.ToString();
        }
    }
}
