using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public sealed class PunchCardData
    {
        public const int Columns = 4;
        public const int Rows = 6;
        public const int HoleCount = Columns * Rows;

        [SerializeField] private bool[] holes = new bool[HoleCount];

        public bool IsPunched(int column, int row)
        {
            CheckPosition(column, row);
            return holes[column * Rows + row];
        }
        
        public void Punch(int column, int row)
        {
            CheckPosition(column, row);
            holes[column * Rows + row] = true;
        }

        public PunchCardData Clone()
        {
            var copy = new PunchCardData();
            Array.Copy(holes, copy.holes, HoleCount);
            return copy;
        }

        public bool Matches(PunchCardData other)
        {
            if (other == null) return false;
            for (int position = 0; position < HoleCount; position++)
            {
                if (holes[position] != other.holes[position])
                    return false;
            }
            return true;
        }
        
        public uint ToKey()
        {
            uint key = 0;
            for (int position = 0; position < HoleCount; position++)
            {
                if (!holes[position])
                    continue;
                //|= bitwise operator
                //<< bit shift 
                key |= 1u << position;
            }
            return key;
        }

        private static void CheckPosition(int column, int row)
        {
            if (column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException(nameof(column));
            if (row < 0 || row >= Rows)
                throw new ArgumentOutOfRangeException(nameof(row));
        }
    }
}