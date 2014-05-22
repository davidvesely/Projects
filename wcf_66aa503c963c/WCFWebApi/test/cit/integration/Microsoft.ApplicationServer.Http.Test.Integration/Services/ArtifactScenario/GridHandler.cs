// <copyright>
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.ApplicationServer.Http
{
    using Microsoft.ApplicationServer.Http.Dispatcher;

    // Converts X,Y coordinates into a grid position
    public class GridHandler : HttpOperationHandler<int, int, GridPosition>
    {
        public GridHandler() : base("gridPosition") { }

        protected override GridPosition OnHandle(int gridX, int gridY)
        {
            return new GridPosition(gridX, gridY);
        }
    }
}
