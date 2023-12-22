using System.Collections.Generic;

namespace TuringMachineSimulator
{
    enum NodeType
    {
        Main, Block, While, DoWhile, RepeatUntil, If, IfElse, Write,
        Left, Right, Exit, Error, Continue, Break, SymbolList, Function, FunctionCall,
        Switch, Case, Default
    };

    internal abstract class Node
    {
        private NodeType type;
        Node root;

        public Node Root
        {
            set
            {
                root = value;
            }
            get
            {
                return root;
            }
        }

        public NodeType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Node(NodeType type)
        {
            this.type = type;
        }
    }

    class SymbolNode : Node
    {
        private string symbols;
        private bool hasNot;

        public string Symbols
        {
            get
            {
                return symbols;
            }
            set
            {
                symbols = value;
            }
        }
        public bool HasNot
        {
            get
            {
                return hasNot;
            }
            set
            {
                hasNot = value;
            }
        }


        public SymbolNode(NodeType type) : base(type)
        {
            this.symbols = "";
        }
    }
    class PrimaryNode : Node
    {
        public PrimaryNode(NodeType type) : base(type)
        {
        }
    }

    class WriteNode : Node
    {
        private char symbol;

        public char Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }

        public WriteNode(NodeType type = NodeType.Write) : base(type)
        {
        }

    }

    class StatementNode : Node
    {
        private SymbolNode symbols;

        public SymbolNode Symbols
        {
            get { return symbols; }
            set { symbols = value;}
        }
        public StatementNode(NodeType type) : base(type)
        {

        }
    }

    class IfNode : StatementNode
    {
        private Node statement;

        public Node Statement
        {
            set { statement = value; }
            get { return statement; }
        }

        public IfNode(NodeType type = NodeType.If) : base(type)
        {

        }
    }

    class IfElseNode : StatementNode
    {
        private Node ifStatement;
        private Node elseStatement;

        public Node IfStatement
        {
            set
            {
                ifStatement = value;
            }
            get { return ifStatement; }
        }

        public Node ElseStatement
        {
            set
            {
                elseStatement = value;
            }
            get { return elseStatement; }
        }

        public IfElseNode(NodeType type = NodeType.IfElse) : base(type)
        {

        }
    }

    class LoopNode : StatementNode
    {
        private List<int> continueStates;
        private List<int> breakStates;

        private int continueState;
        private int breakState;

        private Node statement;

        public List<int> BreakStates
        {
            get
            {
                return breakStates;
            }

            set 
            {
                breakStates = value;
            }
        }
        public List<int> ContinueStates
        {
            get
            {
                return continueStates;
            }
            set
            {
                continueStates = value;
            }
        }

        public int ContinueState
        {
            get
            {
                return continueState;
            }
            set
            {
                continueState = value;
            }
        }

        public int BreakState
        {
            get
            {
                return breakState;
            }
            set
            {
                breakState = value;
            }
        }

        public Node Statement
        {
            set { statement = value; }
            get { return statement; }
        }


        public LoopNode(NodeType type) : base(type)
        {
            this.continueStates = new List<int>();
            this.breakStates = new List<int>();
        }
    }

    class WhileNode : LoopNode
    {
        public WhileNode(NodeType type = NodeType.While) : base(type)
        {

        }
    }

    class DoWhileNode : LoopNode
    {
        public DoWhileNode(NodeType type = NodeType.DoWhile) : base(type)
        {

        }
    }

    class RepeatUntilNode : LoopNode
    {
        public RepeatUntilNode(NodeType type = NodeType.RepeatUntil) : base(type)
        {

        }
    }

    class BlockNode : StatementNode
    {
        private List<Node> statements;

        public List<Node> Statements
        {
            get { return statements; }
            set { statements = value; }
        }

        public BlockNode(NodeType type = NodeType.Block) : base(type)
        {
            this.statements = new List<Node>();
        }
    }

    class MainNode : StatementNode
    {
        private Node statement;

        public Node Statement
        {
            get
            {
                return statement;
            }
            set
            {
                statement = value;
            }
        }

        public MainNode(NodeType type = NodeType.Main) : base(type)
        {

        }
    }

    class FlowControllNode : Node
    {
        private Node ownerLoop;

        public Node OwnerLoop
        {
            get
            {
                return ownerLoop;
            }
            set
            {
                ownerLoop = value;
            }
        }

        public FlowControllNode(NodeType type) : base(type)
        {

        }

    }

    class SwitchNode : Node
    {
        private List<Node> cases;
        private Dictionary<char, Node> branches;
        private Node defaultNode;
        private bool hasDefault;

        public List<Node> Cases
        {
            get { return cases; }
            set { cases = value; }
        }

        public Dictionary<char, Node> Branches
        {
            get { return branches; }
            set { branches = value; }
        }

        public Node DefaultNode
        {
            get { return defaultNode; }
            set { defaultNode = value; }
        }

        public bool HasDefault
        {
            get { return hasDefault; }
            set { hasDefault = value; }
        }

        public SwitchNode(NodeType type = NodeType.Switch) : base(type)
        {
            this.Branches = new Dictionary<char, Node>();
            this.Cases = new List<Node>();
        }
    }

    class CaseNode : Node
    {
        private Node statement;
        private char symbol;
        private int entryState;

        public Node Statement
        {
            get
            {
                return statement;
            }
            set
            {
                statement = value;
            }
        }

        public char Symbol
        {
            get 
            { 
                return symbol; 
            }
            set
            {
                symbol = value;
            }
        }

        public int EntryState
        {
            get
            {
                return entryState;
            }
            set
            {
                entryState = value;
            }
        }


        public CaseNode(NodeType type = NodeType.Case) : base(type)
        {

        }

    }

    class DefaultNode : Node
    {
        private Node statement;
        private int entryState;

        public Node Statement
        {
            get 
            { 
                return statement; 
            }
            set
            {
                statement = value;
            }
        }

        public int EntryState
        {
            get 
            { 
                return entryState; 
            }
            set 
            { 
                entryState = value; 
            }
        }

        public DefaultNode(NodeType type = NodeType.Default) : base(type)
        {

        }

    }
}