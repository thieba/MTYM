using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardMaster
{
    /// <summary>
    /// Line class used for the yard line
    /// </summary>
    public class YardLine : BasicLine
    {
        #region attributes
        private char _letter;
        #endregion

        #region constructors
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="letter">car letter to find</param>
        /// <param name="cars">string of the cars line</param>
        /// <param name="index">index of the line</param>
        public YardLine(char letter, string cars, int index):
            base(cars, index)
        {
            _letter = letter;
        }
        #endregion

        /// <summary>
        /// Check if there is asked car left in the line
        /// </summary>
        /// <returns>true if there is no more asked cars</returns>
        public bool IsTrash()
        {
            return Cars.IndexOf(_letter) == -1;
        }

        /// <summary>
        /// Return how many space available in the line
        /// </summary>
        /// <returns>spaces availables</returns>
        public int SpaceAvailable()
        {
            return 10 - Cars.Length;
        }

        /// <summary>
        /// Return the space capacity once the asked car removed
        /// </summary>
        /// <returns>the space capacity</returns>
        public int TrashCapacity()
        {
            int index = Cars.LastIndexOf(_letter) + 1;
            if (index == 0)
                return 10 - Cars.Length;
            return 10 - (Cars.Length - index);
        }

        /// <summary>
        /// Return the number of asked cars left in the line
        /// </summary>
        /// <returns>the number of asked car left</returns>
        public int CarsCount()
        {
            return Cars.Count(x => x == _letter);
        }

        /// <summary>
        /// Return the number of spaces needed from other lines to makes the line a trash
        /// </summary>
        /// <returns>number of spaces neeeded</returns>
        public int SpaceNeeded()
        {
            int index = Cars.LastIndexOf(_letter);
            if (index < 0)
                return 0;
            return Cars.Remove(index).Replace(_letter.ToString(), "").Length;
        }

        /// <summary>
        /// Return the number of spaces needed from other lines to pull the next asked car
        /// </summary>
        /// <returns>number of spaces needed</returns>
        public int SpaceNeededForNext()
        {
            return Cars.IndexOf(_letter);
        }

        /// <summary>
        /// Return the number of cars ready to be transfered to the train line
        /// </summary>
        /// <returns>number of cars ready</returns>
        public int CarsReady()
        {
            return Cars.TakeWhile(x => x == _letter).Count();
        }

        /// <summary>
        /// Return the number of movements you will need to pull off every asked cars from the line
        /// </summary>
        /// <returns>number of movement needed</returns>
        public int MovementNeeded()
        {
            var count = 0;
            if (!IsTrash())
            {
                var dump = Cars;
                int index;
                while ((index = dump.IndexOf(_letter)) != -1)
                {
                    count += (index + 2) / 3;
                    dump = dump.Substring(index, dump.Length - index);
                    index = dump.TakeWhile(x => x == _letter).Count();
                    count += (index + 2) / 3;
                    dump = dump.Substring(index, dump.Length - index);
                }
            }
            return count;
        }
    }
}
