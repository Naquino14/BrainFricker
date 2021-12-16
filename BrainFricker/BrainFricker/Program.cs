using System;
using System.Text;

namespace BrainFricker
{
    public class Program
    {
        static readonly byte 
            nxt = 0x3e,
            prv = 0x3c,
            inc = 0x2b,
            din = 0x2d,
            oup = 0x2e,
            inp = 0x2c,
            swh = 0x5b,
            ewh = 0x5d;
        public static void Main(string[] args)
        {
            if (args != null)
                using (FileStream fs = new FileStream(args[0], FileMode.Open))
                    if (fs.Name.Contains(".bf"))
                    {
                        // This - 3 exists because the first 3 bytes of the txt file are padding, 239, 187, 191.
                        var buf = new byte[new FileInfo(fs.Name).Length-3];
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

            // Start and end position of a while loop in the tape
            //int startLoop = 0, endLoop = 0;

            // Allows for multiple while loops inside itself
            // How it works:
            // index 0 is first loop
            // index 1 is a loop inside the first loop
            // index 2 is a loop inside the second loop
            // and so on and so fourth

            List<int> startLoop = new List<int>(), endLoop = new List<int>();

            for (var codePos = 0; codePos < code.Length; codePos++) // interpret script
            {
                // parse byte
                byte c = code[codePos];
                if (c == nxt)
                    tapePos++;
                else if (c == prv)
                    tapePos--;
                else if (c == inc)
                    tape[tapePos]++;
                else if (c == din)
                    tape[tapePos]--;
                else if (c == oup)
                    output.Add(tape[tapePos]);
                else if (c == inp)
                {
                    try
                    {
                        tape[tapePos] = (byte)args[argPos];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        tape[tapePos] = 0x00;
                    }
                    finally
                    {
                        argPos++;
                    }
                }
                else if (c == swh)
                {
                    startLoop.Add(codePos); // add position of the [ character
                }
                else if (c == ewh)
                {
                    
                    if (tape[tapePos] != 0x00)
                    {
                        codePos = startLoop.Last();
                    }
                    else
                    {
                        // Exit the while loop
                        // Remove it from the count
                        startLoop.RemoveAt(startLoop.Count - 1);
                    }
                }
                // if there's a character that isn't supposed to be there, just skip it
            }

            String ret = "";

            for(int i = 0; i < output.Count; i++)
            {
                ret += (char)output[i];
            }
            return ret; // output array to string
        }
    }
}