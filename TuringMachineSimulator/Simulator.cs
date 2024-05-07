using System;
using System.Collections.Generic;
using System.Linq;

namespace TuringMachineSimulator
{
    /// <summary>
    /// The class representing Turing Machine Simulator
    /// </summary>
    public class Simulator
    {
        /// <summary>
        /// Enum describing the state of the simulator
        /// </summary>
        public enum MachineState
        {
            Running, Terminated, Failed
        }

        char _emptySymbol;
        readonly Dictionary<(string, char), string> _lambda;
        readonly Dictionary<(string, char), string> _delta;
        readonly Dictionary<(string, char), string> _nyu;
        readonly string _directions;
        string _initialState;
        string _haltState;
        List<string> _stateSymbols;
        string _alphabetSymbols;
        string _currentState;
        char _currentSymbol;

        public Tape tape;
        public int NumSteps { get; set; }
        public bool isFinished;

        public Simulator()
        {
            tape = new Tape();
            _directions = "<@>";

            _lambda = new Dictionary<(string, char), string> { };
            _delta = new Dictionary<(string, char), string> { };
            _nyu = new Dictionary<(string, char), string> { };

            isFinished = false;

        }
        public void SetInput(string input)
        {
            if (input.Length == 0)
            {
                throw new Exception("Expected non empty string.");
            }

            tape.Set(input, _emptySymbol);
            Reset();
            NumSteps = 0;
        }

        public void Reset()
        {
            _currentState = _initialState;
        }

        public void SetConfiguration(string input)
        {
            string alphabetSymbols = "";
            List<string> stateSymbols = new List<string>();
            string inOutAlphabet;
            string states;
            string lambdaDeltaNyu;

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

            this._alphabetSymbols = alphabetSymbols;
            this._stateSymbols = stateSymbols;


            int rows = stateSymbols.Count;
            int columns = alphabetSymbols.Length;

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
                        throw new Exception($"Error: Configuration values for state {stateSymbols[row]} and below are not given.");
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

                    string[] splitTokens = cleanedToken.Split(',');
                    leftToken = splitTokens[0];
                    rightToken = splitTokens[1];
                    dirToken = splitTokens[2];

                    if (!alphabetSymbols.Contains(leftToken))
                    {
                        throw new Exception($"Unknown symbol {leftToken} in {row} {column}");
                    }

                    if (!stateSymbols.Contains(rightToken))
                    {
                        throw new Exception($"Unknown state {rightToken} in {row} {column}");
                    }

                    if (!_directions.Contains(dirToken))
                    {
                        throw new Exception($"Unknown direction {dirToken} in {row} {column}");
                    }

                    (string, char) key = (stateSymbols[row], alphabetSymbols[column]);
                    _lambda[key] = rightToken;
                    _delta[key] = leftToken;
                    _nyu[key] = dirToken;

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
                _initialState = lines[lineNumber].Trim();
                ++lineNumber;
                if (_initialState.Length > 0)
                {
                    break;
                }

                if (lineNumber == lines.Count() - 1)
                {
                    throw new Exception($"Intial state symbol is not given.");
                }
            }

            if (!stateSymbols.Contains(_initialState))
            {
                throw new Exception($"Unknown initial state {_initialState}");
            }

            while (true)
            {
                _haltState = lines[lineNumber].Trim();
                ++lineNumber;
                if (_haltState.Length > 0)
                {
                    break;
                }

                if (lineNumber == lines.Count() - 1)
                {
                    throw new Exception($"Halt state symbol is not given");
                }
            }

            if (!stateSymbols.Contains(_haltState))
            {
                throw new Exception($"Unknown halt state {_haltState}");
            }

            while (true)
            {
                _emptySymbol = char.Parse(lines[lineNumber].Trim());
                ++lineNumber;

                if (_emptySymbol.ToString().Length > 0)
                {
                    break;
                }

                if (lineNumber == lines.Count() - 1)
                {
                    throw new Exception($"Halt state symbol is not given");
                }
            }

            if (!alphabetSymbols.Contains(_emptySymbol.ToString()))
            {
                throw new Exception($"Unknown empty symbol {_emptySymbol}");
            }
        }

        public MachineState Step()
        {
            _currentSymbol = tape.Get(tape.Position);

            (string, char) key = (_currentState, _currentSymbol);

            if (!(_lambda.ContainsKey(key) && _delta.ContainsKey(key) && _nyu.ContainsKey(key)))
            {
                return MachineState.Failed;
            }

            string newState = _lambda[key];
            string newSymbol = _delta[key];
            string move = _nyu[key];

            tape.Write(newSymbol);
            tape.Move(move);

            _currentState = newState;

            if (newState == _haltState)
            {
                return MachineState.Terminated;
            }

            NumSteps++;
            return MachineState.Running;
        }

        public string GetLayout()
        {
            return tape.GetTapeVisiblePart();
        }

    }
}
