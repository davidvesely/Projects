// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Net.Http
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods to allow strongly typed objects to be read from <see cref="HttpContent"/> instances.
    /// </summary>
    public static class HttpContentExtensionMethods
    {
        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object of the specified type
        /// from the <paramref name="content"/> instance.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <param name="type">The type of the object to read.</param>
        /// <returns>A <see cref="Task"/> that will yield an object instance of the specified type.</returns>
        public static Task<object> ReadAsAsync(this HttpContent content, Type type)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return BuildObjectContent(type, content).ReadAsAsync();
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object of the specified type
        /// from the <paramref name="content"/> instance.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <param name="type">The type of the object to read.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances to use.</param>
        /// <returns>A <see cref="Task"/> that will return an object instance of the specified type.</returns>
        public static Task<object> ReadAsAsync(this HttpContent content, Type type, IEnumerable<MediaTypeFormatter> formatters)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return BuildObjectContent(type, content, formatters).ReadAsAsync();
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object or default value
        /// of the specified type from the <paramref name="content"/> instance.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <param name="type">The type of the object to read.</param>
        /// <returns>A <see cref="Task"/> that will yield an object instance of the specified type or the
        /// default value for that type if it was not possible to read the object from the <paramref name="content"/>.
        /// </returns>
        public static Task<object> ReadAsOrDefaultAsync(this HttpContent content, Type type)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return BuildObjectContent(type, content).ReadAsOrDefaultAsync();
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object or default value
        /// of the specified type from the <paramref name="content"/> instance.
        /// </summary>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <param name="type">The type of the object to read.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances to use.</param>
        /// <returns>A <see cref="Task"/> that will yield an object instance of the specified type or the
        /// default value for that type if it was not possible to read the object from the <paramref name="content"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents the output parameter, not an input parameter.")]
        public static Task<object> ReadAsOrDefaultAsync(this HttpContent content, Type type, IEnumerable<MediaTypeFormatter> formatters)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return BuildObjectContent(type, content, formatters).ReadAsOrDefaultAsync();
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object of the specified 
        /// type <typeparamref name="T"/> from the <paramref name="content"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <returns>An object instance of the specified type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents the output parameter, not an input parameter.")]
        public static Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return BuildObjectContent<T>(content).ReadAsAsync();
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object of the specified 
        /// type <typeparamref name="T"/> from the <paramref name="content"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances to use.</param>
        /// <returns>An object instance of the specified type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents the output parameter, not an input parameter.")]
        public static Task<T> ReadAsAsync<T>(this HttpContent content, IEnumerable<MediaTypeFormatter> formatters)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return BuildObjectContent<T>(content, formatters).ReadAsAsync();
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object or default value
        /// of the specified type <typeparamref name="T"/> 
        /// from the <paramref name="content"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <returns>A <see cref="Task"/> that will yield object instance of the specified type or
        /// the default value of that type if it was not possible to read from the <paramref name="content"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents the output parameter, not an input parameter.")]
        public static Task<T> ReadAsOrDefaultAsync<T>(this HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return BuildObjectContent<T>(content).ReadAsOrDefaultAsync();
        }

        /// <summary>
        /// Returns a <see cref="Task"/> that will yield an object or default value
        /// of the specified type <typeparamref name="T"/> 
        /// from the <paramref name="content"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="content">The <see cref="HttpContent"/> instance from which to read.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances to use.</param>
        /// <returns>A <see cref="Task"/> that will yield object instance of the specified type or
        /// the default value of that type if it was not possible to read from the <paramref name="content"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The T represents the output parameter, not an input parameter.")]
        public static Task<T> ReadAsOrDefaultAsync<T>(this HttpContent content, IEnumerable<MediaTypeFormatter> formatters)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            return BuildObjectContent<T>(content, formatters).ReadAsOrDefaultAsync();
        }

        private static ObjectContent BuildObjectContent(Type type, HttpContent content)
        {
            ObjectContent objectContent = content as ObjectContent;
            if (objectContent == null)
            {
                objectContent = new ObjectContent(type, content);
            }

            return objectContent;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        private static ObjectContent BuildObjectContent(Type type, HttpContent content, IEnumerable<MediaTypeFormatter> formatters)
        {
            ObjectContent objectContent = content as ObjectContent;
            if (objectContent == null)
            {
                objectContent = new ObjectContent(type, content);
            }

            // the HttpContent is already an object content, no need to wrap it again
            // however, we need to use the formatters collection passed in via the extension method
            objectContent.SetFormatters(formatters);

            return objectContent;
        }

        private static ObjectContent<T> BuildObjectContent<T>(HttpContent content)
        {
            ObjectContent<T> objectContent = content as ObjectContent<T>;
            if (objectContent == null)
            {
                objectContent = new ObjectContent<T>(content);
            }

            return objectContent;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "caller becomes owner.")]
        private static ObjectContent<T> BuildObjectContent<T>(HttpContent content, IEnumerable<MediaTypeFormatter> formatters)
        {
            ObjectContent<T> objectContent = content as ObjectContent<T>;
            if (objectContent == null)
            {
                objectContent = new ObjectContent<T>(content);
            }

            // the HttpContent is already an object content, no need to wrap it again
            // however, we need to use the formatters collection passed in via the extension method
            objectContent.SetFormatters(formatters);

            return objectContent;
        }
    }
}
