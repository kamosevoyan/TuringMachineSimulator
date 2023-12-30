using System;
using System.Collections.Generic;
using System.Linq;

namespace TuringMachineSimulator
{
    internal class Compiler
    {
        private readonly Parser parser;
        private string globalSymbols;

        public string GlobalSymbols
        {
            get { return globalSymbols; }
        }
            

        public Compiler()
        {
            this.parser = new Parser();
        }

        public string Compile()
        {
            MainNode node = this.parser.Parse();

            string symbols = this.parser.GlobalSymbols.Symbols;
            this.globalSymbols = symbols;

            List<string> lines = new List<string>();

            string temp = "";
            int stateNumber = 0;

            CompileNode(node, lines, ref stateNumber, symbols);

            temp += symbols[0];

            for (int i = 1; i < symbols.Length; ++i)
            {
                temp += "," + symbols.Substring(i, 1);
            }
            temp += "\n";

            temp += "q0";
            for (int i = 1; i < stateNumber; ++i)
            {
                temp += ",q" + i.ToString();
            }
            temp += ",h\n";

            for (int i = 0; i < lines.Count; ++i)
            {
                temp += lines[i];
            }

            temp += "q0\nh\n_\n";


            return temp;
        }

        private void CompileNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            switch (node.Type)
            {
                case NodeType.Left:
                    {
                        CompileLeftNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Right:
                    {
                        CompileRightNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Exit:
                    {
                        CompileExitNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Error:
                    {
                        CompileErrorNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Continue:
                    {
                        CompileContinueNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Break:
                    {
                        CompileBreakNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Write:
                    {
                        CompileWriteNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Main:
                    {
                        CompileMainNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Block:
                    {
                        CompileBlockNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.If:
                    {
                        CompileIfNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.IfElse:
                    {
                        CompileIfElseNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.While:
                    {
                        CompileWhileNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.DoWhile:
                    {
                        CompileDoWhileNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.RepeatUntil:
                    {
                        CompileRepeatUntilNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case NodeType.Switch:
                    {
                        CompileSwitchNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                default:
                    {
                        throw new SyntaxErrorException("Unexpected node to compile.");
                    }
            }
        }

        private void CompileLeftNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            ++stateNumber;

            temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",<";
            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + stateNumber.ToString() + ",<";
            }
            temp += "\n";
            lines.Add(temp);
        }
        private void CompileRightNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            ++stateNumber;

            temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",>";
            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + stateNumber.ToString() + ",>";
            }
            temp += "\n";
            lines.Add(temp);
        }
        private void CompileExitNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            ++stateNumber;

            temp += globalSymbols.Substring(0, 1) + ",h,@";
            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",h,@";
            }
            temp += "\n";
            lines.Add(temp);
        }

        private void CompileErrorNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            ++stateNumber;

            temp += "X";

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + "X";
            }
            temp += "\n";
            lines.Add(temp);
        }

        private void CompileContinueNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            ++stateNumber;

            lines.Add("");
            ((node as FlowControllNode).OwnerLoop as LoopNode).ContinueStates.Add(stateNumber);
        }


        private void CompileBreakNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            ++stateNumber;

            lines.Add("");
            ((node as FlowControllNode).OwnerLoop as LoopNode).BreakStates.Add(stateNumber);
        }

        private void CompileWriteNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            ++stateNumber;

            temp = (node as WriteNode).Symbol.ToString();
            temp += ",q" + stateNumber.ToString() + ",@";

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + (node as WriteNode).Symbol.ToString() + ",q" + stateNumber.ToString() + ",@";
            }
            temp += "\n";
            lines.Add(temp);
        }

        private void CompileMainNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            CompileNode((node as MainNode).Statement, lines, ref stateNumber, globalSymbols);
        }

        private void CompileBlockNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            BlockNode blockNode = node as BlockNode;
            for (int i = 0; i < blockNode.Statements.Count; ++i)
            {
                CompileNode(blockNode.Statements[i], lines, ref stateNumber, globalSymbols);
            }
        }
        private void CompileIfNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;
            int currentLine;

            ++stateNumber;
            oldStateNumber = stateNumber;
            lines.Add("");
            currentLine = lines.Count - 1;

