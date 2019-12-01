using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InsstagramTool
{
    class DiChuyenForm
    {
        Control panel = null;
        Form form = null;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public DiChuyenForm(Form form, Control panel)
        {
            this.form = form;
            this.panel = panel;
            this.panel.MouseMove += new MouseEventHandler(mouseMove);
            this.panel.MouseDown += new MouseEventHandler(mouseDown);
            this.panel.MouseUp += new MouseEventHandler(mouseUp);
        }

        public void mouseMove(object sender, MouseEventArgs args)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                form.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }
        public void mouseDown(object sender, MouseEventArgs args)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = form.Location;
        }
        public void mouseUp(object sender, MouseEventArgs args)
        {
            dragging = false;
        }

    }
}
