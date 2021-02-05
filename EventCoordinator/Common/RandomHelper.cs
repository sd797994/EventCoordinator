using System;
using System.Collections.Generic;
using System.Text;

namespace EventCoordinator.Common
{
    public class RandomHelper
    {
        public static bool GetRandomBool(bool? @default = null)
        {
            if (@default != null)
                return @default.Value;
            var rd = new Random(Guid.NewGuid().GetHashCode());
            return rd.Next(0, 2) == 0;
        }
        public static int GetRandomInt(int min,int max)
        {
            var rd = new Random(Guid.NewGuid().GetHashCode());
            return rd.Next(min, max);
        }
    }
}
