using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace JsonXmlBeautify
{
    /// <summary>
    /// A modern sliding switch: a rounded track with a knob that animates from
    /// the left side (JSON) to the right side (XML). The active label is
    /// highlighted. Raises <see cref="Toggled"/> when the selection changes.
    /// </summary>
    internal sealed class ToggleSwitch : Control
    {
        private bool _isXml;        // false = JSON (left), true = XML (right)
        private float _knobPos;     // 0 = fully left, 1 = fully right
        private readonly System.Windows.Forms.Timer _animTimer;

        public event EventHandler? Toggled;

        public ToggleSwitch()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);

            Size = new Size(190, 40);
            Cursor = Cursors.Hand;
            BackColor = Theme.Background;
            ForeColor = Theme.Text;

            _animTimer = new System.Windows.Forms.Timer { Interval = 15 };
            _animTimer.Tick += OnAnimationTick;
        }

        /// <summary>True when XML is selected; false for JSON.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool IsXml
        {
            get => _isXml;
            set
            {
                if (_isXml == value)
                    return;

                _isXml = value;
                _animTimer.Start();
                Toggled?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            const float step = 0.18f;
            float target = _isXml ? 1f : 0f;

            if (Math.Abs(_knobPos - target) <= step)
            {
                _knobPos = target;
                _animTimer.Stop();
            }
            else
            {
                _knobPos += _knobPos < target ? step : -step;
            }

            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            IsXml = !_isXml;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.Clear(BackColor);

            // Layout: [ JSON ]  ( pill switch )  [ XML ] — switch centered,
            // labels hugging it on each side.
            int trackHeight = Math.Min(26, Height - 6);
            int trackWidth = trackHeight * 2 + 14;           // long enough that track shows past the knob
            int trackX = (Width - trackWidth) / 2;
            int trackY = (Height - trackHeight) / 2;
            var track = new Rectangle(trackX, trackY, trackWidth - 1, trackHeight - 1);

            // Track: light gray when JSON (off), accent when XML (on); always
            // outlined so it reads clearly against the dark background.
            Color trackColor = _isXml ? Theme.Accent : Color.FromArgb(96, 96, 104);
            using (GraphicsPath trackPath = RoundedRect(track, track.Height))
            {
                using (var trackBrush = new SolidBrush(trackColor))
                    g.FillPath(trackBrush, trackPath);
                using (var borderPen = new Pen(Theme.Border))
                    g.DrawPath(borderPen, trackPath);
            }

            // Circular white knob that slides between the two ends, sized to
            // leave a visible ring of track around it.
            int margin = 4;
            int knobDiameter = track.Height - 2 * margin;
            int minX = track.X + margin;
            int maxX = track.Right - margin - knobDiameter;
            int knobX = minX + (int)Math.Round((maxX - minX) * _knobPos);
            int knobY = track.Y + margin;
            using (var knobBrush = new SolidBrush(Color.White))
            {
                g.FillEllipse(knobBrush, knobX, knobY, knobDiameter, knobDiameter);
            }

            // Labels flank the switch; the active side is emphasized.
            var leftRect = new Rectangle(0, 0, track.X - 6, Height);
            var rightRect = new Rectangle(track.Right + 6, 0, Width - track.Right - 6, Height);
            using var leftFormat = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center
            };
            using var rightFormat = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Center
            };
            using var activeBrush = new SolidBrush(Theme.Text);
            using var inactiveBrush = new SolidBrush(Theme.SubtleText);

            g.DrawString("JSON", Font, _isXml ? inactiveBrush : activeBrush, leftRect, leftFormat);
            g.DrawString("XML", Font, _isXml ? activeBrush : inactiveBrush, rightRect, rightFormat);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _animTimer.Dispose();
            base.Dispose(disposing);
        }

        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = Math.Max(1, Math.Min(radius, Math.Min(r.Width, r.Height)));
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);                 // top-left
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);         // top-right
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);  // bottom-right
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);         // bottom-left
            path.CloseFigure();
            return path;
        }
    }
}
