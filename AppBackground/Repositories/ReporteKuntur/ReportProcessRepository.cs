using AppBackground.Entities.ReporteKuntur.Parameters;
using AppBackground.Entities.ReporteKuntur.Response.Email;
using AppBackground.Entities.ReporteKuntur.Response.ResponseControl;
using LoadMasiveSCTR.Util;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppBackground.Entities.Statements;
using AppBackground.Entities.ReporteKuntur;

namespace AppBackground.Repositories.ReportKuntur
{
    public class ReportProcessRepository
    {
        static string Package = "LAFT.PKG_LAFT_GESTION_ALERTAS";

        //Método para introducir un datatable a una entidad
        public static class CommonMethod
        {
            public static List<T> ConvertToList<T>(DataTable dt)
            {
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
                var properties = typeof(T).GetProperties();
                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();
                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                        {
                            try
                            {
                                pro.SetValue(objT, row[pro.Name]);
                            }
                            catch (Exception ex) { }
                        }
                    }
                    return objT;
                }).ToList();
            }
        }

        //Método para guardar datos de una tabla en una entidad
        public List<ConfigFields> GetFieldsConfiguration(DataTable Config, ExecuteProcess parameters)
        {
            List<ConfigFields> FieldConfiglist = new List<ConfigFields>();
            FieldConfiglist = CommonMethod.ConvertToList<ConfigFields>(Config);
            return FieldConfiglist;
        }

        //Método para obtener las columnas para la tabla en proceso.
        private List<string> getColumnsName(DataTable table)
        {
            List<string> columns = new List<string>();
            foreach (DataColumn column in table.Columns)
            {
                columns.Add(column.ColumnName);
            }
            return columns;
        }
        
        //Método para obtener la data de ramos individuales para el reporte excel.
        public DataSet GetDataReport(ExecuteProcess parameters)
        {
            DataSet dataReport = new DataSet();
            DataTable dataCab = new DataTable();
            DataTable dataDet = new DataTable();
            int Pcode;
            string Pmessage;

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        cmd.CommandText = string.Format("{0}.{1}", Package, "SPS_GENERAR_REPORTES");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_IDPROCESS", OracleDbType.Varchar2).Value = parameters.IdProcess;
                        cmd.Parameters.Add("P_NBRANCH", OracleDbType.Int32).Value = parameters.IdBranch;
                        cmd.Parameters.Add("P_TIPO_DESCARGA", OracleDbType.Varchar2).Value = parameters.IdType;
                        cmd.Parameters.Add("P_NUSERCODE", OracleDbType.Int32).Value = parameters.IdUser;
                        cmd.Parameters.Add("P_FEINI", OracleDbType.Varchar2).Value = parameters.StartDate;
                        cmd.Parameters.Add("P_FEFIN", OracleDbType.Varchar2).Value = parameters.EndDate;
                        var P_ES_ERROR = new OracleParameter("P_ES_ERROR", OracleDbType.Int32, ParameterDirection.Output);
                        var P_ERROR_MSG = new OracleParameter("P_ERROR_MSG", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_ES_ERROR.Size = 2000;
                        P_ERROR_MSG.Size = 4000;
                        cmd.Parameters.Add(P_ES_ERROR);
                        cmd.Parameters.Add(P_ERROR_MSG);
                        cmd.Parameters.Add("P_RESULTADO_CAB", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("P_RESULTADO_DET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_ES_ERROR.Value.ToString());
                        Pmessage = P_ERROR_MSG.Value.ToString();

                        if (Pcode == 1)
                        {
                            throw new Exception(Pmessage);
                        }

                        //Obtenemos el cursor de Cabecera
                        OracleRefCursor cursorCab = (OracleRefCursor)cmd.Parameters["P_RESULTADO_CAB"].Value;
                        dataCab.Load(cursorCab.GetDataReader());
                        dataCab.TableName = "CABECERA";
                        dataReport.Tables.Add(dataCab);
                        var cantCab = dataCab.Rows.Count;

                        //Obtenemos el cursor de Detalle
                        OracleRefCursor cursorDet = (OracleRefCursor)cmd.Parameters["P_RESULTADO_DET"].Value;
                        dataDet.Load(cursorDet.GetDataReader());
                        dataDet.TableName = "DETALLE";
                        dataReport.Tables.Add(dataDet);
                        var cantDet = dataDet.Rows.Count;
                        cn.Close();
                    }
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return dataReport;
        }
              
        //Método para obtener los campos para el excel.
        public DataTable GetDataFields(DataTable data, ExecuteProcess parameters)
        {
            DataTable dataFields = new DataTable();

            try
            {
                var ValIdBranch = parameters.IdBranch;

                if (ValIdBranch == 66 || ValIdBranch == 74 || ValIdBranch == 77 || ValIdBranch == 73)
                {
                    parameters.IdBranch = ValIdBranch;
                }

                else
                {
                    parameters.IdBranch = 999;
                }

                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        IDataReader reader = null;

                        cmd.CommandText = string.Format("{0}.{1}", Package, "SPS_LIST_FIELDS_BRANCH");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_NPROFILE", OracleDbType.Int32).Value = parameters.IdProfile;
                        cmd.Parameters.Add("P_NBRANCH", OracleDbType.Int32).Value = parameters.IdBranch;
                        cmd.Parameters.Add("P_STIPO", OracleDbType.Varchar2).Value = parameters.IdType;
                        cmd.Parameters.Add("C_TABLE", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        cn.Open();

                        reader = cmd.ExecuteReader();

                        if (reader != null)
                        {
                            dataFields.Load(reader);
                            var cant = dataFields.Rows.Count;
                        }
                        cn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataFields;
        }

        //Obtener datos en base al método de configuracion de campos y el método filtro de datos.
        public DataTable GetFilteredReportData(DataTable Data, DataTable Config, ExecuteProcess parameters)
        {
            var ValIdBranch = parameters.IdBranch;

            if (ValIdBranch == 66 || ValIdBranch == 74 || ValIdBranch == 77 || ValIdBranch == 73)
            {
                parameters.IdBranch = ValIdBranch;
            }
            else
            {
                parameters.IdBranch = 999;
            }
            try
            {
                string[] ListColumnsFields = Config.AsEnumerable().Select(s => s.Field<string>("SFIELDNAME")).ToArray<string>();

                List<string> ListColumnsData = getColumnsName(Data);

                foreach (string NameColumn in ListColumnsData)
                {
                    if (!ListColumnsFields.Contains(NameColumn))
                    {
                        Data.Columns.Remove(NameColumn);
                    }
                }
                var cantidad = getColumnsName(Data).Count;

                ValIdBranch = (parameters.IdBranch == 999) ? parameters.IdBranch = ValIdBranch : parameters.IdBranch;

            }

            catch (Exception ex)
            {
                throw ex;
            }
            return Data;
        }

        //Método para actualizar el estado de cada reporte.
        public string UpdateStatus(string st, string idHeaderProcess)
        {
            int Pcode;           
            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;            

                        cmd.CommandText = string.Format("{0}.{1}", Package, "SPS_UPD_CAB_PROCESS_STATUS");

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_ID", OracleDbType.Varchar2).Value = idHeaderProcess;
                        cmd.Parameters.Add("P_NSTATUSPROC", OracleDbType.Int32).Value = st;
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 2000;
                        P_MESSAGE.Size = 2000;
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_MESSAGE);
                        cn.Open();

                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());                       
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception($"Process Failer - {idHeaderProcess}", LogHelper.Paso.ExecuteStore, ex.Message);
            }
            return st;
        }

        //Método para enviar correo.
        public EmailParams EmailList(int IdUser)
        {
            int Pcode;

            string Pmessage = string.Empty;

            EmailParams user = new EmailParams();

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        cmd.CommandText = string.Format("{0}.{1}", Package, "SPS_OBT_CORREO_ENVIO");


                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_USUARIO", OracleDbType.Int32).Value = IdUser;
                        var P_SEMAIL = new OracleParameter("P_SEMAIL", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_SNAME = new OracleParameter("P_SNAME", OracleDbType.Varchar2, ParameterDirection.Output);
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_SEMAIL.Size = 4000;
                        P_MESSAGE.Size = 4000;
                        P_SNAME.Size = 4000;
                        cmd.Parameters.Add(P_SEMAIL);
                        cmd.Parameters.Add(P_SNAME);
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_MESSAGE);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                        user.Email = Convert.ToString(P_SEMAIL.Value.ToString());
                        user.Name = Convert.ToString(P_SNAME.Value.ToString());
                        Pmessage = P_MESSAGE.Value.ToString();
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Exception($"Process Failer - {IdUser}", LogHelper.Paso.ExecuteStore, ex.Message);
            }

            return user;
        }


        public List<ReporteN1> GetAlertList()
        {
            List<ReporteN1> result = new List<ReporteN1>();
            try
            {

                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        cmd.CommandText = string.Format("{0}.{1}", Package, "SP_GET_LISTA_ALERTA");

                        cmd.CommandType = CommandType.StoredProcedure;


                        var RC1 = new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output);
                        cmd.Parameters.Add(RC1);




                        cn.Open();

                        cmd.ExecuteNonQuery();
                        OracleDataReader odr = ((OracleRefCursor)RC1.Value).GetDataReader();
                        while (odr.Read())
                        {
                            ReporteN1 item = new ReporteN1();

                            item.alertId = Convert.ToInt32(odr["NIDALERTA"].ToString());
                            item.alertName = odr["SNOMBRE_ALERTA"] == DBNull.Value ? string.Empty : odr["SNOMBRE_ALERTA"].ToString();
                            item.alertDescription = odr["SDESCRIPCION_ALERTA"] == DBNull.Value ? string.Empty : odr["SDESCRIPCION_ALERTA"].ToString();
                            item.statusDescription = odr["SDESESTADO"] == DBNull.Value ? string.Empty : odr["SDESESTADO"].ToString();
                            item.registerDate = odr["DFECHA_REGISTRO"] == DBNull.Value ? string.Empty : odr["DFECHA_REGISTRO"].ToString();
                            item.userName = odr["NIDUSUARIO_MODIFICA"] == DBNull.Value ? string.Empty : odr["NIDUSUARIO_MODIFICA"].ToString();
                            item.alertStatus = odr["SESTADO"] == DBNull.Value ? string.Empty : odr["SESTADO"].ToString();
                            item.bussinessDays = Convert.ToInt32(odr["NDIASUTILREENVIO"].ToString());
                            item.reminderSender = odr["SACTIVA_REENVIO"] == DBNull.Value ? string.Empty : odr["SACTIVA_REENVIO"].ToString();
                            item.userFullName = odr["SNOMUSUARIO"] == DBNull.Value ? string.Empty : odr["SNOMUSUARIO"].ToString();
                            item.sennalDescripcion = odr["SDESGRUPO_SENAL"] == DBNull.Value ? string.Empty : odr["SDESGRUPO_SENAL"].ToString();
                            item.regimenSim = odr["NINDICA_REGIMEN_SIMP"] == DBNull.Value ? string.Empty : odr["NINDICA_REGIMEN_SIMP"].ToString();
                            item.regimenGen = odr["NINDICA_REGIMEN_GRAL"] == DBNull.Value ? string.Empty : odr["NINDICA_REGIMEN_GRAL"].ToString();
                            result.Add(item);
                        }
                        odr.Close();

                        cmd.Dispose();
                    }
                }

            }
            catch (Exception ex)
            {
                    throw ex;
            }
            return result;
        }

        public dynamic Udate()
        {
            int Pcode;

            string Pmessage = string.Empty;

            EmailParams user = new EmailParams();

            try
            {
                using (OracleConnection cn = new OracleConnection(ConfigurationManager.ConnectionStrings["Conexion"].ToString()))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = cn;

                        cmd.CommandText = string.Format("{0}.{1}", Package, "SP_UPD_RIESGOS");


                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("P_ID", OracleDbType.Int32).Value = 1;
                        cmd.Parameters.Add("P_RIESGO", OracleDbType.Varchar2).Value = "9"; //2 
                        cmd.Parameters.Add("P_VALIDADOR", OracleDbType.Varchar2).Value = "GEOGRAFICA";
                       
                        var P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int32, ParameterDirection.Output);
                        var P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                        P_NCODE.Size = 4000;
                        P_MESSAGE.Size = 4000;
                       
                       
                        cmd.Parameters.Add(P_NCODE);
                        cmd.Parameters.Add(P_MESSAGE);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        Pcode = Convert.ToInt32(P_NCODE.Value.ToString());
                       
                        Pmessage = P_MESSAGE.Value.ToString();
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex;
            }

            return user;
        }

    }
}
