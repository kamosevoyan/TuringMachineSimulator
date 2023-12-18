using System;
using System.Collections.Generic;
using System.Linq;

namespace TuringMachineSimulator
{
    internal class Compiler
    {
        private Parser parser;

        public Compiler()
        {
            this.parser = new Parser();
        }

        public string Compile()
        {
            MainNode node = this.parser.Parse();

            string symbols = this.parser.GlobalSymbols.symbols;
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
            switch (node.type)
            {
                case TYPE.LEFT:
                    {
                        CompileLeftNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.RIGHT:
                    {
                        CompileRightNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.EXIT:
                    {
                        CompileExitNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.ERROR:
                    {
                        CompileErrorNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.CONTINUE:
                    {
                        CompileContinueNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.BREAK:
                    {
                        CompileBreakNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.WRITE:
                    {
                        CompileWriteNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.MAIN:
                    {
                        CompileMainNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.BLOCK:
                    {
                        CompileBlockNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.IF:
                    {
                        CompileIfNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.IF_ELSE:
                    {
                        CompileIfElseNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.WHILE:
                    {
                        CompileWhileNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.DO_WHILE:
                    {
                        CompileDoWhileNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.REPEAT_UNTIL:
                    {
                        CompileRepeatUntilNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.SWITCH:
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
            ((node as FlowControllNode).ownerLoop as LoopNode).continueStates.Add(stateNumber);
        }


        private void CompileBreakNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            ++stateNumber;

            lines.Add("");
            ((node as FlowControllNode).ownerLoop as LoopNode).breakStates.Add(stateNumber);
        }

        private void CompileWriteNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            ++stateNumber;

            temp = (node as WriteNode).symbol.ToString();
            temp += ",q" + stateNumber.ToString() + ",@";

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + (node as WriteNode).symbol.ToString() + ",q" + stateNumber.ToString() + ",@";
            }
            temp += "\n";
            lines.Add(temp);
        }

        private void CompileMainNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            CompileNode((node as MainNode).statement, lines, ref stateNumber, globalSymbols);
        }

        private void CompileBlockNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            BlockNode blockNode = node as BlockNode;
            for (int i = 0; i < blockNode.statements.Count; ++i)
            {
                CompileNode(blockNode.statements[i], lines, ref stateNumber, globalSymbols);
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
            bool hasNot = ifNode.symbols.hasNot;

            CompileNode(ifNode.statement, lines, ref stateNumber, globalSymbols);

            if (hasNot ^ ifNode.symbols.symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ ifNode.symbols.symbols.Contains(globalSymbols[i]))
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
            bool hasNot = ifElseNode.symbols.hasNot;

            CompileNode(ifElseNode.ifStatement, lines, ref stateNumber, globalSymbols);
            lines.Add("");

            ++stateNumber;

            int elseOldStateNumber = stateNumber;
            int elseCurrentLine = lines.Count - 1;

            CompileNode(ifElseNode.elseStatement, lines, ref stateNumber, globalSymbols);


            if (hasNot ^ ifElseNode.symbols.symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + elseOldStateNumber.ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ ifElseNode.symbols.symbols.Contains(globalSymbols[i]))
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

            bool hasNot = whileNode.symbols.hasNot;

            CompileNode(whileNode.statement, lines, ref stateNumber, globalSymbols);


            if (hasNot ^ whileNode.symbols.symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + (stateNumber + 1).ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ whileNode.symbols.symbols.Contains(globalSymbols[i]))
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

            whileNode.continueState = oldStateNumber - 1;
            whileNode.breakState = stateNumber;

            PutFlowControls(node, lines, ref stateNumber, globalSymbols);
        }

        private void CompileDoWhileNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;

            oldStateNumber = stateNumber;
            DoWhileNode doWhileNode = node as DoWhileNode;

            bool hasNot = doWhileNode.symbols.hasNot;

            CompileNode(doWhileNode.statement, lines, ref stateNumber, globalSymbols);

            ++stateNumber;

            if (hasNot ^ doWhileNode.symbols.symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ doWhileNode.symbols.symbols.Contains(globalSymbols[i]))
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

            doWhileNode.continueState = oldStateNumber;
            doWhileNode.breakState = stateNumber;

            PutFlowControls(node, lines, ref stateNumber, globalSymbols);
        }

        private void CompileRepeatUntilNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;

            oldStateNumber = stateNumber;
            RepeatUntilNode repeatUntilNode = node as RepeatUntilNode;

            bool hasNot = repeatUntilNode.symbols.hasNot;

            CompileNode(repeatUntilNode.statement, lines, ref stateNumber, globalSymbols);

            ++stateNumber;

            if (hasNot ^ repeatUntilNode.symbols.symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ repeatUntilNode.symbols.symbols.Contains(globalSymbols[i]))
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

            repeatUntilNode.continueState = oldStateNumber;
            repeatUntilNode.breakState = stateNumber;

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

            for (int i = 0; i < switchNode.cases.Count; ++i)
            {
                if (switchNode.cases[i].type == TYPE.CASE)
                {
                    (switchNode.cases[i] as CaseNode).entryState = stateNumber;
                    CompileNode((switchNode.cases[i] as CaseNode).statement, lines, ref stateNumber, globalSymbols);
                }
                else
                {
                    (switchNode.cases[i] as DefaultNode).entryState = stateNumber;
                    CompileNode((switchNode.cases[i] as DefaultNode).statement, lines, ref stateNumber, globalSymbols);
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

            if (switchNode.branches.ContainsKey(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + (switchNode.branches[globalSymbols[0]] as CaseNode).entryState.ToString() + ",@";
            }
            else
            {
                if (switchNode.hasDefault)
                {
                    temp += globalSymbols.Substring(0, 1) + ",q" + (switchNode.branches[globalSymbols[0]] as CaseNode).entryState.ToString() + ",@";
                }
                else
                {
                    temp += globalSymbols.Substring(0, 1) + ",q" + stateNumber.ToString() + ",@";
                }
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (switchNode.branches.ContainsKey(globalSymbols[i]))
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + (switchNode.branches[globalSymbols[i]] as CaseNode).entryState.ToString() + ",@";
                }
                else
                {
                    if (switchNode.hasDefault)
                    {
                        temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + (switchNode.branches[globalSymbols[i]] as CaseNode).entryState.ToString() + ",@";
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

            for (int i = 0; i < loopNode.continueStates.Count; ++i)
            {
                temp = "";

                temp += globalSymbols.Substring(0, 1) + ",q" + (loopNode.continueState) + ",@";

                for (int j = 1; j < globalSymbols.Length; ++j)
                {
                    temp += "\t|\t" + globalSymbols.Substring(j, 1) + ",q" + (loopNode.continueState).ToString() + ",@";
                }
                temp += "\n";
                lines[loopNode.continueStates[i] - 1] = temp;
            }


            for (int i = 0; i < loopNode.breakStates.Count; ++i)
            {
                temp = "";

                temp += globalSymbols.Substring(0, 1) + ",q" + (loopNode.breakState) + ",@";
                for (int j = 1; j < globalSymbols.Length; ++j)
                {
                    temp += "\t|\t" + globalSymbols.Substring(j, 1) + ",q" + (loopNode.breakState) + ",@";
                }
                temp += "\n";
                lines[loopNode.breakStates[i] - 1] = temp;
            }

        }


        public void SetStream(string stream)
        {
            this.parser.SetStream(stream);
        }
    }
}