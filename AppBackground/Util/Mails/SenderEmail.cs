using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using AppBackground.Entities.ReporteKuntur.Parameters;
using AppBackground.Entities.ReporteKuntur.Response.Email;
using AppBackground.Interfaces;
using AppBackground.Repositories.ReportKuntur;

namespace AppBackground.Util.Mails
{
    public class SenderEmail : IMail
    {
        public class Message
        {
            public string Address { get; set; }
            public string Subject { get; set; }
            public string BodyResponse { get; set; }
        }

        //Obtiene valores del archivo de configuración del proyecto.
        public string GetValueConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        //Crear el asunto del mensaje
        public string ComposeSubject(string keysubject)
        {
            string subject = string.Empty;
            subject = GetValueConfig("subject");
            return subject;
        }

        //Llenar variables del mensaje
        public string ComposeBody(String data, string IdProcess, EmailParams user, string bodyResponse, ExecuteProcess process)
        {
            var reportCountName = "el reporte ";
            var reportsCountName = "los reportes ";
            var failreport = " no se pudo generar";
            var failreports = " no se pudieron generar";

            //Damos formato correcto a la fecha de inicio
            var startDate = process.StartDate;
            var iniDate = startDate.Insert(4, "/");
            var ultSDate = iniDate.Insert(7, "/");
            var finalSDate = Convert.ToDateTime(ultSDate).ToString("dd/MM/yyyy");

            //Damos formato correcto a la fecha de Fin
            var endDate = process.EndDate;
            var finDate = endDate.Insert(4, "/");
            var ultEDate = finDate.Insert(7, "/");
            var finalEDate = Convert.ToDateTime(ultEDate).ToString("dd/MM/yyyy");


            //Validacion a que ramo corresponde
            var BranchName = process.DesBranch;
            if (data == "2")           
            {
                try
                {
                    if (process.IdType == "1")
                    {
                        string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig("bodyTemplate"));
                        string readText = File.ReadAllText(path);
                        return readText

                            .Replace("[Nombre]", user.Name)
                            .Replace("[Resultado]", string.Format("{0}", "Se acaba de generar "))
                            .Replace("[Reporte]", string.Format("<strong>{0}</strong>", reportCountName + "de CABECERA"))
                            .Replace("[Ramo]", string.Format("<strong>{0}</strong>", BranchName))
                            .Replace("[FechaInicio]", string.Format("<strong>{0}</strong>", finalSDate))
                            .Replace("[FechaFin]", string.Format("<strong>{0}</strong>", finalEDate))
                            .Replace("[Respuesta]", string.Format("{0}", "Por favor puede descargar el reporte desde la plataforma "))
                            .Replace("[Aplicacion]", string.Format("<strong>{0}</strong>", GetValueConfig("aplicationName")))
                            .Replace("[Indicacion]", string.Format("<strong>{0}</strong>", "– opción monitoreo de reportes de cierre, consultando el ID: "))
                            .Replace("[IdProceso]", string.Format("<strong>{0}</strong>", IdProcess))
                            .Replace("[Instruccion]", string.Format("<strong>{0}</strong>", "Le pedimos que pueda verificarlo en la pantalla de monitoreo de reportes."));
                    }
                    else if (process.IdType == "2")
                    {
                        string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig("bodyTemplate"));
                        string readText = File.ReadAllText(path);
                        return readText

                           .Replace("[Nombre]", user.Name)
                            .Replace("[Resultado]", string.Format("{0}", "Se acaban de generar "))
                            .Replace("[Reporte]", string.Format("<strong>{0}</strong>", reportsCountName + "de DETALLE"))
                            .Replace("[Ramo]", string.Format("<strong>{0}</strong>", BranchName))
                            .Replace("[FechaInicio]", string.Format("<strong>{0}</strong>", finalSDate))
                            .Replace("[FechaFin]", string.Format("<strong>{0}</strong>", finalEDate))
                            .Replace("[Respuesta]", string.Format("{0}", "Por favor puede descargar los reportes desde la plataforma "))
                            .Replace("[Aplicacion]", string.Format("<strong>{0}</strong>", GetValueConfig("aplicationName")))
                            .Replace("[Indicacion]", string.Format("<strong>{0}</strong>", "– opción monitoreo de reportes de cierre, consultando el ID: "))
                            .Replace("[IdProceso]", string.Format("<strong>{0}</strong>", IdProcess))
                            .Replace("[Instruccion]", string.Format("{0}", "Le pedimos que porfavor pueda verificarlo en la pantalla de monitoreo de reportes."));
                    }
                    else if (process.IdType == "3")
                    {
                        string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig("bodyTemplate"));
                        string readText = File.ReadAllText(path);
                        return readText

                           .Replace("[Nombre]", user.Name)
                            .Replace("[Resultado]", string.Format("{0}", "Se acaban de generar "))
                            .Replace("[Reporte]", string.Format("<strong>{0}</strong>", reportsCountName + "de CABECERA y DETALLE"))
                            .Replace("[Ramo]", string.Format("<strong>{0}</strong>", BranchName))
                            .Replace("[FechaInicio]", string.Format("<strong>{0}</strong>", finalSDate))
                            .Replace("[FechaFin]", string.Format("<strong>{0}</strong>", finalEDate))
                            .Replace("[Respuesta]", string.Format("{0}", "Por favor puede descargar los reportes desde la plataforma "))
                            .Replace("[Aplicacion]", string.Format("<strong>{0}</strong>", GetValueConfig("aplicationName")))
                            .Replace("[Indicacion]", string.Format("<strong>{0}</strong>", "– opción monitoreo de reportes de cierre, consultando el ID: "))
                            .Replace("[IdProceso]", string.Format("<strong>{0}</strong>", IdProcess))
                            .Replace("[Instruccion]", string.Format("{0}", "Le pedimos que porfavor pueda verificarlo en la pantalla de monitoreo de reportes."));
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                return bodyResponse;
            }

