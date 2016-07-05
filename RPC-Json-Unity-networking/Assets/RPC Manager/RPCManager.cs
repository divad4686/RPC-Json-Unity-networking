﻿using System;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

namespace Unity_RPC
{
    public class RPCManager : IRPCManager
    {   
        List<IRPCHandler> listeners;

        public RPCManager ()
        {           
            listeners = new List<IRPCHandler>();
        }

        public void addListener(IRPCHandler handler){
            if(!listeners.Contains(handler))
                listeners.Add(handler);
        }

        public void removeListener(IRPCHandler handler){
            if(listeners.Contains(handler))
                listeners.Remove(handler);
        }

        // Get a message from the server, choose what action to executed depending if it is a request, a response, notification or an error
        public void HandleMessage(string json)
        {            
            //Debug.Log("Message: " + json);
            var data = Json.Deserialize(json) as Dictionary<string,object>;
            if(data.ContainsKey("error")) // Response error from a request this client made to the server
            {                   
                var idAction = data["id"].ToString();
                var errorData = data["error"] as Dictionary<string,object>;
                int code = Convert.ToInt32(errorData["code"]);
                string errorMessage = errorData["message"].ToString();

                // Check if there is data inside the error
                Dictionary<string,object> dataError = null;
                if(errorData.ContainsKey("data"))
                    dataError = errorData["data"] as Dictionary<string,object>;
                
                handleError(idAction,code,errorMessage,dataError);
            }
            else if(data.ContainsKey("result")) // result the server is sending to this client, from a request this client made to the server previously
            {
                var idAction = data["id"].ToString();
                object result = data["result"];
                handleResponse(idAction,result);
            }
            else // request message or notification. A request the server is asking to this client, or a notification the server is sending to this client
            {
                string method = data["method"].ToString();
                Dictionary<string,object> param = null;
                if(data.ContainsKey("params"))
                    param = Json.Deserialize(data["params"].ToString()) as Dictionary<string,object>;                

                string idAction = null;
                if(data.ContainsKey("id")) // if it doesn't have id, it is a notification
                    idAction = data["id"].ToString();   
                handleRequest(method,idAction,param);
            }
        }

        void handleError(string idAction,int code,string errorMessage, Dictionary<string,object> errorData = null)
        {
            foreach(var handler in listeners)
            {
                if(handler.haveActionIdResponse(idAction))
                    handler.handleError(idAction,code,errorMessage,errorData);
            }
        }

        void handleRequest(string method,string idAction = null,IDictionary<string,object>param = null)
        {           
            foreach(var handler in listeners)
            {
                if(handler.haveRequestMethod(method))
                    handler.handleRequest(method,idAction,param);
            }
        }

        void handleResponse(string idAction,object result)
        {   
            foreach(var handler in listeners)
            {
                if(handler.haveActionIdResponse(idAction))
                    handler.handleResponse(idAction,result);
            }
        }

        // Call a method in the server
        public string formatRequest(string method,string idAction,IDictionary<string,object> param = null)
        {
            return formatRequestInternal(method,param,idAction);
        }

        // Call a method in the server
        public string formatRequest(string method,string idAction,IList param = null)
        {
            return formatRequestInternal(method,param,idAction);
        }

        // Send a notification from this client to the server
        public string formatNotification (string method,IDictionary<string,object> param = null)
        {
            return formatRequestInternal(method,param); 
        }

        // Send a response from a request the server made to this client
		public string formatResponse(string idAction,object result)
        {
            Dictionary<string,object> rpc = new Dictionary<string, object>();
            rpc["jsonrpc"] = "2.0";
            rpc["id"] = idAction;
            rpc["result"] = result;
            return Json.Serialize(rpc);
        }

        // Send a error to the server from a request it made to this client
		public string formatResponseError(string idAction,IDictionary<string,object> data,Exception e)
        {
            Dictionary<string,object> rpc = new Dictionary<string, object>();
            rpc["jsonrpc"] = "2.0";
            rpc["id"] = idAction;

            Dictionary<string, object> errorDefinition = new Dictionary<string, object>();
            errorDefinition["code"] = 1;
            errorDefinition["message"] = e.ToString();

            if(data != null)
			    errorDefinition["data"] = data;

            rpc["error"] = errorDefinition;
            return Json.Serialize(rpc);
        }

        string formatRequestInternal(string method,object param = null,string idAction = null)
        {
            Dictionary<string,object> data = new Dictionary<string, object>();
            data["jsonrpc"] = "2.0";
            data["method"] = method;
            if(param != null)
                data["params"] = Json.Serialize(param);
            // if idAction is null, it is a notification
            if(idAction != null)
                data["id"] = idAction;
            return Json.Serialize(data);
        }
    }

    public interface IRPCManager
    {
        void addListener(IRPCHandler handler);
        void removeListener(IRPCHandler handler);
        void HandleMessage(string json);
        string formatRequest(string method,string idAction,IDictionary<string,object> param = null);
        string formatNotification (string method,IDictionary<string,object> param = null);
        string formatResponse(string idAction,object result);
		string formatResponseError(string idAction,IDictionary<string,object> data,Exception e);
    }
}


