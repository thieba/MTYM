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

        private int FindLineToPullToTrash()
        {
            int index = -1;
            int spaceAvailable = SpaceSum();

            foreach (var line in _lines)
            {
                if ((line.SpaceNeeded() <= (spaceAvailable - line.SpaceAvailable())) && 
                    (index < 0 || _lines[index].TrashCapacity() < line.TrashCapacity()))
                    index = line.Index;
            }
            return index;
        }

        private int FindLineToPullToDistribution()
        {
            int spaceAvailable = SpaceSum();
            return _lines.Where(x => x.SpaceNeededForNext() <= (spaceAvailable - x.SpaceAvailable()))
                             .OrderBy(x => x.CarsCount()).OrderBy(x => x.SpaceNeededForNext()).First().Index;
        }

        private void DistributeToTrash(int index)
        {
            while (!_lines[index].IsTrash())
                Distribute(index);
#if DEBUG
            Status();
#endif
        }

        private void Distribute(int index)
        {
            int spaceToFill = _lines[index].SpaceNeededForNext();
            int nbGCars = _lines[index].CarsCount();

            while (nbGCars == _lines[index].CarsCount())
            {
                var linesToPush = _lines.Where(x => x.Index != index && x.SpaceAvailable() != 0)
                                        .OrderBy(x => 10 - x.SpaceNeededForNext())
                                        .OrderBy(x => (x.SpaceNeededForNext() + Math.Max(spaceToFill, 3)) % 3).ToList();
                for (int i = 0, s = Math.Min(3, spaceToFill); s > 0;)
                {
                    if (linesToPush[i].SpaceAvailable() >= s)
                    {
                        spaceToFill -= s;
                        _lines[index].Move(_lines[linesToPush[i].Index], s);
                        CheckCarsReady();
                        break;
                    }
                    if (i++ >= linesToPush.Count)
                    {
                        i = i % linesToPush.Count;
                        s--;
                    }
                }
            }
        }

        private void Process()
        {
            int index = BigestTrash();

            if (index >= 0)
                ProcessFullMoveList(index);
            else
            {
                index = FindLineToPullToTrash();

                if (index >= 0)
                    DistributeToTrash(index);
                else
                    Distribute(FindLineToPullToDistribution());
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

        public void IsResolvable()
        {
            var spaceSum = SpaceSum();
            foreach (var line in _lines)
            {
                if (!line.IsTrash() && (spaceSum - line.SpaceAvailable()) >= line.SpaceNeededForNext())
                    return;
            }
            throw new InvalidDataException("There is no solution");
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
#if DEBUG
            Status();
#endif
            while (!IsResolved())
            {
                IsResolvable();
                CheckCarsReady();
                Process();
            }
#if DEBUG
            Status();
#endif
            Console.WriteLine("Done");
            return true;
        }

        #region helpers
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
        #endregion
    }
}