            else
            {
                try
                {
                    if (process.IdType == "1" )
                    {
                        string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig("bodyTemplate"));

                        string readText = File.ReadAllText(path);
                        return readText

                            .Replace("[Nombre]", user.Name)
                                .Replace("[Resultado]", string.Format("<strong>{0}</strong>","Le informamos que "+ failreport + "" ))
                                .Replace("[Reporte]", string.Format("<strong>{0}</strong>", reportCountName + "de CABECERA" + " con el ID: " + process.IdProcess))
                                .Replace("[Ramo]", string.Format("<strong>{0}</strong>", BranchName))
                                .Replace("[FechaInicio]", string.Format("<strong>{0}</strong>", finalSDate))
                                .Replace("[FechaFin]", string.Format("<strong>{0}</strong>", finalEDate))
                                .Replace("[Respuesta]", string.Format("{0}", " Le pedimos porfavor que pueda "))
                                .Replace("[Aplicacion]", string.Format("<strong>{0}</strong>", " contactar a soporte. "))
                                .Replace("[Indicacion]", string.Format("<strong>{0}</strong>", " "))
                                .Replace("[IdProceso]", string.Format("<strong>{0}</strong>", " "))
                                .Replace("[Instruccion]", string.Format("<strong>{0}</strong>", " "));
                    }
                    else if (process.IdType == "2")
                    {
                        string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig("bodyTemplate"));

                        string readText = File.ReadAllText(path);
                        return readText

                            .Replace("[Nombre]", user.Name)
                                .Replace("[Resultado]", string.Format("<strong>{0}</strong>", "Le informamos que " + failreport))
                                .Replace("[Reporte]", string.Format("<strong>{0}</strong>", reportCountName + " de DETALLE" + " con el ID: " + process.IdProcess ))
                                .Replace("[Ramo]", string.Format("<strong>{0}</strong>", BranchName))
                                .Replace("[FechaInicio]", string.Format("<strong>{0}</strong>", finalSDate))
                                .Replace("[FechaFin]", string.Format("<strong>{0}</strong>", finalEDate))
                                .Replace("[Respuesta]", string.Format("{0}", " Le pedimos porfavor que pueda "))
                                .Replace("[Aplicacion]", string.Format("<strong>{0}</strong>", " contactar a soporte. "))
                                .Replace("[Indicacion]", string.Format("<strong>{0}</strong>", " "))
                                .Replace("[IdProceso]", string.Format("<strong>{0}</strong>", " "))
                                .Replace("[Instruccion]", string.Format("<strong>{0}</strong>", " "));
                    }
                    else if (process.IdType == "3")
                    {
                        string path = string.Format("{0}{1}", GetValueConfig("Templates"), GetValueConfig("bodyTemplate"));

                        string readText = File.ReadAllText(path);
                        return readText

                            .Replace("[Nombre]", user.Name)
                                .Replace("[Resultado]", string.Format("<strong>{0}</strong>", "Le informamos que " + failreports))
                                .Replace("[Reporte]", string.Format("<strong>{0}</strong>", reportsCountName + "de CABECERA y DETALLE" + " con el ID: " + process.IdProcess))
                                .Replace("[Ramo]", string.Format("<strong>{0}</strong>", BranchName))
                                .Replace("[FechaInicio]", string.Format("<strong>{0}</strong>", finalSDate))
                                .Replace("[FechaFin]", string.Format("<strong>{0}</strong>", finalEDate))
                                .Replace("[Respuesta]", string.Format("{0}", " Le pedimos porfavor que pueda "))
                                .Replace("[Aplicacion]", string.Format("<strong>{0}</strong>", " contactar a soporte. "))
                                .Replace("[Indicacion]", string.Format("<strong>{0}</strong>", " "))
                                .Replace("[IdProceso]", string.Format("<strong>{0}</strong>", " "))
                                .Replace("[Instruccion]", string.Format("<strong>{0}</strong>", " "));   
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }

                return bodyResponse;
            }
        }

        //Método que envía el mensaje
        public void EmailSender(String data, EmailParams user, string IdProcess, ExecuteProcess process)
        {
            if (IdProcess != null)
            {
                var message = new MailMessage();             
                message.To.Add(new MailAddress("marco.urrutia@materiagris.pe"));
                string subject = string.Empty;
                string bodyResponse = string.Empty;
                //Devuelve el asunto
                subject = ComposeSubject(subject);
                message.Subject = subject;
                bodyResponse = ComposeBody(data, IdProcess, user, bodyResponse, process);
                message.Body = bodyResponse;
                message.IsBodyHtml = true;

                string fileName = string.Format("{0}{1}", GetValueConfig("Templates"), "logo.png");

                AlternateView av = AlternateView.CreateAlternateViewFromString(bodyResponse, null, MediaTypeNames.Text.Html);
                LinkedResource lr = new LinkedResource(fileName, MediaTypeNames.Image.Jpeg);
                message.AlternateViews.Add(av);
                lr.ContentId = "Logo";
                av.LinkedResources.Add(lr);

                try
                {
                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(message);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}


