using AppBackground.Class;
using AppBackground.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppBackground
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() > 0)
            {
                IApplication application = new ReporteKuntur();
                application.Execute(args[0].Split('|'));
            }
        }
    }
}
