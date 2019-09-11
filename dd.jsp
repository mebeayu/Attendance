
<%@ page language="java" contentType="text/html; charset=UTF-8" %>
<%@ page import="java.util.*,HT.HTSrvAPI,java.math.*,java.net.*" %>
<%@ page import="com.yxp.mydb.Mingyuandaiban" %>
<%@ page import="javax.xml.namespace.QName" %>
<%@ page import="org.apache.axis.client.Call" %>
<%@ page import="org.apache.axis.client.Service" %>
<%@ include file="/systeminfo/init_wev8.jsp" %>
<%@ page import="org.apache.http.client.HttpClient" %>
<%@ page import="org.apache.http.impl.client.DefaultHttpClient" %>
<%@ page import="org.apache.commons.httpclient.methods.PostMethod" %>
<%@ page import="yxp.test.CoremailUtil" %>
<HTML>
<HEAD>
<script type="text/javascript" language="javascript" src="/js/jquery/jquery-1.4.2.min_wev8.js"></script>
<jsp:useBean id="Util" class="weaver.general.Util" scope="page" />
<jsp:useBean id="rs" class="weaver.conn.RecordSet" scope="page"/>
<%

int userid=user.getUID();
String loginid_y="";
String mobile_y="";
String sql1="select loginid,mobile from hrmresource where id="+userid;
rs.executeSql(sql1);
if(rs.next()){
	loginid_y=rs.getString(1);
	mobile_y=rs.getString(2);
}


 String url="http://220.163.109.237/IDWebService//Platform/MsgCenter.asmx";//提供接口的地址
 String soapaction="http://www.chinasap.cn/";   //域名，这是在server定义的
 String dbmun="";
String City=loginid_y;        
         Service service=new Service();
         try{
             Call call=(Call)service.createCall();            
             call.setTargetEndpointAddress(url);            
             call.setOperationName(new QName(soapaction,"GetTotalWaitCount")); //设置要调用哪个方法
             call.addParameter(new QName(soapaction,"accountName"), //设置要传递的参数
                     org.apache.axis.encoding.XMLType.XSD_STRING,
                    javax.xml.rpc.ParameterMode.IN);
             call.setReturnType(new QName(soapaction,"GetTotalWaitCount"),Vector.class); //要返回的数据类型（自定义类型）
            
//           call.setReturnType(org.apache.axis.encoding.XMLType.XSD_STRING);//（标准的类型）
            
             call.setUseSOAPAction(true);
             call.setSOAPActionURI(soapaction + "GetTotalWaitCount");    
                        
             Vector v=(Vector)call.invoke(new Object[]{City});//调用方法并传递参数        
             for(int i=0;i<v.size();i++)
             {
				if(i==1){
					 dbmun=v.get(i)+"";
				}
                
             }            
            
         }catch(Exception ex)
         {
         ex.printStackTrace();
         }    


String dst = "";
String srcText=loginid_y+"|192.168.27.101";
for (int i = 0; i < srcText.length(); i++){
	//将中文字符转为10进制整数，然后转为16进制unicode字符
	int iAsc = (int)srcText.charAt(i) + 101;
	if (iAsc > 65535)
	{
		iAsc -= 65535;
	}
	dst += "&#" + iAsc + ";";
}
String resultStr=URLEncoder.encode(dst);
if("".equals(dbmun)){
	dbmun="0";
}


//HR
String gettokenurl="http://192.168.27.188/ws/GetFunc_Code.asmx";
String hraction="http://tempuri.org/";
Service servicehr=new Service();
String hrtoken="yxp";
try{
	 Call callhr=(Call)servicehr.createCall();       
	 callhr.setTargetEndpointAddress(gettokenurl);            
	 callhr.setOperationName(new QName(hraction,"GetToken")); //设置要调用哪个方法
	 callhr.addParameter("UserCode",org.apache.axis.encoding.XMLType.XSD_STRING,javax.xml.rpc.ParameterMode.IN);
	 callhr.addParameter("ExpirDate",org.apache.axis.encoding.XMLType.XSD_INTEGER,javax.xml.rpc.ParameterMode.IN);
	 callhr.setReturnType(new QName(hraction,"GetToken"),Vector.class); //要返回的数据类型（自定义类型）
	 callhr.setReturnType(org.apache.axis.encoding.XMLType.XSD_STRING);//（标准的类型）
	 callhr.setUseSOAPAction(true);
	 callhr.setSOAPActionURI(hraction + "GetToken");    	
	 //hrtoken=(String)callhr.invoke(new Object[]{"hr2",0});//调用方法并传递参数   
	 
	 Vector v2=(Vector)callhr.invoke(new Object[]{"hr2",0});//调用方法并传递参数        
             for(int i=0;i<v2.size();i++)
             {
				if(i==1){
					 hrtoken=v2.get(i)+"";
				}
                
             }      
	 
	               
}catch(Exception ex)
{
ex.printStackTrace();
} 


//zxxx
String dst2="";
String srcText2=mobile_y+"|192.168.27.101";
for (int i = 0; i < srcText2.length(); i++){
	//将中文字符转为10进制整数，然后转为16进制unicode字符
	int iAsc = (int)srcText2.charAt(i) + 101;
	if (iAsc > 65535)
	{
		iAsc -= 65535;
	}
	dst2 += "&#" + iAsc + ";";
}
String resultStr2=URLEncoder.encode(dst);

