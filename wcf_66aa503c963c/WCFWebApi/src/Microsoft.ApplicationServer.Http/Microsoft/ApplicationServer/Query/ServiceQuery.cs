// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Query
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.ApplicationServer.Http;

    /// <summary>
    /// Represents an <see cref="System.Linq.IQueryable"/>.
    /// </summary>
    internal class ServiceQuery
    {
        /// <summary>
        /// Gets or sets a list of query parts.
        /// </summary>
        public IEnumerable<ServiceQueryPart> QueryParts
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents a single query operator to be applied to a query
    /// </summary>
    internal class ServiceQueryPart
    {
        private string _queryOperator;
        private string _expression;

        /// <summary>
        /// Public constructor
        /// </summary>
        public ServiceQueryPart()
        {
        }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="queryOperator">The query operator</param>
        /// <param name="expression">The query expression</param>
        public ServiceQueryPart(string queryOperator, string expression)
        {
            if (queryOperator == null)
            {
                throw new ArgumentNullException("queryOperator");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (queryOperator != "filter" && queryOperator != "orderby" &&
               queryOperator != "skip" && queryOperator != "top")
            {
                throw new ArgumentException(SR.InvalidQueryOperator(queryOperator), "queryOperator");
            }

            this._queryOperator = queryOperator;
            this._expression = expression;
        }

        /// <summary>
        /// Gets or sets the query operator. Must be one of the supported operators : "where", "orderby", "skip", or "take".
        /// </summary>
        public string QueryOperator
        {
            get
            {
                return this._queryOperator;
            }
            set
            {
                this._queryOperator = value;
            }
        }

        /// <summary>
        /// Gets or sets the query expression.
        /// </summary>
        public string Expression
        {
            get
            {
                return this._expression;
            }
            set
            {
                this._expression = value;
            }
        }

        /// <summary>
        /// Returns a string representation of this <see cref="ServiceQueryPart"/>
        /// </summary>
        /// <returns>The string representation of this <see cref="ServiceQueryPart"/></returns>
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}={1}", this.QueryOperator, this.Expression);
        }
    }
}
