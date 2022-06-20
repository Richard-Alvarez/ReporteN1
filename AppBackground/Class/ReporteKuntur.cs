using AppBackground.Entities;
using AppBackground.Entities.ReporteKuntur.Parameters;
using AppBackground.Entities.ReporteKuntur.Response.Email;
using AppBackground.Util.Mails;
using AppBackground.Interfaces;
using AppBackground.Repositories.ReportKuntur;
using AppBackground.Util.GenerateFiles;
using LoadMasiveSCTR.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using static AppBackground.Util.Mails.SenderEmail;
using static AppBackground.Entities.Statements;
using AppBackground.Entities.ReporteKuntur;

namespace AppBackground.Class
{
    public class ReporteKuntur : IApplication
    {
        public void Execute(object[] parameters)
        {
            if (parameters != null && parameters.Count() == 1)
            {
                ExecuteProcess process = new ExecuteProcess()
                {
                    IdProcess = parameters[0].ToString(),
                   
                };
                ReportProcessRepository repository = new ReportProcessRepository();
                SenderEmail sender = new SenderEmail();
                EmailParams userdata = new EmailParams();                
                Message msg = new Message();
                StatusParams st = new StatusParams();
                var fileHeaderName = "-CABECERA";
                var fileDetailName = "-DETALLE";

                try
                {
                    repository.Udate();
                   // List<ReporteN1> data = repository.GetAlertList();
                   
                }
                catch (Exception ex)
                {
                    throw ex;

                }

            //    try
            //    {
            //        DataSet data = repository.GetDataReport(process);
            //        var ValType = process.IdType;

            //        if (data.Tables[0].Rows.Count > 0 || data.Tables[1].Rows.Count > 1 || data.Tables[0].Rows.Count > 0 && data.Tables[1].Rows.Count > 0)
            //        {
            //            st.status = repository.UpdateStatus("2", process.IdProcess);
            //            //CABECERA
            //            if (process.IdType == "1" || process.IdType == "3")
            //            {
            //                process.IdType = "1";
            //                DataTable config = repository.GetDataFields(data.Tables[0], process);
            //                DataTable dataFiltered = repository.GetFilteredReportData(data.Tables[0], config, process);
            //                List<ConfigFields> configuration = repository.GetFieldsConfiguration(config, process);
            //                IFiles FileHeader = new Excel(dataFiltered, process.IdProcess + fileHeaderName, process.DesBranch + " - CABECERA", GenerateFolder(process.IdProcess), configuration);
            //                userdata = (repository.EmailList(process.IdUser));
            //                ValType = (process.IdType == "1") ? process.IdType = ValType : process.IdType;
            //                if (process.IdType == "1")
            //                {
            //                    sender.EmailSender(st.status, userdata, process.IdProcess, process);
            //                }
            //            }

            //            //DETALLE
            //            if (process.IdType == "2" || process.IdType == "3")
            //            {
            //                process.IdType = "2";
            //                DataTable config = repository.GetDataFields(data.Tables[1], process);
            //                DataTable dataFiltered = repository.GetFilteredReportData(data.Tables[1], config, process);
            //                List<ConfigFields> configuration = repository.GetFieldsConfiguration(config, process);
            //                IFiles FileDetail = new Excel(dataFiltered, process.IdProcess + fileDetailName, process.DesBranch + " - DETALLE", GenerateFolder(process.IdProcess), configuration);
            //                userdata = (repository.EmailList(process.IdUser));
            //                ValType = (process.IdType == "2") ? process.IdType = ValType : process.IdType;
            //                if (process.IdType == "2" || process.IdType == "3")
            //                {                              
            //                    sender.EmailSender(st.status, userdata, process.IdProcess, process);
            //                }
            //            }
            //        }

            //        else
            //        {
            //            st.status = repository.UpdateStatus("4", process.IdProcess);
            //            if (process.IdType == "1" || process.IdType == "3")
            //            {
            //                process.IdType = "1";
            //                DataTable config = repository.GetDataFields(data.Tables[0], process);
            //                DataTable dataFiltered = repository.GetFilteredReportData(data.Tables[0], config, process);
            //                List<ConfigFields> configuration = repository.GetFieldsConfiguration(config, process);                            
            //                userdata = (repository.EmailList(process.IdUser));
            //                ValType = (process.IdType == "1") ? process.IdType = ValType : process.IdType;
            //                if (process.IdType == "1")
            //                {                          
            //                    sender.EmailSender(st.status, userdata, process.IdProcess, process);
            //                }
            //            }

            //            //DETALLE
            //            if (process.IdType == "2" || process.IdType == "3")
            //            {
            //                process.IdType = "2";
            //                DataTable config = repository.GetDataFields(data.Tables[1], process);
            //                DataTable dataFiltered = repository.GetFilteredReportData(data.Tables[1], config, process);
            //                List<ConfigFields> configuration = repository.GetFieldsConfiguration(config, process);                           
            //                userdata = (repository.EmailList(process.IdUser));
            //                ValType = (process.IdType == "2") ? process.IdType = ValType : process.IdType;
            //                if (process.IdType == "2" || process.IdType == "3")
            //                {
            //                    st.status = "4";
            //                    sender.EmailSender(st.status, userdata, process.IdProcess, process);
            //                }

            //            }                        
            //            repository.UpdateStatus("4", process.IdProcess);
            //        }

            //    }

            //    catch (Exception ex)
            //    {
            //        //ex.Message = "No se han encontrado información para generar el reporte";
            //        st.status = repository.UpdateStatus("3", process.IdProcess);
            //        userdata = (repository.EmailList(process.IdUser));
            //        sender.EmailSender(st.status, userdata, process.IdProcess, process);
            //    }
            //
            }
            else
            {
               // LogHelper.Exception(string.Empty, LogHelper.Paso.ParametersIncomplete, "missing Parameters");
            }
        }

        //Crear la carpeta del archivo excel
        public string GenerateFolder(string Id)
        {
            string Folder = CombinePath(ConfigurationManager.AppSettings["PathSave"].ToString(), Id);
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
            return Folder;
        }
        private String CombinePath(string Path, string File)
        {
            return String.Format(@"{0}\{1}", Path, File);
        }

        #region dummies
        private DataTable GenerateTableDamy()
        {
            DataTable data = new DataTable();

            DataColumn column = new DataColumn();
            //column.DataType = System.Type.GetType("System.Decimal"); ;
            column.Namespace = "MONTO1";
            data.Columns.Add(column);

            DataColumn column2 = new DataColumn();
            //column2.DataType = System.Type.GetType("System.Decimal"); ;
            column2.Namespace = "MONTO2";
            data.Columns.Add(column2);


            for (int i = 0; i < 4; i++)
            {
                DataRow row = data.NewRow();
                row[0] = "1.2222";
                row[1] = "124.22";
                data.Rows.Add(row);
            }
            return data;
        }
        #endregion

    }
}
