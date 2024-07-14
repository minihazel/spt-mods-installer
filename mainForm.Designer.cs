namespace spt_mods_installer
{
    partial class mainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            panelTitle = new Panel();
            panelTitleDetector = new Label();
            btnThemeSwitch = new Button();
            dropdownOpen = new ComboBox();
            panelTitleImage = new PictureBox();
            panelTitleText = new Panel();
            panelTitleNotice = new Label();
            panelTitleName = new Label();
            panelSeparator1 = new Panel();
            panelDragDrop = new Panel();
            titleHistory = new Label();
            titleDragDrop = new Label();
            timerConfirmation = new System.Windows.Forms.Timer(components);
            panelTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelTitleImage).BeginInit();
            panelTitleText.SuspendLayout();
            panelDragDrop.SuspendLayout();
            SuspendLayout();
            // 
            // panelTitle
            // 
            panelTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelTitle.Controls.Add(panelTitleDetector);
            panelTitle.Controls.Add(btnThemeSwitch);
            panelTitle.Controls.Add(dropdownOpen);
            panelTitle.Controls.Add(panelTitleImage);
            panelTitle.Controls.Add(panelTitleText);
            panelTitle.Location = new Point(22, 30);
            panelTitle.Name = "panelTitle";
            panelTitle.Size = new Size(953, 109);
            panelTitle.TabIndex = 0;
            // 
            // panelTitleDetector
            // 
            panelTitleDetector.Font = new Font("Bahnschrift Light", 10F);
            panelTitleDetector.ForeColor = Color.DodgerBlue;
            panelTitleDetector.Location = new Point(145, 71);
            panelTitleDetector.Name = "panelTitleDetector";
            panelTitleDetector.Padding = new Padding(33, 4, 0, 0);
            panelTitleDetector.Size = new Size(805, 38);
            panelTitleDetector.TabIndex = 5;
            panelTitleDetector.Text = "Could not detect SPT";
            // 
            // btnThemeSwitch
            // 
            btnThemeSwitch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnThemeSwitch.Cursor = Cursors.Hand;
            btnThemeSwitch.Font = new Font("Bahnschrift Light", 14F);
            btnThemeSwitch.Location = new Point(910, 8);
            btnThemeSwitch.Name = "btnThemeSwitch";
            btnThemeSwitch.Size = new Size(34, 34);
            btnThemeSwitch.TabIndex = 7;
            btnThemeSwitch.Text = "○";
            btnThemeSwitch.UseVisualStyleBackColor = true;
            btnThemeSwitch.Click += btnThemeSwitch_Click;
            // 
            // dropdownOpen
            // 
            dropdownOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dropdownOpen.Cursor = Cursors.Hand;
            dropdownOpen.DropDownStyle = ComboBoxStyle.DropDownList;
            dropdownOpen.Font = new Font("Bahnschrift Light", 15F);
            dropdownOpen.FormattingEnabled = true;
            dropdownOpen.Location = new Point(645, 9);
            dropdownOpen.Name = "dropdownOpen";
            dropdownOpen.Size = new Size(250, 32);
            dropdownOpen.TabIndex = 6;
            dropdownOpen.SelectedIndexChanged += dropdownOpen_SelectedIndexChanged;
            // 
            // panelTitleImage
            // 
            panelTitleImage.Image = (Image)resources.GetObject("panelTitleImage.Image");
            panelTitleImage.Location = new Point(30, 3);
            panelTitleImage.Name = "panelTitleImage";
            panelTitleImage.Size = new Size(106, 103);
            panelTitleImage.SizeMode = PictureBoxSizeMode.StretchImage;
            panelTitleImage.TabIndex = 2;
            panelTitleImage.TabStop = false;
            // 
            // panelTitleText
            // 
            panelTitleText.Controls.Add(panelTitleNotice);
            panelTitleText.Controls.Add(panelTitleName);
            panelTitleText.Font = new Font("Bahnschrift Light", 25F);
            panelTitleText.Location = new Point(142, 0);
            panelTitleText.Name = "panelTitleText";
            panelTitleText.Size = new Size(326, 68);
            panelTitleText.TabIndex = 1;
            // 
            // panelTitleNotice
            // 
            panelTitleNotice.Font = new Font("Bahnschrift Light", 9F);
            panelTitleNotice.ForeColor = Color.FromArgb(50, 50, 50);
            panelTitleNotice.Location = new Point(3, 47);
            panelTitleNotice.Name = "panelTitleNotice";
            panelTitleNotice.Padding = new Padding(33, 0, 0, 0);
            panelTitleNotice.Size = new Size(320, 20);
            panelTitleNotice.TabIndex = 4;
            panelTitleNotice.Text = "For use with SPT 3.X.X or above";
            // 
            // panelTitleName
            // 
            panelTitleName.Font = new Font("Bender", 25F);
            panelTitleName.Location = new Point(3, 8);
            panelTitleName.Name = "panelTitleName";
            panelTitleName.Padding = new Padding(30, 0, 0, 0);
            panelTitleName.Size = new Size(320, 40);
            panelTitleName.TabIndex = 2;
            panelTitleName.Text = "SPT Mod Installer";
            panelTitleName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelSeparator1
            // 
            panelSeparator1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelSeparator1.Location = new Point(0, 145);
            panelSeparator1.Name = "panelSeparator1";
            panelSeparator1.Size = new Size(997, 20);
            panelSeparator1.TabIndex = 1;
            panelSeparator1.Paint += panelSeparator1_Paint;
            // 
            // panelDragDrop
            // 
            panelDragDrop.AllowDrop = true;
            panelDragDrop.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelDragDrop.Controls.Add(titleHistory);
            panelDragDrop.Controls.Add(titleDragDrop);
            panelDragDrop.Location = new Point(12, 171);
            panelDragDrop.Name = "panelDragDrop";
            panelDragDrop.Size = new Size(973, 442);
            panelDragDrop.TabIndex = 2;
            panelDragDrop.DragDrop += panelDragDrop_DragDrop;
            panelDragDrop.DragEnter += panelDragDrop_DragEnter;
            // 
            // titleHistory
            // 
            titleHistory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            titleHistory.AutoSize = true;
            titleHistory.Font = new Font("Bahnschrift Light", 10F);
            titleHistory.ForeColor = Color.FromArgb(90, 90, 90);
            titleHistory.Location = new Point(3, 422);
            titleHistory.Name = "titleHistory";
            titleHistory.Size = new Size(175, 17);
            titleHistory.TabIndex = 6;
            titleHistory.Text = "Mod Placeholder installed";
            titleHistory.TextAlign = ContentAlignment.BottomLeft;
            titleHistory.Visible = false;
            // 
            // titleDragDrop
            // 
            titleDragDrop.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            titleDragDrop.Font = new Font("Bahnschrift Light", 10F);
            titleDragDrop.ForeColor = Color.FromArgb(90, 90, 90);
            titleDragDrop.Location = new Point(3, 190);
            titleDragDrop.Name = "titleDragDrop";
            titleDragDrop.Padding = new Padding(33, 0, 0, 0);
            titleDragDrop.Size = new Size(967, 60);
            titleDragDrop.TabIndex = 5;
            titleDragDrop.Text = "📥 Drag and drop any archive\r\n\r\nSupported formats: .rar  /  .zip  /  .7z";
            titleDragDrop.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // timerConfirmation
            // 
            timerConfirmation.Interval = 3000;
            timerConfirmation.Tick += timerConfirmation_Tick;
            // 
            // mainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(997, 625);
            Controls.Add(panelDragDrop);
            Controls.Add(panelSeparator1);
            Controls.Add(panelTitle);
            Font = new Font("Bahnschrift Light", 11F);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1013, 664);
            Name = "mainForm";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += mainForm_FormClosing;
            Load += mainForm_Load;
            panelTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelTitleImage).EndInit();
            panelTitleText.ResumeLayout(false);
            panelDragDrop.ResumeLayout(false);
            panelDragDrop.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTitle;
        private Panel panelTitleText;
        private Panel panelSeparator1;
        private PictureBox panelTitleImage;
        private Label panelTitleName;
        private Label panelTitleNotice;
        private Label panelTitleDetector;
        private Panel panelDragDrop;
        private Label titleDragDrop;
        private ComboBox dropdownOpen;
        private Button btnThemeSwitch;
        private Label titleHistory;
        private System.Windows.Forms.Timer timerConfirmation;
    }
}
