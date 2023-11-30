using Microsoft.AspNetCore.Mvc;

namespace Silver.Web.Model
{
    public class ApiControllerBase : ControllerBase
    {
        public ResponseModel json { get; set; } = new ResponseModel();
    }

    public class ResponseModel
    {
        public int code { get; set; } = ErrorCodetaticVars.Succcess;
        public string msg { get; set; } = MessagestaticVars.Succcess;
        public dynamic data { get; set; }
    }

    public static class ApiResponseModel
    {

        public static ResponseModel ToRestResponse(this RestResult result)
        {
            return new ResponseModel() { code = result.code, msg = result.message };
        }

        public static ResponseModel ToRestPageResponse<T>(this RestPageResult<T> result)
        {
            return new ResponseModel() { code = result.code, msg = result.message, data = new { list = result.list, total = result.total } };
        }

    }

    public class MessagestaticVars
    {
        /// <summary>
        /// 数据成功
        /// </summary>
        public static string Succcess = "成功";

        /// <summary>
        /// 数据删除成功
        /// </summary>
        public static string DeleteSuccess { get; set; } = "数据删除成功";
        /// <summary>
        /// 数据删除失败
        /// </summary>
        public static string DeleteFailure { get; set; } = "数据删除失败";
        /// <summary>
        /// 系统禁止删除此数据
        /// </summary>
        public static string DeleteProhibitDelete { get; set; } = "系统禁止删除此数据";
        /// <summary>
        /// 此数据含有子类信息，禁止删除
        /// </summary>
        public static string DeleteIsHaveChildren { get; set; } = "此数据含有子类信息，禁止删除";
        /// <summary>
        /// 数据处理异常
        /// </summary>
        public static string DataHandleEx { get; set; } = "数据接口出现异常";
        /// <summary>
        /// 数据添加成功
        /// </summary>
        public static string CreateSuccess { get; set; } = "数据添加成功";
        /// <summary>
        /// 数据添加失败
        /// </summary>
        public static string CreateFailure { get; set; } = "数据添加失败";
        /// <summary>
        /// 数据移动成功
        /// </summary>
        public static string MoveSuccess { get; set; } = "数据移动成功";
        /// <summary>
        /// 数据移动失败
        /// </summary>
        public static string MoveFailure { get; set; } = "数据移动失败";
        /// <summary>
        /// 系统禁止添加数据
        /// </summary>
        public static string CreateProhibitCreate { get; set; } = "系统禁止添加数据";
        /// <summary>
        /// 数据编辑成功
        /// </summary>
        public static string EditSuccess { get; set; } = "数据编辑成功";
        /// <summary>
        /// 数据编辑失败
        /// </summary>
        public static string EditFailure { get; set; } = "数据编辑失败";
        /// <summary>
        /// 系统禁止编辑此数据
        /// </summary>
        public static string EditProhibitEdit { get; set; } = "系统禁止编辑此数据";
        /// <summary>
        /// 数据已存在
        /// </summary>
        public static string DataIsHave { get; set; } = "数据已存在";
        /// <summary>
        /// 数据不存在
        /// </summary>
        public static string DataisNo { get; set; } = "数据不存在";
        /// <summary>
        /// 请提交必要的参数
        /// </summary>
        public static string DataParameterError { get; set; } = "请提交必要的参数";
        /// <summary>
        /// 数据插入成功
        /// </summary>
        public static string InsertSuccess { get; set; } = "数据插入成功！";
        /// <summary>
        /// 数据插入失败
        /// </summary>
        public static string InsertFailure { get; set; } = "数据插入失败！";
        /// <summary>
        /// Excel导出失败
        /// </summary>
        public static string ExcelExportFailure { get; set; } = "Excel导出失败";
        /// <summary>
        /// Excel导出成功
        /// </summary>
        public static string ExcelExportSuccess { get; set; } = "Excel导出成功";
        /// <summary>
        /// 获取数据成功
        /// </summary>
        public static string GetDataSuccess { get; set; } = "获取数据成功！";
        /// <summary>
        /// 获取数据异常
        /// </summary>
        public static string GetDataException { get; set; } = "获取数据异常！";
        /// <summary>
        /// 获取数据失败
        /// </summary>
        public static string GetDataFailure { get; set; } = "获取数据失败！";
        /// <summary>
        /// 设置数据成功
        /// </summary>
        public static string SetDataSuccess { get; set; } = "设置数据成功！";
        /// <summary>
        /// 设置数据异常
        /// </summary>
        public static string SetDataException { get; set; } = "设置数据异常！";
        /// <summary>
        /// 设置数据失败
        /// </summary>
        public static string SetDataFailure { get; set; } = "设置数据失败！";

        /// <summary>
        /// Token验证失败
        /// </summary>
        public static string TokenFailure { get; set; } = "Token验证失败";

        /// <summary>
        /// 服务器故障
        /// </summary>
        public static string ServiceFailure { get; set; } = "服务器故障";

    }

    public class ErrorCodetaticVars
    {

        /// <summary>
        /// 成功
        /// </summary>
        public static int Succcess { get; set; } = 1;

