using System;
using System.Collections.Generic;
using System.Text;
using Rhino.Mocks;

namespace RhinoToNSubstitute.Tests.MockRepository
{
    class Input
    {
        public Input()
        {
            var webService = MockRepository.GenerateMock<IWebService>();
            var logger = MockRepository.GenerateStub<ILogger>();

            var channel = MockRepository.GenerateStub<IClientChannel>();
            channel
                .Stub(c => c.Extensions)
                .Return(MockRepository.GenerateStub<IExtensionCollection<IContextChannel>>());
            channel.Extensions
                .Stub(e => e.Find<ChannelExtension<IWebService>>())
                .Return(new ChannelExtension<IWebService>(webService));
            channel.Extensions
                .Stub(e => e.Find<ChannelExtension<Logger>>())
                .Return(new ChannelExtension<Logger>(logger));
        }
    }
}
