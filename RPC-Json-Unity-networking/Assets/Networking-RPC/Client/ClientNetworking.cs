using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Unity_RPC;
using System.Collections.Generic;
using System;

namespace Unity_RPC{
    public class ClientNetworking {
        protected short msgTypeId = 10000;
        protected string _ip;
        protected int _port;
        protected NetworkClient _client;

        protected IRPCParser _rcpParser;

        public ClientNetworking(IRPCParser parser, string ip,int port)
        {
            _port = port;
            _ip = ip;
            _rcpParser = parser;

            var config = new ConnectionConfig ();

            config.AddChannel (QosType.Reliable);
            config.AddChannel (QosType.Unreliable);

            _client = new NetworkClient ();
            _client.Configure (config, 1);

            RegisterHandlers ();
        }

        public virtual void connect()
        {
            _client.Connect (_ip, _port);
        }

        void OnApplicationQuit() {
            if (_client != null) {
                _client.Disconnect ();
                _client.Shutdown ();
                _client = null;
            }
        }

        void RegisterHandlers () {
            _client.RegisterHandler (msgTypeId, onMessageReceived);
            _client.RegisterHandler(MsgType.Connect, OnConnected);
            _client.RegisterHandler(MsgType.Disconnect, OnDisconnected);
            _client.RegisterHandler(MsgType.Error, OnError);
        }

        void onMessageReceived(NetworkMessage message)
        {
            string json = message.ReadMessage<jsonStringMessage>().json;
            _rcpParser.HandleMessage(json,
                (result,actionId) =>
                {
                    sendResponse(actionId,result);
                },
                (actionId,data,e) =>
                {
                    sendResponseError(actionId,data,e);
                }
            );
        }

        protected virtual void OnConnected(NetworkMessage message) {        
            
        }

        protected virtual void OnDisconnected(NetworkMessage message) {
            
        }

        protected virtual void OnError(NetworkMessage message) {          
            
        }

        public void sendRequest(string method,string idAction,IDictionary<string,object>param)
        {
            string json = _rcpParser.formatRequest(method,idAction,param);
            sendToServer(json);
        }

        // Send a notification from this client to the server
        public void sendNotification(string method,IDictionary<string,object> param = null)
        {
            if(param == null)
                param = new Dictionary<string, object>();        
            string json = _rcpParser.formatNotification(method,param);
            sendToServer(json);
        }

        // Send a response from a request the server made to this client
        public void sendResponse(string idAction,object result)
        {        
            string json = _rcpParser.formatResponse(idAction,result);
            sendToServer(json);             
        }

        // Send a error to the server from a request it made to this client
        public void sendResponseError(string idAction,Dictionary<string,object> data,Exception e)
        {         
            string json = _rcpParser.formatResponseError(idAction,data,e);
            sendToServer(json);
        }

        protected virtual void sendToServer(string json)
        {
            var message = new jsonStringMessage();
            message.json = json;
            _client.Send(msgTypeId,message);
        }
    }
}