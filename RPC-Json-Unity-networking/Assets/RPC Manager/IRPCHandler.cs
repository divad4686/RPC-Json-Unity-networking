using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Unity_RPC
{
	public interface IRPCHandler
	{
		// Methods that handle requests from the server
		bool haveRequestMethod(string method);

		// Actions that handle responses from the server
		bool haveActionIdResponse(string idAction);

		// Handle a request form the server
		void handleRequest(string method,string idAction = null,IDictionary<string,object>param = null);

		// response the server send to this class after we make a request
        void handleResponse(string idAction,object result);

		// Handle a error send from the server after we make a request to it
		void handleError(string idAction,int code,string errorMessage, IDictionary<string,object> errorData = null);
	}
}

