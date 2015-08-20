using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardMaster
{
    public class YardMaster
    {
        private List<YardLine> _lines;
        private BasicLine _trainLine;

        public YardMaster(string path)
        {
            _trainLine = new BasicLine("");
            _lines = new List<YardLine>();
            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException(path + " is not found");
                var c = GetALetter();
                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        _lines.Add(new YardLine(c, sr.ReadLine()));
                    }
                }
                if (_lines.Count() != 6)
                    throw new InvalidDataException("The input file doesn't provide 6 lines");
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }

        }

        private char GetALetter()
        {
            Console.WriteLine("Enter a car letter:");
            return Console.ReadKey().KeyChar;
        }

        private int BigestTrash()
        {
            var index = -1;
            for (int i = 0; i < 6; ++i)
            {
                if (_lines[i].IsTrash() && (index == -1 || _lines[index].TrashCapacity() < _lines[i].TrashCapacity()))
                    index = i;
            }
            return index;
        }

        private void Status()
        {
            Console.WriteLine("\n----");
            foreach (var line in _lines)
            {
                Console.WriteLine(string.Format("{0}, isATrash:{1}, TrashCapacity:{2}, MovementNeeded:{3}, SpaceNeeded:{4}", line.Cars, line.IsTrash(), line.TrashCapacity(), line.MovementNeeded(), line.SpaceNeeded()));
            }
            Console.WriteLine("----\n");
        }

        public bool Resolve()
        {
            Status();
            int index = BigestTrash();
            Console.WriteLine(string.Format("The new trash is line {0} > with a capacity of {1}", index + 1, _lines[index].TrashCapacity()));
            return false;
        }
    }
}
