using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace YunNetworkDisk.Models
{
    public class CommonResult
    {
        static JavaScriptSerializer jss = new JavaScriptSerializer();

        /// <summary>
        /// 操作成功
        /// </summary>
        public CommonResult()
        {
            this.code = 0;
            this.errmsg = string.Empty;
            this.data = string.Empty;
        }
        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="errMsg">错误信息</param>
        public CommonResult(string errMsg)
        {
            this.code = -1;
            this.errmsg = errMsg;
            this.data = string.Empty;
        }
        public CommonResult(int code, string errMsg, string data)
        {
            this.code = code;
            this.errmsg = errMsg;
            this.data = data;
        }
        public CommonResult(int code, string errMsg, object data)
        {
            this.code = code;
            this.errmsg = errMsg;
            this.data = data;
        }
        /// <summary>
        /// 操作成功(返回json字符串)
        /// </summary>
        /// <returns></returns>
        public static string ToJsonStr()
        {
            var instance = new CommonResult();
            return jss.Serialize(instance);
        }
        /// <summary>
        /// 操作失败(返回json字符串)
        /// </summary>
        /// <returns></returns>
        public static string ToJsonStr(string errMsg)
        {
            var instance = new CommonResult(errMsg);
            return jss.Serialize(instance);
        }
        /// <summary>
        /// 操作结果(返回json字符串)
        /// </summary>
        /// <returns></returns>
        public static string ToJsonStr(int code, string errMsg, string data)
        {
            var instance = new CommonResult(code, errMsg, data);
            return jss.Serialize(instance);
        }
        /// <summary>
        /// 操作结果(返回json字符串)
        /// </summary>
        /// <returns></returns>
        public static string ToJsonStr(int code, string errMsg, object data)
        {
            var instance = new CommonResult(code, errMsg, data);
            return jss.Serialize(instance);
        }
        /// <summary>
        /// 操作成功(返回CommonResult实例)
        /// </summary>
        /// <returns></returns>
        public static CommonResult Instance()
        {
            return Instance(0, string.Empty, string.Empty);
        }
        /// <summary>
        /// 操作失败(返回CommonResult实例)
        /// </summary>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static CommonResult Instance(string errMsg = null)
        {
            return Instance(-1, errMsg, string.Empty);
        }
        /// <summary>
        /// 操作结果(返回CommonResult实例)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="errMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CommonResult Instance(int code = 0, string errMsg = null, string data = null)
        {
            return new CommonResult(code, errMsg, data);
        }
        public int code { get; set; }
        public string errmsg { get; set; }
        public object data { get; set; }
    }
}