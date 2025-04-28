using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Program
    {
        static void Main(string[] args) {
            using (Window window = new Window(1600, 1600))
            {
                window.Run();
            }
        }
    }
}
