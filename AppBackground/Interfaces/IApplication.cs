using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBackground.Entities
{
    public interface IApplication
    { 
        void Execute(Object[] parameters);        
    }
}
