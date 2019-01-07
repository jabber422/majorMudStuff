namespace MMudObjects
{

    public class RoomExit
    {
        public string Exit;
        public RoomExit(string exit)
        {
            //there are non displayable characters hidden in the exit strings.  I think this n, '<some capital letter>', '\b', o, r, t, h
            char[] str = exit.ToCharArray();
            string result = "";
            foreach (char c in str)
            {
                if (c == '\b')
                {
                    result = result.Remove(result.Length-1);
                }
                else
                {
                    result += c.ToString();
                }
            }
            this.Exit = result;
        }

        public override string ToString()
        {
            return this.Exit;
        }
    }
}