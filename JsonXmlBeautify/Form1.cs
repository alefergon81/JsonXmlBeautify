using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace JsonXmlBeautify
{
    public partial class Form1 : Form
    {
        // Immersive dark mode title bar (Windows 10 2004+ / Windows 11).
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        // Toggle redraw while applying many SelectionColor runs to avoid flicker.
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int WM_SETREDRAW = 0x000B;

        public Form1()
        {
            InitializeComponent();
            ApplyTheme();
            WireEvents();
            LoadAppIcon();
        }

        // Title-bar / taskbar icon: load the multi-resolution app.ico that is
        // embedded in the assembly (also set as the exe's ApplicationIcon).
        private void LoadAppIcon()
        {
            using System.IO.Stream? stream =
                GetType().Assembly.GetManifestResourceStream("JsonXmlBeautify.app.ico");
            if (stream is not null)
                Icon = new Icon(stream);
        }

        private FormatKind CurrentFormat => toggleFormat.IsXml ? FormatKind.Xml : FormatKind.Json;

        // ---- Theme ------------------------------------------------------

        private void ApplyTheme()
        {
            BackColor = Theme.Background;
            ForeColor = Theme.Text;

            rootLayout.BackColor = Theme.Background;
            centerPanel.BackColor = Theme.Background;
            toggleFormat.Font = Theme.UiFont;

            StyleEditor(txtInput);
            StyleEditor(txtOutput);

            StyleButton(btnValidate);
            StyleButton(btnBeautify);
            StyleButton(btnMinify);
            StyleButton(btnConvert);
        }

        private static void StyleEditor(RichTextBox box)
        {
            box.BackColor = Theme.Surface;
            box.ForeColor = Theme.Text;
            box.Font = Theme.EditorFont;
        }

        private void StyleButton(Button button)
        {
            button.BackColor = Theme.Accent;
            button.ForeColor = Theme.OnAccent;
            button.Font = Theme.UiFont;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Theme.AccentHover;
            button.FlatAppearance.MouseDownBackColor = Theme.AccentHover;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            int useDark = 1;
            DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int));
        }

        // ---- Window state persistence -----------------------------------

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            RestoreWindowState();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveWindowState();
            base.OnFormClosing(e);
        }

        private void RestoreWindowState()
        {
            var saved = WindowSettings.Load();
            if (saved is null || saved.Width <= 0 || saved.Height <= 0)
            {
                WindowState = FormWindowState.Maximized; // first run / no saved state
                return;
            }

            var bounds = new Rectangle(saved.X, saved.Y, saved.Width, saved.Height);
            if (!IsVisibleOnAnyScreen(bounds))
            {
                // Saved position is off all current screens: re-center on primary.
                Rectangle wa = Screen.PrimaryScreen!.WorkingArea;
                bounds.X = wa.X + Math.Max(0, (wa.Width - bounds.Width) / 2);
                bounds.Y = wa.Y + Math.Max(0, (wa.Height - bounds.Height) / 2);
            }

            StartPosition = FormStartPosition.Manual;
            Bounds = bounds;
            WindowState = saved.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private static bool IsVisibleOnAnyScreen(Rectangle bounds)
        {
            foreach (Screen screen in Screen.AllScreens)
                if (screen.WorkingArea.IntersectsWith(bounds))
                    return true;
            return false;
        }

        private void SaveWindowState()
        {
            bool maximized = WindowState == FormWindowState.Maximized;
            // Use RestoreBounds for maximized/minimized so we persist the normal size.
            Rectangle bounds = WindowState == FormWindowState.Normal ? Bounds : RestoreBounds;

            new WindowSettings
            {
                X = bounds.X,
                Y = bounds.Y,
                Width = bounds.Width,
                Height = bounds.Height,
                Maximized = maximized
            }.Save();
        }

        // ---- Events -----------------------------------------------------

        private void WireEvents()
        {
            toggleFormat.Toggled += OnFormatToggled;
            btnValidate.Click += (_, _) => ShowResult(FormatService.Validate(txtInput.Text, CurrentFormat));
            btnBeautify.Click += (_, _) => ShowBeautified(FormatService.Beautify(txtInput.Text, CurrentFormat));
            btnMinify.Click += (_, _) => ShowResult(FormatService.Minify(txtInput.Text, CurrentFormat));
            btnConvert.Click += (_, _) => ShowResult(FormatService.Convert(txtInput.Text, CurrentFormat));
        }

        private void OnFormatToggled(object? sender, EventArgs e)
        {
            btnConvert.Text = toggleFormat.IsXml ? "Convert To JSON" : "Convert To XML";
        }

        // Plain output for Validate / Minify / Convert and all error messages.
        // Assigning Text then recoloring the whole document clears any per-token
        // coloring left over from a previous Beautify.
        private void ShowResult(FormatResult result)
        {
            txtOutput.Text = Normalize(result.Output);
            txtOutput.SelectAll();
            txtOutput.SelectionColor = result.Success ? Theme.Text : Theme.Error;
            txtOutput.Select(0, 0);
        }

        // Successful Beautify output is syntax-colored; a parse error falls back
        // to the plain error rendering.
        private void ShowBeautified(FormatResult result)
        {
            if (!result.Success)
            {
                ShowResult(result);
                return;
            }

            string text = Normalize(result.Output);
            List<TokenSpan> spans = SyntaxHighlighter.Highlight(text, CurrentFormat);
            ApplyHighlight(text, spans);
        }

        private void ApplyHighlight(string text, List<TokenSpan> spans)
        {
            SendMessage(txtOutput.Handle, WM_SETREDRAW, false, 0);
            try
            {
                txtOutput.Text = text;
                txtOutput.SelectAll();
                txtOutput.SelectionColor = Theme.Text; // base color for punctuation/whitespace

                foreach (TokenSpan span in spans)
                {
                    txtOutput.Select(span.Start, span.Length);
                    txtOutput.SelectionColor = ColorFor(span.Color);
                }

                txtOutput.Select(0, 0);
            }
            finally
            {
                SendMessage(txtOutput.Handle, WM_SETREDRAW, true, 0);
                txtOutput.Invalidate();
            }
        }

        private static Color ColorFor(TokenColor color) => color switch
        {
            TokenColor.Property => Theme.SyntaxProperty,
            TokenColor.String => Theme.SyntaxString,
            TokenColor.Number => Theme.SyntaxNumber,
            TokenColor.Keyword => Theme.SyntaxKeyword,
            TokenColor.Bracket0 => Theme.BracketColors[0],
            TokenColor.Bracket1 => Theme.BracketColors[1],
            TokenColor.Bracket2 => Theme.BracketColors[2],
            _ => Theme.SyntaxPunctuation
        };

        // RichTextBox counts a line break as one character; normalizing to "\n"
        // keeps tokenizer offsets aligned with RichTextBox selection indices.
        private static string Normalize(string text) =>
            text.Replace("\r\n", "\n").Replace("\r", "\n");
    }
}
