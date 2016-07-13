# RPC-Json-Unity-networking
Example of client and server handling messages using the RPC-Json protocol and Unity networking 

The main code is inside [Assets folder](https://github.com/divad4686/RPC-Json-Unity-networking/tree/master/RPC-Json-Unity-networking/Assets) 

This is a implementation of a module I made on the game I'm working on to send messages between my clients and the server, using Unity networking to send messages formatted with the [Json-RPC specification 2.0](http://www.jsonrpc.org/specification)

In the examples folder you can see how to create a server and a client.

First lets look on how to create a server:

```cs
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

        // class that receive calls from the client
        Calculator calc = new Calculator(handler);
	}
}
```

The RPCHandler class takes care of managing and invoking the functions required for requests, responses, notifications and errors.

RPCParser, have functions to format parameters in a Json-RPC formated string. It also have the `HandleMessage` which receives a Json-RPC formated string, parse it and calls the respective IRPCHandler function, depending if it is a requets, a response a notification or an error.

In the Calculator class we can see how we register to handle a request call from the client

```cs
using UnityEngine;
using System.Collections;
using Unity_RPC;
using System.Collections.Generic;
using System;

public class Calculator:RPCHandler {

    // Need the server to send back responses to the client
    IRPCListener _listener;

    public Calculator(IRPCListener listener)
    {
        _listener = listener;
        _listener.addRequestListener("substract",substract);
    }

    object substract(IDictionary<string, object> param)
    {
        int x = Int32.Parse(param["x"].ToString());
        int y = Int32.Parse(param["y"].ToString());
        int ans = x - y;
        return ans;
    }
}
```

Acording to the specification, a response must return a object type, so our function that receive requests does just that. We add it to the IRPCListener with the method name the client can use to invoke it.

Lets see the client component:

```cs
using UnityEngine;
using System.Collections;
using Unity_RPC;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class ClientNetworkingComponent : MonoBehaviour {
    
    public Text label;

    ClientNetworking _client;
	// Use this for initialization
	void Start () {
        RPCHandler handler = new RPCHandler();
        RPCParser parser = new RPCParser(handler);

        _client = new ClientNetworking(parser,"localhost",7777);
        _client.connect();

        handler.addResponseListener("numberResult",handleResult,handleError);
               
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void substract()
    {
        _client.sendRequest("substract","numberResult",new Dictionary<string,object>()
            {
                {"x",2},
                {"y",1}
            });
    }

    void handleResult(object result)
    {
        int total = Int32.Parse(result.ToString());
        label.text = total.ToString();
    }

    void handleError(int code,string message,IDictionary<string,object> data)
    {
        Debug.LogError(message);
    }
}
```
Like the server, first we create the RPC handler and parser, then we make the network client connecting it to localhost and finally we add a function that receive a response from the server with id "numberResult". When the user call the substract functions, we send a request to the server on the method "substract" with response id "numberResult".
