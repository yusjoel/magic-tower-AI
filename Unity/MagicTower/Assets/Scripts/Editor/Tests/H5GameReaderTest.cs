using Gempoll.Extensions;
using Gempoll.H5Game;
using NUnit.Framework;
using System.Text;

namespace Gempoll.Editor.Tests
{
    public class H5GameReaderTest
    {
        [Test]
        public void TestReadDataJs()
        {
            var reader = new Reader(@"F:\Games\Play\PC\蓝宝石实战塔\project\");
            reader.ReadDataJs();
            Assert.AreEqual("[MT0, MT1, MT2, MT3]", reader.FloorIds.Serialize());

            var gameInfo = reader.GameInfo;
            Assert.AreEqual(4, gameInfo.FloorCount);
            Assert.AreEqual("lbsszt", reader.GameName);
            Assert.AreEqual("(2000,100,100,0,0,0,0,0,0)", gameInfo.Hero.ToString());
            Assert.AreEqual(0, gameInfo.HeroFloor);
            Assert.AreEqual(6, gameInfo.HeroPositionX);
            Assert.AreEqual(12, gameInfo.HeroPositionY);

            Assert.AreEqual(1, gameInfo.AttackOfRedJewel);
            Assert.AreEqual(1, gameInfo.DefenseOfBlueJewel);
            Assert.AreEqual(5, gameInfo.MagicDefenseOfGreenJewel);

            Assert.AreEqual(100, gameInfo.HitPointOfRedPotion);
            Assert.AreEqual(250, gameInfo.HitPointOfBluePotion);
            Assert.AreEqual(500, gameInfo.HitPointOfYellowPotion);
            Assert.AreEqual(800, gameInfo.HitPointOfGreenPotion);
        }

        [Test]
        public void TestReadFloors()
        {
            var reader = new Reader(@"F:\Games\Play\PC\蓝宝石实战塔\project\");
            reader.ReadDataJs();
            reader.ReadFloors();

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();

            var grid = reader.GameInfo.Grid;
            int floorCount = reader.GameInfo.FloorCount;
            int rowCount = reader.GameInfo.RowCount;
            int columnCount = reader.GameInfo.ColumnCount;
            for (int i = 0; i < floorCount; i++)
            for (int j = 0; j < rowCount; j++)
            {
                for (int k = 0; k < columnCount; k++)
                {
                    if (k == 0)
                        stringBuilder.Append("    [");
                    stringBuilder.Append(grid[i, j, k].ToString().PadLeft(3));
                    stringBuilder.Append(k < columnCount - 1 ? "," : "]");
                }

                stringBuilder.AppendLine(j < rowCount - 1 ? "," : "");
            }

            string expected = @"
    [  4,  4,  4,  4,  4,  4, 87,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,121,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4, 45,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4],
    [  4,  4,  4,  4,  4,  4,  0,  4,  4,  4,  4,  4,  4]
    [ 28,  0,  0,201,  0,  1, 87,  1,  0,205, 28, 28, 28],
    [  1,  1,  1,  1,  0,  1,245,  1,  0,  1,  1, 28, 28],
    [ 28, 28,  0,202,  0,  1,  0,  1,209,  1,  1, 28, 28],
    [  1,  1,  1,  1,  0,  1,215,  1,  0,  1, 28,  1,  1],
    [  0, 28,  0,202,  0,  1,  0,  1,  0,209, 28, 28,  1],
    [201,  1,  1,  1,  0,  1,  0,  1,209,  1,  1,  1, 28],
    [ 28, 28,  1,  1,  0,201,  0,201,  0,  1, 28, 28, 28],
    [  1,  1,  1,  1,  0,  1,  0,  1,  0,210, 28, 28, 28],
    [ 28, 28,  0,205,  0,  1,  0,  1,209,  1,  1,  1,  1],
    [ 28, 28,  0,  1,  0,  1,  0,  1,  0,  1, 28, 28, 28],
    [  1,  1,  1,205,  0,  1,  0,  1,  0,  1, 28, 28, 28],
    [ 28,202, 28, 28,  1,  1,121,  1,  0,  1,  1,  1,209],
    [ 28,202, 28,  1,  1,  1,  0,  1,  0,  0,209,  0,  0]
    [ 28,  0,  0,201,  0,  4, 87,  4,  0,  0, 28, 28, 28],
    [  1,  1,  1,  1,  0,  4,230,  4,213,  1,  1,  1,  1],
    [ 28,201, 28,202,  0,  4, 47,  4,  0,205, 28, 28, 28],
    [209,  1,  1,  1,  0,  4, 47,  4,  0,  1,  1, 28,213],
    [ 28, 28,  1,  1,  0,  4, 33,  4,  0,213, 28,213,  1],
    [ 28,  1, 28,205,  0,  4, 33,  4,209,  1,210,  1, 28],
    [  1, 28, 28,  1,  0,201, 34,209,  0,210, 28, 28, 28],
    [  1, 28,  1,  1,  0,  4,  0,  4,  0,210, 28, 28, 28],
    [ 28,  1,  1,  0,  0,  4,  0,  4,209,210,  1,  1,  1],
    [  0,  0,  0,213,  0,  4,  0,  4,  0,  1,  1,  1,  1],
    [  1,  1,  1,  1,  0,  4,  0,  4,  0,  0,  1,  1,  1],
    [ 28, 28,209,213,  0,  4,121,  4,209,  1,  0, 28, 28],
    [ 28, 28,209,213,  0,  4,  0,  4, 28,209, 28, 28, 28]
    [  1,  1,  1,  1,  1,  1,229,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [ 31, 31, 31, 31, 31, 31,248,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,236,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1],
    [  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1]
";
            Assert.AreEqual(expected, stringBuilder.ToString());
        }
    }
}
