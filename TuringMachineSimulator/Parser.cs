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

        public SymbolNode globalSymbols
        {
            get { return _globalSymbols; }
            set { _globalSymbols = value; }
        }

        public Parser()
        {
            //this.tokens = new List<KEYWORD>();
            //this.values = new List<string>();

            //this.globalSymbols = new SymbolNode(TYPE.SYMBOL_LIST);
            
            this.lexer = new Lexer();
        }

        private KEYWORD offsetToken(int offset)
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
                this.tokens.Add(this.lexer.currentKeyword);
                this.values.Add(this.lexer.currentValue);
                state = this.lexer.nextToken();
            }
            while (state);

            this.tokens.Add(KEYWORD.END);
            this.values.Add("");
        }

        public void setStream(string stream)
        {
            // Initialize the corresponding fields
            this.tokens = new List<KEYWORD>();
            this.values = new List<string>();
            this.globalSymbols = new SymbolNode(TYPE.SYMBOL_LIST);
            this.tokenCount = 0;

            this.lexer.setStream(stream);
            this.getAllTokens();
        }

        public void nextToken()
        {
            this.tokenCount++;
        }
        
        public MainNode parse()
        {
            this.getGlobalSymbols();
            this.nextToken();

            MainNode node = parseMainStatement();
            node.root = null;
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.END)
            {
                throw new Exception("Invalid sytax");
            }

            return node;
        }
        private SymbolNode parseSymbolsStatement()
        {
            SymbolNode node = new SymbolNode(TYPE.SYMBOL_LIST);
            bool isGlobal;
            bool hasNot;

            if (this.globalSymbols.symbols.Length == 0)
            {
                isGlobal = true;
            }
            else
            {
                isGlobal= false;
            }

            if (this.offsetToken(1) == KEYWORD.NOT)
            {
                this.nextToken();

                if (isGlobal)
                {
                    throw new Exception("Main statement should not have 'not' attribute.");
                }
                hasNot = true;
            }
            else
            {
                hasNot = false;
            }

            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.L_PAR)
            {
                throw new Exception("Expected left parenrhesis.");
            }

            bool lastWasComma = false;
            bool firsWasSymbol = false;

            while (true)
            {
                this.nextToken();

                if (this.offsetToken(0) == KEYWORD.R_PAR)
                {
                    if (this.offsetToken(-1) == KEYWORD.L_PAR)
                    {
                        throw new Exception("Empty parenthesis statement.");
                    }
                    break;
                }


                if ((this.offsetToken(0) != KEYWORD.SYMBOL) && (!firsWasSymbol))
                {
                    throw new Exception("Expected symbol before comma.");
                }

                firsWasSymbol = true;

                if (this.offsetToken(0) == KEYWORD.SYMBOL)
                {
                    if (node.symbols.Contains(this.offsetTokenValue(0)))
                    {
                        throw new Exception($"Symbol {this.offsetTokenValue(0)} is already given.");
                    }

                    if (!isGlobal)
                    {
                        if (!this.globalSymbols.symbols.Contains(this.offsetTokenValue(0)))
                        {
                            throw new Exception($"The symbol {this.offsetTokenValue(0)} is not given in main symbols.");
                        }
                    }

                    node.symbols = node.symbols + this.offsetTokenValue(0);
                    lastWasComma = false;
                }
                else if (this.offsetToken(0) == KEYWORD.COMMA)
                {
                    lastWasComma = true;
                    continue;
                }
                else
                {
                    throw new Exception("Expected symbol.");
                }
            }

            if (lastWasComma)
            {
                throw new Exception("Expected symbol after comma.");
            }

            node.hasNot = hasNot;

            return node;
        }

        private void getGlobalSymbols()
        {
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.GLOBAL_SYMBOLS)
            {
                throw new Exception("Global symbols are not given.");
            }

            SymbolNode node = parseSymbolsStatement();

            if(!node.symbols.Contains("_"))
            {
                throw new Exception("Global symbols should contain empty '_' symbol.");
            }

            this.globalSymbols = node;
        }

        private MainNode parseMainStatement()
        {
            var tmp = this.offsetToken(0);
            if (this.offsetToken(0) != KEYWORD.MAIN)
            {
                throw new Exception("Expected a statement.");
            }

            this.nextToken();

            if ((this.offsetToken(0) != KEYWORD.L_PAR) || (this.offsetToken(1) != KEYWORD.R_PAR))
            {
                throw new Exception("Incorrect statement");
            }

            this.nextToken();
            MainNode node = new MainNode();
            node.symbols = this.globalSymbols;

            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.L_BR)
            {
                throw new Exception("Main statement should only contain block statement.");
            }

            node.statement = parseStatement(node);
            return node;
        }

        private Node parseStatement(Node root)
        {

            if (this.offsetToken(0) == KEYWORD.IF)
            {
                return parseIfStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.WHILE)
            {
                return parseWhileStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.DO)
            {
                return parseDoWhileStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.REPEAT)
            {
                return parseRepeatUntilStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.L_BR)
            {
                return parseBlockStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.LEFT)
            {
                return parseLeftStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.RIGHT)
            {
                return parseRightStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.EXIT)
            {
                return parseExitStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.ERROR)
            {
                return parseErrorStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.WRITE)
            {
                return parseWriteStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.CONTINUE)
            {
                return parseContinueStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.BREAK)
            {
                return parseBreakStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.SWITCH)
            {
                return parseSwitchStatement(root);
            }
            else
            if (this.offsetToken(0) == KEYWORD.CASE)
            {
                throw new Exception("Case statement should only be in switch statement.");
            }
            else
            if (this.offsetToken(0) == KEYWORD.DEFAULT)
            {
                throw new Exception("Default statement should only be in switch statement.");
            }
            else
            if (this.offsetToken(0) == KEYWORD.SEMICOLON)
            {
                throw new Exception("Semicolon should follow by an primary statement");
            }
            else
            if (this.offsetToken(0) == KEYWORD.R_BR)
            {
                throw new Exception("Empty single statement");
            }
            else
            if (this.offsetToken(0) == KEYWORD.ELSE)
            {
                throw new Exception("Else statement should follow by an if statement");
            }
            else
            if (this.offsetToken(0) == KEYWORD.END)
            {
                throw new Exception("Expected statement");
            }
            else
            if (this.offsetToken(0) == KEYWORD.MAIN)
            {
                throw new Exception("Main statement should occur only once.");
            }
            else
            {
                throw new Exception($"Unknow statement {this.offsetToken(0)} .");
            }

        }

        private Node parseIfStatement(Node root) 
        {
            Node node0 = null;
            Node node1 = null;

            IfNode firstNode = new IfNode();
            firstNode.root = root;

            SymbolNode symbolNode = parseSymbolsStatement();
            this.nextToken();

            node0 = parseStatement(firstNode);

            if (this.offsetToken(1) == KEYWORD.ELSE)
            {
                this.nextToken();
                this.nextToken();

                firstNode.statement = null;

                IfElseNode node = new IfElseNode();
                node.root = root;

                node1 = parseStatement(node);
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

        private Node parseWhileStatement(Node root)
        {
            Node node0 = null;

            WhileNode node = new WhileNode();
            node.root = root;
            SymbolNode symbolNode = parseSymbolsStatement();

            this.nextToken();

            node0 = parseStatement(node);

            node.statement = node0;
            node.symbols = symbolNode;
            return node;
        }

        private Node parseDoWhileStatement(Node root) 
        {
            Node node0 = null;
            DoWhileNode node = new DoWhileNode();
            node.root = root;
            this.nextToken();

            node0 = parseStatement(node);

            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.WHILE) 
            {
                throw new Exception("Incorrect Do while statement");
            }

            SymbolNode symbolNode = parseSymbolsStatement();

            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");
            }

            node.statement = node0;
            node.symbols = symbolNode;
            return node;

        }
        private Node parseRepeatUntilStatement(Node root) 
        {
            Node node0 = null;

            RepeatUntilNode node = new RepeatUntilNode();
            node.root = root;
            this.nextToken();

            node0 = parseStatement(node);
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.UNTIL)
            {
                throw new Exception("Incorrect Repeat/Until statement.");
            }

            SymbolNode symbolNode = parseSymbolsStatement();

            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");
            }

            node.statement = node0;
            node.symbols = symbolNode;
            return node;
        }

        private Node parseBlockStatement(Node root)
        {
            Node node0 = null;

            BlockNode node = new BlockNode();
            node.root = root;

            this.nextToken();

            if (this.offsetToken(0) == KEYWORD.R_BR)
            {
                throw new Exception("Empty block statement");
            }

            while (this.offsetToken(0) != KEYWORD.R_BR)
            {
                node0 = parseStatement(node);
                node.statements.Add(node0);
                this.nextToken();
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

        private Node parseLeftStatement(Node root)
        {
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");
            }

            return new PrimaryNode(TYPE.LEFT);
        }

        private Node parseRightStatement(Node root)
        {
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");
            }

            return new PrimaryNode(TYPE.RIGHT);
        }

        private Node parseExitStatement(Node root)
        {
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");
            }

            return new PrimaryNode(TYPE.EXIT);
        }

        private Node parseErrorStatement(Node root)
        {
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");
            }

            return new PrimaryNode(TYPE.ERROR);
        }

        private Node parseWriteStatement(Node root)
        {
            SymbolNode symbolNode = parseSymbolsStatement();

            if (symbolNode.symbols.Length > 1)
            {
                throw new Exception("Expected only one symbol in write statement.");
            }

            WriteNode node = new WriteNode();
            node.symbol = symbolNode.symbols[0];

            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");
            }

            return node;

        }

        private bool inLoop(Node node)
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
        private Node parseContinueStatement(Node root)
        {
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");

            }

            FlowControllNode node = new FlowControllNode(TYPE.CONTINUE);
            node.root = root;

            if (!inLoop(node))
            {
                throw new Exception("Flow control statement continue should only be within a loop");
            }

            return node;
        }

        private Node parseBreakStatement(Node root)
        {
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.SEMICOLON)
            {
                throw new Exception("Expected semicolon");

            }

            FlowControllNode node = new FlowControllNode(TYPE.BREAK);
            node.root = root;

            if (!inLoop(node))
            {
                throw new Exception("Flow control statement continue should only be within a loop");
            }

            return node;
        }

        private Node parseSwitchStatement(Node root)
        {
            Node node0 = null;

            this.nextToken();

            if ((this.offsetToken(0) != KEYWORD.L_PAR) ||
                (this.offsetToken(1) != KEYWORD.R_PAR))
            {
                throw new Exception("Incorrect statement");
            }

            this.nextToken();
            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.L_BR)
            {
                throw new Exception("Switch statement should only contain block statement.");
            }

            this.nextToken();

            SwitchNode node = new SwitchNode();
            node.root = root;

            while (this.offsetToken(0) != KEYWORD.R_BR)
            {
                if ((this.offsetToken(0) == KEYWORD.CASE) ||
                    (this.offsetToken(0) == KEYWORD.DEFAULT))
                {
                    node0 = parseCaseAndDefaultStatements(node);
                }
                else
                {
                    throw new Exception("Only case statements and default statement are alloved in switch statement");
                }

                char potentialKey = (node0 as CaseNode).symbol;

                if (node.branches.ContainsKey(potentialKey))
                {
                    throw new Exception($"Duplicated value {potentialKey} in switch statement");
                }

                node.branches[potentialKey] = node0;

                if (node0.type == TYPE.DEFAULT) 
                {
                    if (node.hasDefault)
                    {
                        throw new Exception("Multiple default labels in one switch statement.");
                    }

                    node.hasDefault = true;
                    node.defaultNode = node0;
                }

                node.cases.Add(node0);

                this.nextToken();
            }

            return node;
        }

        private Node parseCaseAndDefaultStatements(Node root) 
        {
            Node node0 = null;

            if (this.offsetToken(0) == KEYWORD.DEFAULT)
            {
                return parseDefaultStatement(root);
            }

            SymbolNode symbolNode = parseSymbolsStatement();

            if (symbolNode.symbols.Length > 1)
            {
                throw new Exception("Expected only one symbol in case statement");
            }

            this.nextToken();

            CaseNode node = new CaseNode();
            node.root = root;

            node.symbol = symbolNode.symbols[0];

            this.nextToken();

            node0 = parseStatement(node);
            node.statement = node0;

            return node;

        }

        private Node parseDefaultStatement(Node root) 
        {
            Node node0 = null;

            this.nextToken();

            if (this.offsetToken(0) != KEYWORD.COLON) 
            {
                throw new Exception("Expected colon ofter default statement");
            }

            this.nextToken();

            DefaultNode node = new DefaultNode();
            node.root = root;

            node0 = parseStatement(node);
            node.statement = node0;

            return node;


        }

    }
  }
