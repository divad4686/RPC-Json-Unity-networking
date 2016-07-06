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
