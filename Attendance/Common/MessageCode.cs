using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MessageCode
    {
        public const int SUCCESS = 0;//成功
        public const int UNKONWN = -1;//未知错误
        public const int LOGIN_FAIL = 1;//登录失败
        public const int ERROR_EXECUTE_SQL = 2;//执行语句失败
        public const int ERROR_NO_DATA = 3;//未能获取到数据
        public const int ERROR_TOKEN_LENGTH = 4;//Token解析失败
        public const int ERROR_TOKEN_TIMEOUT = 5;//Token过期
        public const int ERROR_TOKEN_VALIDATE = 6;//Token验证失败
        public const int ERROR_USER_EXSIT = 8;//用户名已经存在
        public const int ERROR_NO_AUTH = 10;//你没有权限
        public const int ERROR_PASSWORD = 11;//密码错误

        public static string TranslateMessageCode(int Code)
        {
            string result = "";
            switch (Code)
            {
      
                case ERROR_PASSWORD:
                    result = "用户名或密码错误";
                    break;
                case ERROR_NO_AUTH:
                    result = "没有权限";
                    break;
               
                case ERROR_USER_EXSIT:
                    result = "用户名已经存在";
                    break;
                case UNKONWN:
                    result = "未知错误";
                    break;

                case SUCCESS:
                    result = "成功";
                    break;
                case LOGIN_FAIL:
                    result = "用户名或密码错误，登录失败";
                    break;
                case ERROR_EXECUTE_SQL:
                    result = "执行语句失败";
                    break;
                case ERROR_NO_DATA:
                    result = "未能获取到数据";
                    break;
                case ERROR_TOKEN_LENGTH:
                    result = "Token解析失败";
                    break;
                case ERROR_TOKEN_TIMEOUT:
                    result = "Token过期";
                    break;
                case ERROR_TOKEN_VALIDATE:
                    result = "Token验证失败，请重新登录";
                    break;
                default:
                    result = "未定义消息";
                    break;
            }
            return result;
        }
    }
}
