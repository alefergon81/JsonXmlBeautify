namespace JsonXmlBeautify
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private TableLayoutPanel rootLayout;
        private RichTextBox txtInput;
        private TableLayoutPanel centerPanel;
        private ToggleSwitch toggleFormat;
        private Button btnValidate;
        private Button btnBeautify;
        private Button btnMinify;
        private Button btnConvert;
        private RichTextBox txtOutput;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            rootLayout = new TableLayoutPanel();
            txtInput = new RichTextBox();
            centerPanel = new TableLayoutPanel();
            toggleFormat = new ToggleSwitch();
            btnValidate = new Button();
            btnBeautify = new Button();
            btnMinify = new Button();
            btnConvert = new Button();
            txtOutput = new RichTextBox();

            rootLayout.SuspendLayout();
            centerPanel.SuspendLayout();
            SuspendLayout();

            // rootLayout: left editor | center action column | right editor
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.ColumnCount = 3;
            rootLayout.RowCount = 1;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 210F));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rootLayout.Padding = new Padding(12);
            rootLayout.Controls.Add(txtInput, 0, 0);
            rootLayout.Controls.Add(centerPanel, 1, 0);
            rootLayout.Controls.Add(txtOutput, 2, 0);

            // txtInput: raw/unformatted input (left). RichTextBox wraps long lines
            // (no horizontal scrollbar) and auto-hides the vertical scrollbar.
            txtInput.Dock = DockStyle.Fill;
            txtInput.WordWrap = true;
            txtInput.AcceptsTab = true;
            txtInput.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtInput.BorderStyle = BorderStyle.FixedSingle;
            txtInput.Margin = new Padding(0, 0, 8, 0);

            // centerPanel: vertically centered toggle + action buttons
            centerPanel.Dock = DockStyle.Fill;
            centerPanel.ColumnCount = 1;
            centerPanel.RowCount = 7;
            centerPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));   // top spacer
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // toggle
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // validate
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // beautify
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // minify
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));  // convert
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));   // bottom spacer
            centerPanel.Controls.Add(toggleFormat, 0, 1);
            centerPanel.Controls.Add(btnValidate, 0, 2);
            centerPanel.Controls.Add(btnBeautify, 0, 3);
            centerPanel.Controls.Add(btnMinify, 0, 4);
            centerPanel.Controls.Add(btnConvert, 0, 5);

            // toggleFormat: JSON (default) / XML sliding switch
            toggleFormat.Anchor = AnchorStyles.None;
            toggleFormat.Size = new Size(190, 40);
            toggleFormat.Margin = new Padding(0, 6, 0, 10);

            // Action buttons
            ConfigureButton(btnValidate, "Validate");
            ConfigureButton(btnBeautify, "Beautify");
            ConfigureButton(btnMinify, "Minify");
            ConfigureButton(btnConvert, "Convert To XML"); // JSON is the default mode

            // txtOutput: result (right), read-only but selectable/copyable.
            // RichTextBox enables per-token coloring, line wrapping, and an
            // auto-hiding vertical scrollbar.
            txtOutput.Dock = DockStyle.Fill;
            txtOutput.WordWrap = true;
            txtOutput.ReadOnly = true;
            txtOutput.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtOutput.BorderStyle = BorderStyle.FixedSingle;
            txtOutput.Margin = new Padding(8, 0, 0, 0);

            // Form1
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 560);
            MinimumSize = new Size(640, 400);
            // Window size/position/state is restored at runtime (see Form1.cs);
            // it falls back to maximized when there is no saved state.
            Text = "JSON / XML Beautify";
            Controls.Add(rootLayout);

            rootLayout.ResumeLayout(false);
            rootLayout.PerformLayout();
            centerPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private static void ConfigureButton(Button button, string text)
        {
            button.Text = text;
            button.Anchor = AnchorStyles.None;
            button.Size = new Size(170, 40);
            button.Margin = new Padding(0, 5, 0, 5);
            button.FlatStyle = FlatStyle.Flat;
            button.UseVisualStyleBackColor = false;
        }

        #endregion
    }
}
