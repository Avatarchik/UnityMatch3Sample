using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Match3Core
{
    static class BoardUtils
    {
        static readonly IDictionary<char, BlockColor> CharToBlockTypeMapping = new Dictionary<char, BlockColor>();

        static BoardUtils()
        {
            foreach (BlockColor blockType in Enum.GetValues(typeof(BlockColor)).Cast<BlockColor>())
            {
                String blockTypeName = Enum.GetName(typeof (BlockColor), blockType);
                CharToBlockTypeMapping.Add(blockTypeName[0], blockType);
            }            
        }

        public static Match3Game GetFromFile(string filename)
        {
            var lineIndex = 0;
            BlockColor[,] blocks = null;
            var allowedBlockTypes = new HashSet<BlockColor>();

            foreach (var line in File.ReadAllLines(filename))
            {
                if (lineIndex == 0)
                {
                    String[] split = line.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    int rowsCount;
                    int.TryParse(split[0], out rowsCount);
                    int columnsCount;
                    int.TryParse(split[1], out columnsCount);
                    blocks = new BlockColor[rowsCount,columnsCount];
                }
                else if (lineIndex == 1)
                {
                    string trimmedLine = line.Trim();
                    foreach (var c in trimmedLine)
                    {
                        allowedBlockTypes.Add(GetBlockTypeByFirstLetter(c));
                    }
                }
                else
                {
                    string trimmedLine = line.Trim();
                    int columnIndex = 0;
                    foreach (var c in trimmedLine)
                    {
                        blocks[lineIndex - 2, columnIndex] = GetBlockTypeByFirstLetter(c);
                        columnIndex++;
                    } 
                }
                lineIndex++;
            }
            if (blocks == null)
            {
                throw new Exception("File " + filename + " can't be parsed as a game board.");
            }
            return new Match3Game(blocks, allowedBlockTypes);
        }

        private static BlockColor GetBlockTypeByFirstLetter(char c)
        {
            BlockColor result;
            if (CharToBlockTypeMapping.TryGetValue(c, out result))
            {
                return result;
            }
            throw new Exception("Unknown type " + c + ". Possible variants are: " + GetAllowedMappingKeys());
        }

        private static String GetAllowedMappingKeys()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var key in CharToBlockTypeMapping.Keys)
            {
                builder.Append(key);
            }
            return builder.ToString();
        }
    }
}
