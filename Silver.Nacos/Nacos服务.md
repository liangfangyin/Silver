### 1、服务注册与注销

#### 1.1.Net站点注册

站点Program.cs文件：

```c#
//注册
builder.Services.AddNacosAspNet(new NacosRegisterInstanceRequest() { ip="localhost",port=6001 });
//引用用于站点关闭注销
app.UseNacosAspNet(app.Lifetime);
```

NacosRegisterInstanceRequest类说明：

```c#
/// <summary>
    /// 注册
    /// </summary>
    public class NacosRegisterInstanceRequest
    {

        /// <summary>
        /// 是 服务实例IP
        /// </summary>
        public string ip { get; set; } = "";

        /// <summary>
        /// 是   服务实例port
        /// </summary>
        public int port { get; set; } = 80;

        /// <summary>
        /// Nacos地址
        /// </summary>
        public string serverAddresses { get; set; } = "http://127.0.0.1:8848";

        /// <summary>
        /// 否   命名空间ID
        /// </summary>
        public string namespaceId { get; set; } = "";

        /// <summary>
        /// 否   权重
        /// </summary>
        public double weight { get; set; } = 100;

        /// <summary>
        /// 否   权重
        /// </summary>
        public bool enabled { get; set; } = true;

        /// <summary>
        /// 否   是否健康
        /// </summary>
        public bool healthy { get; set; } = true;

        /// <summary>
        /// 否   扩展信息
        /// </summary>
        public string metadata { get; set; } = "";

        /// <summary>
        /// 否   集群名
        /// </summary>
        public string clusterName { get; set; } = "DEFAULT";

        /// <summary>
        /// 是   服务名
        /// </summary>
        public string serviceName { get; set; } = "";

        /// <summary>
        /// 否   分组名
        /// </summary>
        public string groupName { get; set; } = "DEFAULT_GROUP";

        /// <summary>
        /// 否   是否临时实例
        /// </summary>
        public bool ephemeral { get; set; } = true;

        /// <summary>
        /// 是   用户名
        /// </summary>
        public string userName { get; set; } = "nacos";

        /// <summary>
        /// 是   密码
        /// </summary>
        public string passWord { get; set; } = "nacos";

        /// <summary>
        /// 否   心跳时间 毫秒
        /// </summary>
        public int bearTimeOut { get; set; } = 1000;

        /// <summary>
        /// 请求超时时间
        /// </summary>
        public int defaultTimeOut { get; set; } = 5000;

    }
```

#### 1.2.客户端注册

```c#
//注册
NacosExtensions.AddNacosClient(new NacosRegisterInstanceRequest() { ip="localhost",port=6001 });
//注销
NacosExtensions.UseNacosClient()
```

NacosRegisterInstanceRequest见1.1备注。



### 2、配置管理

```c#
//获取配置：
NacosExtensions.SelectOfConfig(new NacosConfigRequest(){ ..... });
NacosExtensions.SelectOfConfig<T>(new NacosConfigRequest(){ ..... });
//发布设置
NacosExtensions.SetOfConfig(new NacosAddConfigRequest(){ .... });
//删除设置
NacosExtensions.DeleteOfConfig(new NacosDeleteConfigRequest(){  .... });
```

NacosConfigRequest类备注：

```c#
public class NacosConfigRequest
{

        /// <summary>
        /// 租户信息，对应 Nacos 的命名空间ID字段--否
        /// </summary>
        public string tenant { get; set; } = "";

        /// <summary>
        /// 配置 ID--是
        /// </summary>
        public string dataId { get; set; } = "";

        /// <summary>
        /// 配置分组--是
        /// </summary>
        public string group { get; set; } = "";

}
```

NacosAddConfigRequest类备注：

```c#
public class NacosAddConfigRequest
{

        /// <summary>
        /// 否   租户信息，对应 Nacos 的命名空间ID字段
        /// </summary>
        public string tenant { get; set; } = "";

        /// <summary>
        /// 是   配置 ID
        /// </summary>
        public string dataId { get; set; } = "";

        /// <summary>
        /// 是   配置分组
        /// </summary>
        public string group { get; set; } = "";

        /// <summary>
        /// 是   配置内容
        /// </summary>
        public string content { get; set; } = "";

        /// <summary>
        /// 否   配置类型
        /// </summary>
        public string type { get; set; } = "";

}
```

