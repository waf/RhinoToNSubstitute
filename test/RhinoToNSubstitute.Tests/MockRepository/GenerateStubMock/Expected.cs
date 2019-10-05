using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;

namespace RhinoToNSubstitute.Tests.MockRepository
{
    class Input
    {
        public Input()
        {
            var webService = Substitute.For<IWebService>();
            var logger = Substitute.For<ILogger>();

            var channel = Substitute.For<IClientChannel>();
            channel
                .Extensions
                .Returns(Substitute.For<IExtensionCollection<IContextChannel>>());
            channel.Extensions
                .Find<ChannelExtension<IWebService>>()
                .Returns(new ChannelExtension<IWebService>(webService));
            channel.Extensions
                .Find<ChannelExtension<Logger>>()
                .Returns(new ChannelExtension<Logger>(logger));
        }
    }
}
