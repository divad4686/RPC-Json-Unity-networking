using UnityEngine;
using System.Collections;
using Unity_RPC;

/// <summary>
/// Example entry point of the server
/// </summary>
public class ServerComponent : MonoBehaviour {


	// Use this for initialization
	void Start () {
        Application.runInBackground = true;

        // RPC
        RPCHandler handler = new RPCHandler();
        IRPCParser rpcParser = new RPCParser(handler);

        // Server
        ServerNetworking server = new ServerNetworking(rpcParser,7777);
        server.CreateServer();

        Calculator calc = new Calculator(handler);
	}
}
