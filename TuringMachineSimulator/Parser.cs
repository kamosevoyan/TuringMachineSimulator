using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TuringMachineSimulator
{
    internal class Parser
    {
        private int tokenCount;
        private SymbolNode globalSymbols;
        private readonly Lexer lexer;
        private List<TokenType> tokens;
        private List<string> values;

        public SymbolNode GlobalSymbols
        {
            get 
            { 
                return globalSymbols; 
            }
            set 
            { 
                globalSymbols = value; 
            }
        }

        public Parser()
        {
            this.lexer = new Lexer();
        }

        private TokenType GetOffsetToken(int offset)
        {
            if ((this.tokenCount + offset < this.tokens.Count) && (this.tokenCount + offset >= 0))
            {
                return this.tokens[this.tokenCount + offset];
            }
            else
            {
                return this.tokens[this.tokenCount];
            }
        }

        private string offsetTokenValue(int offset)
        {
            if ((this.tokenCount + offset < this.tokens.Count) && (this.tokenCount + offset <= 0))
            {
                return this.values[this.tokenCount + offset];
            }
            else
            {
                return this.values[this.tokenCount];
            }
        }

        private void getAllTokens()
        {
            bool state;
            do
            {
                this.tokens.Add(this.lexer.CurrentKeyword);
                this.values.Add(this.lexer.CurrentValue);
                state = this.lexer.NextToken();
            }
            while (state);

            this.tokens.Add(TokenType.End);
            this.values.Add("");
        }

        public void SetStream(string stream)
        {
            this.tokens = new List<TokenType>();
            this.values = new List<string>();
            this.GlobalSymbols = new SymbolNode(NodeType.SymbolList);
            this.tokenCount = 0;

            this.lexer.SetStream(stream);
            this.getAllTokens();
        }

        private (int, int) GetCurrentTokenPosition()
        {
            return this.lexer.tokenPositions[this.tokenCount - 1];
        }

        public void NextToken()
        {
            this.tokenCount++;
        }

        public MainNode Parse()
        {
            this.GetGlobalSymbols();
            this.NextToken();

            MainNode node = ParseMainStatement();
            node.Root = null;
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.End)
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

            if (this.GlobalSymbols.Symbols.Length == 0)
            {
                isGlobal = true;
            }
            else
            {
                isGlobal = false;
            }

            if (this.GetOffsetToken(1) == TokenType.Not)
            {
                this.NextToken();

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

            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.LeftParenthesis)
            {
                throw new SyntaxErrorException($"Expected left parenthesis in {GetCurrentTokenPosition()}.");
            }

            bool lastWasComma = false;
            bool firsWasSymbol = false;

            while (true)
            {
                this.NextToken();

                if (this.GetOffsetToken(0) == TokenType.RightParenthesis)
                {
                    if (this.GetOffsetToken(-1) == TokenType.LeftParenthesis)
                    {
                        throw new SyntaxErrorException($"Empty parenthesis statement in {GetCurrentTokenPosition()}.");
                    }
                    break;
                }


                if ((this.GetOffsetToken(0) != TokenType.Symbol) && (!firsWasSymbol))
                {
                    throw new SyntaxErrorException($"Expected symbol before comma in {GetCurrentTokenPosition()}.");
                }

                firsWasSymbol = true;

                if (this.GetOffsetToken(0) == TokenType.Symbol)
                {
                    if (node.Symbols.Contains(this.offsetTokenValue(0)))
                    {
                        throw new SyntaxErrorException($"Symbol {this.offsetTokenValue(0)} is already given in {GetCurrentTokenPosition()}.");
                    }

                    if (!isGlobal)
                    {
                        if (!this.GlobalSymbols.Symbols.Contains(this.offsetTokenValue(0)))
                        {
                            throw new SyntaxErrorException($"The symbol {this.offsetTokenValue(0)} is not given in main symbols in {GetCurrentTokenPosition()}.");
                        }
                    }

                    node.Symbols = node.Symbols + this.offsetTokenValue(0);
                    lastWasComma = false;
                }
                else if (this.GetOffsetToken(0) == TokenType.Comma)
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

            node.HasNot = hasNot;

            return node;
        }

        private void GetGlobalSymbols()
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.GlobalSymbols)
            {
                throw new SyntaxErrorException("Global symbols are not given.");
            }

            SymbolNode node = ParseSymbolsStatement();

            if (!node.Symbols.Contains("_"))
            {
                throw new SyntaxErrorException("Global symbols should contain empty '_' symbol.");
            }

            this.GlobalSymbols = node;
        }

        private MainNode ParseMainStatement()
        {
            if (this.GetOffsetToken(0) != TokenType.Main)
            {
                throw new SyntaxErrorException("Expected a statement.");
            }

            this.NextToken();

            if ((this.GetOffsetToken(0) != TokenType.LeftParenthesis) || (this.GetOffsetToken(1) != TokenType.RightParenthesis))
            {
                throw new SyntaxErrorException($"Incorrect statement in {GetCurrentTokenPosition()}.");
            }

            this.NextToken();
            MainNode node = new MainNode();
            node.Symbols = this.GlobalSymbols;

            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.LeftBrace)
            {
                throw new SyntaxErrorException("Main statement should only contain block statement.");
            }

            node.Statement = ParseStatement(node);
            return node;
        }

        private Node ParseStatement(Node root)
        {

            if (this.GetOffsetToken(0) == TokenType.If)
            {
                return ParseIfStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.While)
            {
                return ParseWhileStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Do)
            {
                return ParseDoWhileStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Repeat)
            {
                return ParseRepeatUntilStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.LeftBrace)
            {
                return ParseBlockStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Left)
            {
                return ParseLeftStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Right)
            {
                return ParseRightStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Exit)
            {
                return ParseExitStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Error)
            {
                return ParseErrorStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Write)
            {
                return ParseWriteStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Continue)
            {
                return ParseContinueStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Break)
            {
                return ParseBreakStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Switch)
            {
                return ParseSwitchStatement(root);
            }
            if (this.GetOffsetToken(0) == TokenType.Case)
            {
                throw new SyntaxErrorException($"Case statement should only be in switch statement in {GetCurrentTokenPosition()}.");
            }
            if (this.GetOffsetToken(0) == TokenType.Default)
            {
                throw new SyntaxErrorException($"Default statement should only be in switch statement in {GetCurrentTokenPosition()}.");
            }
            if (this.GetOffsetToken(0) == TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Semicolon should follow by an primary statement in {GetCurrentTokenPosition()}.");
            }
            if (this.GetOffsetToken(0) == TokenType.RightBrace)
            {
                throw new SyntaxErrorException($"Empty single statement in {GetCurrentTokenPosition()}.");
            }
            if (this.GetOffsetToken(0) == TokenType.Else)
            {
                throw new SyntaxErrorException($"Else statement should only be followed by an if statement in  in {GetCurrentTokenPosition()}.");
            }
            if (this.GetOffsetToken(0) == TokenType.End)
            {
                throw new SyntaxErrorException("Expected statement");
            }
            if (this.GetOffsetToken(0) == TokenType.Main)
            {
                throw new SyntaxErrorException($"Main statement should occur only once, {GetCurrentTokenPosition()}.");
            }

            throw new SyntaxErrorException($"Unknow statement {this.GetOffsetToken(0)} in {GetCurrentTokenPosition()}.");

        }

        private Node ParseIfStatement(Node root)
        {
            IfNode firstNode = new IfNode();
            firstNode.Root = root;

            SymbolNode symbolNode = ParseSymbolsStatement();
            this.NextToken();

            Node node0 = ParseStatement(firstNode);

            if (this.GetOffsetToken(1) == TokenType.Else)
            {
                this.NextToken();
                this.NextToken();

                firstNode.Statement = null;

                IfElseNode node = new IfElseNode();
                node.Root = root;

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
            WhileNode node = new WhileNode();
            node.Root = root;
            SymbolNode symbolNode = ParseSymbolsStatement();

            this.NextToken();

            Node node0 = ParseStatement(node);
            node.Statement = node0;
            node.Symbols = symbolNode;
            return node;
        }

        private Node ParseDoWhileStatement(Node root)
        {
            DoWhileNode node = new DoWhileNode();
            node.Root = root;
            this.NextToken();

            Node node0 = ParseStatement(node);

            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.While)
            {
                throw new SyntaxErrorException($"Incorrect Do while statement in {GetCurrentTokenPosition()}.");
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            node.Statement = node0;
            node.Symbols = symbolNode;
            return node;

        }
        private Node ParseRepeatUntilStatement(Node root)
        {
            RepeatUntilNode node = new RepeatUntilNode();
            node.Root = root;
            this.NextToken();

            Node node0 = ParseStatement(node);
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Until)
            {
                throw new SyntaxErrorException($"Incorrect Repeat/Until statement in {GetCurrentTokenPosition()}.");
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}");
            }

            node.Statement = node0;
            node.Symbols = symbolNode;
            return node;
        }

        private Node ParseBlockStatement(Node root)
        {
            BlockNode node = new BlockNode();
            node.Root = root;

            this.NextToken();

            if (this.GetOffsetToken(0) == TokenType.RightBrace)
            {
                throw new SyntaxErrorException($"Empty block statement in {GetCurrentTokenPosition()}.");
            }

            while (this.GetOffsetToken(0) != TokenType.RightBrace)
            {
                Node node0 = ParseStatement(node);
                node.Statements.Add(node0);
                this.NextToken();
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
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(NodeType.Left);
        }

        private Node ParseRightStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(NodeType.Right);
        }

        private Node ParseExitStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(NodeType.Exit);
        }

        private Node ParseErrorStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
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

            WriteNode node = new WriteNode();
            node.Symbol = symbolNode.Symbols[0];

            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
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
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");

            }

            FlowControllNode node = new FlowControllNode(NodeType.Continue);
            node.Root = root;

            if (!IsInLoop(node))
            {
                throw new SyntaxErrorException("Flow control statement continue should only be within a loop");
            }

            return node;
        }

        private Node ParseBreakStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Semicolon)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");

            }

            FlowControllNode node = new FlowControllNode(NodeType.Break);
            node.Root = root;

            if (!IsInLoop(node))
            {
                throw new SyntaxErrorException($"Flow control statement continue should only be within a loop, {GetCurrentTokenPosition()}.");
            }

            return node;
        }

        private Node ParseSwitchStatement(Node root)
        {
            this.NextToken();

            if ((this.GetOffsetToken(0) != TokenType.LeftParenthesis) ||
                (this.GetOffsetToken(1) != TokenType.RightParenthesis))
            {
                throw new SyntaxErrorException($"Incorrect statement in {GetCurrentTokenPosition()}");
            }

            this.NextToken();
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.LeftBrace)
            {
                throw new SyntaxErrorException($"Switch statement should only contain block statement, {GetCurrentTokenPosition()}.");
            }

            this.NextToken();

            SwitchNode node = new SwitchNode();
            node.Root = root;

            while (this.GetOffsetToken(0) != TokenType.RightBrace)
            {
                Node node0;
                if ((this.GetOffsetToken(0) == TokenType.Case) ||
                        (this.GetOffsetToken(0) == TokenType.Default))
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

                this.NextToken();
            }

            return node;
        }

        private Node ParseCaseAndDefaultStatements(Node root)
        {
            if (this.GetOffsetToken(0) == TokenType.Default)
            {
                return ParseDefaultStatement(root);
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            if (symbolNode.Symbols.Length > 1)
            {
                throw new SyntaxErrorException($"Expected only one symbol in case statement in {GetCurrentTokenPosition()}.");
            }

            this.NextToken();

            CaseNode node = new CaseNode();
            node.Root = root;

            node.Symbol = symbolNode.Symbols[0];

            this.NextToken();

            Node node0 = ParseStatement(node);
            node.Statement = node0;

            return node;

        }

        private Node ParseDefaultStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != TokenType.Colon)
            {
                throw new SyntaxErrorException($"Expected colon after default statement in {GetCurrentTokenPosition()}.");
            }

            this.NextToken();

            DefaultNode node = new DefaultNode();
            node.Root = root;

            Node node0 = ParseStatement(node);
            node.Statement = node0;

            return node;
        }

    }
}
