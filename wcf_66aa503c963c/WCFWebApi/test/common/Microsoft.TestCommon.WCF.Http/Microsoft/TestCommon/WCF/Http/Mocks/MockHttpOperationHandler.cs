// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.TestCommon.WCF.Http.Mocks
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationServer.Http.Description;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class MockHttpOperationHandler : HttpOperationHandler
    {
        public Func<IEnumerable<HttpParameter>> OnGetInputParametersCallback { get; set; }
        public Func<IEnumerable<HttpParameter>> OnGetOutputParametersCallback { get; set; }
        public Func<object[], object[]> OnHandleCallback { get; set; }

        protected override IEnumerable<HttpParameter> OnGetInputParameters()
        {
            Assert.IsNotNull(this.OnGetInputParametersCallback, "Set OnGetInputParametersCallback first.");
            return this.OnGetInputParametersCallback();
        }

        protected override IEnumerable<HttpParameter> OnGetOutputParameters()
        {
            Assert.IsNotNull(this.OnGetOutputParametersCallback, "Set OnGetOutputParametersCallback first.");
            return this.OnGetOutputParametersCallback();
        }

        protected override object[] OnHandle(object[] input)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input);
        }
    }

    public class MockHttpOperationHandler<T, TOutput> : HttpOperationHandler<T, TOutput>
    {
        public Func<T, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T input)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input);
        }
    }

    public class MockHttpOperationHandler<T1, T2, TOutput> : HttpOperationHandler<T1, T2, TOutput>
    {
        public Func<T1, T2, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, TOutput> : HttpOperationHandler<T1, T2, T3, TOutput>
    {
        public Func<T1, T2, T3, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, TOutput> : HttpOperationHandler<T1, T2, T3, T4, TOutput>
    {
        public Func<T1, T2, T3, T4, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, TOutput> : HttpOperationHandler<T1, T2, T3, T4, T5, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, TOutput> : HttpOperationHandler<T1, T2, T3, T4, T5, T6, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, TOutput> : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, TOutput> : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOutput> : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8, T9 input9)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8, input9);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOutput> : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8, T9 input9, T10 input10)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8, input9, input10);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOutput> 
        : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8, T9 input9, T10 input10, T11 input11)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8, input9, input10, input11);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOutput>
    : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8, 
                                            T9 input9, T10 input10, T11 input11, T12 input12)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8, 
                                         input9, input10, input11, input12);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOutput>
            : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8,
                                            T9 input9, T10 input10, T11 input11, T12 input12, T13 input13)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8,
                                         input9, input10, input11, input12, input13);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOutput>
        : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8,
                                            T9 input9, T10 input10, T11 input11, T12 input12, T13 input13, T14 input14)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8,
                                         input9, input10, input11, input12, input13, input14);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOutput>
        : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8,
                                            T9 input9, T10 input10, T11 input11, T12 input12, T13 input13, T14 input14, T15 input15)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8,
                                         input9, input10, input11, input12, input13, input14, input15);
        }
    }

    public class MockHttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOutput>
        : HttpOperationHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOutput>
    {
        public Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TOutput> OnHandleCallback { get; set; }

        public MockHttpOperationHandler(string name)
            : base(name)
        {
        }

        protected override TOutput OnHandle(T1 input1, T2 input2, T3 input3, T4 input4, T5 input5, T6 input6, T7 input7, T8 input8,
                                            T9 input9, T10 input10, T11 input11, T12 input12, T13 input13, T14 input14, T15 input15, T16 input16)
        {
            Assert.IsNotNull(this.OnHandleCallback, "Set OnHandleCallback first.");
            return this.OnHandleCallback(input1, input2, input3, input4, input5, input6, input7, input8,
                                         input9, input10, input11, input12, input13, input14, input15, input16);
        }
    }

}
