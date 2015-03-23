namespace Match3Core
{
    public struct BlockRemoval
    {
        public Block block;
        public BlockRemovalReason removalReason;
        public int destinationRow;
        public int destinationColumn;

        public BlockRemoval(Block block, BlockRemovalReason removalReason, int destinationRow, int destinationColumn)
        {
            this.block = block;
            this.removalReason = removalReason;
            this.destinationRow = destinationRow;
            this.destinationColumn = destinationColumn;
        }
    }
}