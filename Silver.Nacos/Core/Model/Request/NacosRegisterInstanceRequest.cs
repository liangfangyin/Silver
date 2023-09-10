using Silver.Basic;
using System;

namespace Silver.Nacos.Core.Model.Request
{
    /// <summary>
    /// 注册
    /// </summary>
    public class NacosRegisterInstanceRequest
    {

        private string _ip = "";
        /// <summary>
        /// 是 服务实例IP
        /// </summary> 
        public string ip
        {
            get
            {
                if (!string.IsNullOrEmpty(_ip))
                {
                    return _ip;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:IP")))
                {
                    return "";
                }
                return _ip = ConfigurationUtil.GetSection("Nacos:IP");
            }
            set
            {
                _ip = value;
            }
        }


        public int _port = 0;
        /// <summary>
        /// 是   服务实例port
        /// </summary>
        public int port
        {
            get
            {
                if (_port > 0)
                {
                    return _port;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:Port")))
                {
                    return 80;
                }
                return _port = ConfigurationUtil.GetSection("Nacos:Port").ToInt();
            }
            set
            {
                _port = value;
            }
        }


        public string _serverAddresses = "";
        /// <summary>
        /// Nacos地址
        /// </summary>
        public string serverAddresses
        {
            get
            {
                if (!string.IsNullOrEmpty(_serverAddresses))
                {
                    return _serverAddresses;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:ServerAddresses")))
                {
                    return "http://127.0.0.1:8848";
                }
                return _serverAddresses = ConfigurationUtil.GetSection("Nacos:ServerAddresses");
            }
            set
            {
                _serverAddresses = value;
            }
        }


        public string _namespaceId = "";
        /// <summary>
        /// 否   命名空间ID
        /// </summary>
        public string namespaceId
        {
            get
            {
                if (!string.IsNullOrEmpty(_namespaceId))
                {
                    return _namespaceId;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:NamespaceId")))
                {
                    return "";
                }
                return _namespaceId = ConfigurationUtil.GetSection("Nacos:NamespaceId");
            }
            set
            {
                _namespaceId = value;
            }
        }

        public double _weight;
        /// <summary>
        /// 否   权重
        /// </summary>
        public double weight
        {
            get
            {
                if (_weight > 0)
                {
                    return _weight;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:Weight")))
                {
                    return 100;
                }
                return _weight = ConfigurationUtil.GetSection("Nacos:Weight").ToInt();
            }
            set
            {
                _weight = value;
            }
        }

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

        public string _clusterName;
        /// <summary>
        /// 否   集群名
        /// </summary>
        public string clusterName
        {
            get
            {
                if (!string.IsNullOrEmpty(_clusterName))
                {
                    return _clusterName;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:ClusterName")))
                {
                    return "DEFAULT";
                }
                return _clusterName = ConfigurationUtil.GetSection("Nacos:ClusterName");
            }
            set
            {
                _clusterName = value;
            }
        }

        public string _serviceName;
        /// <summary>
        /// 是   服务名
        /// </summary>
        public string serviceName
        {
            get
            {
                if (!string.IsNullOrEmpty(_serviceName))
                {
                    return _serviceName;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:ServiceName")))
                {
                    return Guid.NewGuid().ToString();
                }
                return _serviceName = ConfigurationUtil.GetSection("Nacos:ServiceName");
            }
            set
            {
                _serviceName = value;
            }
        }

        public string _groupName;
        /// <summary>
        /// 否   分组名
        /// </summary>
        public string groupName
        {
            get
            {
                if (!string.IsNullOrEmpty(_groupName))
                {
                    return _groupName;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:GroupName")))
                {
                    return "DEFAULT_GROUP";
                }
                return _groupName = ConfigurationUtil.GetSection("Nacos:GroupName");
            }
            set
            {
                _groupName = value;
            }
        }

        /// <summary>
        /// 否   是否临时实例
        /// </summary>
        public bool ephemeral { get; set; } = true;


        public string _userName;
        /// <summary>
        /// 是   用户名
        /// </summary>
        public string userName
        {
            get
            {
                if (!string.IsNullOrEmpty(_userName))
                {
                    return _userName;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:UserName")))
                {
                    return "nacos";
                }
                return _userName = ConfigurationUtil.GetSection("Nacos:UserName");
            }
            set
            {
                _userName = value;
            }
        }

        public string _passWord;
        /// <summary>
        /// 是   密码
        /// </summary>
        public string passWord
        {
            get
            {
                if (!string.IsNullOrEmpty(_passWord))
                {
                    return _passWord;
                }
                if (string.IsNullOrEmpty(ConfigurationUtil.GetSection("Nacos:PassWord")))
                {
                    return "nacos";
                }
                return _passWord = ConfigurationUtil.GetSection("Nacos:PassWord");
            }
            set
            {
                _passWord = value;
            }
        }

        /// <summary>
        /// 否   心跳时间 毫秒
        /// </summary>
        public int bearTimeOut { get; set; } = 2000;

        /// <summary>
        /// 请求超时时间
        /// </summary>
        public int defaultTimeOut { get; set; } = 5000;

    }
}
