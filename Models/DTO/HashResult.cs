﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace coreWebApiDemo.Models.DTO
{
    public class HashResult
    {
        public string Hash { get; set; }
        public byte[] Salt { get; set; }
    }
}
