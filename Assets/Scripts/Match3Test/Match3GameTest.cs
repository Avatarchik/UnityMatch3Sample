using System.Collections.Generic;
using Match3Core;
using NUnit.Framework;

namespace Match3Test
{

    [TestFixture]
    public class Match3GameTest
    {
        static readonly HashSet<BlockColor> AllowedTypes = new HashSet<BlockColor> { BlockColor.Red, BlockColor.Green, BlockColor.Blue };

        static readonly BlockColor[,] NoPossibleActions =
            {
                {BlockColor.Red, BlockColor.Green, BlockColor.Green},
                {BlockColor.Blue, BlockColor.Green, BlockColor.Red},
                {BlockColor.Red, BlockColor.Blue, BlockColor.Red}
            };


        static readonly BlockColor[,] Simple =
            {
                {BlockColor.Red, BlockColor.Green, BlockColor.Green, BlockColor.Red},
                {BlockColor.Green, BlockColor.Green, BlockColor.Blue, BlockColor.Green},
                {BlockColor.Blue, BlockColor.Green, BlockColor.Green, BlockColor.Red},
                {BlockColor.Blue, BlockColor.Blue, BlockColor.Blue, BlockColor.Red}
            };

        static readonly BlockColor[,] ManyMatches =
            {
                {BlockColor.Blue, BlockColor.Green, BlockColor.Green, BlockColor.Red},
                {BlockColor.Blue, BlockColor.Green, BlockColor.Green, BlockColor.Red},
                {BlockColor.Blue, BlockColor.Green, BlockColor.Green, BlockColor.Red},
                {BlockColor.Blue, BlockColor.Red, BlockColor.Red, BlockColor.Red}
            };

        [Test]
        public void TestHasMatches1()
        {
            var board = new Match3Game(Simple, AllowedTypes);
            Assert.IsTrue(board.HasMatches());
        }

        [Test]
        public void TestHasMatches2()
        {
            var board = new Match3Game(NoPossibleActions, AllowedTypes);
            Assert.IsFalse(board.HasMatches());
        }

        [Test]
        public void TestHasMatches3()
        {
            var board = new Match3Game(ManyMatches, AllowedTypes);
            Assert.IsTrue(board.HasMatches());
        }

        [Test]
        public void TestGetMatches1()
        {
            var board = new Match3Game(Simple, AllowedTypes);

            var matches = board.GetMatches();

            Assert.IsTrue(matches.Count == 2);
            Assert.IsTrue(matches.Contains(new MatchSet(0, 1, 3, MatchSet.Alignment.Vertical, BlockColor.Green)));
            Assert.IsTrue(matches.Contains(new MatchSet(3, 0, 3, MatchSet.Alignment.Horizontal, BlockColor.Blue)));

        }

        [Test]
        public void TestGetMatches2()
        {
            var board = new Match3Game(NoPossibleActions, AllowedTypes);

            var matches = board.GetMatches();

            Assert.IsTrue(matches.Count == 0);
        }

        [Test]
        public void TestGetMatches3()
        {
            var board = new Match3Game(ManyMatches, AllowedTypes);

            var matches = board.GetMatches();

            Assert.IsTrue(matches.Count == 5);
            Assert.IsTrue(matches.Contains(new MatchSet(0, 0, 4, MatchSet.Alignment.Vertical, BlockColor.Blue)));
            Assert.IsTrue(matches.Contains(new MatchSet(0, 1, 3, MatchSet.Alignment.Vertical, BlockColor.Green)));
            Assert.IsTrue(matches.Contains(new MatchSet(0, 2, 3, MatchSet.Alignment.Vertical, BlockColor.Green)));
            Assert.IsTrue(matches.Contains(new MatchSet(0, 3, 4, MatchSet.Alignment.Vertical, BlockColor.Red)));
            Assert.IsTrue(matches.Contains(new MatchSet(3, 1, 3, MatchSet.Alignment.Horizontal, BlockColor.Red)));
        }

        [Test]
        public void TestHasActions1()
        {

            Match3Game match3Game = new Match3Game(Simple, AllowedTypes);
            Assert.IsTrue(match3Game.HasAvailableActions());
        }

        [Test]
        public void TestHasActions2()
        {
            var board = new Match3Game(ManyMatches, AllowedTypes);
            Assert.IsTrue(board.HasAvailableActions());
        }

