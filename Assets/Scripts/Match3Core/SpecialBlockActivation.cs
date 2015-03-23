namespace Match3Core
{
    public struct SpecialBlockActivation
    {
        public BlockType type;
        public int row;
        public int column;

        public SpecialBlockActivation(BlockType type, int row, int column)
        {
            this.type = type;
            this.row = row;
            this.column = column;
        }
    }
}
