using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Unity_RPC
{
	public abstract class RPCHandlerBase : IRPCHandler
	{
		protected List<string> requestMethods;
		protected List<string> idActions;

		protected RPCHandlerBase()
		{
			requestMethods =  new List<string>();
			idActions = new List<string>();
		}

		public bool haveRequestMethod(string method){
			return requestMethods.Contains(method);
		}

		public bool haveActionIdResponse(string idAction){
			return idActions.Contains(idAction);
		}

		// Calls the server does to this class
		public abstract void handleRequest(string method,string idAction = null,IDictionary<string,object>param = null);

		// response the server send to this class after we make a request
        public abstract void handleResponse(string idAction,object result);

		public abstract void handleError(string idAction,int code,string errorMessage, IDictionary<string,object> errorData = null);
	}
}

