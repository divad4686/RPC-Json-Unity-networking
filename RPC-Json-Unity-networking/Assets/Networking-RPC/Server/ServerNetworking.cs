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
        private int _port = 7777;
        private int _maxConnections = 1000;
        private short messageType = 10000;

        IRCPParser _rpcParser;


        public ServerNetworking (IRCPParser rcpParser)
        { 
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
            string userId = netMes.conn.connectionId.ToString();
            _rpcParser.HandleMessage(message.json,
                (result,actionId)=>
                {
                    sendResponse(userId,actionId,result);
                },
                (actionId,errorData,e)=>
                {
                    sendResponseError(userId,actionId,errorData,e);
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

        public void sendRequest(string userId,string method,string idAction,IDictionary<string,object> param = null)
        {
            var json = _rpcParser.formatRequest(method,idAction,param);
            sendToClient(userId,json);
        }

        public void sendNotification(string userId,string method,IDictionary<string,object> param = null)
        {
            var json = _rpcParser.formatNotification(method,param);
            sendToClient(userId,json);
        }

        public void sendResponse(string userId,string idAction,object result)
        {
            var json = _rpcParser.formatResponse(idAction,result);
            sendToClient(userId,json);
        }

        public void sendResponseError(string userId,string idAction,IDictionary<string,object>data, Exception e)
        {
            var json = _rpcParser.formatResponseError(idAction,data,e);
            sendToClient(userId,json);
        }

        protected virtual void sendToClient(string userId,string json)
        {            
            var message = new jsonStringMessage();
            message.json = json;
            int connectionId = getConnectionIdForUser(userId);
            NetworkServer.SendToClient(connectionId,messageType,message);
        }

        protected virtual int getConnectionIdForUser(string userId)
        {
            // By default the user id is the same as the connection ID
            return Int32.Parse(userId);
        }
    }

    public interface IServer
    {
        void CreateServer();
        void sendRequest(string userId,string method,string idAction,IDictionary<string,object> param = null);
        void sendNotification(string userId,string method,IDictionary<string,object> param = null);
        void sendResponse(string userId,string idAction,object result);
        void sendResponseError(string userId,string idAction,IDictionary<string,object>data,Exception e);
    }
}