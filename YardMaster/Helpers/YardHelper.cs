using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YardMaster.Helpers
{
    public class YardHelper
    {
        /// <summary>
        /// Give the status of the lines
        /// </summary>
        public static void Status(YardMaster yardMaster)
        {
            Console.WriteLine("\n----");
            foreach (var line in yardMaster.Lines)
            {
                Console.WriteLine(string.Format("{0}, isATrash:{1}, TrashCapacity:{2}, MovementNeeded:{3}, SpaceNeeded:{4}, SpaceAvailable:{5}",
                    line.Cars,
                    line.IsTrash(),
                    line.TrashCapacity(),
                    line.MovementNeeded(),
                    line.SpaceNeeded(),
                    line.SpaceAvailable()));
            }
            Console.WriteLine("----\n" + yardMaster.TrainLine.Cars.Length + "\n----\n");
        }
    }
}
