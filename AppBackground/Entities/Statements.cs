﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBackground.Entities
{
    public class Statements
    {
        public enum Application
        {
            ReporteKuntur  = 0
        }
        public enum Status
        {
            Registrate = 0,
            Initial = 1,
            correct = 2,
            error = 3
        }
    }
}
