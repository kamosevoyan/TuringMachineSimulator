using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TuringMachineSimulator;

namespace TuringMachineSimulator
{
    enum KEYWORD
    {
        MAIN, REPEAT, UNTIL, DO, WHILE, IF, ELSE, WRITE, LEFT, RIGHT, EXIT, ERROR, L_BR, R_BR, L_PAR, R_PAR,
        COLON, SEMICOLON, SYMBOL, COMMA, END, CONTINUE, BREAK, NOT, FUNCTION, NAME, GLOBAL_SYMBOLS,
        SWITCH, CASE, DEFAULT
    }
    internal class Lexer
    {
        private KEYWORD _keyword;
        private string _value;
        private int position;
        private Dictionary<string, KEYWORD> keyMap;
        private string tokenPattern;
        private string reader;
        private List<string> tokens;
        public List<(int, int)> tokenPositions;

        public Lexer()
        {
            this.keyMap = new Dictionary<string, KEYWORD>();
            this.keyMap["main"] = KEYWORD.MAIN;
            this.keyMap["while"] = KEYWORD.WHILE;
            this.keyMap["repeat"] = KEYWORD.REPEAT;
            this.keyMap["until"] = KEYWORD.UNTIL;
            this.keyMap["do"] = KEYWORD.DO;
            this.keyMap["if"] = KEYWORD.IF;
            this.keyMap["else"] = KEYWORD.ELSE;
            this.keyMap["write"] = KEYWORD.WRITE;
            this.keyMap["left"] = KEYWORD.LEFT;
            this.keyMap["right"] = KEYWORD.RIGHT;
            this.keyMap["exit"] = KEYWORD.EXIT;
            this.keyMap["error"] = KEYWORD.ERROR;
            this.keyMap["not"] = KEYWORD.NOT;

            this.keyMap["continue"] = KEYWORD.CONTINUE;
            this.keyMap["break"] = KEYWORD.BREAK;

            this.keyMap["{"] = KEYWORD.L_BR;
            this.keyMap["}"] = KEYWORD.R_BR;
            this.keyMap["("] = KEYWORD.L_PAR;
            this.keyMap[")"] = KEYWORD.R_PAR;
            this.keyMap[";"] = KEYWORD.SEMICOLON;
            this.keyMap[":"] = KEYWORD.COLON;
            this.keyMap[","] = KEYWORD.COMMA;


            this.keyMap["global_symbols"] = KEYWORD.GLOBAL_SYMBOLS;
            this.keyMap["switch"] = KEYWORD.SWITCH;
            this.keyMap["case"] = KEYWORD.CASE;
            this.keyMap["default"] = KEYWORD.DEFAULT;

            this.tokenPattern = @"\w(\w\d)+";

            this.keyMap["function"] = KEYWORD.FUNCTION;
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
                this._keyword = this.keyMap[temp];
                this._value = temp;
            }
            else if (temp.Length == 1)
            {
                this._keyword = KEYWORD.SYMBOL;
                this._value = temp;
            }
            else
            {
                if (!IsCorrectToken(temp))
                {
                    throw new SyntaxErrorException($"Unacceptable name {temp} in {this.tokenPositions[this.position - 1]}");
                }
                this._keyword = KEYWORD.NAME;
                this._value = temp;
            }

            return true;
        }
        public string CurrentValue
        {
            get
            {
                return this._value;
            }
        }
        public KEYWORD CurrentKeyword
        {
            get
            {
                return this._keyword;
            }
        }
    }
}