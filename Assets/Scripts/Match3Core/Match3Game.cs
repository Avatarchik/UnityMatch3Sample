using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Match3Core
{
    /// <summary>
    /// Игра
    /// </summary>
    public class Match3Game
    {
        private const int MinMatchLength = 3;
        private readonly Block[,] blocks;
        private readonly HashSet<BlockColor> allowedTypes;
        private readonly Random random = new Random();
        //private readonly Queue<BlockType>[] spawners;
        private readonly int rowCount;
        private readonly int columnCount;
        private readonly BlockColor[] allowedColorsAsArray;
        private readonly List<MatchSet> matches = new List<MatchSet>();
        private int blockId;
        private static readonly Block EmptyBlock = new Block(-1, BlockColor.Empty);

        public Match3Game(int rows, int columns, HashSet<BlockColor> allowedTypes)
        {
            if (rows < MinMatchLength || columns < MinMatchLength)
            {
                throw new ArgumentOutOfRangeException("Board dimension must be equal or greater than " + MinMatchLength);
            }

            rowCount = rows;
            columnCount = columns;
            this.allowedTypes = allowedTypes;
            allowedColorsAsArray = allowedTypes.ToArray();
            allowedTypes.Add(BlockColor.Rainbow);

            blocks = new Block[rowCount, columnCount];
            isRemoved = new bool[rowCount, columnCount];
            isInMatch = new int[rowCount, columnCount];

            do
            {
                FillRandom();
            } while (HasMatches() || !HasAvailableActions());
        }

        public Match3Game(BlockColor[,] blockColors, HashSet<BlockColor> allowedTypes)
        {
            if (blockColors.GetLength(0) < MinMatchLength || blockColors.GetLength(1) < MinMatchLength)
            {
                throw new ArgumentOutOfRangeException("Board dimension must be equal or greater than " + MinMatchLength);
            }

            blocks = new Block[blockColors.GetLength(0), blockColors.GetLength(1)];

            for (int row = 0; row < blockColors.GetLength(0); row++)
            {
                for (int column = 0; column < blockColors.GetLength(1); column++)
                {
                    blocks[row, column] = CreateNewBlock(blockColors[row, column]);
                }
            }

            this.allowedTypes = allowedTypes;
            allowedColorsAsArray = allowedTypes.ToArray();
            allowedTypes.Add(BlockColor.Rainbow);

            rowCount = blockColors.GetLength(0);
            columnCount = blockColors.GetLength(1);

            isRemoved = new bool[rowCount, columnCount];
            isInMatch = new int[rowCount, columnCount];

            //            spawners = new Queue<BlockType>[columnsCount];
            //            for (int i = 0; i < columnsCount; i++)
            //            {
            //                spawners[i] = new Queue<BlockType>();
            //            }
        }

        private Block CreateNewBlock(BlockColor color)
        {
            return CreateNewBlock(color, BlockType.Normal);
        }

        public int RowCount
        {
            get { return rowCount; }
        }

        public int ColumnCount
        {
            get { return columnCount; }
        }

        private void FillRandom()
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (blocks[i, j].color != BlockColor.Static)
                    {
                        BlockColor randomBlockColor = allowedColorsAsArray.GetRandomElement(random);
                        blocks[i, j] = CreateNewBlock(randomBlockColor);
                    }
                }
            }
        }

        public bool HasMatches()
        {
            return GetMatches().Count > 0;
        }

        public List<MatchSet> GetMatches()
        {
            matches.Clear();
            // First check rows
            FindHorizontalMatches();
            // Then check columns
            FindVerticalMatches();
            return matches;
        }

        private void FindHorizontalMatches()
        {
            for (int row = 0; row < rowCount; row++)
            {
                BlockColor currentColor = BlockColor.Empty;
                int currentMatchLength = 0;

                for (int column = 0; column < columnCount; column++)
                {
                    Block block = blocks[row, column];
                    bool hasEmptyBlocksBeneath = GetEmptyBlocksBeneathCount(row, column) > 0;

                    // Если блок запрещенного типа (Empty или Static) или висит в воздухе
                    if (!allowedTypes.Contains(block.color) || hasEmptyBlocksBeneath)
                    {
                        if (currentMatchLength >= MinMatchLength)
                        {
                            matches.Add(new MatchSet(row, column - currentMatchLength + 1, currentMatchLength, MatchSet.Alignment.Horizontal, currentColor));
                        }
                        currentColor = BlockColor.Empty;
                        currentMatchLength = 0;
                    }

                    // Если блок того же типа и не висит в воздухе
                    else if (block.color == currentColor)
                    {
                        currentMatchLength++;

                        //специальная проверка для последнего столбца
                        if (column == columnCount - 1)
                        {
                            if (currentMatchLength >= MinMatchLength)
                            {
                                matches.Add(new MatchSet(row, column - currentMatchLength + 1, currentMatchLength, MatchSet.Alignment.Horizontal, currentColor));
                            }
                        }
                    }
                    else
                    {
                        if (currentMatchLength >= MinMatchLength)
                        {
                            matches.Add(new MatchSet(row, column - currentMatchLength, currentMatchLength, MatchSet.Alignment.Horizontal, currentColor));
                        }
                        currentColor = block.color;
                        currentMatchLength = 1;
                    }
                }
            }
        }

        private void FindVerticalMatches()
        {
            for (int column = 0; column < columnCount; column++)
            {
                BlockColor currentColor = BlockColor.Empty;
                int currentMatchLength = 0;

                for (int row = 0; row < rowCount; row++)
                {
                    Block block = blocks[row, column];
                    bool hasEmptyBlocksBeneath = GetEmptyBlocksBeneathCount(row, column) > 0;

                    //Если блок запрещенного типа (Empty или Static) или висит в воздухе
                    if (!allowedTypes.Contains(block.color) || hasEmptyBlocksBeneath)
                    {
                        if (currentMatchLength >= MinMatchLength)
                        {
                            matches.Add(new MatchSet(row - currentMatchLength + 1, column, currentMatchLength, MatchSet.Alignment.Vertical, currentColor));
                        }
                        currentColor = BlockColor.Empty;
                        currentMatchLength = 0;
                    }

                    //Если блок того же типа и не висит в воздухе
                    else if (block.color == currentColor)
                    {
                        currentMatchLength++;

                        //специальная проверка для последней строки
                        if (row == rowCount - 1)
                        {
                            if (currentMatchLength >= MinMatchLength)
                            {
                                matches.Add(new MatchSet(row - currentMatchLength + 1, column, currentMatchLength, MatchSet.Alignment.Vertical, currentColor));
                            }
                        }
                    }
                    else
                    {
                        if (currentMatchLength >= MinMatchLength)
                        {
                            matches.Add(new MatchSet(row - currentMatchLength, column, currentMatchLength, MatchSet.Alignment.Vertical, currentColor));
                        }
                        currentColor = block.color;
                        currentMatchLength = 1;
                    }
                }
            }
        }

        private int GetEmptyBlocksBeneathCount(int row, int column)
        {
            int count = 0;
            for (int i = row + 1; i < rowCount; i++)
            {
                if (blocks[i, column].color == BlockColor.Empty)
                {
                    count++;
                }
            }
            return count;
        }

        private readonly List<Block> original = new List<Block>();
        private readonly List<Block> copy = new List<Block>();

        public void Shuffle()
        {
            original.Clear();
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (blocks[i, j].color != BlockColor.Static)
                    {
                        original.Add(blocks[i, j]);
                    }
                }
            }

            copy.Clear();
            copy.AddRange(original);
            copy.Shuffle(random);
            int index = 0;
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    if (blocks[i, j].color != BlockColor.Static)
                    {
                        blocks[i, j] = copy[index++];
                    }
                }
            }
        }

        private readonly int[,] isInMatch;
        private readonly bool[,] isRemoved;
        private readonly HashSet<BlockAndPosition> activatedBlocks = new HashSet<BlockAndPosition>();
        private readonly HashSet<BlockAndPosition> newActivatedBlocks = new HashSet<BlockAndPosition>();

        public bool RemoveMatches(List<SpecialBlockActivation> specialBlockActivations, List<BlockRemoval> removedBlocks, List<BlockAndPosition> createdBlocks)
        {
            Array.Clear(isRemoved, 0, isRemoved.Length);
            Array.Clear(isInMatch, 0, isInMatch.Length);
            specialBlockActivations.Clear();
            activatedBlocks.Clear();
            removedBlocks.Clear();
            createdBlocks.Clear();

            var matchSets = GetMatches();
            FindCrossingMatches(matchSets);
            ProcessMatches(removedBlocks, matchSets);


            if (playerJustUsedMegaBlock)
            {
                activatedBlocks.Add(megaBlockPosition); // запоминаем положение мега-блока. нужно для отрисовки
                playerJustUsedMegaBlock = false;
            }

            CreateCrossBlocks(createdBlocks);
            GenerateNewBlocks(createdBlocks, matchSets); // за исключение блоков-крестов, их ы создали на прошлом шаге

            return activatedBlocks.Count > 0;
        }

        private void GenerateNewBlocks(List<BlockAndPosition> createdBlocks, List<MatchSet> matchSets)
        {
            for (int i = 0; i < matchSets.Count; i++)
            {
                MatchSet matchSet = matchSets[i];

                bool shouldGenerateNewBlock = false;
                BlockType generatedBlockType = BlockType.Normal;

                if (matchSet.length > 4)
                {
                    shouldGenerateNewBlock = true;
                    generatedBlockType = BlockType.Mega;
                }
                else if (matchSet.length == 4)
                {
                    shouldGenerateNewBlock = true;
                    generatedBlockType = BlockType.Bomb;
                }

                if (shouldGenerateNewBlock)
                {
                    int row;
                    int column;
                    if (FindBestPositionForNewBlock(matchSet, out row, out column)) // добавляем только если есть пустое место
                    {
                        var newBlock = CreateNewBlock(generatedBlockType == BlockType.Mega ? BlockColor.Rainbow : matchSet.color, generatedBlockType);
                        blocks[row, column] = newBlock;
                        createdBlocks.Add(new BlockAndPosition(newBlock, row, column));
                    }
                }
            }
        }

        private void CreateCrossBlocks(List<BlockAndPosition> createdBlocks)
        {
            for (int i = 0; i < listOfCrossingMatches.Count; i++)
            {
                var crossingMatches = listOfCrossingMatches[i];
                var block = blocks[crossingMatches.horizontal.startRow, crossingMatches.vertical.startColumn];
                if (block.type == BlockType.Normal)
                {
                    var newBlock = CreateNewBlock(crossingMatches.horizontal.color, BlockType.Cross);
                    blocks[crossingMatches.horizontal.startRow, crossingMatches.vertical.startColumn] = newBlock;
                    createdBlocks.Add(new BlockAndPosition(newBlock, crossingMatches.horizontal.startRow, crossingMatches.vertical.startColumn));
                }
            }
        }

        private void ProcessMatches(List<BlockRemoval> removedBlocks, List<MatchSet> matchSets)
        {
            for (int i = 0; i < matchSets.Count; i++)
            {
                MatchSet matchSet = matchSets[i];

                // удаляем блоки или активируем, если это "активные" блоки
                for (int j = 0; j < matchSet.length; j++)
                {
                    int row, column;
                    if (matchSet.alignment == MatchSet.Alignment.Horizontal)
                    {
                        row = matchSet.startRow;
                        column = matchSet.startColumn + j;
                    }
                    else
                    {
                        row = matchSet.startRow + j;
                        column = matchSet.startColumn;
                    }

                    Block block = blocks[row, column];
                    if (block.type != BlockType.Normal)
                    {
                        activatedBlocks.Add(new BlockAndPosition(block, row, column));
                    }
                    else if (block.color != BlockColor.Empty)
                    {
                        blocks[row, column] = EmptyBlock;
                        removedBlocks.Add(new BlockRemoval(block, BlockRemovalReason.DestroyedByMatching, row, column));
                    }
                }
            }
        }

        public bool TryChainReaction(List<SpecialBlockActivation> specialBlockActivations, List<BlockRemoval> removedBlocks)
        {
            specialBlockActivations.Clear();
            removedBlocks.Clear();

            if (activatedBlocks.Count == 0)
            {
                return false;
            }

            newActivatedBlocks.Clear();
            foreach (var blockAndPosition in activatedBlocks)
            {
                ApplyEffect(blockAndPosition, specialBlockActivations, removedBlocks);
            }
            activatedBlocks.Clear();
            activatedBlocks.UnionWith(newActivatedBlocks);
            return true;
        }

        private bool FindBestPositionForNewBlock(MatchSet matchSet, out int bestRow, out int bestColumn)
        {
            if (matchSet.alignment == MatchSet.Alignment.Horizontal)
            {
                int row = matchSet.startRow;
                int leftColumn = matchSet.startColumn;
                int rightColumn = matchSet.startColumn + matchSet.length - 1;

                if (swapRow1 == row && swapColumn1 >= leftColumn && swapColumn1 <= rightColumn)
                {
                    if (blocks[swapRow1, swapColumn1].type == BlockType.Normal)
                    {
                        bestRow = swapRow1;
                        bestColumn = swapColumn1;
                        return true;
                    }
                }

                if (swapRow2 == row && swapColumn2 >= leftColumn && swapColumn2 <= rightColumn)
                {
                    if (blocks[swapRow2, swapColumn2].type == BlockType.Normal)
                    {
                        bestRow = swapRow2;
                        bestColumn = swapColumn2;
                        return true;
                    }
                }


                float centerColumn = matchSet.startColumn + (matchSet.length - 1)/2f;
                for (int i = 0; i < (matchSet.length + 1)/2; i++)
                {
                    int left = (int) (centerColumn - i);
                    int right = (int) (centerColumn + i);
                    if (left >= leftColumn)
                    {
                        if (blocks[row, left].type == BlockType.Normal)
                        {
                            bestRow = row;
                            bestColumn = left;
                            return true;
                        }
                    }
                    if (right <= rightColumn)
                    {
                        if (blocks[row, right].type == BlockType.Normal)
                        {
                            bestRow = row;
                            bestColumn = right;
                            return true;
                        }
                    }
                }

                bestRow = 0;
                bestColumn = 0;
                return false;
            }
            else
            {
                int column = matchSet.startColumn;
                int topRow = matchSet.startRow;
                int bottomRow = matchSet.startRow + matchSet.length - 1;

                if (swapColumn1 == column && swapRow1 >= topRow && swapRow1 <= bottomRow)
                {
                    if (blocks[swapRow1, swapColumn1].type == BlockType.Normal)
                    {
                        bestRow = swapRow1;
                        bestColumn = swapColumn1;
                        return true;
                    }
                }

                if (swapColumn2 == column && swapRow2 >= topRow && swapRow2 <= bottomRow)
                {
                    if (blocks[swapRow2, swapColumn2].type == BlockType.Normal)
                    {
                        bestRow = swapRow2;
                        bestColumn = swapColumn2;
                        return true;
                    }
                }


                float centerRow = matchSet.startRow + (matchSet.length - 1)/2f;

                for (int i = 0; i < (matchSet.length + 1)/2; i++)
                {
                    int top = (int) (centerRow - i);
                    int bottom = (int) (centerRow + i);
                    if (top >= topRow)
                    {
                        if (blocks[top, column].type == BlockType.Normal)
                        {
                            bestRow = top;
                            bestColumn = column;
                            return true;
                        }
                    }
                    if (bottom <= bottomRow)
                    {
                        if (blocks[bottom, column].type == BlockType.Normal)
                        {
                            bestRow = bottom;
                            bestColumn = column;
                            return true;
                        }
                    }
                }

                bestRow = 0;
                bestColumn = 0;
                return false;
            }
        }

        private List<CrossingMatches> listOfCrossingMatches = new List<CrossingMatches>();

        private void FindCrossingMatches(List<MatchSet> matchSets)
        {
            listOfCrossingMatches.Clear();

            for (int i = 0; i < matchSets.Count - 1; i++)
            {
                var first = matchSets[i];
                for (int j = i + 1; j < matchSets.Count; j++)
                {
                    var second = matchSets[j];
                    if (first.alignment == MatchSet.Alignment.Horizontal && second.alignment == MatchSet.Alignment.Vertical)
                    {
                        if (second.startColumn >= first.startColumn && second.startColumn < first.startColumn + first.length)
                        {
                            if (first.startRow >= second.startRow && first.startRow < second.startRow + second.length)
                            {
                                listOfCrossingMatches.Add(new CrossingMatches(first, second));
                            }
                        }
                    }
                    else if (first.alignment == MatchSet.Alignment.Vertical && second.alignment == MatchSet.Alignment.Horizontal)
                    {
                        if (first.startColumn >= second.startColumn && first.startColumn < second.startColumn + second.length)
                        {
                            if (second.startRow >= first.startRow && second.startRow < first.startRow + first.length)
                            {
                                listOfCrossingMatches.Add(new CrossingMatches(second, first));
                            }
                        }
                    }
                }
            }
        }

        private void ApplyEffect(BlockAndPosition activatedBlock, List<SpecialBlockActivation> blockActivations, List<BlockRemoval> removedBlocks)
        {
            int row = activatedBlock.row;
            int column = activatedBlock.column;

            var blockType = activatedBlock.block.type;
            switch (blockType)
            {
                case BlockType.Mega:
                    DestroyActivatedBlock(row, column, blockActivations, blockType);
                    blocks[row, column].color = colorToDestroy;
                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            if (blocks[i, j].color == colorToDestroy)
                            {
                                DestroyOrActivateBlock(i, j, removedBlocks, BlockRemovalReason.DestroyedByLaserBurst);
                            }
                        }
                    }

                    break;
                case BlockType.Cross:
                    DestroyActivatedBlock(row, column, blockActivations, blockType);
                    for (int i = 0; i < columnCount; i++)
                    {
                        DestroyOrActivateBlock(row, i, removedBlocks, BlockRemovalReason.DestroyedByMatching);
                    }
                    for (int i = 0; i < rowCount; i++)
                    {
                        DestroyOrActivateBlock(i, column, removedBlocks, BlockRemovalReason.DestroyedByCrossExplosion);
                    }

                    break;
                case BlockType.Bomb:
                    DestroyActivatedBlock(row, column, blockActivations, blockType);
                    for (int i = row - 1; i <= row + 1; i++)
                    {
                        for (int j = column - 1; j <= column + 1; j++)
                        {
                            DestroyOrActivateBlock(i, j, removedBlocks, BlockRemovalReason.DestroyedByBombExplosion);
                        }
                    }
                    break;
            }
        }

        private void DestroyOrActivateBlock(int row, int column, List<BlockRemoval> removedBlocks, BlockRemovalReason blockRemovalReason)
        {
            if (row < 0 || row >= rowCount || column < 0 || column >= columnCount)
            {
                return;
            }

            var block = blocks[row, column];

            if (block.type != BlockType.Normal && block.type != BlockType.Mega)
            {
                var blockAndPosition = new BlockAndPosition(block, row, column);
                if (!activatedBlocks.Contains(blockAndPosition))
                {
                    newActivatedBlocks.Add(blockAndPosition);
                    removedBlocks.Add(new BlockRemoval(block, blockRemovalReason, row, column));
                }
                else
                {
                    Console.WriteLine("Already contains");
                }
            }
            else if (block.color != BlockColor.Empty)
            {
                removedBlocks.Add(new BlockRemoval(block, blockRemovalReason, row, column));
                blocks[row, column] = EmptyBlock;
            }
        }

        private void DestroyActivatedBlock(int row, int column, List<SpecialBlockActivation> blockActivations, BlockType blockType)
        {
            if (row < 0 || row >= rowCount || column < 0 || column >= columnCount)
            {
                return;
            }

            blockActivations.Add(new SpecialBlockActivation(blockType, row, column));

            blocks[row, column].type = BlockType.Normal;
        }


        private Block CreateNewBlock(BlockColor color, BlockType type)
        {
            return new Block(blockId++, color, type);
        }

        public bool HasAvailableActions()
        {
            //возможны 12 вариантов 

            for (int row = 0; row < rowCount; row++)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    var blockColor = blocks[row, column].color;
                    if (blockColor == BlockColor.Rainbow)
                    {
                        return true;
                    }

                    if (row < rowCount - 1 && column < columnCount - 2)
                    {
                        var b00 = blocks[row, column].color;
                        var b01 = blocks[row, column + 1].color;
                        var b02 = blocks[row, column + 2].color;
                        var b10 = blocks[row + 1, column].color;
                        var b11 = blocks[row + 1, column + 1].color;
                        var b12 = blocks[row + 1, column + 2].color;

                        // *
                        //  **
                        if (AreBlockAllowedAndTheSame(b00, b11, b12))
                        {
                            return true;
                        }

                        //  * 
                        // * *
                        if (AreBlockAllowedAndTheSame(b01, b10, b12))
                        {
                            return true;
                        }

                        //   *
                        // **
                        if (AreBlockAllowedAndTheSame(b02, b10, b11))
                        {
                            return true;
                        }

                        //  **
                        // *
                        if (AreBlockAllowedAndTheSame(b01, b02, b10))
                        {
                            return true;
                        }

                        // * *
                        //  * 
                        if (AreBlockAllowedAndTheSame(b00, b02, b11))
                        {
                            return true;
                        }

                        // **
                        //   *
                        if (AreBlockAllowedAndTheSame(b00, b01, b12))
                        {
                            return true;
                        }
                    }

                    if (row < rowCount - 2 && column < columnCount - 1)
                    {
                        var b00 = blocks[row, column].color;
                        var b01 = blocks[row, column + 1].color;
                        var b10 = blocks[row + 1, column].color;
                        var b11 = blocks[row + 1, column + 1].color;
                        var b20 = blocks[row + 2, column].color;
                        var b21 = blocks[row + 2, column + 1].color;

                        // *
                        //  *
                        //  *
                        if (AreBlockAllowedAndTheSame(b00, b11, b21))
                        {
                            return true;
                        }

                        //  *
                        // *
                        //  *
                        if (AreBlockAllowedAndTheSame(b01, b10, b21))
                        {
                            return true;
                        }

                        //  *
                        //  *
                        // *
                        if (AreBlockAllowedAndTheSame(b01, b11, b20))
                        {
                            return true;
                        }

                        //  *
                        // *
                        // *
                        if (AreBlockAllowedAndTheSame(b01, b10, b20))
                        {
                            return true;
                        }

                        // *
                        //  *
                        // *
                        if (AreBlockAllowedAndTheSame(b00, b11, b20))
                        {
                            return true;
                        }

                        // *
                        // *
                        //  *
                        if (AreBlockAllowedAndTheSame(b00, b10, b21))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool AreBlockAllowedAndTheSame(BlockColor block1, BlockColor block2, BlockColor block3)
        {
            return allowedColorsAsArray.Contains(block1) && block1 == block2 && block1 == block3;
        }

        private int GetEmptyBlocksCountInColumn(int column)
        {
            return GetEmptyBlocksBeneathCount(-1, column);
        }

        public bool UpdateGravity()
        {
            if (movingBlocks.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < movingBlocks.Count; i++)
            {
                var activeBlock = movingBlocks[i];
                blocks[activeBlock.destionationRow, activeBlock.initialColumn] = activeBlock.block;
            }
            return true;
        }

        public bool TrySwapBlocks(int row1, int column1, int row2, int column2)
        {
            if ((Math.Abs(row1 - row2) == 1 && column1 == column2) || (row1 == row2 && Math.Abs(column1 - column2) == 1))
            {
                var block1 = blocks[row1, column1];
                var block2 = blocks[row2, column2];

                if (block1.color == block2.color)
                {
                    return false;
                }

                blocks[row1, column1] = block2;
                blocks[row2, column2] = block1;

                if (block1.color == BlockColor.Rainbow || block2.color == BlockColor.Rainbow)
                {
                    playerJustUsedMegaBlock = true;
                    if (block1.color == BlockColor.Rainbow)
                    {
                        colorToDestroy = block2.color;
                        megaBlockPosition = new BlockAndPosition(block1, row2, column2);
                    }
                    else
                    {
                        colorToDestroy = block1.color;
                        megaBlockPosition = new BlockAndPosition(block2, row1, column1);
                    }

                    return true;
                }

                if (HasMatches())
                {
                    swapRow1 = row1;
                    swapColumn1 = column1;
                    swapRow2 = row2;
                    swapColumn2 = column2;
                    return true;
                }

                blocks[row1, column1] = block1;
                blocks[row2, column2] = block2;
            }
            return false;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(rowCount*(columnCount + 1));
            for (int i = 0; i < rowCount; i++)
            {
                builder.Append(i).Append(" ");
                for (int j = 0; j < columnCount; j++)
                {
                    BlockColor blockColor = blocks[i, j].color;
                    string firstLetter = blockColor.ToString().Substring(0, 1);
                    builder.Append(firstLetter);
                }
                builder.AppendLine();
            }
            builder.AppendLine();
            builder.Append("  ");
            for (int i = 0; i < columnCount; i++)
            {
                builder.Append(i);
            }
            builder.AppendLine();

            return builder.ToString();
        }

        public Block this[int row, int column]
        {
            get { return blocks[row, column]; }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (blocks != null ? blocks.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ rowCount;
                hashCode = (hashCode*397) ^ columnCount;
                hashCode = (hashCode*397) ^ (allowedTypes != null ? allowedTypes.GetHashCode() : 0);
                return hashCode;
            }
        }

        private readonly List<BlockAndMovement> movingBlocks = new List<BlockAndMovement>();
        private int swapRow1;
        private int swapColumn1;
        private int swapRow2;
        private int swapColumn2;
        private bool playerJustUsedMegaBlock;
        private BlockColor colorToDestroy;
        private BlockAndPosition megaBlockPosition;

        public List<BlockAndMovement> GetMovingBlocks()
        {
            movingBlocks.Clear();

            for (int row = rowCount - 2; row >= 0; row--)
            {
                for (int column = 0; column < columnCount; column++)
                {
                    var block = blocks[row, column];
                    if (block.color == BlockColor.Empty)
                    {
                        continue;
                    }

                    int emptyBlockBeneathCount = GetEmptyBlocksBeneathCount(row, column);
                    if (emptyBlockBeneathCount > 0)
                    {
                        var activeBlock = new BlockAndMovement(block, row, column, row + emptyBlockBeneathCount);
                        movingBlocks.Add(activeBlock);
                    }
                }
            }

            for (int column = 0; column < columnCount; column++)
            {
                var emptyBlocksInColumn = GetEmptyBlocksCountInColumn(column);
                for (int i = emptyBlocksInColumn - 1; i >= 0; i--)
                {
                    var block = CreateNewBlock(allowedColorsAsArray.GetRandomElement(random));
                    var activeBlock = new BlockAndMovement(block, i - emptyBlocksInColumn, column, i);
                    movingBlocks.Add(activeBlock);
                }
            }

            return movingBlocks;
        }

        public void SetType(int row, int column, BlockType blockType)
        {
            blocks[row, column].type = blockType;
        }

        public void SetTypeAndColor(int row, int column, BlockType blockType, BlockColor blockColor)
        {
            blocks[row, column].type = blockType;
            blocks[row, column].color = blockColor;
        }
    }

    public struct CrossingMatches
    {
        public MatchSet horizontal;
        public MatchSet vertical;

        public CrossingMatches(MatchSet horizontal, MatchSet vertical)
        {
            this.horizontal = horizontal;
            this.vertical = vertical;
        }

        public bool Contains(MatchSet matchSet)
        {
            return horizontal.Equals(matchSet) || vertical.Equals(matchSet);
        }
    }

    public struct BlockAndMovement
    {
        public Block block;
        public int initialRow;
        public int initialColumn;
        public int destionationRow;

        public BlockAndMovement(Block block, int initialRow, int initialColumn, int destionationRow)
        {
            this.block = block;
            this.initialRow = initialRow;
            this.initialColumn = initialColumn;
            this.destionationRow = destionationRow;
        }
    }

    public struct MatchSet
    {
        public int startRow;
        public int startColumn;
        public int length;
        public BlockColor color;

        public enum Alignment
        {
            Horizontal,
            Vertical
        }

        public readonly Alignment alignment;

        public MatchSet(int startRow, int startColumn, int length, Alignment alignment, BlockColor color)
        {
            this.startRow = startRow;
            this.startColumn = startColumn;
            this.length = length;
            this.alignment = alignment;
            this.color = color;
        }

        public bool Equals(MatchSet other)
        {
            return startRow == other.startRow && startColumn == other.startColumn && length == other.length && alignment == other.alignment;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is MatchSet && Equals((MatchSet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = startRow;
                hashCode = (hashCode*397) ^ startColumn;
                hashCode = (hashCode*397) ^ length;
                hashCode = (hashCode*397) ^ (int) alignment;
                return hashCode;
            }
        }
    }
}