﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Reflection;
using Attendance.Models;

namespace Attendance.Common
{
    public class Excel : IDisposable
    {
        private string fileName = null; //文件名
        private IWorkbook workbook = null;
        private FileStream fs = null;
        private bool disposed;
        private ISheet sheet = null;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (fs != null)
                        fs.Close();
                }

                fs = null;
                disposed = true;
            }
        }
        public void OpenExcel(string path, int indexSheet = 1)
        {
            try
            {
                fileName = path;
                fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);
                int index = indexSheet - 1;
                sheet = workbook.GetSheetAt(index);
            }
            catch (Exception ex)
            {

                
            }
            

        }
        public void CloseExcel()
        {
            Dispose();
        }
        public int GetColcount()
        {
            IRow firstRow = sheet.GetRow(0);
            if (firstRow==null)
            {
                return 0;
            }
            int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数
            return cellCount;
        }
        public int GetRowCount()
        {
            int rowCount = sheet.LastRowNum;
            return rowCount;
        }
        public void SetCellValue( int row,int col, string value)
        {
            IRow Row = sheet.GetRow(row - 1);
            Row.Cells[col - 1].SetCellValue(value);
        }
        public string GetCellValue(int row, int col)
        {
            try
            {
                IRow Row = sheet.GetRow(row - 1);
                ICell cell = Row.GetCell(col - 1);
                string cellValue = cell.StringCellValue;
                return cellValue;
            }
            catch (Exception)
            {

                return null;
            }
            
        }


    }
}