        [Test]
        public void TestHasActions3()
        {
            var board = new Match3Game(NoPossibleActions, AllowedTypes);
            Assert.IsFalse(board.HasAvailableActions());
        }

        [Ignore]
        [Test]
        public void TestShuffle()
        {
            Match3Game match3Game = new Match3Game(12, 12, AllowedTypes);

            var blocksByTypeOriginal = new Dictionary<BlockColor, int>();

            GetBlockCountByType(match3Game, blocksByTypeOriginal);

            for (int i = 0; i < 10; i++)
            {
                match3Game.Shuffle();
                var blocksByType = new Dictionary<BlockColor, int>();
                GetBlockCountByType(match3Game, blocksByType);

                foreach (var allowedType in AllowedTypes)
                {
                    int originalCount;
                    blocksByTypeOriginal.TryGetValue(allowedType, out originalCount);
                    int shuffledCount;
                    blocksByType.TryGetValue(allowedType, out shuffledCount);
                    Assert.True(originalCount == shuffledCount);
                }
            }
        }

        private static void GetBlockCountByType(Match3Game match3Game, Dictionary<BlockColor, int> blockTypeCount)
        {
            for (int row = 0; row < match3Game.RowCount; row++)
            {
                for (int column = 0; column < match3Game.ColumnCount; column++)
                {
                    var block = match3Game[row, column];
                    int count;
                    blockTypeCount.TryGetValue(block.color, out count);
                    blockTypeCount[block.color] = ++count;
                }
            }
        }

        [Test]
        public void RemoveMatches_ShouldClearBoard()
        {
            var match3Game = new Match3Game(ManyMatches, AllowedTypes);

            List<SpecialBlockActivation> blockActivation = new List<SpecialBlockActivation>();
            List<BlockRemoval> removedBlocks2 = new List<BlockRemoval>();
            List<BlockAndPosition> createdBlocks = new List<BlockAndPosition>();

            match3Game.RemoveMatches(blockActivation, removedBlocks2, createdBlocks);

            for (int row = 0; row < match3Game.RowCount; row++)
            {
                for (int column = 0; column < match3Game.ColumnCount; column++)
                {
                    Assert.That(match3Game[row, column].color == BlockColor.Empty);
                }
            }
        }

        [Test]
        public void TestRemovalMatches_ShouldNotAlterBoard()
        {
            var match3Game = new Match3Game(NoPossibleActions, AllowedTypes);
            List<SpecialBlockActivation> blockActivation = new List<SpecialBlockActivation>();
            List<BlockRemoval> removedBlocks2 = new List<BlockRemoval>();
            List<BlockAndPosition> createdBlocks = new List<BlockAndPosition>();

            match3Game.RemoveMatches(blockActivation, removedBlocks2, createdBlocks);

            for (int row = 0; row < match3Game.RowCount; row++)
            {
                for (int column = 0; column < match3Game.ColumnCount; column++)
                {
                    Assert.That(match3Game[row, column].color != BlockColor.Empty);
                }
            }
        }

        [Test]
        public void TestRemovalMatches_ShouldAlterSomeCells()
        {
            var simpleBoardContentsAfterRemoval = new BlockColor[,]
            {
                {BlockColor.Red, BlockColor.Empty, BlockColor.Green, BlockColor.Red},
                {BlockColor.Green, BlockColor.Empty, BlockColor.Blue, BlockColor.Green},
                {BlockColor.Blue, BlockColor.Empty, BlockColor.Green, BlockColor.Red},
                {BlockColor.Empty, BlockColor.Empty, BlockColor.Empty, BlockColor.Red}
            };


            var match3Game = new Match3Game(Simple, AllowedTypes);
            List<SpecialBlockActivation> blockActivation = new List<SpecialBlockActivation>();
            List<BlockRemoval> removedBlocks2 = new List<BlockRemoval>();
            List<BlockAndPosition> createdBlocks = new List<BlockAndPosition>();

            match3Game.RemoveMatches(blockActivation, removedBlocks2, createdBlocks);

            for (int row = 0; row < match3Game.RowCount; row++)
            {
                for (int column = 0; column < match3Game.ColumnCount; column++)
                {
                    Assert.That(match3Game[row, column].color == simpleBoardContentsAfterRemoval[row, column]);
                }
            }
        }
    }
}
