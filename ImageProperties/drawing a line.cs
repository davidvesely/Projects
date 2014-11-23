private void zoomPicBox1_MouseDown(object sender, MouseEventArgs e)
{
	//if (img != null)
	//{
	//    switch (e.Button)
	//    {
	//        case MouseButtons.Middle: // Pan mode
	//            zoomPicBox1.SetPanMode(true, e);
	//            break;
	//        case MouseButtons.Left: // Draw Line mode
	//            x0 = (int)Math.Round((e.X - zoomPicBox1.AutoScrollPosition.X) / zoomFactor[zoomIndex]);
	//            y0 = (int)Math.Round((e.Y - zoomPicBox1.AutoScrollPosition.Y) / zoomFactor[zoomIndex]);
	//            drag = true;
	//            break;
	//        default:
	//            break;
	//    }
	//}
	//zoomPicBox1.SetPanMode(true, e);
}

private void zoomPicBox1_MouseUp(object sender, MouseEventArgs e)
{
	//if (drag && (img != null))
	//{
	//    cx = x - x0;
	//    cy = y - y0;
	//    Pen pen = new Pen(Color.Blue);
	//    gB.DrawLine(pen, x0, y0, x, y);
	//    zoomPicBox1.Image = img;
	//    drag = false;
	//    pen.Dispose();
	//}
	//zoomPicBox1.SetPanMode(false);
}

private void zoomPicBox1_Paint(object sender, PaintEventArgs e)
{
	if (drag)
	{
		Graphics g = e.Graphics;
		Pen pen = new Pen(Color.Blue);
		g.DrawLine(pen, x0, y0, x, y);
		pen.Dispose();
	}
}

private void zoomPicBox1_MouseMove(object sender, MouseEventArgs e)
{
	//if (drag)
	//{
	//    x = (int)Math.Round((e.X - zoomPicBox1.AutoScrollPosition.X) / zoomFactor[zoomIndex]);
	//    y = (int)Math.Round((e.Y - zoomPicBox1.AutoScrollPosition.Y) / zoomFactor[zoomIndex]);
	//    cx = x - x0;
	//    cy = y - y0;
	//    zoomPicBox1.Invalidate();
	//}
}