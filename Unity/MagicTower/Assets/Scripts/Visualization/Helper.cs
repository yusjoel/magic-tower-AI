using System.Collections.Generic;

namespace Gempoll.Visualization
{
    public static class Helper
    {
        private static Dictionary<int, int> itemIdSpriteIndexMap;

        public static DoorType GetDoorType(int n)
        {
            int doorType = n - ObjectId.DOOR_YELLOW;
            return (DoorType) doorType;
        }

        public static int GetItemSpriteIndex(int itemId)
        {
            if (itemIdSpriteIndexMap == null)
                itemIdSpriteIndexMap = new Dictionary<int, int>
                {
                    [ObjectId.YELLOW_KEY] = 0,
                    [ObjectId.BLUE_KEY] = 1,
                    [ObjectId.RED_KEY] = 2,
                    [ObjectId.GREEN_KEY] = 3,
                    [ObjectId.SWORD] = 4,
                    [ObjectId.SHIELD] = 5,
                    [ObjectId.BLUE_JEWEL] = 6,
                    [ObjectId.RED_JEWEL] = 7,
                    [ObjectId.GREEN_JEWEL] = 8,
                    [ObjectId.RED_POTION] = 10,
                    [ObjectId.BLUE_POTION] = 11,
                    [ObjectId.YELLOW_POTION] = 15,
                    [ObjectId.GREEN_POTION] = 19
                };
            int index;
            if (!itemIdSpriteIndexMap.TryGetValue(itemId, out index))
                index = 0;
            return index;
        }

        public static int GetMonsterSpriteIndex(int monsterId)
        {
            int index = (monsterId - ObjectId.MONSTER_BOUND) * 2;
            if (monsterId == ObjectId.BOSS_INDEX)
                index = 112;
            return index;
        }
    }
}
