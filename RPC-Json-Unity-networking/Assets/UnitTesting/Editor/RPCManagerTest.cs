using UnityEngine;
using System.Collections;
using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;

namespace Unity_RPC.tests{
    [TestFixture]
    public class RPCManagerTest {

        // The manager to test
        RPCManager manager;

        // General handler to test the RPC manager
        IRPCHandler mockHandler;

        // create a IRPCHandler substitute
        IRPCHandler createHandler()
        {
            var handler = Substitute.For<IRPCHandler>();
            return handler;
        }

        [SetUp]
        public void init()
        {
            // Init a new manager for each test
            manager = new RPCManager();

            // Init a basic handler before each test
            mockHandler = createHandler();
        }

        [Test]
        public void testRequestMessage()
        {
            // the json RPC formated string to test, taken from http://www.jsonrpc.org/specification examples
            string jsonRequest = "{\"jsonrpc\": \"2.0\", \"method\": \"subtract\", \"params\": [42, 23], \"id\": 1}";

            //add the substract method to the mockHandler
            mockHandler.haveRequestMethod("subtract").Returns(true);

            manager.addListener(mockHandler);

            // Send the message to the handler
            manager.HandleMessage(jsonRequest);

            // check if the manager received a call to handle the request
            mockHandler.Received().handleRequest("subtract","1",Arg.Any<IDictionary<string,object>>());
        }

        [Test]
        public void testResponseMessage()
        {
            string jsonRequest = "{\"jsonrpc\": \"2.0\", \"result\": 19, \"id\": 1}";

            mockHandler.haveActionIdResponse("1").Returns(true);

            manager.addListener(mockHandler);

            manager.HandleMessage(jsonRequest);

            mockHandler.Received().handleResponse("1",Arg.Any<object>());
        }

        [Test]
        public void testErrorMessage()
        {
            string jsonError = "{\"jsonrpc\": \"2.0\", \"error\": {\"code\": -32700, \"message\": \"Parse error\"}, \"id\": 1}";

            mockHandler.haveActionIdResponse("1").Returns(true);

            manager.addListener(mockHandler);

            manager.HandleMessage(jsonError);

            mockHandler.Received().handleError("1",-32700,"Parse error",null);

        }

        [Test]
        public void testFormatRequestWithList()
        {
            List<int> param = new List<int>{42,23};
            string json = manager.formatRequest("subtract","1",param);

            string expected = "{\"jsonrpc\": \"2.0\", \"method\": \"subtract\", \"params\": \"[42, 23]\", \"id\": \"1\"}";

            Assert.AreEqual(expected.Replace(" ",""),json.Replace(" ",""));
        }
    }
}