        /// <summary>
        /// 数据删除成功
        /// </summary>
        public static int DeleteSuccess { get; set; } = 1;

        /// <summary>
        /// 数据删除失败
        /// </summary>
        public static int DeleteFailure { get; set; } = 5;

        /// <summary>
        /// 系统禁止删除此数据
        /// </summary>
        public static int DeleteProhibitDelete { get; set; } = 51;

        /// <summary>
        /// 此数据含有子类信息，禁止删除
        /// </summary>
        public static int DeleteIsHaveChildren { get; set; } = 52;

        /// <summary>
        /// 数据处理异常
        /// </summary>
        public static int DataHandleEx { get; set; } = 10;

        /// <summary>
        /// 数据操作失败
        /// </summary>
        public static int DataFailure { get; set; } = 5;

        /// <summary>
        /// 数据授权
        /// </summary>
        public static int DataAuth { get; set; } = 6;

        /// <summary>
        /// 数据添加成功
        /// </summary>
        public static int CreateSuccess { get; set; } = 1;

        /// <summary>
        /// 数据添加失败
        /// </summary>
        public static int CreateFailure { get; set; } = 5;

        /// <summary>
        /// 数据移动成功
        /// </summary>
        public static int MoveSuccess { get; set; } = 1;

        /// <summary>
        /// 数据移动失败
        /// </summary>
        public static int MoveFailure { get; set; } = 5;

        /// <summary>
        /// 系统禁止添加数据
        /// </summary>
        public static int CreateProhibitCreate { get; set; } = 56;

        /// <summary>
        /// 数据编辑成功
        /// </summary>
        public static int EditSuccess { get; set; } = 1;

        /// <summary>
        /// 数据编辑失败
        /// </summary>
        public static int EditFailure { get; set; } = 5;

        /// <summary>
        /// 系统禁止编辑此数据
        /// </summary>
        public static int EditProhibitEdit { get; set; } = 58;

        /// <summary>
        /// 数据已存在
        /// </summary>
        public static int DataIsHave { get; set; } = 2;

        /// <summary>
        /// 数据不存在
        /// </summary>
        public static int DataisNo { get; set; } = 3;

        /// <summary>
        /// 请提交必要的参数
        /// </summary>
        public static int DataParameterError { get; set; } = 4;

        /// <summary>
        /// 数据插入成功
        /// </summary>
        public static int InsertSuccess { get; set; } = 1;

        /// <summary>
        /// 数据插入失败
        /// </summary>
        public static int InsertFailure { get; set; } = 5;

        /// <summary>
        /// Excel导出失败
        /// </summary>
        public static int ExcelExportFailure { get; set; } = 71;

        /// <summary>
        /// Excel导出成功
        /// </summary>
        public static int ExcelExportSuccess { get; set; } = 1;

        /// <summary>
        /// 获取数据成功
        /// </summary>
        public static int GetDataSuccess { get; set; } = 1;

        /// <summary>
        /// 获取数据异常
        /// </summary>
        public static int GetDataException { get; set; } = 11;

        /// <summary>
        /// 获取数据失败
        /// </summary>
        public static int GetDataFailure { get; set; } = 12;

        /// <summary>
        /// 设置数据成功
        /// </summary>
        public static int SetDataSuccess { get; set; } = 1;

        /// <summary>
        /// 设置数据异常
        /// </summary>
        public static int SetDataException { get; set; } = 13;

        /// <summary>
        /// 设置数据失败
        /// </summary>
        public static int SetDataFailure { get; set; } = 14;

        /// <summary>
        /// Token验证失败
        /// </summary>
        public static int TokenFailure { get; set; } = 401;

        /// <summary>
        /// 服务器故障
        /// </summary>
        public static int ServiceFailure { get; set; } = 501;

    }

    public class RestResult
    {
        public RestResult()
        {
            this.code = 1;
            this.message = "成功";
        }

        public RestResult(int code, string message)
        {
            this.code = code;
            this.message = message;
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public int code { get; set; } = 1;

        /// <summary>
        /// 提示语
        /// </summary>
        public string message { get; set; } = "成功";

    }

    public class RestDataResult<T>
    {
        public RestDataResult()
        {
            this.code = 1;
            this.message = "成功";
        }

        public RestDataResult(int code, string message)
        {
            this.code = code;
            this.message = message;
        }

        public RestDataResult(int code, T data, string message)
        {
            this.code = code;
            this.message = message;
            this.data = data;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public int code { get; set; } = 1;

        /// <summary>
        /// 提示语
        /// </summary>
        public string message { get; set; } = "成功";

        /// <summary>
        /// 数据
        /// </summary>
        public T data { get; set; }

    }

    public class RestPageResult<T>
    {
        public RestPageResult(List<T> list, int total)
        {
            this.code = code;
            this.message = message;
            this.list = list;
            this.total = total;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public int code { get; set; } = 1;

        /// <summary>
        /// 提示语
        /// </summary>
        public string message { get; set; } = "成功";

        /// <summary>
        /// 列表
        /// </summary>
        public List<T> list { get; set; } = new List<T>();

        /// <summary>
        /// 条数
        /// </summary>
        public int total { get; set; }

    }


}
