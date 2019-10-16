using System;
using System.Collections.Generic;

namespace Gempoll
{
    /// <summary>
    ///     状态
    /// </summary>
    public class State
    {
        private static readonly Comparer comparer = new Comparer();

        public static readonly int StopCount = 45;

        /// <summary>
        ///     路径
        /// </summary>
        public readonly List<string> Route;

        private readonly Graph graph;

        public int Count;

        /// <summary>
        ///     英雄节点
        /// </summary>
        public Node Current;

        public int Id;

        /// <summary>
        ///     访问过的节点
        /// </summary>
        public bool[] VisitedNodes;

        /// <summary>
        ///     使用商店的次数, -1代表还没有访问过商店
        /// </summary>
        private int shopTime;

        public State(Graph graph, Node node)
        {
            this.graph = graph;
            Current = node;
            Count = 0;
            VisitedNodes = new bool[this.graph.Nodes.Count];
            VisitedNodes[Current.Id] = true;
            Route = new List<string> { Current.ToString() };
            shopTime = -1;
            EatItem();
        }

        public State(State another)
        {
            graph = another.graph;
            Current = another.Current;
            VisitedNodes = another.VisitedNodes.Clone() as bool[];
            Route = new List<string>(another.Route);
            shopTime = another.shopTime;
            Count = another.Count;
        }

        /// <summary>
        ///     尽可能吃掉周边宝物
        /// </summary>
        public void EatItem()
        {
            bool has = true;
            while (has)
            {
                has = false;
                foreach (var node in Current.LinkedNodes)
                {
                    if (VisitedNodes[node.Id]) continue;
                    if (!node.ShouldEat(graph.ShouldEat ? Current.Hero : null)) continue;

                    has = true;
                    Current = Current.Merge(node, VisitedNodes);
                    if (node.Item != null && (node.Item.Special & 1) != 0)
                        shopTime = Math.Max(shopTime, 0);
                    Visit(node);
                    break;
                }
            }
            // Use shop
            while (graph.Shop.Buy(Current.Hero, shopTime))
                shopTime++;
        }

        /// <summary>
        ///     比较器
        /// </summary>
        /// <returns></returns>
        public static Comparer GetComparer()
        {
            return comparer;
        }

        /// <summary>
        ///     评估函数
        /// </summary>
        /// <returns></returns>
        public int GetScore()
        {
            return Current.GetScore();
        }

        public long Hash()
        {
            // TODO: 点数无法超过long的位数, 也就是64个
            long val = 0;
            int i = 0;
            foreach (var node in graph.Nodes)
            {
                if (node.Doors.Count == 0 && node.Monsters.Count == 0) continue;

                if (VisitedNodes[node.Id]) val |= 1L << i;
                i++;
            }
            return val;
        }

        /// <summary>
        ///     合并节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public State Merge(Node node)
        {
            if (VisitedNodes[node.Id] || !Current.LinkedNodes.Contains(node))
                return null;

            var another = Current.Merge(node, VisitedNodes);
            if (another == null) return null;

            Current = another;
            Visit(node);
            Route.Add(Current.ToString());
            EatItem();
            Count++;
            return this;
        }

        /// <summary>
        ///     是否要结束遍历
        /// </summary>
        /// <returns></returns>
        public bool ShouldStop()
        {
            // TODO: 为什么不能访问节点数超过45?
            if (Count > StopCount)
                return true;

            // Boss被打死？
            if (graph.NodeIdOfBoss >= 0 && VisitedNodes[graph.NodeIdOfBoss])
                return true;

            return false;
        }

        // TODO: 放到Node.Merge()中, 并且可以把Merge改成Visit
        public void Visit(Node node)
        {
            if (!VisitedNodes[node.Id] && Current.LinkedNodes.Remove(node))
            {
                foreach (var monster in node.Monsters)
                    Current.Hero.Money += monster.Money;
                VisitedNodes[node.Id] = true;
            }
        }

        /// <summary>
        ///     状态的比较类, 用于<see cref="Medallion.Collections.PriorityQueue{T}" />
        /// </summary>
        public class Comparer : IComparer<State>
        {
            public int Compare(State o1, State o2)
            {
                if (o1 == null)
                    throw new ArgumentNullException(nameof(o1));

                if (o2 == null)
                    throw new ArgumentNullException(nameof(o2));

                if (o1.Count == o2.Count) return o2.GetScore() - o1.GetScore();

                return o1.Count - o2.Count;
            }
        }
    }
}
