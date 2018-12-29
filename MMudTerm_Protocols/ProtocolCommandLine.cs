using System;
using System.Collections.Generic;

namespace MMudTerm_Protocols
{
    //contains all the TermCmd that make up one line (delim '\n')
    public class ProtocolCommandLine
    {
        public List<TermCmd> Fragments;

        public ProtocolCommandLine(Queue<TermCmd> fragments)
        {
            this.Fragments = new List<TermCmd>(fragments);
        }

        //strip out the protcol markup and return the line as text
        public override string ToString()
        {
            String result = "";
            foreach (TermCmd cmd in this.Fragments)
            {
                result += cmd.ToString();
            }
            return result;
        }
    }
}
