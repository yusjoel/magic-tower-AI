using System;
using System.Collections.Generic;

namespace Gempoll
{
    public class State
    {
        private static readonly Comparer comparer = new Comparer();

        public static readonly int STOP_COUNT = 45;

        private readonly Graph graph;

        public readonly List<string> route;

        public int cnt;

        public Node current;

        public int Id;

        private int shopTime;

        public bool[] visited;

        public State(Graph _graph, Node node)
        {
            graph = _graph;
            current = node;
            cnt = 0;
            visited = new bool[graph.list.Count];
            visited[current.id] = true;
            route = new List<string>();
            route.Add(current.ToString());
            shopTime = -1;
            eatItem();
        }

        public State(State another)
        {
            graph = another.graph;
            current = another.current;
            visited = another.visited.Clone() as bool[];
            route = new List<string>(another.route);
            shopTime = another.shopTime;
            cnt = another.cnt;
        }

        /// <summary>
        ///     尽可能吃掉周边宝物
        /// </summary>
        public void eatItem()
        {
            bool has = true;
            while (has)
            {
                has = false;
                foreach (var node in current.linked)
                {
                    if (visited[node.id]) continue;
                    if (!node.shouldEat(graph.shouldEat ? current.hero : null)) continue;

                    has = true;
                    current = current.merge(node, visited);
                    if (node.item != null && (node.item.special & 1) != 0)
                        shopTime = Math.Max(shopTime, 0);
                    visit(node);
                    break;
                }
            }
            // Use shop
            while (graph.shop.useShop(current.hero, shopTime))
                shopTime++;
        }

        public static Comparer GetComparer()
        {
            return comparer;
        }

        public int getScore()
        {
            return current.getScore();
        }

        public long hash()
        {
            long val = 0;
            int i = 0;
            foreach (var node in graph.list)
            {
                if (node.doors.Count == 0 && node.monsters.Count == 0) continue;

                if (visited[node.id]) val |= 1L << i;
                i++;
            }
            return val;
        }

        public State merge(Node node)
        {
            if (visited[node.id] || !current.linked.Contains(node))
                return null;

            var another = current.merge(node, visited);
            if (another == null) return null;

            current = another;
            visit(node);
            route.Add(current.ToString());
            eatItem();
            cnt++;
            return this;
        }

        public bool shouldStop()
        {
            if (cnt > STOP_COUNT) return true;

            // Boss被打死？
            if (graph.bossId >= 0 && visited[graph.bossId]) return true;

            return false;
        }

        public void visit(Node node)
        {
            if (!visited[node.id] && current.linked.Remove(node))
            {
                foreach (var monster in node.monsters)
                    current.hero.money += monster.money;
                visited[node.id] = true;
            }
        }

        public class Comparer : IComparer<State>
        {
            public int Compare(State o1, State o2)
            {
                if (o1 == null)
                    throw new ArgumentNullException(nameof(o1));

                if (o2 == null)
                    throw new ArgumentNullException(nameof(o2));

                if (o1.cnt == o2.cnt) return o2.getScore() - o1.getScore();

                return o1.cnt - o2.cnt;
            }
        }
    }
}
