using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;

namespace MarkupRemove
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("HTML markup remover for Anki\n");
            Console.WriteLine("Please enter full path to the input file: ");
            var inFile = Console.ReadLine();
            Console.WriteLine("Please enter full path to desired output file: ");
            var outFile = Console.ReadLine();

            byte[] arr1;
            using (FileStream fs = File.Open(inFile,
                FileMode.Open, FileAccess.Read))
            {
                bool inTag = false;
                byte[] arr = new byte[fs.Length];
                fs.Read(arr, 0, (int)fs.Length);
                string conv = Encoding.UTF8.GetString(arr);

                int i = 0;
                Timer t = new Timer();
                t.Interval = 1000;
                t.AutoReset = true;
                t.Elapsed += (sender, eventArgs) => PrintProgress(i, conv.Length);
                t.Start();
                PrintProgress(0, conv.Length);
                for (; i < conv.Length; i++)
                {
                    try
                    {   
                        if (conv[i] == '<')
                        {
                            inTag = true;
                        }
                        if (inTag)
                        {
                            if (conv[i] == '>')
                            {
                                if (conv[i - 1] != '\t' && conv[i+1] != '\t')
                                    conv = conv.Substring(0, i + 1) + '\t' + conv.Substring(i + 1);
                                inTag = false;
                            }
                            conv = conv.Substring(0, i) + conv.Substring(i + 1);
                            i--;
                        }
                        if (conv[i] == '"')
                        {
                            conv = conv.Substring(0, i) + conv.Substring(i + 1);
                            i--;
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                arr1 = Encoding.UTF8.GetBytes(conv);
            }
            using (FileStream fs = File.Open(outFile,
                FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(arr1, 0, arr1.Length);
            }
            Console.WriteLine("Done!");
        }

        static void PrintProgress(int i, int len)
        {
            Console.Clear();
            Console.WriteLine("Please wait, this may take a while...");
            Console.WriteLine($"{(float)i / len * 100}% completed");
        }
    }
}
