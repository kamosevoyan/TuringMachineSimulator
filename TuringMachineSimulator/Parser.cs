using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TuringMachineSimulator
{
    /// <summary>
    /// Parser of the designed language
    /// </summary>
    internal class Parser
    {
        private int _tokenCount;
        private readonly Lexer _lexer;
        private List<TokenType> _tokens;
        private List<string> _values;

        public Parser()
        {
            _lexer = new Lexer();
        }
        public SymbolNode GlobalSymbols { get; private set; }               

        public void SetStream(string stream)
        {
            _tokens = new List<TokenType>();
            _values = new List<string>();
            GlobalSymbols = new SymbolNode(NodeType.SymbolList);
            _tokenCount = 0;

            _lexer.SetStream(stream);
            GetAllTokens();
        }

        private (int, int) GetCurrentTokenPosition()
        {
            return _lexer.TokenPositions[_tokenCount - 1];
        }

        public void NextToken()
        {
            _tokenCount++;
        }

        public MainNode Parse()
        {
            GetGlobalSymbols();
            NextToken();

            MainNode node = ParseMainStatement();
            node.Root = null;
            NextToken();

            if (GetOffsetToken(0) != TokenType.End)
            {
                throw new SyntaxErrorException("Invalid syntax");
            }

            return node;
        }
        private SymbolNode ParseSymbolsStatement()
        {
            SymbolNode node = new SymbolNode(NodeType.SymbolList);
            bool isGlobal;
            bool hasNot;

            if (GlobalSymbols.Symbols.Length == 0)
            {
                isGlobal = true;
            }
            else
            {
                isGlobal = false;
            }

            if (GetOffsetToken(1) == TokenType.Not)
            {
                NextToken();

                if (isGlobal)
                {
                    throw new SyntaxErrorException("Main statement should not have 'not' attribute.");
                }
                hasNot = true;
            }
            else
            {
                hasNot = false;
            }

            NextToken();

            if (GetOffsetToken(0) != TokenType.LeftParenthesis)
            {
                throw new SyntaxErrorException($"Expected left parenthesis in {GetCurrentTokenPosition()}.");
            }

            bool lastWasComma = false;
            bool firsWasSymbol = false;

            while (true)
            {
                NextToken();

                if (GetOffsetToken(0) == TokenType.RightParenthesis)
                {
                    if (GetOffsetToken(-1) == TokenType.LeftParenthesis)
                    {
                        throw new SyntaxErrorException($"Empty parenthesis statement in {GetCurrentTokenPosition()}.");
                    }
                    break;
                }


                if ((GetOffsetToken(0) != TokenType.Symbol) && (!firsWasSymbol))
                {
                    throw new SyntaxErrorException($"Expected symbol before comma in {GetCurrentTokenPosition()}.");
                }

                firsWasSymbol = true;

                if (GetOffsetToken(0) == TokenType.Symbol)
                {
                    if (node.Symbols.Contains(GetOffsetTokenValue(0)))
                    {
                        throw new SyntaxErrorException($"Symbol {GetOffsetTokenValue(0)} is already given in {GetCurrentTokenPosition()}.");
                    }

                    if (!isGlobal)
                    {
                        if (!GlobalSymbols.Symbols.Contains(GetOffsetTokenValue(0)))
                        {
                            throw new SyntaxErrorException($"The symbol {GetOffsetTokenValue(0)} is not given in main symbols in {GetCurrentTokenPosition()}.");
                        }
                    }

                    node.Symbols += GetOffsetTokenValue(0);
                    lastWasComma = false;
                }
                else if (GetOffsetToken(0) == TokenType.Comma)
                {
                    lastWasComma = true;
                    continue;
                }
                else
                {
                    throw new SyntaxErrorException($"Expected symbol in {GetCurrentTokenPosition()}.");
                }
            }

            if (lastWasComma)
            {
                throw new SyntaxErrorException($"Expected symbol after comma in {GetCurrentTokenPosition()}.");
            }

            node.HasNegation = hasNot;

            return node;
        }

        private void GetGlobalSymbols()
        {
            NextToken();

            if (GetOffsetToken(0) != TokenType.GlobalSymbols)
            {
                throw new SyntaxErrorException("Global symbols are not given.");
            }

            SymbolNode node = ParseSymbolsStatement();

            if (!node.Symbols.Contains("_"))
            {
                throw new SyntaxErrorException("Global symbols should contain empty '_' symbol.");
            }

            GlobalSymbols = node;
        }

        private MainNode ParseMainStatement()
        {
            if (GetOffsetToken(0) != TokenType.Main)
            {
                throw new SyntaxErrorException("Expected a statement.");
            }

            NextToken();

            if ((GetOffsetToken(0) != TokenType.LeftParenthesis) || (GetOffsetToken(1) != TokenType.RightParenthesis))
            {
                throw new SyntaxErrorException($"Incorrect statement in {GetCurrentTokenPosition()}.");
            }

            NextToken();
            MainNode node = new MainNode{Symbols = GlobalSymbols};

            NextToken();

            if (GetOffsetToken(0) != TokenType.LeftBrace)
            {
                throw new SyntaxErrorException("Main statement should only contain block statement.");
            }

            node.Statement = ParseStatement(node);
            return node;
        }

        private Node ParseStatement(Node root)
        {

            if (GetOffsetToken(0) == TokenType.If)
            {
                return ParseIfStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.While)
            {
                return ParseWhileStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Do)
            {
                return ParseDoWhileStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Repeat)
            {
                return ParseRepeatUntilStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.LeftBrace)
            {
                return ParseBlockStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Left)
            {
                return ParseLeftStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Right)
            {
                return ParseRightStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Exit)
            {
                return ParseExitStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Error)
            {
                return ParseErrorStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Write)
            {
                return ParseWriteStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Continue)
            {
                return ParseContinueStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Break)
            {
                return ParseBreakStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Switch)
            {
                return ParseSwitchStatement(root);
            }
            if (GetOffsetToken(0) == TokenType.Case)
            {
                throw new SyntaxErrorException($"Case statement should only be in switch statement in {GetCurrentTokenPosition()}.");
            }
            if (GetOffsetToken(0) == TokenType.Default)
            {
                throw new SyntaxErrorException($"Default statement should only be in switch statement in {GetCurrentTokenPosition()}.");
            }
            if (GetOffsetToken(0) == TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Semicolon should follow by an primary statement in {GetCurrentTokenPosition()}.");
            }
            if (GetOffsetToken(0) == TokenType.RightBrace)
            {
                throw new SyntaxErrorException($"Empty single statement in {GetCurrentTokenPosition()}.");
            }
            if (GetOffsetToken(0) == TokenType.Else)
            {
                throw new SyntaxErrorException($"Else statement should only be followed by an if statement in  in {GetCurrentTokenPosition()}.");
            }
            if (GetOffsetToken(0) == TokenType.End)
            {
                throw new SyntaxErrorException("Expected statement");
            }
            if (GetOffsetToken(0) == TokenType.Main)
            {
                throw new SyntaxErrorException($"Main statement should occur only once, {GetCurrentTokenPosition()}.");
            }

            throw new SyntaxErrorException($"Unknow statement {GetOffsetToken(0)} in {GetCurrentTokenPosition()}.");

        }

        private Node ParseIfStatement(Node root)
        {
            IfNode firstNode = new IfNode{Root = root};

            SymbolNode symbolNode = ParseSymbolsStatement();
            NextToken();

            Node node0 = ParseStatement(firstNode);

            if (GetOffsetToken(1) == TokenType.Else)
            {
                NextToken();
                NextToken();

                firstNode.Statement = null;

                IfElseNode node = new IfElseNode{Root = root};

                Node node1 = ParseStatement(node);
                node0.Root = node;

                node.IfStatement = node0;
                node.ElseStatement = node1;
                node.Symbols = symbolNode;

                return node;
            }

            firstNode.Statement = node0;
            firstNode.Symbols = symbolNode;
            return firstNode;
        }

        private Node ParseWhileStatement(Node root)
        {
            WhileNode node = new WhileNode{Root = root};

            SymbolNode symbolNode = ParseSymbolsStatement();

            NextToken();

            Node node0 = ParseStatement(node);
            node.Statement = node0;
            node.Symbols = symbolNode;
            return node;
        }

        private Node ParseDoWhileStatement(Node root)
        {
            DoWhileNode node = new DoWhileNode{Root = root};
            NextToken();

            Node node0 = ParseStatement(node);

            NextToken();

            if (GetOffsetToken(0) != TokenType.While)
            {
                throw new SyntaxErrorException($"Incorrect Do while statement in {GetCurrentTokenPosition()}.");
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            node.Statement = node0;
            node.Symbols = symbolNode;
            return node;

        }
        private Node ParseRepeatUntilStatement(Node root)
        {
            RepeatUntilNode node = new RepeatUntilNode{Root = root};
            NextToken();

            Node node0 = ParseStatement(node);
            NextToken();

            if (GetOffsetToken(0) != TokenType.Until)
            {
                throw new SyntaxErrorException($"Incorrect Repeat/Until statement in {GetCurrentTokenPosition()}.");
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}");
            }

            node.Statement = node0;
            node.Symbols = symbolNode;
            return node;
        }

        private Node ParseBlockStatement(Node root)
        {
            BlockNode node = new BlockNode{Root = root};

            NextToken();

            if (GetOffsetToken(0) == TokenType.RightBrace)
            {
                throw new SyntaxErrorException($"Empty block statement in {GetCurrentTokenPosition()}.");
            }

            while (GetOffsetToken(0) != TokenType.RightBrace)
            {
                Node node0 = ParseStatement(node);
                node.Statements.Add(node0);
                NextToken();
            }

            if ((node as BlockNode).Root.Type == NodeType.Main)
            {
                int j = (node as BlockNode).Statements.Count - 1;

                if (((node as BlockNode).Statements[j].Type != NodeType.Exit) &&
                ((node as BlockNode).Statements[j].Type != NodeType.Error))
                {
                    node.Statements.Add(new PrimaryNode(NodeType.Exit));
                }
            }
            return node;

        }

        private Node ParseLeftStatement(Node root)
        {
            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(NodeType.Left);
        }

        private Node ParseRightStatement(Node root)
        {
            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(NodeType.Right);
        }

        private Node ParseExitStatement(Node root)
        {
            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(NodeType.Exit);
        }

        private Node ParseErrorStatement(Node root)
        {
            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(NodeType.Error);
        }

        private Node ParseWriteStatement(Node root)
        {
            SymbolNode symbolNode = ParseSymbolsStatement();

            if (symbolNode.Symbols.Length > 1)
            {
                throw new SyntaxErrorException($"Expected only one symbol in write statement in {GetCurrentTokenPosition()}.");
            }

            WriteNode node = new WriteNode{Symbol = symbolNode.Symbols[0]};

            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return node;
        }

        private bool IsInLoop(Node node)
        {
            Node it = node;

            while (
                (it.Root.Type != NodeType.While) &&
                (it.Root.Type != NodeType.DoWhile) &&
                (it.Root.Type != NodeType.RepeatUntil) &&
                (it.Root.Type != NodeType.Main)
                )
            {
                it = it.Root;
            }

            if (it.Root.Type == NodeType.Main)
            {
                return false;
            }

            (node as FlowControllNode).OwnerLoop = it.Root;
            return true;
        }
        private Node ParseContinueStatement(Node root)
        {
            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");

            }

            FlowControllNode node = new FlowControllNode(NodeType.Continue){Root = root};

            if (!IsInLoop(node))
            {
                throw new SyntaxErrorException("Flow control statement continue should only be within a loop");
            }

            return node;
        }

        private Node ParseBreakStatement(Node root)
        {
            NextToken();

            if (GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");

            }

            FlowControllNode node = new FlowControllNode(NodeType.Break){Root = root};

            if (!IsInLoop(node))
            {
                throw new SyntaxErrorException($"Flow control statement continue should only be within a loop, {GetCurrentTokenPosition()}.");
            }

            return node;
        }

        private Node ParseSwitchStatement(Node root)
        {
            NextToken();

            if ((GetOffsetToken(0) != TokenType.LeftParenthesis) ||
                (GetOffsetToken(1) != TokenType.RightParenthesis))
            {
                throw new SyntaxErrorException($"Incorrect statement in {GetCurrentTokenPosition()}");
            }

            NextToken();
            NextToken();

            if (GetOffsetToken(0) != TokenType.LeftBrace)
            {
                throw new SyntaxErrorException($"Switch statement should only contain block statement, {GetCurrentTokenPosition()}.");
            }

            NextToken();

            SwitchNode node = new SwitchNode{Root = root};

            while (GetOffsetToken(0) != TokenType.RightBrace)
            {
                Node node0;
                if ((GetOffsetToken(0) == TokenType.Case) ||
                        (GetOffsetToken(0) == TokenType.Default))
                {
                    node0 = ParseCaseAndDefaultStatements(node);
                }
                else
                {
                    throw new SyntaxErrorException($"Only case statements and default statement are allowed in switch statement, {GetCurrentTokenPosition()}.");
                }

                char potentialKey = (node0 as CaseNode).Symbol;

                if (node.Branches.ContainsKey(potentialKey))
                {
                    throw new SyntaxErrorException($"Duplicated value {potentialKey} in switch statement in {GetCurrentTokenPosition()}.");
                }

                node.Branches[potentialKey] = node0;

                if (node0.Type == NodeType.Default)
                {
                    if (node.HasDefault)
                    {
                        throw new SyntaxErrorException($"Multiple default labels in one switch statement in {GetCurrentTokenPosition()}.");
                    }

                    node.HasDefault = true;
                    node.DefaultNode = node0;
                }

                node.Cases.Add(node0);

                NextToken();
            }

            return node;
        }

        private Node ParseCaseAndDefaultStatements(Node root)
        {
            if (GetOffsetToken(0) == TokenType.Default)
            {
                return ParseDefaultStatement(root);
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            if (symbolNode.Symbols.Length > 1)
            {
                throw new SyntaxErrorException($"Expected only one symbol in case statement in {GetCurrentTokenPosition()}.");
            }

            NextToken();

            CaseNode node = new CaseNode{Root = root, Symbol = symbolNode.Symbols[0]};

            NextToken();

            Node node0 = ParseStatement(node);
            node.Statement = node0;

            return node;

        }

        private Node ParseDefaultStatement(Node root)
        {
            NextToken();

            if (GetOffsetToken(0) != TokenType.Colon)
            {
                throw new SyntaxErrorException($"Expected colon after default statement in {GetCurrentTokenPosition()}.");
            }

            NextToken();

            DefaultNode node = new DefaultNode{Root = root};

            Node node0 = ParseStatement(node);
            node.Statement = node0;

            return node;
        }

        private TokenType GetOffsetToken(int offset)
        {
            if ((_tokenCount + offset < _tokens.Count) && (_tokenCount + offset >= 0))
            {
                return _tokens[_tokenCount + offset];
            }
            else
            {
                return _tokens[_tokenCount];
            }
        }

        private string GetOffsetTokenValue(int offset)
        {
            if ((_tokenCount + offset < _tokens.Count) && (_tokenCount + offset <= 0))
            {
                return _values[_tokenCount + offset];
            }
            else
            {
                return _values[_tokenCount];
            }
        }

        private void GetAllTokens()
        {
            bool state;
            do
            {
                _tokens.Add(_lexer.CurrentKeyword);
                _values.Add(_lexer.CurrentValue);
                state = _lexer.NextToken();
            }
            while (state);

            _tokens.Add(TokenType.End);
            _values.Add("");
        }
    }
}
