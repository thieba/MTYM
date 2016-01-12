using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YardMaster.Helpers;

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

        #region properties
        public List<YardLine> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                _lines = value;
            }
        }

        public BasicLine TrainLine
        {
            get
            {
                return _trainLine;
            }
            set
            {
                _trainLine = value;
            }
        }
        #endregion

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="path">path of the input file to use</param>
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

        /// <summary>
        /// CTOR No blocking
        /// </summary>
        /// <param name="path">path of the input file to use</param>
        /// <param name="c">car to find</param>
        public YardMaster(string path, char c)
        {
            _trainLine = new BasicLine();
            _lines = new List<YardLine>();


            if (!File.Exists(path))
                throw new FileNotFoundException(path + " is not found");
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


        /// <summary>
        /// Get the letter looking for
        /// </summary>
        /// <returns>the letter given</returns>
        private char GetLetter()
        {
            Console.WriteLine("Enter a car letter:");
            return Console.ReadKey().KeyChar;
        }

        /// <summary>
        /// Calculate the total space from all the lines
        /// </summary>
        /// <returns>return the number of spaces</returns>
        private int SpaceSum()
        {
            return _lines.Select(x => x.SpaceAvailable()).Sum();
        }

        /// <summary>
        /// Find the biggest trash to use
        /// </summary>
        /// <returns>index of the line found</returns>
        private int BiggestTrash()
        {
            int index = -1;

            for (int i = 0; i < 6; ++i)
            {
                if (_lines[i].IsTrash() && (index == -1 || _lines[index].TrashCapacity() < _lines[i].TrashCapacity()))
                    index = i;
            }
            return index;
        }

        /// <summary>
        /// Find the line which need the less space from the other lines to become a trash
        /// </summary>
        /// <returns>index of the line found</returns>
        private int GetSmallestSpaceNeeded()
        {
            return _lines.Min(x => x.SpaceNeeded());
        }

        /// <summary>
        /// Will backtrack the solutions to find the optimize line to pull
        /// </summary>
        /// <param name="trashIndex">index of the trash line</param>
        /// <param name="spaceAvailable">space available in the trash line</param>
        /// <param name="stack">stack of the used lines</param>
        /// <returns></returns>
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

        /// <summary>
        /// Will find the optimize solution to transform the maximum line to trash using a trash line
        /// </summary>
        /// <param name="trashIndex">index of the trash line</param>
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

        /// <summary>
        /// Find a line where we can get all the asked cars
        /// </summary>
        /// <returns>return the index of the car line</returns>
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

        /// <summary>
        /// Find the most optimized line to get the next car from
        /// </summary>
        /// <returns>return the index of the car line</returns>
        private int FindLineToPullToDistribution()
        {
            int spaceAvailable = SpaceSum();
            return _lines.Where(x => x.SpaceNeededForNext() <= (spaceAvailable - x.SpaceAvailable()))
                             .OrderBy(x => x.CarsCount()).OrderBy(x => x.SpaceNeededForNext()).First().Index;
        }

        /// <summary>
        /// Will distribute all the cars from a line until there's no more car asked in the line
        /// </summary>
        /// <param name="index">index of the line where get the asked car</param>
        private void DistributeToTrash(int index)
        {
            while (!_lines[index].IsTrash())
                Distribute(index);
#if DEBUG
            YardHelper.Status(this);
#endif
        }

        /// <summary>
        /// Distribute the top cars from a line to the others in order to access to an asked car
        /// </summary>
        /// <param name="index">index of the line where get the asked car</param>
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

        /// <summary>
        /// Will find the best opportunity to get a asked car and will process the moves
        /// </summary>
        private void Process()
        {
            int index = BiggestTrash();

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

        /// <summary>
        /// Check if there is cars ready to pull from the left
        /// </summary>
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

        /// <summary>
        /// Check if the yard is resolvable
        /// </summary>
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

        /// <summary>
        /// Check if the yard is resolved
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Resolve the yard test
        /// </summary>
        public void Resolve()
        {
#if DEBUG
            YardHelper.Status(this);
#endif
            while (!IsResolved())
            {
                IsResolvable();
                CheckCarsReady();
                Process();
            }
#if DEBUG
            YardHelper.Status(this);
#endif
            Console.WriteLine("Done");
        }
    }
}
