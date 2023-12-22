using System;
using System.Collections.Generic;
using System.Linq;

namespace TuringMachineSimulator
{
    enum TokenType
    {
        Main, Repeat, Until, Do, While, If, Else, Write, Left, Right, Exit, Error, LeftBrace, RightBrace, LeftParenthesis, RightParenthesis,
        Colon, Semicolon, Symbol, Comma, End, Continue, Break, Not, Function, Name, GlobalSymbols,
        Switch, Case, Default
    }
    internal class Lexer
    {
        private TokenType currentKeyword;
        private string currentValue;
        private int position;
        private readonly Dictionary<string, TokenType> keyMap;
        private readonly string tokenPattern;
        private string reader;
        private List<string> tokens;
        public List<(int, int)> tokenPositions;

        public Lexer()
        {
            this.keyMap = new Dictionary<string, TokenType>();
            this.keyMap["main"] = TokenType.Main;
            this.keyMap["while"] = TokenType.While;
            this.keyMap["repeat"] = TokenType.Repeat;
            this.keyMap["until"] = TokenType.Until;
            this.keyMap["do"] = TokenType.Do;
            this.keyMap["if"] = TokenType.If;
            this.keyMap["else"] = TokenType.Else;
            this.keyMap["write"] = TokenType.Write;
            this.keyMap["left"] = TokenType.Left;
            this.keyMap["right"] = TokenType.Right;
            this.keyMap["exit"] = TokenType.Exit;
            this.keyMap["error"] = TokenType.Error;
            this.keyMap["not"] = TokenType.Not;

            this.keyMap["continue"] = TokenType.Continue;
            this.keyMap["break"] = TokenType.Break;

            this.keyMap["{"] = TokenType.LeftBrace;
            this.keyMap["}"] = TokenType.RightBrace;
            this.keyMap["("] = TokenType.LeftParenthesis;
            this.keyMap[")"] = TokenType.RightParenthesis;
            this.keyMap[";"] = TokenType.Semicolon;
            this.keyMap[":"] = TokenType.Colon;
            this.keyMap[","] = TokenType.Comma;


            this.keyMap["global_symbols"] = TokenType.GlobalSymbols;
            this.keyMap["switch"] = TokenType.Switch;
            this.keyMap["case"] = TokenType.Case;
            this.keyMap["default"] = TokenType.Default;

            this.tokenPattern = @"\w(\w\d)+";

            this.keyMap["function"] = TokenType.Function;
        }
        public void SetStream(string stream)
        {
            this.tokenPositions = new List<(int, int)> { };
            this.tokens = new List<string> { };
            this.position = 0;

            this.reader = stream;
            this.CorrectStream();
        }
        private bool IsCorrectToken(string token)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(this.tokenPattern);
            System.Text.RegularExpressions.Match match = regex.Match(token);

            return match.Success;
        }
        private void CorrectStream()
        {
            string temp;
            temp = reader;
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
                this.tokens = this.tokens.Concat(trimmedTokens).ToList();

                foreach (string token in splittedLine)
                {
                    this.tokenPositions.Add((lineNumber, _line.IndexOf(token)));
                }
                ++lineNumber;
            }
        }
        public bool NextToken()
        {
            if (this.position >= this.tokens.Count)
            {
                return false;
            }
            string temp = this.tokens[this.position++].Trim();

            if (this.keyMap.ContainsKey(temp))
            {
                this.currentKeyword = this.keyMap[temp];
                this.currentValue = temp;
            }
            else if (temp.Length == 1)
            {
                this.currentKeyword = TokenType.Symbol;
                this.currentValue = temp;
            }
            else
            {
                if (!IsCorrectToken(temp))
                {
                    throw new SyntaxErrorException($"Unacceptable name {temp} in {this.tokenPositions[this.position - 1]}");
                }
                this.currentKeyword = TokenType.Name;
                this.currentValue = temp;
            }

            return true;
        }
        public string CurrentValue
        {
            get
            {
                return this.currentValue;
            }
        }
        public TokenType CurrentKeyword
        {
            get
            {
                return this.currentKeyword;
            }
        }
    }
}