using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using coreWebApiDemo.Models.DTO;

namespace coreWebApiDemo.Business.Services
{
    public interface IHashService
    {
        HashResult HashString(string input);
    }
}
