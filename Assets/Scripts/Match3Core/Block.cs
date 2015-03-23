namespace Match3Core
{
    public struct Block
    {
        public readonly int id;
        public BlockColor color;
        public BlockType type;

        public Block(int id, BlockColor color) : this(id, color, BlockType.Normal)
        {
        }

        public Block(int id, BlockColor color, BlockType type)
        {
            this.id = id;
            this.color = color;
            this.type = type;
        }

        public bool Equals(Block other)
        {
            return id == other.id && color == other.color && type == other.type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Block && Equals((Block) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = id;
                hashCode = (hashCode*397) ^ (int) color;
                hashCode = (hashCode*397) ^ (int) type;
                return hashCode;
            }
        }
    }
}
