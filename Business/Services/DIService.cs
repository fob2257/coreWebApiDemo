using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebApiDemo.Business.Services
{
  public class DIService : IDIService
  {
    public string DoSomethingMethod(string message)
    {
      return $"This message: \"{message}\" came back from Dependency Injection.";
    }
  }
}
