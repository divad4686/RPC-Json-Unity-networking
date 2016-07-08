using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using MiniJSON;
using System.Collections.Generic;
using System;

/// <summary>
/// Server
/*///////////////////// Communication protocol (based on JSON-RPC 2.0): http://www.jsonrpc.org/specification ///////////////////*/

using Unity_RPC;
using System.Linq;

namespace Unity_RPC{

    public class ServerNetworking : IServer {

        // Server configuration
        protected int _port;
        private int _maxConnections = 1000;
        private short messageType = 10000;

        protected IRPCParser _rpcParser;


        public ServerNetworking (IRPCParser rcpParser,int port)
        { 
            _port = port;
            _rpcParser = rcpParser;
        }

        public void CreateServer() {
            RegisterHandlers ();

            var config = new ConnectionConfig ();

            config.AddChannel (QosType.Reliable);
            config.AddChannel (QosType.Unreliable);

            var ht = new HostTopology (config, _maxConnections);

            if (!NetworkServer.Configure (ht)) {
                Debug.Log ("No server created error en configure");
                return;
            } else {
                if(NetworkServer.Listen (_port))
                    Debug.Log ("Server created");   
                else
                    Debug.Log ("No server created, error en Listen");    
            }
            Debug.Log("Port: " + _port);
        }

        void OnApplicationQuit() {
            NetworkServer.Shutdown ();
        }

        private void RegisterHandlers () {
            RegisterHandler (messageType, OnMessageReceived);
            RegisterHandler (MsgType.Connect, OnClientConnected);
            RegisterHandler (MsgType.Disconnect, OnClientDisconnected);
        }

        private void RegisterHandler(short t, NetworkMessageDelegate handler) {
            NetworkServer.RegisterHandler (t, handler);
        }

        protected virtual void OnMessageReceived(NetworkMessage netMes)
        {
            var message = netMes.ReadMessage<jsonStringMessage>();

            // By default the userId is the connection ID. This could be changed to use your own userID from Database or whatever, this is the main reason OnMessageReceived is Virtual
            int connectionId = netMes.conn.connectionId;
            _rpcParser.HandleMessage(message.json,
                (result,actionId)=>
                {
                    sendResponse(connectionId,actionId,result);
                },
                (actionId,errorData,e)=>
                {
                    sendResponseError(connectionId,actionId,errorData,e);
                }
            );
        }

        protected virtual void OnClientConnected(NetworkMessage netMes) 
        {   
            // Can do stuff on child class 
        }

        protected virtual void OnClientDisconnected(NetworkMessage netMes) 
        {
            // Can do stuff on child class    
        }

        public void sendRequest(int connectionId,string method,string idAction,IDictionary<string,object> param = null)
        {
            var json = _rpcParser.formatRequest(method,idAction,param);
            sendToClient(connectionId,json);
        }

        public void sendNotification(int connectionId,string method,IDictionary<string,object> param = null)
        {
            var json = _rpcParser.formatNotification(method,param);
            sendToClient(connectionId,json);
        }

        public void sendResponse(int connectionId,string idAction,object result)
        {
            var json = _rpcParser.formatResponse(idAction,result);
            sendToClient(connectionId,json);
        }

        public void sendResponseError(int connectionId,string idAction,IDictionary<string,object>data, Exception e)
        {
            var json = _rpcParser.formatResponseError(idAction,data,e);
            sendToClient(connectionId,json);
        }

        protected virtual void sendToClient(int connectionId,string json)
        {            
            var message = new jsonStringMessage();
            message.json = json;
            NetworkServer.SendToClient(connectionId,messageType,message);
        }
    }

    public interface IServer
    {
        void CreateServer();
        void sendRequest(int connectionId,string method,string idAction,IDictionary<string,object> param = null);
        void sendNotification(int connectionId,string method,IDictionary<string,object> param = null);
        void sendResponse(int connectionId,string idAction,object result);
        void sendResponseError(int connectionId,string idAction,IDictionary<string,object>data,Exception e);
    }
}