            IfNode ifNode = node as IfNode;
            bool hasNot = ifNode.Symbols.HasNot;

            CompileNode(ifNode.Statement, lines, ref stateNumber, globalSymbols);

            if (hasNot ^ ifNode.Symbols.Symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ ifNode.Symbols.Symbols.Contains(globalSymbols[i]))
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + oldStateNumber.ToString() + ",@";
                }
                else
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + stateNumber.ToString() + ",@";
                }
            }

            temp += "\n";
            lines[currentLine] = temp;
        }

        private void CompileIfElseNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;
            int currentLine;

            ++stateNumber;
            oldStateNumber = stateNumber;

            lines.Add("");
            currentLine = lines.Count - 1;

            IfElseNode ifElseNode = node as IfElseNode;
            bool hasNot = ifElseNode.Symbols.HasNot;

            CompileNode(ifElseNode.IfStatement, lines, ref stateNumber, globalSymbols);
            lines.Add("");

            ++stateNumber;

            int elseOldStateNumber = stateNumber;
            int elseCurrentLine = lines.Count - 1;

            CompileNode(ifElseNode.ElseStatement, lines, ref stateNumber, globalSymbols);


            if (hasNot ^ ifElseNode.Symbols.Symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + elseOldStateNumber.ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ ifElseNode.Symbols.Symbols.Contains(globalSymbols[i]))
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + oldStateNumber.ToString() + ",@";
                }
                else
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + elseOldStateNumber.ToString() + ",@";
                }
            }

            temp += "\n";
            lines[currentLine] = temp;

            temp = "";

            temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + stateNumber.ToString() + ",@";
            }

            temp += "\n";
            lines[elseCurrentLine] = temp;

        }

        private void CompileWhileNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;
            int curentLine;

            ++stateNumber;
            oldStateNumber = stateNumber;

            lines.Add("");
            curentLine = lines.Count - 1;

            WhileNode whileNode = node as WhileNode;

            bool hasNot = whileNode.Symbols.HasNot;

            CompileNode(whileNode.Statement, lines, ref stateNumber, globalSymbols);


            if (hasNot ^ whileNode.Symbols.Symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + (stateNumber + 1).ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ whileNode.Symbols.Symbols.Contains(globalSymbols[i]))
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + oldStateNumber.ToString() + ",@";
                }
                else
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + (stateNumber + 1).ToString() + ",@";
                }
            }


            temp += "\n";
            lines[curentLine] = temp;

            temp = "";
            ++stateNumber;

            temp += globalSymbols.Substring(0, 1) + ",q" + (oldStateNumber - 1).ToString() + ",@";

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + (oldStateNumber - 1).ToString() + ",@";
            }

            temp += "\n";

            lines.Add(temp);

            whileNode.ContinueState = oldStateNumber - 1;
            whileNode.BreakState = stateNumber;

            PutFlowControls(node, lines, ref stateNumber, globalSymbols);
        }

        private void CompileDoWhileNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;

            oldStateNumber = stateNumber;
            DoWhileNode doWhileNode = node as DoWhileNode;

            bool hasNot = doWhileNode.Symbols.HasNot;

            CompileNode(doWhileNode.Statement, lines, ref stateNumber, globalSymbols);

            ++stateNumber;

            if (hasNot ^ doWhileNode.Symbols.Symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ doWhileNode.Symbols.Symbols.Contains(globalSymbols[i]))
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + oldStateNumber.ToString() + ",@";
                }
                else
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + stateNumber.ToString() + ",@";
                }
            }

            temp += "\n";
            lines.Add(temp);

            doWhileNode.ContinueState = oldStateNumber;
            doWhileNode.BreakState = stateNumber;

            PutFlowControls(node, lines, ref stateNumber, globalSymbols);
        }

        private void CompileRepeatUntilNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;

            oldStateNumber = stateNumber;
            RepeatUntilNode repeatUntilNode = node as RepeatUntilNode;

            bool hasNot = repeatUntilNode.Symbols.HasNot;

            CompileNode(repeatUntilNode.Statement, lines, ref stateNumber, globalSymbols);

            ++stateNumber;

            if (hasNot ^ repeatUntilNode.Symbols.Symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ repeatUntilNode.Symbols.Symbols.Contains(globalSymbols[i]))
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + oldStateNumber.ToString() + ",@";
                }
                else
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + stateNumber.ToString() + ",@";
                }
            }

            temp += "\n";
            lines.Add(temp);

            repeatUntilNode.ContinueState = oldStateNumber;
            repeatUntilNode.BreakState = stateNumber;

            PutFlowControls(node, lines, ref stateNumber, globalSymbols);

        }

        private void CompileSwitchNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            int currentLine = 0;
            string temp = "";
            ++stateNumber;

            lines.Add("");
            currentLine = lines.Count - 1;

            SwitchNode switchNode = node as SwitchNode;

            List<int> poses = new List<int> { };

            for (int i = 0; i < switchNode.Cases.Count; ++i)
            {
                if (switchNode.Cases[i].Type == NodeType.Case)
                {
                    (switchNode.Cases[i] as CaseNode).EntryState = stateNumber;
                    CompileNode((switchNode.Cases[i] as CaseNode).Statement, lines, ref stateNumber, globalSymbols);
                }
                else
                {
                    (switchNode.Cases[i] as DefaultNode).EntryState = stateNumber;
                    CompileNode((switchNode.Cases[i] as DefaultNode).Statement, lines, ref stateNumber, globalSymbols);
                }

                ++stateNumber;
                lines.Add("");
                poses.Add(lines.Count - 1);
            }

            for (int i = 0; i < poses.Count; ++i)
            {
                temp = "";
                temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";

                for (int j = 1; j < globalSymbols.Length; ++j)
                {
                    temp += "\t|\t" + globalSymbols.Substring(j, 1) + ",q" + stateNumber.ToString() + ",@";
                }

                temp += "\n";
                lines[poses[i]] = temp;
            }

            temp = "";

            if (switchNode.Branches.ContainsKey(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + (switchNode.Branches[globalSymbols[0]] as CaseNode).EntryState.ToString() + ",@";
            }
            else
            {
                if (switchNode.HasDefault)
                {
                    temp += globalSymbols.Substring(0, 1) + ",q" + (switchNode.Branches[globalSymbols[0]] as CaseNode).EntryState.ToString() + ",@";
                }
                else
                {
                    temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";
                }
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (switchNode.Branches.ContainsKey(globalSymbols[i]))
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + (switchNode.Branches[globalSymbols[i]] as CaseNode).EntryState.ToString() + ",@";
                }
                else
                {
                    if (switchNode.HasDefault)
                    {
                        temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + (switchNode.Branches[globalSymbols[i]] as CaseNode).EntryState.ToString() + ",@";
                    }
                    else
                    {
                        temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + stateNumber.ToString() + ",@";
                    }

                }
            }
            temp += "\n";

            lines[currentLine] = temp;
        }

        private void PutFlowControls(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp;

            LoopNode loopNode = node as LoopNode;

            for (int i = 0; i < loopNode.ContinueStates.Count; ++i)
            {
                temp = "";

                temp += globalSymbols.Substring(0, 1) + ",q" + (loopNode.ContinueState) + ",@";

                for (int j = 1; j < globalSymbols.Length; ++j)
                {
                    temp += "\t|\t" + globalSymbols.Substring(j, 1) + ",q" + (loopNode.ContinueState).ToString() + ",@";
                }
                temp += "\n";
                lines[loopNode.ContinueStates[i] - 1] = temp;
            }


            for (int i = 0; i < loopNode.BreakStates.Count; ++i)
            {
                temp = "";

                temp += globalSymbols.Substring(0, 1) + ",q" + (loopNode.BreakState) + ",@";
                for (int j = 1; j < globalSymbols.Length; ++j)
                {
                    temp += "\t|\t" + globalSymbols.Substring(j, 1) + ",q" + (loopNode.BreakState) + ",@";
                }
                temp += "\n";
                lines[loopNode.BreakStates[i] - 1] = temp;
            }

        }


        public void SetStream(string stream)
        {
            this.parser.SetStream(stream);
        }
    }
}