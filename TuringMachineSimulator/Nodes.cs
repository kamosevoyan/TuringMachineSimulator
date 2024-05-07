using System.Collections.Generic;

namespace TuringMachineSimulator
{
    /// <summary>
    /// Enum describing the type of nodes in AST
    /// </summary>
    enum NodeType
    {
        Main, Block, While, DoWhile, RepeatUntil, If, IfElse, Write,
        Left, Right, Exit, Error, Continue, Break, SymbolList, Function, FunctionCall,
        Switch, Case, Default
    };

    internal abstract class Node
    {
        public Node(NodeType type)
        {
            Type = type;
        }

        public Node Root { get; set; }

        public NodeType Type { get; private set; }

    }

    class SymbolNode : Node
    {

        public SymbolNode(NodeType type) : base(type)
        {
            Symbols = "";
        }
        public string Symbols { get; set; }
        public bool HasNegation { get; set; }

    }
    class PrimaryNode : Node
    {
        public PrimaryNode(NodeType type) : base(type)
        {

        }
    }

    class WriteNode : Node
    {

        public WriteNode(NodeType type = NodeType.Write) : base(type)
        {
        }
        public char Symbol { get; set; }

    }

    class StatementNode : Node
    {
        public StatementNode(NodeType type) : base(type)
        {
        }
        public SymbolNode Symbols { get; set; }

    }

    class IfNode : StatementNode
    {

        public IfNode(NodeType type = NodeType.If) : base(type)
        {

        }
        public Node Statement { get; set; }
    }

    class IfElseNode : StatementNode
    {
        public IfElseNode(NodeType type = NodeType.IfElse) : base(type)
        {

        }

        public Node IfStatement { get; set; }

        public Node ElseStatement { get; set; }

    }

    class LoopNode : StatementNode
    {
        public LoopNode(NodeType type) : base(type)
        {
            ContinueStates = new List<int>();
            BreakStates = new List<int>();
        }

        public List<int> BreakStates { get; set; }
        public List<int> ContinueStates { get; set; }

        public int ContinueState { get; set; }

        public int BreakState { get; set; }

        public Node Statement { get; set; }


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
        public BlockNode(NodeType type = NodeType.Block) : base(type)
        {
            Statements = new List<Node>();
        }
        public List<Node> Statements { get; set; }
    }

    class MainNode : StatementNode
    {
        public MainNode(NodeType type = NodeType.Main) : base(type)
        {

        }
        public Node Statement { get; set; }
    }

    class FlowControllNode : Node
    {
        public FlowControllNode(NodeType type) : base(type)
        {

        }
        public Node OwnerLoop { get; set; }
    }

    class SwitchNode : Node
    {
        public SwitchNode(NodeType type = NodeType.Switch) : base(type)
        {
            Branches = new Dictionary<char, Node>();
            Cases = new List<Node>();
        }

        public List<Node> Cases { get; set; }
        public Dictionary<char, Node> Branches { get; set; }
        public Node DefaultNode { get; set; }
        public bool HasDefault { get; set; }

    }

    class CaseNode : Node
    {
        public CaseNode(NodeType type = NodeType.Case) : base(type)
        {

        }
        public Node Statement { get; set; }
        public char Symbol { get; set; }
        public int EntryState { get; set; }
    }

    class DefaultNode : Node
    {
        public DefaultNode(NodeType type = NodeType.Default) : base(type)
        {

        }
        public Node Statement { get; set; }
        public int EntryState { get; set; }
    }
}