using Attendance.Models;
using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttClient
{
    public class AttLogic
    {
        private Excel excel;
        public AttLogic()
        {
            excel = new Excel();
        }
        public List<Person> GetPersonBase(string path)
        {
            List<Person> list = new List<Person>();
            DBOA dboa = new DBOA();
            excel.OpenExcel(path);
            int index = 4;
            while(true)
            {
                Person p = new Person();
                p.LASTNAME = excel.GetCellValue("A", index);
                if (p.LASTNAME == ""|| p.LASTNAME == null) break;
                p.WorkDay = int.Parse(excel.GetCellValue("D", index));
                p.AttDay = int.Parse(excel.GetCellValue("E", index));
                p.LateCount = int.Parse(excel.GetCellValue("F", index));
                p.EarlyCount = int.Parse(excel.GetCellValue("H", index));

                DataSet ds = dboa.ExeQuery($@"select ID from HRMRESOURCE where LASTNAME='{p.LASTNAME}' and SUBCOMPANYID1=68");
                if (ds.Tables[0].Rows.Count == 0) p.UID = "";
                else p.UID = ds.Tables[0].Rows[0][0].ToString();
                list.Add(p);
                index += 2;
                
            }
            dboa.Close();
            excel.CloseExcel();
            return list;
        }
    }
}
