namespace BrainFricker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args != null)
                using (FileStream fs = new FileStream(args[0], FileMode.Open))
                    if (fs.Name.Contains(".bf"))
                    {
                        var buf = new byte[new FileInfo(fs.Name).Length - 3]; // This - 3 exists because the first 3 bytes of the txt file are padding, 239, 187, 191.
                        fs.Position = 3;
                        fs.Read(buf, 0, buf.Length);
                        Console.WriteLine("Input args:");
                        Console.WriteLine(Interpret(buf, Console.ReadLine()));
                    }
                    else
                        throw new InvalidDataException();
            else
                throw new ArgumentNullException();

        }

        static string Interpret(byte[] code, string? args_)
        {
            var tape = new byte[30000];
            int tapePos = 0, argPos = 0;
            var output = new List<byte>();

            string args;
            if (args_ != null)
                args = args_;
            else
                args = string.Empty;

            List<int> startLoop = new List<int>(), endLoop = new List<int>();

            for (var codePos = 0; codePos < code.Length; codePos++) // TODO: handle underflow and overflow
            {
                var c = code[codePos];
                if (c == 0x3e)
                    tapePos++;
                else if (c == 0x3c)
                    tapePos--;
                else if (c == 0x2b)
                    tape[tapePos]++;
                else if (c == 0x2d)
                    tape[tapePos]--;
                else if (c == 0x2e)
                    output.Add(tape[tapePos]);
                else if (c == 0x2c)
                    try { tape[tapePos] = (byte)args[argPos]; }
                    catch (IndexOutOfRangeException u) { tape[tapePos] = 0x00; }
                    finally { argPos++; }
                else if (c == 0x5b)
                    startLoop.Add(codePos);
                else if (c == 0x5d)
                    if (tape[tapePos] != 0x00)
                        codePos = startLoop.Last();
                    else
                        startLoop.RemoveAt(startLoop.Count - 1);
            }

            var ret = "";
            for(int i = 0; i < output.Count; i++)
                ret += (char)output[i];
            return ret;
        }
    }
}