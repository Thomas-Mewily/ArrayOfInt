using System;
using System.Collections.Generic;
using System.Linq;

namespace LowLevel
{
    public class ExtraConsole
    {
        public ConsoleColor ColorBackground;
        public ConsoleColor ColorForeground;
        public List<Line> Lines;

        public ExtraConsole()
        {
            Clear();
            ResetColor();
        }

        public void Clear()
        {
            Lines = new List<Line>
            {
                new Line()
            };
        }

        public void ResetColor(ConsoleColor colorForeground = ConsoleColor.White, ConsoleColor colorBackground = ConsoleColor.Black)
        {
            ColorForeground = colorForeground;
            ColorBackground = colorBackground;
        }

        public void CreateNewLine() => Lines.Add(new Line());
        public void DeleteLastLine() 
        {
            Lines.RemoveAt(Lines.Count);
            if (Lines.Count == 0) { CreateNewLine(); }
        }

        public static string NewLine = "\n";

        public void Write(params object[] obj) => obj.ToList().ForEach(o => Write(o.ToString()));
        public void Write(string txt)
        {
            if (txt == NewLine)
            {
                CreateNewLine();
            }
            else
            {
                var all = txt.Split(NewLine);
                for (int i = 0; i < all.Length; i++) 
                {
                    Lines[^1].AllWord.Add(new Word(all[i], ColorForeground, ColorBackground));
                    if(i != all.Length - 1) 
                    {
                        CreateNewLine();
                    }
                }
            }
        }
        public void WriteLine(object obj) { Write(obj); CreateNewLine(); }
        public void WriteLine(params object[] obj) { Write(obj); CreateNewLine(); }

        public void WriteToConsole(int lineBegin = 0) 
        {
            for(int i = 0; i < Lines.Count; i++) 
            {
                Lines[i].WriteToConsole();
            }
            Useful.ResetConsoleColor();
            Console.WriteLine();
        }

        public void Read() 
        {
            
        }
    }

    public class Line
    {
        public List<Word> AllWord;

        public Line()
        {
            AllWord = new List<Word>();
        }

        public void WriteToConsole() 
        { 
            AllWord.ForEach(w => w.WriteToConsole());
            Console.WriteLine();
        }
    }

    public class Word
    {
        public string Text;
        public ConsoleColor ColorBackground;
        public ConsoleColor ColorForeground;

        //public Word(ExtraConsole extraConsole) : this("", extraConsole.ColorForeground, extraConsole.ColorBackground) { }
        public Word(string text, ConsoleColor colorForeground, ConsoleColor colorBackground)
        {
            Text = text;
            ColorForeground = colorForeground;
            ColorBackground = colorBackground;
        }

        public void WriteToConsole()
        {
            Useful.ResetConsoleColor(ColorForeground, ColorBackground);
            Console.Write(Text);
        }
    }
}
