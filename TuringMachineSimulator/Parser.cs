using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TuringMachineSimulator
{
    internal class Parser
    {
        private List<KEYWORD> tokens;
        private List<string> values;
        Lexer lexer;

        private int tokenCount;

        private SymbolNode _globalSymbols;

        public SymbolNode GlobalSymbols
        {
            get { return _globalSymbols; }
            set { _globalSymbols = value; }
        }

        public Parser()
        {
            this.lexer = new Lexer();
        }

        private KEYWORD GetOffsetToken(int offset)
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

            this.tokens.Add(KEYWORD.END);
            this.values.Add("");
        }

        public void SetStream(string stream)
        {
            // Initialize the corresponding fields
            this.tokens = new List<KEYWORD>();
            this.values = new List<string>();
            this.GlobalSymbols = new SymbolNode(TYPE.SYMBOL_LIST);
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
            node.root = null;
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.END)
            {
                throw new SyntaxErrorException("Invalid syntax");
            }

            return node;
        }
        private SymbolNode ParseSymbolsStatement()
        {
            SymbolNode node = new SymbolNode(TYPE.SYMBOL_LIST);
            bool isGlobal;
            bool hasNot;

            if (this.GlobalSymbols.symbols.Length == 0)
            {
                isGlobal = true;
            }
            else
            {
                isGlobal = false;
            }

            if (this.GetOffsetToken(1) == KEYWORD.NOT)
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

            if (this.GetOffsetToken(0) != KEYWORD.L_PAR)
            {
                throw new SyntaxErrorException($"Expected left parenthesis in {GetCurrentTokenPosition()}.");
            }

            bool lastWasComma = false;
            bool firsWasSymbol = false;

            while (true)
            {
                this.NextToken();

                if (this.GetOffsetToken(0) == KEYWORD.R_PAR)
                {
                    if (this.GetOffsetToken(-1) == KEYWORD.L_PAR)
                    {
                        throw new SyntaxErrorException($"Empty parenthesis statement in {GetCurrentTokenPosition()}.");
                    }
                    break;
                }


                if ((this.GetOffsetToken(0) != KEYWORD.SYMBOL) && (!firsWasSymbol))
                {
                    throw new SyntaxErrorException($"Expected symbol before comma in {GetCurrentTokenPosition()}.");
                }

                firsWasSymbol = true;

                if (this.GetOffsetToken(0) == KEYWORD.SYMBOL)
                {
                    if (node.symbols.Contains(this.offsetTokenValue(0)))
                    {
                        throw new SyntaxErrorException($"Symbol {this.offsetTokenValue(0)} is already given in {GetCurrentTokenPosition()}.");
                    }

                    if (!isGlobal)
                    {
                        if (!this.GlobalSymbols.symbols.Contains(this.offsetTokenValue(0)))
                        {
                            throw new SyntaxErrorException($"The symbol {this.offsetTokenValue(0)} is not given in main symbols in {GetCurrentTokenPosition()}.");
                        }
                    }

                    node.symbols = node.symbols + this.offsetTokenValue(0);
                    lastWasComma = false;
                }
                else if (this.GetOffsetToken(0) == KEYWORD.COMMA)
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

            node.hasNot = hasNot;

            return node;
        }

        private void GetGlobalSymbols()
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.GLOBAL_SYMBOLS)
            {
                throw new SyntaxErrorException("Global symbols are not given.");
            }

            SymbolNode node = ParseSymbolsStatement();

            if (!node.symbols.Contains("_"))
            {
                throw new SyntaxErrorException("Global symbols should contain empty '_' symbol.");
            }

            this.GlobalSymbols = node;
        }

        private MainNode ParseMainStatement()
        {
            var tmp = this.GetOffsetToken(0);
            if (this.GetOffsetToken(0) != KEYWORD.MAIN)
            {
                throw new SyntaxErrorException("Expected a statement.");
            }

            this.NextToken();

            if ((this.GetOffsetToken(0) != KEYWORD.L_PAR) || (this.GetOffsetToken(1) != KEYWORD.R_PAR))
            {
                throw new SyntaxErrorException($"Incorrect statement in {GetCurrentTokenPosition()}.");
            }

            this.NextToken();
            MainNode node = new MainNode();
            node.symbols = this.GlobalSymbols;

            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.L_BR)
            {
                throw new SyntaxErrorException("Main statement should only contain block statement.");
            }

            node.statement = ParseStatement(node);
            return node;
        }

        private Node ParseStatement(Node root)
        {

            if (this.GetOffsetToken(0) == KEYWORD.IF)
            {
                return ParseIfStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.WHILE)
            {
                return ParseWhileStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.DO)
            {
                return ParseDoWhileStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.REPEAT)
            {
                return ParseRepeatUntilStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.L_BR)
            {
                return ParseBlockStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.LEFT)
            {
                return ParseLeftStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.RIGHT)
            {
                return ParseRightStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.EXIT)
            {
                return ParseExitStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.ERROR)
            {
                return ParseErrorStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.WRITE)
            {
                return ParseWriteStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.CONTINUE)
            {
                return ParseContinueStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.BREAK)
            {
                return ParseBreakStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.SWITCH)
            {
                return ParseSwitchStatement(root);
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.CASE)
            {
                throw new SyntaxErrorException($"Case statement should only be in switch statement in {GetCurrentTokenPosition()}.");
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.DEFAULT)
            {
                throw new SyntaxErrorException($"Default statement should only be in switch statement in {GetCurrentTokenPosition()}.");
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Semicolon should follow by an primary statement in {GetCurrentTokenPosition()}.");
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.R_BR)
            {
                throw new SyntaxErrorException($"Empty single statement in {GetCurrentTokenPosition()}.");
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.ELSE)
            {
                throw new SyntaxErrorException($"Else statement should only be followed by an if statement in  in {GetCurrentTokenPosition()}.");
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.END)
            {
                throw new SyntaxErrorException("Expected statement");
            }
            else
            if (this.GetOffsetToken(0) == KEYWORD.MAIN)
            {
                throw new SyntaxErrorException($"Main statement should occur only once, {GetCurrentTokenPosition()}.");
            }
            else
            {
                throw new SyntaxErrorException($"Unknow statement {this.GetOffsetToken(0)} in {GetCurrentTokenPosition()}.");
            }

        }

        private Node ParseIfStatement(Node root)
        {
            Node node0 = null;
            Node node1 = null;

            IfNode firstNode = new IfNode();
            firstNode.root = root;

            SymbolNode symbolNode = ParseSymbolsStatement();
            this.NextToken();

            node0 = ParseStatement(firstNode);

            if (this.GetOffsetToken(1) == KEYWORD.ELSE)
            {
                this.NextToken();
                this.NextToken();

                firstNode.statement = null;

                IfElseNode node = new IfElseNode();
                node.root = root;

                node1 = ParseStatement(node);
                node0.root = node;

                node.ifStatement = node0;
                node.elseStatement = node1;
                node.symbols = symbolNode;

                return node;
            }

            firstNode.statement = node0;
            firstNode.symbols = symbolNode;
            return firstNode;
        }

        private Node ParseWhileStatement(Node root)
        {
            Node node0 = null;

            WhileNode node = new WhileNode();
            node.root = root;
            SymbolNode symbolNode = ParseSymbolsStatement();

            this.NextToken();

            node0 = ParseStatement(node);

            node.statement = node0;
            node.symbols = symbolNode;
            return node;
        }

        private Node ParseDoWhileStatement(Node root)
        {
            Node node0 = null;
            DoWhileNode node = new DoWhileNode();
            node.root = root;
            this.NextToken();

            node0 = ParseStatement(node);

            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.WHILE)
            {
                throw new SyntaxErrorException($"Incorrect Do while statement in {GetCurrentTokenPosition()}.");
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            node.statement = node0;
            node.symbols = symbolNode;
            return node;

        }
        private Node ParseRepeatUntilStatement(Node root)
        {
            Node node0 = null;

            RepeatUntilNode node = new RepeatUntilNode();
            node.root = root;
            this.NextToken();

            node0 = ParseStatement(node);
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.UNTIL)
            {
                throw new SyntaxErrorException($"Incorrect Repeat/Until statement in {GetCurrentTokenPosition()}.");
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}");
            }

            node.statement = node0;
            node.symbols = symbolNode;
            return node;
        }

        private Node ParseBlockStatement(Node root)
        {
            Node node0 = null;

            BlockNode node = new BlockNode();
            node.root = root;

            this.NextToken();

            if (this.GetOffsetToken(0) == KEYWORD.R_BR)
            {
                throw new SyntaxErrorException($"Empty block statement in {GetCurrentTokenPosition()}.");
            }

            while (this.GetOffsetToken(0) != KEYWORD.R_BR)
            {
                node0 = ParseStatement(node);
                node.statements.Add(node0);
                this.NextToken();
            }

            if ((node as BlockNode).root.type == TYPE.MAIN)
            {
                int j = (node as BlockNode).statements.Count - 1;

                if (((node as BlockNode).statements[j].type != TYPE.EXIT) &&
                ((node as BlockNode).statements[j].type != TYPE.ERROR))
                {
                    node.statements.Add(new PrimaryNode(TYPE.EXIT));
                }
            }
            return node;

        }

        private Node ParseLeftStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(TYPE.LEFT);
        }

        private Node ParseRightStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(TYPE.RIGHT);
        }

        private Node ParseExitStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(TYPE.EXIT);
        }

        private Node ParseErrorStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return new PrimaryNode(TYPE.ERROR);
        }

        private Node ParseWriteStatement(Node root)
        {
            SymbolNode symbolNode = ParseSymbolsStatement();

            if (symbolNode.symbols.Length > 1)
            {
                throw new SyntaxErrorException($"Expected only one symbol in write statement in {GetCurrentTokenPosition()}.");
            }

            WriteNode node = new WriteNode();
            node.symbol = symbolNode.symbols[0];

            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");
            }

            return node;

        }

        private bool IsinLoop(Node node)
        {
            Node it = node;

            while (
                (it.root.type != TYPE.WHILE) &&
                (it.root.type != TYPE.DO_WHILE) &&
                (it.root.type != TYPE.REPEAT_UNTIL) &&
                (it.root.type != TYPE.MAIN)
                )
            {
                it = it.root;
            }

            if (it.root.type == TYPE.MAIN)
            {
                return false;
            }

            (node as FlowControllNode).ownerLoop = it.root;
            return true;
        }
        private Node ParseContinueStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");

            }

            FlowControllNode node = new FlowControllNode(TYPE.CONTINUE);
            node.root = root;

            if (!IsinLoop(node))
            {
                throw new SyntaxErrorException("Flow control statement continue should only be within a loop");
            }

            return node;
        }

        private Node ParseBreakStatement(Node root)
        {
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new SyntaxErrorException($"Expected semicolon in {GetCurrentTokenPosition()}.");

            }

            FlowControllNode node = new FlowControllNode(TYPE.BREAK);
            node.root = root;

            if (!IsinLoop(node))
            {
                throw new SyntaxErrorException($"Flow control statement continue should only be within a loop, {GetCurrentTokenPosition()}.");
            }

            return node;
        }

        private Node ParseSwitchStatement(Node root)
        {
            Node node0 = null;

            this.NextToken();

            if ((this.GetOffsetToken(0) != KEYWORD.L_PAR) ||
                (this.GetOffsetToken(1) != KEYWORD.R_PAR))
            {
                throw new SyntaxErrorException($"Incorrect statement in {GetCurrentTokenPosition()}");
            }

            this.NextToken();
            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.L_BR)
            {
                throw new SyntaxErrorException($"Switch statement should only contain block statement, {GetCurrentTokenPosition()}.");
            }

            this.NextToken();

            SwitchNode node = new SwitchNode();
            node.root = root;

            while (this.GetOffsetToken(0) != KEYWORD.R_BR)
            {
                if ((this.GetOffsetToken(0) == KEYWORD.CASE) ||
                    (this.GetOffsetToken(0) == KEYWORD.DEFAULT))
                {
                    node0 = ParseCaseAndDefaultStatements(node);
                }
                else
                {
                    throw new SyntaxErrorException($"Only case statements and default statement are allowed in switch statement, {GetCurrentTokenPosition()}.");
                }

                char potentialKey = (node0 as CaseNode).symbol;

                if (node.branches.ContainsKey(potentialKey))
                {
                    throw new SyntaxErrorException($"Duplicated value {potentialKey} in switch statement in {GetCurrentTokenPosition()}.");
                }

                node.branches[potentialKey] = node0;

                if (node0.type == TYPE.DEFAULT)
                {
                    if (node.hasDefault)
                    {
                        throw new SyntaxErrorException($"Multiple default labels in one switch statement in {GetCurrentTokenPosition()}.");
                    }

                    node.hasDefault = true;
                    node.defaultNode = node0;
                }

                node.cases.Add(node0);

                this.NextToken();
            }

            return node;
        }

        private Node ParseCaseAndDefaultStatements(Node root)
        {
            Node node0 = null;

            if (this.GetOffsetToken(0) == KEYWORD.DEFAULT)
            {
                return ParseDefaultStatement(root);
            }

            SymbolNode symbolNode = ParseSymbolsStatement();

            if (symbolNode.symbols.Length > 1)
            {
                throw new SyntaxErrorException($"Expected only one symbol in case statement in {GetCurrentTokenPosition()}.");
            }

            this.NextToken();

            CaseNode node = new CaseNode();
            node.root = root;

            node.symbol = symbolNode.symbols[0];

            this.NextToken();

            node0 = ParseStatement(node);
            node.statement = node0;

            return node;

        }

        private Node ParseDefaultStatement(Node root)
        {
            Node node0 = null;

            this.NextToken();

            if (this.GetOffsetToken(0) != KEYWORD.COLON)
            {
                throw new SyntaxErrorException($"Expected colon after default statement in {GetCurrentTokenPosition()}.");
            }

            this.NextToken();

            DefaultNode node = new DefaultNode();
            node.root = root;

            node0 = ParseStatement(node);
            node.statement = node0;

            return node;


        }

    }
}
