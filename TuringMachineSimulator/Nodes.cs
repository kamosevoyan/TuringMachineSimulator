using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineSimulator
{
    enum TYPE 
    {	
		MAIN, BLOCK, WHILE, DO_WHILE, REPEAT_UNTIL, IF, IF_ELSE, WRITE, 
		LEFT, RIGHT, EXIT, ERROR, CONTINUE, BREAK, SYMBOL_LIST, FUNCTION, FUNCTION_CALL,
		SWITCH, CASE, DEFAULT
    };

    internal abstract class Node
    {
        private TYPE _type;
        Node _root;

        public Node root
        {
            set
            {
                _root = value;
            }
            get
            {
                return _root;
            }
        }

        public TYPE type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Node(TYPE type)
        {
            this._type = type;
        }
    }

    class SymbolNode : Node
    {
        private string _symbols;
        private bool _hasNot;

        public string symbols
        {
            get
            {
                return _symbols;
            }
            set
            {
                _symbols = value;
            }
        }
        public bool hasNot
        {
            get
            {
                return _hasNot;
            }
            set
            {
                _hasNot = value;
            }
        }


        public SymbolNode(TYPE type):base(type)
        {
            this._symbols = "";
        }
    }
    class PrimaryNode : Node
    {
        public PrimaryNode(TYPE type):base(type)
        {
        }
    }

    class WriteNode : Node 
    {
        private char _symbol;

        public char symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        public WriteNode(TYPE type = TYPE.WRITE) : base(type) 
        {
        }

    }

    class FlowControlNode : Node 
    {
        Node ownerLoop;

        public FlowControlNode(TYPE type) :base(type)
        {
            this.ownerLoop = null;
        }
    }

    class StatementNode : Node 
    { 
        public SymbolNode symbols;

        public StatementNode(TYPE type) : base(type)
        {
          
        }
    }

    class IfNode : StatementNode
    {
        private Node _statement;

        public Node statement
        {
            set { _statement = value; }
            get { return _statement; }
        }

        public IfNode(TYPE type = TYPE.IF) : base(type)
        {

        }
    }

    class IfElseNode : StatementNode
    {
        private Node _ifStatement;
        private Node _elseStatement;

        public Node ifStatement
        {
            set
            {
                _ifStatement = value;
            }
            get { return _ifStatement; }
        }

        public Node elseStatement
        {
            set
            {
                _elseStatement = value;
            }
            get { return _elseStatement; }
        }

        public IfElseNode(TYPE type = TYPE.IF_ELSE) : base(type)
        {

        }
    }

    class LoopNode: StatementNode
    {
        public List<int> continueStates;
        public List<int> breakStates;

        public int continueState;
        public int breakState;

        private Node _statement;

        public Node statement
        {
            set { _statement = value; }
            get { return _statement; }
        }


        public LoopNode(TYPE type) : base(type)
        {
            this.continueStates = new List<int>();
            this.breakStates = new List<int>();
        }
    }

    class WhileNode : LoopNode
    {
        public WhileNode(TYPE type = TYPE.WHILE) : base(type)
        {

        }
    }

    class DoWhileNode : LoopNode
    {
        public DoWhileNode(TYPE type = TYPE.DO_WHILE) : base(type)
        {

        }
    }

    class RepeatUntilNode : LoopNode
    {
        public RepeatUntilNode(TYPE type = TYPE.REPEAT_UNTIL) : base(type)
        {

        }
    }

    class BlockNode : StatementNode
    {
        List<Node> _statements;

        public List<Node> statements
        {
            get { return _statements; }
            set { _statements = value; }
        }

        public BlockNode(TYPE type = TYPE.BLOCK) : base(type) 
        {
            this._statements = new List<Node>();
        }
    }

    class MainNode : StatementNode
    {
        public Node statement;

        public MainNode(TYPE type = TYPE.MAIN) : base(type) 
        {

        }
    }

    class FlowControllNode : Node
    {
        private Node _ownerLoop;

        public Node ownerLoop
        {
            get
            {
                return _ownerLoop;
            }
            set 
            { 
                _ownerLoop = value;
            }
        }

        public FlowControllNode(TYPE type) : base(type) 
        { 
        }

    }

    class SwitchNode : Node
    {
        private List<Node> _cases;
        private Dictionary<char, Node> _branches;
        private Node _defaultNode;
        private bool _hasDefault;

        public List<Node> cases
        {
            get { return _cases; }
            set { _cases = value; }
        }

        public Dictionary<char, Node> branches
        {
            get { return _branches; }
            set { _branches = value; }
        }

        public Node defaultNode
        {
            get { return _defaultNode; }
            set { _defaultNode = value; }
        }

        public bool hasDefault
        {
            get { return _hasDefault; }
            set { _hasDefault = value; }
        }


        public SwitchNode (TYPE type = TYPE.SWITCH) : base(type) 
        {
            this.branches = new Dictionary<char, Node>();
            this.cases = new List<Node>();
        }
    }

    class CaseNode : Node
    {
        public Node statement;
        public char symbol;
        public int entryState;

        public CaseNode(TYPE type = TYPE.CASE) : base(type)
        {
        }

    }

    class DefaultNode : Node
    {
        public Node statement;
        public int entryState;

        public DefaultNode(TYPE type = TYPE.DEFAULT) : base(type)
        {
        }

    }
}