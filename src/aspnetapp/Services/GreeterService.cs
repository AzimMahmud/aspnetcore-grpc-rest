﻿using System.Threading.Tasks;
using Greet.V1;
using Grpc.Core;

namespace aspnetapp.Services
{
    /// <summary>
    /// Greeting Service
    /// </summary>
    public class GreeterService : Greeter.GreeterBase, IGreeterService
    {
        /// <summary>
        /// A method that takes in a request name and responds with a hello message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new HelloReply
            {
                Message = $"Hello {request.Name}"
            });
        }
    }
}
