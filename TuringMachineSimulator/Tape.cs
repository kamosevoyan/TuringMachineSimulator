using System;
using System.Text;

namespace TuringMachineSimulator
{
    /// <summary>
    /// The class representing the tape of Turing Machine
    /// </summary>
    public class Tape
    {
        private StringBuilder _leftSide, _rightSide;
        private char _emptySymbol;
        private const int ExtendSize = 16;
        private const int TapeCount = 16;

        public Tape()
        {

        }
        public int Position { get; set; }

        public void Set(string input, char emptySymbol)
        {
            _emptySymbol = emptySymbol;
            Position = 0;
            string tempRightSide = input;
            string tempLeftSide = "";

            tempRightSide += new string(emptySymbol, Tape.ExtendSize);
            tempLeftSide += new string(emptySymbol, Tape.ExtendSize);

            _leftSide = new StringBuilder(tempLeftSide);
            _rightSide = new StringBuilder(tempRightSide);
        }

        public string GetTapeVisiblePart()
        {
            string result = "";

            if (Position > Tape.TapeCount / 2)
            {
                int begin = Position - Tape.TapeCount / 2;
                int count = Tape.TapeCount;
                result += _rightSide.ToString().Substring(begin, count);
            }
            else if (-Position > Tape.TapeCount / 2)
            {
                int begin = -Position - Tape.TapeCount / 2;
                int count = Tape.TapeCount;
                result = _leftSide.ToString().Substring(begin, count);

                var charArray = result.ToCharArray();
                Array.Reverse(charArray);
                result = new string(charArray);
            }
            else
            {
                string leftSidePart = _leftSide.ToString().Substring(0, Tape.TapeCount / 2 - Position);
                string rightSidePart = _rightSide.ToString().Substring(0, Tape.TapeCount / 2 + Position);
                var charArray = leftSidePart.ToCharArray();
                Array.Reverse(charArray);
                leftSidePart = new string(charArray);

                result = leftSidePart + rightSidePart;
            }

            return result;
        }

        public char Get(int position)
        {
            if (position >= 0)
            {
                return _rightSide[position];
            }

            return _leftSide[-position - 1];
        }

        public void Write(string value)
        {
            if (Position >= 0)
            {
                _rightSide[Position] = value[0];
            }
            else
            {
                _leftSide[-Position - 1] = value[0];
            }
        }

        public void Move(string direction)
        {
            int where;

            switch (direction)
            {
                case "<":
                    {
                        where = -1;
                        break;
                    }
                case "@":
                    {
                        where = 0;
                        break;
                    }
                case ">":
                    {
                        where = 1;
                        break;
                    }
                default:
                    {
                        throw new Exception($"Unknown direction {direction}");
                    }
            }

            Position += where;

            if ((Position >= 0) && (Position >= _rightSide.Length - Tape.TapeCount / 2))
            {
                string temp = _rightSide.ToString();
                temp += new string(_emptySymbol, Tape.ExtendSize);

                _rightSide = new StringBuilder(temp);
            }
            else if (-Position >= _leftSide.Length - Tape.TapeCount / 2)
            {
                string temp = _leftSide.ToString();
                temp += new string(_emptySymbol, Tape.ExtendSize);

                _leftSide = new StringBuilder(temp);
            }
        }
    }
}
