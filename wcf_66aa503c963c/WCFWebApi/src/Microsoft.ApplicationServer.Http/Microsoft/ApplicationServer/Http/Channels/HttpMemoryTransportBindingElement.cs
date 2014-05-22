// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using Microsoft.Server.Common;
    using Microsoft.ServiceModel;

    /// <summary>
    /// Provides a <see cref="BindingElement"/> implementation that creates <see cref="HttpMemoryChannelListener"/> instances. 
    /// </summary>
    internal class HttpMemoryTransportBindingElement : TransportBindingElement
    {
        private static readonly Type httpMemoryBindingElementType = typeof(HttpMemoryTransportBindingElement);
        private static readonly Type channelFactoryType = typeof(IChannelFactory);

        private volatile InputQueue<HttpMemoryHandler> handlerQueue = new InputQueue<HttpMemoryHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryTransportBindingElement"/> class. 
        /// </summary>
        public HttpMemoryTransportBindingElement()
        {
            this.ManualAddressing = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMemoryTransportBindingElement"/> class.
        /// </summary>
        /// <param name="elementToBeCloned">The element to be cloned.</param>
        protected HttpMemoryTransportBindingElement(HttpMemoryTransportBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            this.handlerQueue = elementToBeCloned.handlerQueue;
        }

        /// <summary>
        /// Gets the URI scheme for the transport.
        /// </summary>
        public override string Scheme
        {
            get { return Uri.UriSchemeHttp; }
        }

        /// <summary>
        /// Gets a value that indicates whether the hostname is used to reach the service when matching on the URI.
        /// </summary>
        /// <remarks>Only the value <see cref="F:HostNameComparisonMode.StrongWildcard"/> is supported by the <see cref="HttpMemoryTransportBindingElement"/>.</remarks>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This reflects a common pattern in existing TransportBindingElement implementations.")]
        public HostNameComparisonMode HostNameComparisonMode
        {
            get
            {
                return HostNameComparisonMode.StrongWildcard;
            }
        }

        /// <summary>
        /// Gets an implementation of an <see cref="HttpMessageHandler"/> for this <see cref="HttpMemoryChannel"/> instance which can be 
        /// either accessed directly or from an <see cref="HttpClient"/> for in-memory communication.
        /// </summary>
        /// <param name="timeout">Time span before operation times out.</param>
        /// <remarks>This method blocks the calling thread until a <see cref="HttpMemoryHandler"/> is available or a timeout happens in 
        /// which case an <see cref="TimeoutException"/> is thrown.</remarks>
        /// <returns>The <see cref="HttpMemoryHandler"/> instance.</returns>
        public HttpMemoryHandler GetHttpMemoryHandler(TimeSpan timeout)
        {
            return this.handlerQueue.Dequeue(timeout);
        }

        /// <summary>
        /// Returns a value that indicates whether the binding element can build a channel factory for a specific type of channel.
        /// </summary>
        /// <typeparam name="TChannel">The type of channel the channel factory produces.</typeparam>
        /// <param name="context">The <see cref="BindingContext"/> that provides context for the binding element</param>
        /// <returns>Always false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Existing public API")]
        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw Fx.Exception.ArgumentNull("context");
            }

            return false;
        }

        /// <summary>
        /// Initializes a channel factory for producing channels of a specified type from the binding context.
        /// </summary>
        /// <typeparam name="TChannel">The type of channel the factory builds.</typeparam>
        /// <param name="context">The <see cref="BindingContext"/> that provides context for the binding element</param>
        /// <returns>Throws <see cref="NotSupportedException"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Existing public API")]
        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            throw Fx.Exception.AsError(
                new NotSupportedException(
                    Http.SR.ChannelFactoryNotSupported(httpMemoryBindingElementType.Name, channelFactoryType.Name)));
        }

        /// <summary>
        /// Returns a value that indicates whether the binding element can build a listener for a specific type of channel.
        /// </summary>
        /// <typeparam name="TChannel">The type of channel the listener accepts.</typeparam>
        /// <param name="context">The <see cref="BindingContext"/> that provides context for the binding element</param>
        /// <returns>true if the <see cref="IChannelListener&lt;TChannel&gt;"/> of type <see cref="IChannel"/> can be built by the binding element; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Existing public API")]
        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw Fx.Exception.ArgumentNull("context");
            }

            return HttpMemoryTransportBindingElement.IsChannelShapeSupported<TChannel>();
        }

        /// <summary>
        /// Initializes a channel listener to accept channels of a specified type from the binding context.
        /// </summary>
        /// <typeparam name="TChannel">The type of channel the listener is built to accept.</typeparam>
        /// <param name="context">The <see cref="BindingContext"/> that provides context for the binding element</param>
        /// <returns>The <see cref="IChannelListener&lt;TChannel&gt;"/> of type <see cref="IChannel"/> initialized from the context.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Existing public API")]
        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw Fx.Exception.ArgumentNull("context");
            }

            if (!this.CanBuildChannelListener<TChannel>(context))
            {
                throw Fx.Exception.AsError(
                    new NotSupportedException(
                        Http.SR.ChannelShapeNotSupported(httpMemoryBindingElementType.Name, typeof(TChannel).Name, typeof(IReplyChannel).Name)));
            }

            return (IChannelListener<TChannel>)new HttpMemoryChannelListener(this, context, this.handlerQueue);
        }

        /// <summary>
        /// Returns a copy of the binding element object.
        /// </summary>
        /// <returns>A <see cref="BindingElement"/> object that is a deep clone of the original.</returns>
        public override BindingElement Clone()
        {
            return new HttpMemoryTransportBindingElement(this);
        }

        /// <summary>
        /// Returns a typed object requested, if present, from the appropriate layer in the binding stack
        /// </summary>
        /// <typeparam name="T">A typed object for which the method is querying.</typeparam>
        /// <param name="context">The <see cref="BindingContext"/> that provides context for the binding element</param>
        /// <returns>The typed object T requested if it is present or null if it is not present.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Existing public API")]
        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw Fx.Exception.ArgumentNull("context");
            }

            if (typeof(T) == typeof(MessageVersion))
            {
                return (T)(object)MessageVersion.None;
            }

            return base.GetProperty<T>(context);
        }

        private static bool IsChannelShapeSupported<TChannel>()
        {
            return typeof(IReplyChannel).IsAssignableFrom(typeof(TChannel));
        }
    }
}