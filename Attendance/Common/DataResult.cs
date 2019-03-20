using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Attendance.Common
{
    public class DataResult
    {
        /// <summary>
        /// 消息码
        /// </summary>
        public int error_code { get; set; }

        /// <summary>
        /// 消息信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 结果数据集
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        //public object additional_data { get; set; }

        /// <summary>
        /// 使用DataSet初始化结果对象
        /// </summary>
        /// <param name="data">数据集</param>
        /// <returns>结果对象</returns>
        public static DataResult InitFromDataSet(DataSet data)
        {
            DataResult Resault = new DataResult();
            if (data == null)
            {
                Resault.error_code = MessageCode.ERROR_EXECUTE_SQL;
                Resault.message = MessageCode.TranslateMessageCode(MessageCode.ERROR_EXECUTE_SQL);
            }
            //else if (data.Tables[0].Rows.Count == 0)
            //{
            //  Resault.data = "[]";
            //}
            else if (data.Tables == null || data.Tables.Count == 0)
            {
                Resault.error_code = MessageCode.ERROR_NO_DATA;
                Resault.message = MessageCode.TranslateMessageCode(MessageCode.ERROR_NO_DATA);
            }
            else
            {
                Resault.error_code = MessageCode.SUCCESS;
                Resault.message = MessageCode.TranslateMessageCode(MessageCode.SUCCESS);
                Resault.data = data.Tables[0];
            }

            return Resault;
        }

        /// <summary>
        /// 使用消息码初始化结果对象
        /// </summary>
        /// <param name="code">消息码</param>
        /// <returns>结果对象</returns>
        public static DataResult InitFromMessageCode(int code)
        {
            DataResult Resault = new DataResult();
            Resault.error_code = code;
            Resault.message = MessageCode.TranslateMessageCode(code);
            return Resault;
        }

        /// <summary>
        /// 转换单一列数据集为列表
        /// </summary>
        /// <param name="data">数据集</param>
        /// <returns>列表</returns>
        public static List<object> SingleColumnDataSetToList(DataSet data)
        {
            List<object> Resault = new List<object>();
            if (data != null)
            {
                for (int i = 0, count = data.Tables[0].Rows.Count; i < count; i++)
                {
                    Resault.Add(data.Tables[0].Rows[i][0]);
                }
            }
            return Resault;
        }
    }
}