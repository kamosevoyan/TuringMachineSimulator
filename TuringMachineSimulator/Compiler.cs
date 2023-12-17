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

        public string compile()
        {
            MainNode node = this.parser.parse();

            string symbols = this.parser.globalSymbols.symbols;
            List<string> lines = new List<string>();

            string temp = "";
            int stateNumber = 0;

            compileNode(node, lines, ref stateNumber, symbols);

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

        private void compileNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            switch(node.type) 
            {
                case TYPE.LEFT:
                    {
                        compileLeftNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.RIGHT:
                    {
                        compileRightNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.EXIT:
                    {
                        compileExitNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.ERROR:
                    {
                        compileErrorNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.CONTINUE:
                    {
                        compileContinueNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.BREAK:
                    {
                        compileBreakNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.WRITE:
                    {
                        compileWriteNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.MAIN:
                    {
                        compileMainNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.BLOCK:
                    {
                        compileBlockNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.IF:
                    {
                        compileIfNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.IF_ELSE:
                    {
                        compileIfElseNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.WHILE:
                    {
                        compileWhileNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.DO_WHILE:
                    {
                        compileDoWhileNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.REPEAT_UNTIL:
                    {
                        compileRepeatUntilNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                case TYPE.SWITCH:
                    {
                        compileSwitchNode(node, lines, ref stateNumber, globalSymbols);
                        break;
                    }

                default:
                    {
                        throw new Exception("Unexpected node to compile.");
                    }
            }
        }

        private void compileLeftNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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
        private void compileRightNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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
        private void compileExitNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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

        private void compileErrorNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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

        private void compileContinueNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            ++stateNumber;

            lines.Add("");
            ((node as FlowControllNode).ownerLoop as LoopNode).continueStates.Add(stateNumber);
        }


        private void compileBreakNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            ++stateNumber;

            lines.Add("");
            ((node as FlowControllNode).ownerLoop as LoopNode).breakStates.Add(stateNumber);
        }

        private void compileWriteNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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

        private void compileMainNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            compileNode((node as MainNode).statement, lines, ref stateNumber, globalSymbols);
        }

        private void compileBlockNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            BlockNode blockNode = node as BlockNode;
            for (int i = 0; i < blockNode.statements.Count; ++i)
            {
                compileNode(blockNode.statements[i], lines, ref stateNumber, globalSymbols);
            }
        }
        private void compileIfNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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

            compileNode(ifNode.statement, lines, ref stateNumber, globalSymbols);

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

        private void compileIfElseNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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

            compileNode(ifElseNode.ifStatement, lines, ref stateNumber, globalSymbols);
            lines.Add("");

            ++stateNumber;

            int elseOldStateNumber = stateNumber;
            int elseCurrentLine = lines.Count - 1;

            compileNode(ifElseNode.elseStatement, lines, ref stateNumber, globalSymbols);


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

        private void compileWhileNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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

            compileNode(whileNode.statement, lines, ref stateNumber, globalSymbols);


            if (hasNot ^ whileNode.symbols.symbols.Contains(globalSymbols[0]))
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + oldStateNumber.ToString() + ",@";
            }
            else
            {
                temp += globalSymbols.Substring(0, 1) + ",q" + (stateNumber+1).ToString() + ",@";
            }

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                if (hasNot ^ whileNode.symbols.symbols.Contains(globalSymbols[i]))
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + oldStateNumber.ToString() + ",@";
                }
                else
                {
                    temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + (stateNumber+1).ToString() + ",@";
                }
            }


            temp += "\n";
            lines[curentLine] = temp;

            temp = "";
            ++stateNumber;

            temp += globalSymbols.Substring(0, 1) + ",q" + (oldStateNumber - 1).ToString() + ",@";

            for (int i = 1; i < globalSymbols.Length; ++i)
            {
                temp += "\t|\t" + globalSymbols.Substring(i, 1) + ",q" + (oldStateNumber- 1).ToString() + ",@";
            }

            temp += "\n";

            lines.Add(temp);

            whileNode.continueState = oldStateNumber - 1;
            whileNode.breakState = stateNumber;

            putFlowControls(node, lines, ref stateNumber, globalSymbols);
        }

        private void compileDoWhileNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;

            oldStateNumber = stateNumber;
            DoWhileNode doWhileNode = node as DoWhileNode;

            bool hasNot = doWhileNode.symbols.hasNot;

            compileNode(doWhileNode.statement, lines, ref stateNumber, globalSymbols);

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

            putFlowControls(node, lines, ref stateNumber, globalSymbols);
        }

        private void compileRepeatUntilNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp = "";
            int oldStateNumber;

            oldStateNumber = stateNumber;
            RepeatUntilNode repeatUntilNode = node as RepeatUntilNode;

            bool hasNot = repeatUntilNode.symbols.hasNot;

            compileNode(repeatUntilNode.statement, lines, ref stateNumber, globalSymbols);

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

	        putFlowControls(node, lines, ref stateNumber, globalSymbols);

        }

        private void compileSwitchNode(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
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
                    compileNode((switchNode.cases[i] as CaseNode).statement, lines, ref stateNumber, globalSymbols);
                }
                else
                {
                    (switchNode.cases[i] as DefaultNode).entryState = stateNumber;
                    compileNode((switchNode.cases[i] as DefaultNode).statement, lines, ref stateNumber, globalSymbols);
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

        private void putFlowControls(Node node, List<string> lines, ref int stateNumber, string globalSymbols)
        {
            string temp;

            LoopNode loopNode = node as LoopNode;

	        for (int i = 0; i < loopNode.continueStates.Count; ++i)
	        {
		        temp = "";
		
		        temp += globalSymbols.Substring(0,1) + ",q" + (loopNode.continueState) + ",@";

		        for (int j = 1; j<globalSymbols.Length; ++j)
		        {
			        temp += "\t|\t" +  globalSymbols.Substring(j,1) + ",q" + (loopNode.continueState).ToString() + ",@";
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


        public void setStream(string stream)
        {
            this.parser.setStream(stream);
        }
    }
}