using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppBackground.Entities.ReporteKuntur.Response.Email;
using AppBackground.Entities.ReporteKuntur.Parameters;
using AppBackground.Util.Mails;

namespace AppBackground.Interfaces
{
    public interface IMail
    {
        void EmailSender(String data, EmailParams user,string IdProcess, ExecuteProcess process);
    }
}
