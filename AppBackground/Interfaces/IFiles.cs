using AppBackground.Entities.ReporteKuntur;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBackground.Interfaces
{
    public interface IFiles
    {
         void Generate(DataTable data, List<ConfigFields> configuration);       
    }
}
