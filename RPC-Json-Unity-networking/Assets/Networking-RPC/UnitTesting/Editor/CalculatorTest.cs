using System;
using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using UnityEngine;

namespace Unity_RPC.tests
{
    // TODO add test in case of exception
    [TestFixture]
    public class CalculatorTest
    {/*
        //The calculator to test
        Calculator calc;

        // mock server
        IServer mockServer;

        [SetUp]
        public void init()
        {
            // create the objects we need before each test
            mockServer = Substitute.For<IServer>();
            calc = new Calculator(mockServer);
        }

        [Test]
        public void testSubstract()
        {            
            Dictionary<string,object> param = new Dictionary<string, object>()
            {
                { "x", 3 },
                { "y", 2 },
                { "connectionId", 123 }
            };

            calc.handleRequest("substract","resultSubstract",param);

            // the server should receive a call to send the response to the client
            mockServer.Received().sendResponse(123,"resultSubstract",1);
        }*/
    }
}

