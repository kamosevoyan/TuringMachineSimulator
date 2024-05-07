using System;
using System.Collections.Generic;
using System.Linq;

namespace TuringMachineSimulator
{
    /// <summary>
    /// Enum describing the types of lexer tokenss
    /// </summary>
    enum TokenType
    {
        Main, Repeat, Until, Do, While, If, Else, Write, Left, Right, Exit, Error, LeftBrace, RightBrace, LeftParenthesis, RightParenthesis,
        Colon, Semicolon, Symbol, Comma, End, Continue, Break, Not, Function, Name, GlobalSymbols,
        Switch, Case, Default
    }
    /// <summary>
    /// Lexer for designed language
    /// </summary>
    internal class Lexer
    {   
        private int _position;
        private readonly Dictionary<string, TokenType> _keyMap;
        private readonly string _tokenPattern;
        private string _reader;
        private List<string> _tokens;

        public Lexer()
        {
            _keyMap = new Dictionary<string, TokenType>
            {
                ["main"] = TokenType.Main,
                ["while"] = TokenType.While,
                ["repeat"] = TokenType.Repeat,
                ["until"] = TokenType.Until,
                ["do"] = TokenType.Do,
                ["if"] = TokenType.If,
                ["else"] = TokenType.Else,
                ["write"] = TokenType.Write,
                ["left"] = TokenType.Left,
                ["right"] = TokenType.Right,
                ["exit"] = TokenType.Exit,
                ["error"] = TokenType.Error,
                ["not"] = TokenType.Not,

                ["continue"] = TokenType.Continue,
                ["break"] = TokenType.Break,

                ["{"] = TokenType.LeftBrace,
                ["}"] = TokenType.RightBrace,
                ["("] = TokenType.LeftParenthesis,
                [")"] = TokenType.RightParenthesis,
                [";"] = TokenType.Semicolon,
                [":"] = TokenType.Colon,
                [","] = TokenType.Comma,

                ["global_symbols"] = TokenType.GlobalSymbols,
                ["switch"] = TokenType.Switch,
                ["case"] = TokenType.Case,
                ["default"] = TokenType.Default
            };

            _tokenPattern = @"\w(\w\d)+";

            _keyMap["function"] = TokenType.Function;
        }
        public List<(int, int)> TokenPositions { get; private set; }
        public string CurrentValue { get; private set; }
        public TokenType CurrentKeyword { get; private set; }

        public void SetStream(string stream)
        {
            TokenPositions = new List<(int, int)> { };
            _tokens = new List<string> { };
            _position = 0;

            _reader = stream;
            CorrectStream();
        }
        public bool NextToken()
        {
            if (_position >= _tokens.Count)
            {
                return false;
            }

            string temp = _tokens[_position++].Trim();

            if (_keyMap.ContainsKey(temp))
            {
                CurrentKeyword = _keyMap[temp];
                CurrentValue = temp;
            }
            else if (temp.Length == 1)
            {
                CurrentKeyword = TokenType.Symbol;
                CurrentValue = temp;
            }
            else
            {
                if (!IsCorrectToken(temp))
                {
                    throw new SyntaxErrorException($"Unacceptable name {temp} in {TokenPositions[_position - 1]}");
                }
                CurrentKeyword = TokenType.Name;
                CurrentValue = temp;
            }

            return true;
        }
        private bool IsCorrectToken(string token)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(_tokenPattern);
            System.Text.RegularExpressions.Match match = regex.Match(token);

            return match.Success;
        }
        private void CorrectStream()
        {
            string temp;
            temp = _reader;
            string[] lines;

            temp = System.Text.RegularExpressions.Regex.Replace(temp, @"\(", " ( ");
            temp = System.Text.RegularExpressions.Regex.Replace(temp, @"\)", " ) ");
            temp = System.Text.RegularExpressions.Regex.Replace(temp, @"\{", " { ");
            temp = System.Text.RegularExpressions.Regex.Replace(temp, @"\}", " } ");
            temp = System.Text.RegularExpressions.Regex.Replace(temp, @",", " , ");
            temp = System.Text.RegularExpressions.Regex.Replace(temp, @";", " ; ");
            temp = System.Text.RegularExpressions.Regex.Replace(temp, @":", " : ");

            temp = System.Text.RegularExpressions.Regex.Replace(temp, @"//.*\n", " ");
            temp = System.Text.RegularExpressions.Regex.Replace(temp, @"(\/\*[^(?:\/\*)]*\*\/)", " ");
            temp = System.Text.RegularExpressions.Regex.Replace(temp, @"\t", "    ");


            lines = temp.Split('\n');

            int lineNumber = 0;

            foreach (string _line in lines)
            {
                string line = _line;
                line = System.Text.RegularExpressions.Regex.Replace(line, @"\s+", " ");
                line = line.Trim();

                string[] splittedLine = line.Split(' ');

                if ((splittedLine.Length == 1) && (splittedLine[0] == ""))
                {
                    ++lineNumber;
                    continue;
                }

                string[] trimmedTokens = Array.ConvertAll(splittedLine, s => s.Trim());
                _tokens = _tokens.Concat(trimmedTokens).ToList();

                foreach (string token in splittedLine)
                {
                    TokenPositions.Add((lineNumber, _line.IndexOf(token)));
                }
                ++lineNumber;
            }
        }
    }
}