using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class FigureControl : Control
{
    private readonly Pen pen;
    private readonly List<Point> figureCoordinates;

    public FigureControl(List<Point> coordinates)
    {
        pen = new Pen(Color.Black, 2); // Specify the pen color and thickness

        // Assign the figure coordinates
        figureCoordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;

        // Draw lines connecting the figure coordinates
        for (int i = 0; i < figureCoordinates.Count - 1; i++)
        {
            g.DrawLine(pen, figureCoordinates[i], figureCoordinates[i + 1]);
        }

        // Connect the last coordinate with the first coordinate to complete the figure
        g.DrawLine(pen, figureCoordinates[figureCoordinates.Count - 1], figureCoordinates[0]);
    }
}
