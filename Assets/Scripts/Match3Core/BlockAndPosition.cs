namespace Match3Core
{
    public struct BlockAndPosition
    {
        public readonly Block block;
        public readonly int row;
        public readonly int column;

        public BlockAndPosition(Block block, int row, int column)
        {
            this.block = block;
            this.row = row;
            this.column = column;
        }

        public bool Equals(BlockAndPosition other)
        {
            return block.Equals(other.block) && row == other.row && column == other.column;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is BlockAndPosition && Equals((BlockAndPosition) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = block.GetHashCode();
                hashCode = (hashCode*397) ^ row;
                hashCode = (hashCode*397) ^ column;
                return hashCode;
            }
        }
    }
}