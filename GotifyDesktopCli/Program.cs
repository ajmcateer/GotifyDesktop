using gotifySharp;
using System;
using System.Threading.Tasks;
using Websocket.Client;

namespace GotifyDesktopCli
{
    class Program
    {
        static void Main(string[] args)
        {
            GotifySharp test = new GotifySharp("CQPP_1HyRdCun.o", "http://127.0.0.1:80");
            //Task.Run(async()=> await test.Stream.InitWebSocketAsync());

            Task.Run(async () =>
            {
                await test.Stream.InitWebSocketAsync();
            }).GetAwaiter().GetResult();

            
            

            test.Stream.WsClient.MessageReceived.Subscribe(msg => WsIncomingMessage(msg));
            Console.ReadLine();
        }

        private static void WsIncomingMessage(ResponseMessage msg)
        {
            Console.WriteLine(msg.Text);
        }
    }
}
