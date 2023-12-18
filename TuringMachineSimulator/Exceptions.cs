using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineSimulator
{
    public class SyntaxErrorException : ApplicationException
    {

        public SyntaxErrorException(string message) : base(message) { }

    }
}
