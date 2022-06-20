using AppBackground.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetLight;
using LoadMasiveSCTR.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using AppBackground.Entities.ReporteKuntur;

namespace AppBackground.Util.GenerateFiles
{
    public class Excel : IFiles
    {
        private int RowInital = 2;
        private string Name = "Default";
        private string NameSheet = "Default";
        private string FolderSave = Environment.CurrentDirectory;


        #region builder

        public Excel(DataTable data, List<ConfigFields> configuration)
        {
            Generate(data, configuration);
        }
        public Excel(DataTable data, string name, List<ConfigFields> configuration)
        {
            this.Name = name;
            Generate(data, configuration);
        }
        public Excel(DataTable data, string name, string nameSheet, List<ConfigFields> configuration)
        {
            this.Name = name;
            this.NameSheet = nameSheet;
            Generate(data, configuration);
        }
        public Excel(DataTable data, string name, string nameSheet, string folder, List<ConfigFields> configuration)
        {
            this.Name = name;
            this.NameSheet = nameSheet;
            this.FolderSave = folder;
            Generate(data, configuration);
        }
        #endregion

        public void Generate(DataTable data, List<ConfigFields> configuration)
        {
            SLDocument sl = new SLDocument();
            try
            {
                sl.RenameWorksheet(SLDocument.DefaultFirstSheetName, NameSheet);
                FillTableToExcel(ref sl, data, configuration);
                sl.AutoFitColumn(1, 50);
                sl.SaveAs(CombinePath(FolderSave, $"{Name}.xlsx"));

            }
            catch (Exception ex)
            {
                LogHelper.Exception($"Process fail - {Name}", LogHelper.Paso.GenerateFile, ex.Message);
                throw ex;
            }
            finally
            {
                sl.Dispose();
            }
        }
        private void FillTableToExcel(ref SLDocument sl, DataTable dt, List<ConfigFields> configuration)
        {
            var contador = 1;
            var Style = sl.CreateStyle();
            CreateStyle(ref Style);
            foreach (DataColumn Col in dt.Columns)
            {
                SetValueWithStyle(ref sl, 1, contador, Col.ColumnName, Style);
                contador++;
            }
            int rowTbl = 0;
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 1; i <= dt.Columns.Count; i++)
                {
                    var columncurrent = (DataColumn)dt.Columns[i - 1];
                    ConfigFields item = configuration.Where(column => column.SFIELDNAME == columncurrent.ColumnName).FirstOrDefault();
                    if (item.SCOLDATATYPE == "NUMBER" && dt.Rows[rowTbl][i - 1].ToString() != string.Empty)
                    {
                        sl.SetCellValue(RowInital, i, Convert.ToDecimal(dt.Rows[rowTbl][i - 1].ToString()));
                    }
                    else
                    {
                        sl.SetCellValue(RowInital, i, dt.Rows[rowTbl][i - 1].ToString());
                    }
                }
                RowInital++;
                rowTbl++;
            }
        }
        private void SetValueWithStyle(ref SLDocument sl, int Row, int Column, string Value, SLStyle style)
        {
            sl.SetCellValue(Row, Column, Value);
            sl.SetCellStyle(Row, Column, style);
        }
        private void CreateStyle(ref SLStyle style)
        {
            System.Drawing.Color ColorRGBA = System.Drawing.Color.FromArgb(255, 153, 20);
            style.Fill.SetPattern(PatternValues.Solid, ColorRGBA, System.Drawing.Color.Yellow);
            style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
            style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
            style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
            style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
        }
        private String CombinePath(string Path, string File)
        {
            return String.Format(@"{0}\{1}", Path, File);
        }
    }

}
