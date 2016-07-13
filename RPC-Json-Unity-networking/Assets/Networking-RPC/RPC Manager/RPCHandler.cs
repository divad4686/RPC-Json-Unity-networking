using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Unity_RPC
{
    public delegate void NotificationAction(IDictionary<string,object> param);
    public delegate object RequestAction(IDictionary<string,object>param);
    public delegate void ResponseAction(object result);
    public delegate void ErrorAction(int code,string errorMessage,IDictionary<string,object> errorData);
    
    // TODO: have a dictionary with methods and delegates
    public class RPCHandler : IRPCHandler, IRPCListener
	{
        protected Dictionary<string,NotificationAction> notifications;
        protected Dictionary<string,RequestAction> requests;
        protected Dictionary<string,ResponseAction> responses;
        protected Dictionary<string,ErrorAction> errors;

        public RPCHandler()
		{
            notifications = new Dictionary<string, NotificationAction>();
            requests = new Dictionary<string, RequestAction>();
            responses = new Dictionary<string, ResponseAction>();
            errors = new Dictionary<string, ErrorAction>();
		}

        public void handleNotification(string method, IDictionary<string,object>param = null)
        {
            if(notifications.ContainsKey(method))
                notifications[method](param);
        }
        		
        public object handleRequest(string method,IDictionary<string,object>param = null)
        {
            if(requests.ContainsKey(method)){
                var result = requests[method](param);
                return result;
            }

            return null;
        }
        		
        public void handleResponse(string idAction,object result)
        {
            if(responses.ContainsKey(idAction))
                responses[idAction](result);
        }

		public void handleError(string idAction,int code,string errorMessage, IDictionary<string,object> errorData = null)
        {
            if(errors.ContainsKey(idAction))
                errors[idAction](code,errorMessage,errorData);
        }

        #region IRPCListener implementation

        public void addNotificationListener (string method, NotificationAction onNotification)
        {
            notifications[method] = onNotification;
        }

        public void addRequestListener (string method, RequestAction onRequest)
        {
            requests[method] = onRequest;
        }

        public void addResponseListener (string idAction, ResponseAction onResponse, ErrorAction onError)
        {
            responses[idAction] = onResponse;
            errors[idAction] = onError;
        }

        #endregion
	}

    public interface IRPCHandler
    {
        void handleNotification(string method, IDictionary<string,object>param = null);
        object handleRequest(string method,IDictionary<string,object>param = null);
        void handleResponse(string idAction,object result);
        void handleError(string idAction,int code,string errorMessage, IDictionary<string,object> errorData = null);
    }

    public interface IRPCListener
    {
        void addNotificationListener(string method,NotificationAction onNotification);
        void addRequestListener(string method,RequestAction onRequest);
        void addResponseListener(string idAction,ResponseAction onResponse, ErrorAction onError);
    }
}

