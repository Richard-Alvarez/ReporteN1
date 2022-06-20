using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBackground.Entities.ReporteKuntur.Parameters
{
    public class ExecuteProcess
    {
        public string IdProcess { get; set; }
        public string IdType { get; set; }
        public int IdBranch { get; set; }
        public string DesBranch { get; set; }
        public int IdProduct { get; set; }
        public int IdUser { get; set; }
        public int IdProfile { get; set; }    
        public string StartDate { get; set; }
        public string EndDate { get; set; } 
        public Object Data { get; set; }
    }
}