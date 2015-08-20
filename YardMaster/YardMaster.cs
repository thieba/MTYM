using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardMaster
{
    /// <summary>
    /// Algo : will try to find a trash fitting the space needed by an other (biggest or biggest sum of multiples lines)
    /// If not, will find the bigest space available and try the same
    /// If not will check the total available space for the smallest SpaceNeeded
    /// </summary>

    public class YardMaster
    {
        #region attributes
        private List<YardLine> _lines;
        private BasicLine _trainLine;
        #endregion

        public YardMaster(string path)
        {
            _trainLine = new BasicLine();
            _lines = new List<YardLine>();

            try
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException(path + " is not found");
                var c = GetLetter();
                int index = 0;
                using (StreamReader sr = new StreamReader(path))
                {
                    while (sr.Peek() >= 0)
                    {
                        _lines.Add(new YardLine(c, sr.ReadLine(), index++));
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

        private char GetLetter()
        {
            Console.WriteLine("Enter a car letter:");
            return Console.ReadKey().KeyChar;
        }

        private int SpaceSum()
        {
            return _lines.Select(x => x.SpaceAvailable()).Sum();
        }

        private int BigestTrash()
        {
            int index = -1;

            for (int i = 0; i < 6; ++i)
            {
                if (_lines[i].IsTrash() && (index == -1 || _lines[index].TrashCapacity() < _lines[i].TrashCapacity()))
                    index = i;
            }
            return index;
        }

        private int GetSmallestSpaceNeeded()
        {
            return _lines.Min(x => x.SpaceNeeded());
/*            var smallest = _lines.Where(x => x.SpaceNeeded() == min && x.Index != 0 && x.Index != 5).ToList();

            if (smallest.Count == 0)
                smallest = _lines.Where(x => x.SpaceNeeded() == min).ToList();

            return smallest.First().; INDEX */

        }

        private Stack<int> BacktrackingFullMoveList(int trashIndex, int spaceAvailable, Stack<int> stack)
        {
            var dump = new Stack<int>(stack);
            for (int i = 0; i < 6; ++i)
            {
                if (i != trashIndex && !stack.Contains(i) && _lines[i].SpaceNeeded() <= spaceAvailable)
                {
                    dump.Push(i);
                    var result = BacktrackingFullMoveList(trashIndex, spaceAvailable - _lines[i].SpaceNeeded(), dump);
                    if (result.Sum(x => _lines[x].TrashCapacity()) > stack.Sum(x => _lines[x].TrashCapacity()))
                        stack = new Stack<int>(result);
                    dump.Pop();
                }
            }
            return stack;
        }

        private void ProcessFullMoveList(int trashIndex)
        {
            var stack = new Stack<int>();
            var listOfCommands = BacktrackingFullMoveList(trashIndex, _lines[trashIndex].SpaceAvailable(), stack);
            foreach (var command in listOfCommands)
            {
                while (!_lines[command].IsTrash())
                {
                    _lines[command].Move(_lines[trashIndex], _lines[command].SpaceNeededForNext());
                    CheckCarsReady();
                }
            }
        }

        private void GetTrash()
        {
            int index = BigestTrash();

            if (index >= 0)
            {
                ProcessFullMoveList(index); //should be out
            }
        }

        private void CheckCarsReady()
        {
            int nbCarsReady = 0;

            foreach (var line in _lines)
            {
                if ((nbCarsReady = line.CarsReady()) != 0)
                {
                    line.Move(_trainLine, nbCarsReady);
                }
            }
        }

        private void Status()
        {
            Console.WriteLine("\n----");
            foreach (var line in _lines)
            {
                Console.WriteLine(string.Format("{0}, isATrash:{1}, TrashCapacity:{2}, MovementNeeded:{3}, SpaceNeeded:{4}, SpaceAvailable:{5}", 
                    line.Cars, 
                    line.IsTrash(), 
                    line.TrashCapacity(), 
                    line.MovementNeeded(), 
                    line.SpaceNeeded(), 
                    line.SpaceAvailable()));
            }
            Console.WriteLine("----\n" + _trainLine.Cars.Length + "\n----\n");
        }

        public bool IsResolved()
        {
            bool isResolved = true;
            _lines.ForEach(x =>
            {
                if (!x.IsTrash())
                    isResolved = false;
            });
            return isResolved;
        }

        public bool Resolve()
        {
            Status();
            while (!IsResolved())
            {
                CheckCarsReady();
                GetTrash();
                //Status();
            }
            //Status();
            return true;
        }
    }
}
