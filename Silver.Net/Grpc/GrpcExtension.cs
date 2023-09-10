using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Silver.Basic;
using System;
using System.Net.Http;

namespace Silver.Net.Grpc
{
    public class GrpcExtension<T> : IDisposable
    {
        public T client;
        public GrpcChannel channel;
        public GrpcExtension()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress(ConfigurationUtil.GetSection("Grpc:Service"), new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });
        }

        /// <summary>
        /// Grpc服务端地址
        /// </summary>
        /// <param name="grpcServer"></param>
        public GrpcExtension(string grpcServer)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            channel = GrpcChannel.ForAddress(grpcServer, new GrpcChannelOptions
            {
                HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            });
        }

        public void Dispose()
        {
            if (client != null)
            {
                channel.Dispose();
            }
        }
    }

}
