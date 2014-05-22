// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Net.Http;
    using System.Reflection;
    using Microsoft.Server.Common;

    /// <summary>
    /// Default HTTP message handler factory 
    /// for instantiating a set of HTTP message handler types using their default constructors.
    /// </summary>
    public class HttpMessageHandlerFactory
    {
        private static readonly Type delegatingHandlerType = typeof(DelegatingHandler);

        private ConstructorInfo[] handlerCtors;
        private Func<IEnumerable<DelegatingHandler>> handlerFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageHandlerFactory"/> class given
        /// a set of HTTP message handler types to instantiate using their default constructors.
        /// </summary>
        /// <param name="handlers">An ordered list of HTTP message handler types to be invoked as part of an 
        /// <see cref="HttpMessageHandlerBindingElement"/> instance.
        /// HTTP message handler types must derive from <see cref="DelegatingHandler"/> and have a public constructor
        /// taking exactly one argument of type <see cref="HttpMessageHandler"/>. The handlers are invoked in a 
        /// bottom-up fashion in the incoming path and top-down in the outgoing path.
        /// That is, the last entry is called first 
        /// for an incoming request message but invoked last for an outgoing response message.</param>
        public HttpMessageHandlerFactory(params Type[] handlers)
        {
            if (handlers == null)
            {
                throw Fx.Exception.ArgumentNull("handlers");
            }

            if (handlers.Length == 0)
            {
                throw Fx.Exception.Argument("handlers", Http.SR.InputTypeListEmptyError);
            }

            this.handlerCtors = new ConstructorInfo[handlers.Length];
            for (int index = 0; index < handlers.Length; index++)
            {
                Type handler = handlers[index];
                if (handler == null)
                {
                    throw Fx.Exception.Argument(
                        string.Format(CultureInfo.InvariantCulture, "handlers[{0}]", index),
                        Http.SR.HttpMessageHandlerTypeNotSupported("null", delegatingHandlerType.Name));
                }

                if (!delegatingHandlerType.IsAssignableFrom(handler) || handler.IsAbstract)
                {
                    throw Fx.Exception.Argument(
                        string.Format(CultureInfo.InvariantCulture, "handlers[{0}]", index),
                        Http.SR.HttpMessageHandlerTypeNotSupported(handler.Name, delegatingHandlerType.Name));
                }

                ConstructorInfo ctorInfo = handler.GetConstructor(Type.EmptyTypes);
                if (ctorInfo == null)
                {
                    throw Fx.Exception.Argument(
                        string.Format(CultureInfo.InvariantCulture, "handlers[{0}]", index),
                        Http.SR.HttpMessageHandlerTypeNotSupported(handler.Name, delegatingHandlerType.Name));
                }

                this.handlerCtors[index] = ctorInfo;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageHandlerFactory"/> class given
        /// a <see cref="Func{T}"/> used to create a set of <see cref="DelegatingHandler"/> instances.
        /// </summary>
        /// <param name="handlers">A function to generate an ordered list of <see cref="DelegatingHandler"/> instances 
        /// to be invoked as part of an <see cref="HttpMessageHandler"/> instance.
        /// The handlers are invoked in a bottom-up fashion in the incoming path and top-down in the outgoing path. That is, 
        /// the last entry is called first for an incoming request message but invoked last for an outgoing response message.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required in order to provide a plug in model instead of having to define a new factory.")]
        public HttpMessageHandlerFactory(Func<IEnumerable<DelegatingHandler>> handlers)
        {
            if (handlers == null)
            {
                throw Fx.Exception.ArgumentNull("handlers");
            }

            this.handlerFunc = handlers;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageHandlerFactory"/> class.
        /// </summary>
        protected HttpMessageHandlerFactory() 
        {
        }

        /// <summary>
        /// Creates an instance of an <see cref="HttpMessageHandler"/> using the HTTP message handlers
        /// provided in the constructor.
        /// </summary>
        /// <param name="innerChannel">The inner channel represents the destination of the HTTP message channel.</param>
        /// <returns>The HTTP message channel.</returns>
        public HttpMessageHandler Create(HttpMessageHandler innerChannel)
        {
            if (innerChannel == null)
            {
                throw Fx.Exception.ArgumentNull("innerChannel");
            }

            return this.OnCreate(innerChannel);
        }

        /// <summary>
        /// Creates an instance of an <see cref="HttpMessageHandler"/> using the HTTP message handlers
        /// provided in the constructor.
        /// </summary>
        /// <param name="innerChannel">The inner channel represents the destination of the HTTP message channel.</param>
        /// <returns>The HTTP message channel.</returns>
        protected virtual HttpMessageHandler OnCreate(HttpMessageHandler innerChannel)
        {
            if (innerChannel == null)
            {
                throw Fx.Exception.ArgumentNull("innerChannel");
            }

            // Get handlers either by constructing types or by calling Func
            IEnumerable<DelegatingHandler> handlerInstances = null;
            try
            {
                if (this.handlerFunc != null)
                {
                    handlerInstances = this.handlerFunc.Invoke();
                    if (handlerInstances != null)
                    {
                        foreach (DelegatingHandler handler in handlerInstances)
                        {
                            if (handler == null)
                            {
                                throw Fx.Exception.Argument(
                                    "handlers",
                                    Http.SR.DelegatingHandlerArrayFromFuncContainsNullItem(
                                        delegatingHandlerType.Name,
                                        GetFuncDetails(this.handlerFunc)));
                            }
                        }
                    }
                }
                else if (this.handlerCtors != null)
                {
                    DelegatingHandler[] instances = new DelegatingHandler[this.handlerCtors.Length];
                    for (int cnt = 0; cnt < this.handlerCtors.Length; cnt++)
                    {
                        instances[cnt] = (DelegatingHandler)this.handlerCtors[cnt].Invoke(Type.EmptyTypes);
                    }

                    handlerInstances = instances;
                }
            }
            catch (TargetInvocationException targetInvocationException)
            {
                throw Fx.Exception.AsError(targetInvocationException);
            }

            // Wire handlers up
            HttpMessageHandler pipeline = innerChannel;
            if (handlerInstances != null)
            {
                foreach (DelegatingHandler handler in handlerInstances)
                {
                    if (handler.InnerHandler != null)
                    {
                        throw Fx.Exception.Argument(
                            "handlers", 
                            Http.SR.DelegatingHandlerArrayHasNonNullInnerHandler(
                                delegatingHandlerType.Name, 
                                "InnerHandler", 
                                handler.GetType().Name));
                    }

                    handler.InnerHandler = pipeline;
                    pipeline = handler;
                }
            }

            return pipeline;
        }

        private static string GetFuncDetails(Func<IEnumerable<DelegatingHandler>> func)
        {
            Fx.Assert(func != null, "Func should not be null.");
            MethodInfo m = func.Method;
            Type t = m.DeclaringType;
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", t.FullName, m.Name);
        }
    }
}
