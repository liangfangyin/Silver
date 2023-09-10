using System;
using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Rpc;
using TouchSocket.Rpc.XmlRpc;
using TouchSocket.Sockets;

namespace Silver.Net.Rpc
{
    public class RpcServer : IDisposable
    {
        RpcStore rpcStore;
        public RpcServer(int port=7706, string xmlRpcUrl = "/jsonRpc", string key= "jsonRpcParser")
        {
            rpcStore = new RpcStore(new TouchSocket.Core.Container());
            //添加解析器，解析器根据传输协议，序列化方式的不同，调用RPC服务
            rpcStore.AddRpcParser(key, CreateXmlRpcRpcParser(port, xmlRpcUrl));
            //注册当前程序集的所有服务
            rpcStore.RegisterAllServer();
            ////分享代理，代理文件可通过RRQMTool远程获取。
            //rpcStore.ShareProxy(new IPHost(8848));
        }

        private static IRpcParser CreateXmlRpcRpcParser(int port,string xmlRpcUrl= "/jsonRpc")
        {
            HttpService service = new HttpService(); 
            service.Setup(new TouchSocketConfig().UsePlugin().SetListenIPHosts(new IPHost[] { new IPHost(port) })).Start(); 
            service.AddPlugin<JsonPlugin>(); 
            return service.AddPlugin<XmlRpcParserPlugin>().SetXmlRpcUrl(xmlRpcUrl);
        }

        public class JsonPlugin : HttpPluginBase
        {
            protected override void OnPost(ITcpClientBase client, HttpContextEventArgs e)
            {
                string s = e.Context.Request.GetBody();
                base.OnPost(client, e);
            }
        }

        public void Dispose()
        {
            rpcStore.Dispose();
        }
    }
}
