using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;

namespace OntologyCSharpSDK.Network
{
    public class WebsocketSubscribe
    {
        public async Task SubscribeAsync(string contractFilter, bool subscribeEvents, bool subscribeJsonBlock, bool subscribeHexBlock, bool subscribeBlockTxHashes, string node, IProgress<string> progress)
        {
            try
            {
                var jsonObject = new JObject
                {
                    ["Action"] = "subscribe",
                    ["Version"] = "1.0.0",
                    ["contractAddress"] = contractFilter,
                    ["SubscribeEvent"] = subscribeEvents,
                    ["SubscribeJsonBlock"] = subscribeJsonBlock,
                    ["SubscribeRawBlock"] = subscribeHexBlock,
                    ["SubscribeBlockTxHashs"] = subscribeBlockTxHashes
                };


                var jsonSubscribe = JsonConvert.SerializeObject(jsonObject);

                using (var ws = new WebSocket(node))
                {
                    ws.OnMessage += (sender, e) =>
                {
                    progress.Report(e.Data);
                };

                    ws.OnError += (sender, e) =>
                    {
                        ws.Close();
                    };

                    Action<bool> completed = null;

                    ws.Connect();
                    ws.SendAsync(jsonSubscribe, completed);

                    // While connection is still alive, send heartbeat every 4 minutes (required by websocket server else session expires)
                    while (ws.IsAlive)
                    {
                        var heartbeat = new JObject { ["Action"] = "heartbeat", ["Version"] = "1.0.0" };


                        var jsonHeartbeat = JsonConvert.SerializeObject(heartbeat);
                        ws.SendAsync(jsonHeartbeat, completed);

                        await Task.Delay(200000);
                    }
                }
            }
            catch { throw; }
        }
    }
}
