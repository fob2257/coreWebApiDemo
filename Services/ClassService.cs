using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebApiDemo.Services
{
    public class ClassService : IClassService
    {
        public void DoSomething(string message)
        {
            Console.WriteLine(message);
        }
    }
}
