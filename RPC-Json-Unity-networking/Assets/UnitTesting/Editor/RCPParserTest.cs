using UnityEngine;
using System.Collections;
using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using System;

namespace Unity_RPC.tests{
    [TestFixture]
    public class RCPParserTest {
        
        // The manager to test
        RPCParser parser;

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
            // Init a basic handler before each test
            mockHandler = createHandler();

            // Init a new manager for each test
            parser = new RPCParser(mockHandler);


        }

        [Test]
        public void testRequestMessage()
        {
            // the json RPC formated string to test, taken from http://www.jsonrpc.org/specification examples
            string jsonRequest = "{\"jsonrpc\": \"2.0\", \"method\": \"subtract\", \"params\": [42, 23], \"id\": \"1\"}";

            mockHandler.handleRequest("subtract",Arg.Any<Dictionary<string,object>>()).Returns(2);

            object _result = "";
            string _actionId = "";
            parser.HandleMessage(jsonRequest,(result, actionId) => 
                {
                    _result = result;
                    _actionId = actionId;
                },null
            );
            int res = Int32.Parse(_result.ToString());
            Assert.AreEqual(2,res);
            Assert.AreEqual("1",_actionId);
        }
    }
}