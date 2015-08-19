using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardMaster
{
    public class Line
    {
        #region attributes
        private string _cars;
        private char _letter;
        #endregion

        #region properties
        public string Cars
        {
            get
            {
                return _cars;
            }
            private set
            {
                _cars = value;
            }
        }
        #endregion

        #region constructors
        public Line(char letter, string cars)
        {
            _letter = letter;
            _cars = cars.Replace("0", "");
        }
        #endregion

        public int CapacityTrash()
        {
            int index = Cars.LastIndexOf(_letter);
            if (index != -1)
                return 10 - Cars.Length;
            return 10 - (Cars.Length - index);
        }

        public int SpaceNeeded()
        {
            int index = Cars.LastIndexOf(_letter);
            if (index != -1)
                return 0;
            return Cars.Remove(index).Replace(_letter.ToString(), "").Length;
        }

        public bool IsTrash()
        {
            return Cars.IndexOf(_letter) == -1;
        }
    }
}
