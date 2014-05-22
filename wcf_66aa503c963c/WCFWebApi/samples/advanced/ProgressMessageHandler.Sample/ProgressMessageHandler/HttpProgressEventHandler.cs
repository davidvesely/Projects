// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace ProgressMessageHandler.Sample
{

    /// <summary>
    /// Occurs when data is being exchanged as part of sending an <see cref="T:System.Net.Http.HttpRequestMessage"/> or 
    /// receiving an <see cref="T:System.Net.Http.HttpResponseMessage"/>.
    /// </summary>
    /// <param name="sender">The <see cref="T:System.Net.Http.HttpRequestMessage"/> for which the progress data applies.</param>
    /// <param name="e">The <see cref="HttpProgressEventArgs"/> instance containing the event data.</param>
    public delegate void HttpProgressEventHandler(object sender, HttpProgressEventArgs e);
}