//明源单点
/*String  mysoapaction="http://220.163.109.234:8060/webservice/taskwakeservice.asmx";
String getKeyrl="http://tempuri.org/Map/TaskWakeService/";
Service service2=new Service();
String myres="";
try{
	 Call call2=(Call)service2.createCall();            
	 call2.setTargetEndpointAddress(mysoapaction);            
	 call2.setOperationName(new QName(getKeyrl,"GetKeyTime")); //设置要调用哪个方法
	 call2.setReturnType(org.apache.axis.encoding.XMLType.XSD_STRING);//（标准的类型）
	 call2.setUseSOAPAction(true);
	 call2.setSOAPActionURI(getKeyrl + "GetKeyTime");    	
	 myres=(String)call2.invoke(new Object[]{});//调用方法并传递参数        
}catch(Exception ex)
{
ex.printStackTrace();
}    */



//明源获取代办数量
Mingyuandaiban my=new Mingyuandaiban();
int mynum=my.getdbnum("http://220.163.109.234:8060",loginid_y);



//邮件待办数
String mailNum="";
if(userid!=1){
//CoremailUtil cu=new CoremailUtil();
//mailNum=cu.getmailattr(loginid_y);
}else{
	mailNum="manerger";
}
%>
<style>
.item{
	width:16%;
	height:130px;
	float:left;
	text-align:center;
	cursor:pointer;
	margin-top:20px;
}
.item_1{
	width:70px;
	height:70px;
	
	text-align:center;
	line-height:70px;
	border-radius:35px;
	font-size:20px;
	color:#fff;
	margin:0 auto;
}
.item_1:hover{
	color:gray;
}
.item_2{
	//height:45px;
	width:100%;
	//line-height:45px;
	font-size:12px;
	color:gray;
}
</style>
</HEAD>
<body>
<div class="item" onclick="javascript:att('<%=loginid_y%>')">
	<div class="item_1" style="background-color:rgba(255,0,0,0.7)">考勤</div>
	<div class="item_2">打卡记录查询</div>
</div>

</div>
<!--<div class="item" onclick="javascript:window.open('http://192.168.27.188/ehr/ThirdPartyLogin.aspx?Token=<%=hrtoken%>')">
	<div class="item_1" style="background-color:rgba(191,141,255,0.7)">朗新</div>
	<div  class="item_2">朗新HR人力资源系统</div>
</div>-->
<div class="item" onclick="javascript:window.open('http://220.163.109.227:8080')">
	<div class="item_1" style="background-color:rgba(191,141,255,0.7)">朗新</div>
	<div  class="item_2">朗新HR人力资源系统</div>
</div>
<div class="item" onclick="loginzxxx()">
	<div class="item_1" style="background-color:rgba(3,122,207,0.7)">学习</div>
	<div  class="item_2">在线学习系统登录</div>
</div>
<div class="item" onclick="opmy('<%=loginid_y%>')">
	<div class="item_1" style="background-color:pink">明源</div>
	<div  class="item_2">明源销售管理系统<BR>(待办<a style="color:red;font-size:12px;text-decoration : none"><%=mynum%></a>条)</div>
</div>
<div class="item" onclick="javascript:mail('<%=loginid_y%>')">
	<div class="item_1" style="background-color:rgba(0,128,0,0.7)">邮件</div>
	<div class="item_2">coremail邮件系统<BR>(<a style="color:red;font-size:12px;text-decoration : none"><%=loginid_y%></a>)</div>
</div>
<div class="item" onclick="javascript:zg()">
	<div class="item_1" style="background-color:rgba(0,128,0,0.7)">紫光</div>
	<div class="item_2">紫光档案系统</div>
</div>
<div  class="item" onclick="javascript:window.open('http://220.163.109.237/IDWebSoft/FillData.ashx?action=SSOLogin&Account=<%=resultStr%>')">
	<div class="item_1" style="background-color:rgba(255,0,0,0.7)">赛普</div>
	<div class="item_2">赛普业务管理系统<BR>(待办<a style="color:red;font-size:12px;text-decoration : none"><%=dbmun%></a>条)</div>
<script type="text/javascript">
function loginzxxx(){
	var mobile_yy="<%=mobile_y%>";
	//alert(mobile_yy);
	if(mobile_yy.length<11){
		alert("请完善你的手机号码");
	}else{
		window.open("http://wlxy.ymci.cn");
	}
	
}

function opmy(id){
		//alert(id);
		window.open("/yxp/mygettoken.jsp?lgid="+id);	
}




/*function opmy1(id){
	alert(id);
	
	jQuery.post("/yxp/mygettoken.jsp",{null},function(res){
			alert(res);
			window.open("http://220.163.109.234:8060/Default_OA.aspx?UserCode="+loginid2+"&Password="+res+"&page=/index.aspx");
	});
}*/

function att(id){
	window.open("http://kq.ynctzy.com:86/Att/PointOA?oa_login_id="+id);
}
function mail(id){
	jQuery.post("/yxp/coremaillogin.jsp",{loginid:id},function(res){
			//alert(res);
			window.open(res);
	});
	//window.open("/yxp/coremaillogin.jsp?loginid="+id);
}

function zg(){
	window.open("http://220.163.109.239:8086/ams");
}
</script>

 </body>
 </html>
