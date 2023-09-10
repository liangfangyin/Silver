using System;
using System.Threading.Tasks;
using TouchSocket.Rpc.JsonRpc;
using TouchSocket.Rpc.TouchRpc;

namespace Silver.Net.Rpc
{
    public class RpcClient:IDisposable
    {
        JsonRpcClient jsonRpcClient;
        public RpcClient(string apiUrl = "http://127.0.0.1:7706/Rpc")
        {
            JsonRpcClient jsonRpcClient = new JsonRpcClient();
            jsonRpcClient.Setup(apiUrl);
            jsonRpcClient.Connect(); 
        }
         
        /// <summary>
        /// 调用方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T Send<T>(string method, object[] parameters = default)
        {
            return jsonRpcClient.Invoke<T>(method, InvokeOption.WaitInvoke, parameters);
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<T> SendAsync<T>(string method, object[] parameters = default)
        {
            return await jsonRpcClient.InvokeAsync<T>(method, InvokeOption.WaitInvoke, parameters);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            jsonRpcClient.Dispose();
        }


    }
}
