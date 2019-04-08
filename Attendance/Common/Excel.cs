using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
using Attendance.Models;

namespace Attendance.Common
{
    public class Excel
    {
        private _Workbook _wbk;
        private _Worksheet _wsh;
        Application app;
        Workbooks wbks;
        Sheets shs;
        public void OpenExcel(string path, int indexSheet = 1)
        {
            app = new Application();
            wbks = app.Workbooks;
            _wbk = wbks.Open(path);

            shs = _wbk.Sheets;

            _wsh = (_Worksheet)shs.get_Item(indexSheet);
            _wsh.Visible = XlSheetVisibility.xlSheetVisible;
            //string t = GetCellValue("A",4);

        }
        public void CloseExcel()
        {
            app.Quit();

            //释放掉多余的excel进程
            System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
            app = null;
        }
        public string GetCellValue(string col, int row)
        {
            try
            {
                dynamic value = _wsh.Range[col + row.ToString()].Value;
                if (value == null) return "";
                return value.ToString();
            }
            catch (Exception)
            {
                CloseExcel();
                return "";
            }
           
        }
        public void SetCellValue(string col, int row, string value)
        {
            _wsh.Range[col + row.ToString()].Value = value;
        }
        public string GetCellValue(int row, int col)
        {
            try
            {
                dynamic value = _wsh.Cells[row, col].Value;
                if (value == null) return "";
                return value.ToString();
            }
            catch (Exception)
            {

                CloseExcel();
                return "";
            }
            
        }


    }
}
