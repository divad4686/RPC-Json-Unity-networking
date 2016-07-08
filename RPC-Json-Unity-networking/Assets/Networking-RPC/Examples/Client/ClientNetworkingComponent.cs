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
