using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardMaster
{
    public class BasicLine
    {
        private string _cars;
        private int _index;

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

        public string Name
        {
            get
            {
                if (_index < 0)
                    return "the train line";
                return $"line {(_index + 1)}";
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="cars">string of the cars line</param>
        /// <param name="index">index of the line</param>
        public BasicLine(string cars = "", int index = -1)
        {
            _index = index;
            if (cars.Length > 10)
                throw new ArgumentException($"the line '{cars}' has more than 10 characters");
            Cars = cars.Replace("0", "");
        }

        /// <summary>
        /// Move nb cars to another BasicLine (By 3 max)
        /// </summary>
        /// <param name="moveTo">BasicLine to move the cars to</param>
        /// <param name="nb">number of cars to move</param>
        public void Move(BasicLine moveTo, int nb)
        {
            int num;
            while ((num = Math.Min(nb, 3)) > 0)
            {
                nb -= num;
                string str = Cars.Substring(0, num);
                Console.WriteLine($"Move {num} car ({str}) from {Name} to {moveTo.Name}.");
                Cars = Cars.Substring(num);
                moveTo.Cars = str + moveTo.Cars;
            }
        }
    }
}
