using System.Collections.Generic;

namespace Gempoll.Visualization
{
    public static class Helper
    {
        private static Dictionary<int, int> itemIdSpriteIndexMap;

        public static DoorType GetDoorType(int n)
        {
            int doorType = n - Graph.DOOR_YELLOW;
            return (DoorType) doorType;
        }

        public static int GetItemSpriteIndex(int itemId)
        {
            if (itemIdSpriteIndexMap == null)
                itemIdSpriteIndexMap = new Dictionary<int, int>
                {
                    [Graph.YELLOW_KEY] = 0,
                    [Graph.BLUE_KEY] = 1,
                    [Graph.RED_KEY] = 2,
                    [Graph.GREEN_KEY] = 3,
                    [Graph.SWORD] = 4,
                    [Graph.SHIELD] = 5,
                    [Graph.BLUE_JEWEL] = 6,
                    [Graph.RED_JEWEL] = 7,
                    [Graph.GREEN_JEWEL] = 8,
                    [Graph.RED_POTION] = 10,
                    [Graph.BLUE_POTION] = 11,
                    [Graph.YELLOW_POTION] = 15,
                    [Graph.GREEN_POTION] = 19
                };
            int index;
            if (!itemIdSpriteIndexMap.TryGetValue(itemId, out index))
                index = 0;
            return index;
        }

        public static int GetMonsterSpriteIndex(int monsterId)
        {
            int index = (monsterId - Graph.MONSTER_BOUND) * 2;
            if (monsterId == Graph.BOSS_INDEX)
                index = 112;
            return index;
        }
    }
}