NacosDeleteConfigRequest类备注：

```c#
public class NacosDeleteConfigRequest
{

        /// <summary>
        /// 否   租户信息，对应 Naocs 的命名空间ID字段
        /// </summary>
        public string tenant { get; set; } = "";

        /// <summary>
        /// 是   配置 ID
        /// </summary>
        public string dataId { get; set; } = "";

        /// <summary>
        /// 是   配置分组
        /// </summary>
        public string group { get; set; } = "";
 
}
```



### 3、服务调用

```c#
//获取指定服务实例-权重均衡
NacosExtensions.SelectOneHealthyInstance(new NacosListInstanceRequest(){ ... });

//Get
NacosExtensions.GetForAsk(new ForGetRequest(){ ... });
NacosExtensions.GetForAsk<T>(new ForGetRequest(){ ... });

//Post
NacosExtensions.PostForAsk(new ForGetRequest(){ ... });
NacosExtensions.PostForAsk<T>(new ForGetRequest(){ ... });

//Put
NacosExtensions.PutForAsk(new ForGetRequest(){ ... });
NacosExtensions.PutForAsk<T>(new ForGetRequest(){ ... });

//Delete
NacosExtensions.DeleteForAsk(new ForGetRequest(){ ... });
NacosExtensions.DeleteForAsk<T>(new ForGetRequest(){ ... });

```

NacosListInstanceRequest类备注：

```c#
public class NacosListInstanceRequest
{

        /// <summary>
        /// 是 服务名
        /// </summary>
        public string serviceName { get; set; } = "";

        /// <summary>
        /// 否 分组名
        /// </summary>
        public string groupName { get; set; } = "";

        /// <summary>
        /// 否   命名空间ID
        /// </summary>
        public string namespaceId { get; set; } = "";

        /// <summary>
        /// 否   多个集群用逗号分隔    集群名称
        /// </summary>
        public string clusters { get; set; } = "";

        /// <summary>
        /// 否，默认为false 是否只返回健康实例
        /// </summary>
        public bool healthyOnly { get; set; } = false;
         
}
```

ForGetRequest类备注：

```c#
public class ForGetRequest
    {
        /// <summary>
        /// 服务名称 -必填
        /// </summary>
        public string serverName { get; set; } = "";

        /// <summary>
        /// 分组名-选填
        /// </summary>
        public string groupName { get; set; } = "";

        /// <summary>
        /// 命名空间-选填
        /// </summary>
        public string nameSpaceId { get; set; } = "";

        /// <summary>
        /// 接口地址-必填，如 /v1/member/login
        /// </summary>
        public string urls { get; set; } = "";

        /// <summary>
        /// 参数-选填
        /// </summary>
        public object parames { get; set; } = null;

        /// <summary>
        /// 是否json提交数据-选填
        /// </summary>
        public bool isJson { get; set; } = false;

        /// <summary>
        /// 请求协议
        /// </summary>
        public string agree { get; set; } = "http";

        /// <summary>
        /// 调用超时时间
        /// </summary>
        public int defaultTimeOut { get; set; } = 5000;

        /// <summary>
        /// 头-选填
        /// </summary>
        public WebHeaderCollection header { get; set; } = new WebHeaderCollection();

        /// <summary>
        /// 请求格式-选填
        /// </summary>
        public string contentType { get; set; } = ForGetContentType.formUrlencoded;

    }

    /// <summary>
    /// 请求方式
    /// </summary>
    public class ForGetContentType
    {
        /// <summary>
        /// form 表单
        /// </summary>
        public static string formUrlencoded = "application/x-www-form-urlencoded;charset=utf-8";

        /// <summary>
        /// 表单
        /// </summary>
        public static string formData = "multipart/form-data";

        /// <summary>
        /// json方式
        /// </summary>
        public static string jsonData = "application/json;charset=utf-8";

        /// <summary>
        /// xml方式
        /// </summary>
        public static string xmlData = "text/xml";


    }
```





