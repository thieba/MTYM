using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardMaster
{
    public class YardLine : BasicLine
    {
        #region attributes
        private char _letter;
        #endregion

        #region constructors
        public YardLine(char letter, string cars):
            base(cars)
        {
            _letter = letter;
        }
        #endregion

        public bool IsTrash()
        {
            return Cars.IndexOf(_letter) == -1;
        }

        public int SpaceAvailable()
        {
            return 10 - Cars.Length;
        }

        public int TrashCapacity()
        {
            int index = Cars.LastIndexOf(_letter) + 1;
            if (index == 0)
                return 10 - Cars.Length;
            return 10 - (Cars.Length - index);
        }

        public int SpaceNeeded()
        {
            int index = Cars.LastIndexOf(_letter) + 1;
            if (index == 0)
                return 0;
            return Cars.Remove(index).Replace(_letter.ToString(), "").Length;
        }

        public int SpaceNeededForNext()
        {
            return Cars.IndexOf(_letter) + 1;
        }

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
