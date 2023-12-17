using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TuringMachineSimulator
{
    public class Tape
    {
        StringBuilder leftSide, rightSide;
        char emptySymbol;

        public int position;
        const int extendSize = 16;
        const int tapeCount = 16;

        public Tape()
        {

        }

        public void set(string input, char emptySymbol)
        {
            this.emptySymbol = emptySymbol;
            this.position = 0;
            string tempRightSide = input;
            string tempLeftSide = "";

            tempRightSide += new string(emptySymbol, Tape.extendSize);
            tempLeftSide += new string(emptySymbol, Tape.extendSize);

            this.leftSide = new StringBuilder(tempLeftSide);
            this.rightSide = new StringBuilder(tempRightSide);
        }

        public string getTapeVisiblePart()
        {
            string result = "";

            if (this.position > Tape.tapeCount / 2)
            {
                int begin = this.position - Tape.tapeCount / 2;
                int count = Tape.tapeCount;
                result += this.rightSide.ToString().Substring(begin, count);
            }
            else if (-this.position > Tape.tapeCount / 2)
            {
                int begin = -this.position - Tape.tapeCount / 2;
                int count = Tape.tapeCount;
                result = this.leftSide.ToString().Substring(begin, count);

                var charArray = result.ToCharArray();
                Array.Reverse(charArray);
                result = new string(charArray);
            }
            else
            {
                string leftSidePart = "";
                string rightSidePart = "";

                leftSidePart = this.leftSide.ToString().Substring(0, Tape.tapeCount / 2 - this.position);
                rightSidePart = this.rightSide.ToString().Substring(0, Tape.tapeCount / 2 + this.position);

                var charArray = leftSidePart.ToCharArray();
                Array.Reverse(charArray);
                leftSidePart = new string(charArray);

                result = leftSidePart + rightSidePart;
            }

            return result;
        }

        public char get(int position)
        {
            if (position >= 0)
            {
                return this.rightSide[position];
            }

            return this.leftSide[-position - 1];
        }

        public void write(string value)
        {
            if (this.position >= 0)
            {
                this.rightSide[this.position] = value[0];
            }
            else
            {
                this.leftSide[-position - 1] = value[0];
            }
        }

        public void move(string direction)
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

            this.position += where;

            if ((this.position >= 0) && (this.position >= this.rightSide.Length - Tape.tapeCount / 2))
            {
                string temp = this.rightSide.ToString();
                temp += new string(this.emptySymbol, Tape.extendSize);

                this.rightSide = new StringBuilder(temp);
            }
            else if (-this.position >= this.leftSide.Length - Tape.tapeCount / 2)
            {
                string temp = this.leftSide.ToString();
                temp += new string(this.emptySymbol, Tape.extendSize);

                this.leftSide = new StringBuilder(temp);
            }
        }
    }
    public class Simulator
    {
        public Tape tape;
        public char emptySymbol;
        string initialState;
        string haltState;

        List<string> stateSymbols;
        string alphabetSymbols;
        string directions;

        Dictionary<Tuple<string, char>, string> lambda;
        Dictionary<Tuple<string, char>, string> delta;
        Dictionary<Tuple<string, char>, string> nyu;

        string currentState;
        char currentSymbol;

        public bool isFinished;

        public Simulator()
        {
            this.tape = new Tape();
            this.directions = "<@>";

            this.lambda = new Dictionary<Tuple<string, char>, string> { };
            this.delta = new Dictionary<Tuple<string, char>, string> { };
            this.nyu = new Dictionary<Tuple<string, char>, string> { };

            this.isFinished = false;

        }
        public void setInput(string input)
        {
            if (input.Length == 0)
            {
                throw new Exception("Expected non empty string.");
            }

            this.tape.set(input, this.emptySymbol);
            this.reset();
        }

        public void reset()
        {
            this.currentState = this.initialState;
        }

        public void setConfiguration(string input)
        {
            string alphabetSymbols = "";
            List<string> stateSymbols = new List<string>();
            string inOutAlphabet = "", states = "", lambdaDeltaNyu = "";


            string[] lines = input.Split(new char[] { '\n' });
            int lineNumber = 0;

            while (true)
            {
                inOutAlphabet = lines[lineNumber].Trim();
                ++lineNumber;
                if (inOutAlphabet.Length > 0)
                {
                    break;
                }
            }

            while (true)
            {
                states = lines[lineNumber].Trim();
                ++lineNumber;
                if (states.Length > 0)
                {
                    break;
                }
            }

            foreach (var token in inOutAlphabet.Split(','))
            {
                string cleanedToken = token.Trim();

                if (alphabetSymbols.Contains(cleanedToken))
                {
                    throw new Exception($"Input/Output alphabet symbol {cleanedToken} is already given.");
                }
                alphabetSymbols += cleanedToken;
            }

            foreach (var token in states.Split(','))
            {
                string cleanedToken = token.Trim();

                if (stateSymbols.Contains(cleanedToken))
                {
                    throw new Exception($"State {cleanedToken} is already given.");
                }
                stateSymbols.Add(cleanedToken);
            }

            this.alphabetSymbols = alphabetSymbols;
            this.stateSymbols = stateSymbols;


            int rows = this.stateSymbols.Count;
            int columns = this.alphabetSymbols.Length;

            string leftToken, rightToken, dirToken;

            for (int row = 0; row < rows - 1; row++)
            {
                int column = 0;
                while (true)
                {
                    lambdaDeltaNyu = lines[lineNumber].Trim();
                    ++lineNumber;
                    if (lambdaDeltaNyu.Length > 0)
                    {
                        break;
                    }

                    if (lineNumber == lines.Count() - 1)
                    {
                        throw new Exception($"Error: Configuration values for state {this.stateSymbols[row]} and below are not given.");
                    }
                }

                foreach (var token in lambdaDeltaNyu.Split('|'))
                {
                    string cleanedToken = token.Trim();

                    if (cleanedToken == "X")
                    {
                        column++;
                        continue;
                    }

                    leftToken = cleanedToken.Split(',')[0];
                    rightToken = cleanedToken.Split(',')[1];
                    dirToken = cleanedToken.Split(',')[2];

                    if (!this.alphabetSymbols.Contains(leftToken))
                    {
                        throw new Exception($"Unknown symbol {leftToken} in {row} {column}");
                    }

                    if (!this.stateSymbols.Contains(rightToken))
                    {
                        throw new Exception($"Unknown state {rightToken} in {row} {column}");
                    }

                    if (!this.directions.Contains(dirToken))
                    {
                        throw new Exception($"Unknown direction {dirToken} in {row} {column}");
                    }

                    Tuple<string, char> key = new Tuple<string, char>(this.stateSymbols[row], this.alphabetSymbols[column]);
                    this.lambda[key] = rightToken;
                    this.delta[key] = leftToken;
                    this.nyu[key] = dirToken;

                    column++;
                }

                if (column < columns)
                {
                    throw new Exception($"Less values than expected in {row}, {column}.");
                }
                if (column > columns)
                {
                    throw new Exception($"More values in {row} than expected.");
                }

            }
            while (true)
            {
                this.initialState = lines[lineNumber].Trim();
                ++lineNumber;
                if (initialState.Length > 0)
                {
                    break;
                }

                if (lineNumber == lines.Count() - 1)
                {
                    throw new Exception($"Intial state symbol is not given.");
                }
            }

            if (!this.stateSymbols.Contains(this.initialState))
            {
                throw new Exception($"Unknown initial state {this.initialState}");
            }

            while (true)
            {
                this.haltState = lines[lineNumber].Trim();
                ++lineNumber;
                if (haltState.Length > 0)
                {
                    break;
                }

                if (lineNumber == lines.Count() - 1)
                {
                    throw new Exception($"Halt state symbol is not given");
                }
            }

            if (!this.stateSymbols.Contains(this.haltState))
            {
                throw new Exception($"Unknown halt state {this.haltState}");
            }

            while (true)
            {
                this.emptySymbol = char.Parse(lines[lineNumber].Trim());
                ++lineNumber;

                if (emptySymbol.ToString().Length > 0)
                {
                    break;
                }

                if (lineNumber == lines.Count() - 1)
                {
                    throw new Exception($"Halt state symbol is not given");
                }
            }

            if (!this.alphabetSymbols.Contains(this.emptySymbol.ToString()))
            {
                throw new Exception($"Unknown empty symbol {this.emptySymbol}");
            }
        }

        public bool step()
        {
            this.currentSymbol = this.tape.get(this.tape.position);

            Tuple<string, char> key = new Tuple<string, char>(currentState, currentSymbol);

            string newState = this.lambda[key];
            string newSymbol = this.delta[key];
            string move = this.nyu[key];

            this.tape.write(newSymbol);
            this.tape.move(move);

            this.currentState = newState;

            if (newState == this.haltState)
            {
                return false;
            }

            return true;
        }

        public string getLayout()
        {
            return this.tape.getTapeVisiblePart();
        }

    }
}
