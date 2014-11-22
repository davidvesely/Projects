using System;
using System.Windows.Forms;

namespace QuickStichNamespace
{
    public class MyPanel : Panel
    {
        /// <summary>
        /// To prevent moving the scroll bar of the panel,
        /// when scrolling over a image is needed,
        /// the event is overridden
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Do not call nothing, to prevent
            // moving the scroll bars of the panel
        }
    }
}
