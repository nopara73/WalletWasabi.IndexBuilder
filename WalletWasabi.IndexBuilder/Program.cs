using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WalletWasabi.IndexBuilder
{
    internal class Program
    {
        private static RPCClient RpcTestNet { get; set; }
        private static RPCClient RpcMain { get; set; }

        private static void Main(string[] args)
        {
            try
            {
                var rpcUser = args[0];
                var rpcPassword = args[1];
                RpcTestNet = new RPCClient(
                            credentials: new RPCCredentialString
                            {
                                UserPassword = new NetworkCredential(rpcUser, rpcPassword)
                            },
                            network: Network.TestNet);
                RpcMain = new RPCClient(
                            credentials: new RPCCredentialString
                            {
                                UserPassword = new NetworkCredential(rpcUser, rpcPassword)
                            },
                            network: Network.Main);

                AssertRpcNodeFullyInitialized();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine("Press a key to exit...");
                Console.ReadKey();
            }
        }

        private static void AssertRpcNodeFullyInitialized()
        {
            var blockchainInfoRequest = new RPCRequest(RPCOperations.getblockchaininfo, parameters: null);
            var blockchainInfo = RpcTestNet.GetBlockchainInfoAsync().GetAwaiter().GetResult();

            var blocks = blockchainInfo.Blocks;
            var headers = blockchainInfo.Headers;
            if (blocks != headers)
            {
                throw new NotSupportedException("Bitcoin Core is not fully synchronized on the testnet.");
            }

            blockchainInfoRequest = new RPCRequest(RPCOperations.getblockchaininfo, parameters: null);
            blockchainInfo = RpcMain.GetBlockchainInfoAsync().GetAwaiter().GetResult();

            blocks = blockchainInfo.Blocks;
            headers = blockchainInfo.Headers;
            if (blocks != headers)
            {
                throw new NotSupportedException("Bitcoin Core is not fully synchronized on the mainnet.");
            }
        }
    }
}
