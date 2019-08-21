using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowService.OAWebService;
using WorkflowService.Models;
namespace WorkflowService
{
    public class OAService
    {
        public static int PushGongchu(GongchuForm obj)
        {
            string[] fields_name = new string[] { "com", "owner", "person_type", "dpt", "range", "date_time_gc", "reason", "addrss" };
            WorkflowRequestTableField[] requestFields = new WorkflowRequestTableField[fields_name.Length];
            for (int i = 0; i < requestFields.Length; i++)
            {
                requestFields[i] = new WorkflowRequestTableField();
                requestFields[i].fieldName = fields_name[i];
                requestFields[i].view = true;
                requestFields[i].edit = true;
            }
            requestFields[0].fieldValue = obj.com;
            requestFields[1].fieldValue = obj.owner;
            requestFields[2].fieldValue = obj.person_type;
            requestFields[3].fieldValue = obj.dpt;
            requestFields[4].fieldValue = obj.range;
            requestFields[5].fieldValue = obj.date_time_gc;
            requestFields[6].fieldValue = obj.reason;
            requestFields[7].fieldValue = obj.addrss;

            //实例化服务器请求类
            WorkflowRequestTableRecord[] requestTableRecord = new WorkflowRequestTableRecord[1];
            requestTableRecord[0] = new WorkflowRequestTableRecord();
            requestTableRecord[0].workflowRequestTableFields = requestFields;

            WorkflowMainTableInfo mainTableInfo = new WorkflowMainTableInfo();
            mainTableInfo.requestRecords = requestTableRecord;

            WorkflowBaseInfo baseInfo = new WorkflowBaseInfo();
            baseInfo.workflowId = "25962";

            WorkflowRequestInfo requestInfo = new WorkflowRequestInfo();
            requestInfo.creatorId = obj.owner.ToString();
            requestInfo.requestLevel = "0";
            requestInfo.requestName = $"置业公司公出审批表 {obj.name}{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}";
            requestInfo.workflowMainTableInfo = mainTableInfo;
            requestInfo.workflowBaseInfo = baseInfo;

            WorkflowServicePortTypeClient portTypeClient = new WorkflowServicePortTypeClient();
            int oa_res_id = 0;
            try
            {
                oa_res_id = int.Parse(portTypeClient.doCreateWorkflowRequest(requestInfo, 1));
            }
            catch (Exception)
            {

                return 0;
            }
            return oa_res_id;

        }
    }
}
