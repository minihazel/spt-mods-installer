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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainForm));
            panelTitle = new Panel();
            dropdownOpen = new ComboBox();
            panelTitleImage = new PictureBox();
            panelTitleText = new Panel();
            panelTitleDetector = new Label();
            panelTitleNotice = new Label();
            panelTitleName = new Label();
            panelSeparator1 = new Panel();
            panelDragDrop = new Panel();
            titleDragDrop = new Label();
            panelTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)panelTitleImage).BeginInit();
            panelTitleText.SuspendLayout();
            panelDragDrop.SuspendLayout();
            SuspendLayout();
            // 
            // panelTitle
            // 
            panelTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelTitle.Controls.Add(dropdownOpen);
            panelTitle.Controls.Add(panelTitleImage);
            panelTitle.Controls.Add(panelTitleText);
            panelTitle.Location = new Point(22, 30);
            panelTitle.Name = "panelTitle";
            panelTitle.Size = new Size(953, 109);
            panelTitle.TabIndex = 0;
            // 
            // dropdownOpen
            // 
            dropdownOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dropdownOpen.DropDownStyle = ComboBoxStyle.DropDownList;
            dropdownOpen.Font = new Font("Bahnschrift Light", 13F);
            dropdownOpen.FormattingEnabled = true;
            dropdownOpen.Location = new Point(650, 79);
            dropdownOpen.Name = "dropdownOpen";
            dropdownOpen.Size = new Size(300, 29);
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
            panelTitleText.Controls.Add(panelTitleDetector);
            panelTitleText.Controls.Add(panelTitleNotice);
            panelTitleText.Controls.Add(panelTitleName);
            panelTitleText.Font = new Font("Bahnschrift Light", 25F);
            panelTitleText.Location = new Point(142, 0);
            panelTitleText.Name = "panelTitleText";
            panelTitleText.Size = new Size(505, 109);
            panelTitleText.TabIndex = 1;
            // 
            // panelTitleDetector
            // 
            panelTitleDetector.Font = new Font("Bahnschrift Light", 10F);
            panelTitleDetector.ForeColor = Color.DodgerBlue;
            panelTitleDetector.Location = new Point(3, 79);
            panelTitleDetector.Name = "panelTitleDetector";
            panelTitleDetector.Padding = new Padding(33, 4, 0, 0);
            panelTitleDetector.Size = new Size(491, 27);
            panelTitleDetector.TabIndex = 5;
            panelTitleDetector.Text = "Could not detect SPT-AKI";
            panelTitleDetector.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelTitleNotice
            // 
            panelTitleNotice.Font = new Font("Bahnschrift Light", 9F);
            panelTitleNotice.ForeColor = Color.FromArgb(50, 50, 50);
            panelTitleNotice.Location = new Point(3, 47);
            panelTitleNotice.Name = "panelTitleNotice";
            panelTitleNotice.Padding = new Padding(33, 0, 0, 0);
            panelTitleNotice.Size = new Size(491, 20);
            panelTitleNotice.TabIndex = 4;
            panelTitleNotice.Text = "For use with SPT-AKI 3.X.X or above";
            // 
            // panelTitleName
            // 
            panelTitleName.Font = new Font("Bender", 25F);
            panelTitleName.Location = new Point(3, 8);
            panelTitleName.Name = "panelTitleName";
            panelTitleName.Padding = new Padding(30, 0, 0, 0);
            panelTitleName.Size = new Size(491, 40);
            panelTitleName.TabIndex = 2;
            panelTitleName.Text = "AKI Mod Installer";
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
            panelDragDrop.Controls.Add(titleDragDrop);
            panelDragDrop.Location = new Point(12, 171);
            panelDragDrop.Name = "panelDragDrop";
            panelDragDrop.Size = new Size(973, 442);
            panelDragDrop.TabIndex = 2;
            panelDragDrop.DragDrop += panelDragDrop_DragDrop;
            panelDragDrop.DragEnter += panelDragDrop_DragEnter;
            // 
            // titleDragDrop
            // 
            titleDragDrop.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            titleDragDrop.Font = new Font("Bahnschrift Light", 10F);
            titleDragDrop.ForeColor = Color.FromArgb(90, 90, 90);
            titleDragDrop.Location = new Point(3, 194);
            titleDragDrop.Name = "titleDragDrop";
            titleDragDrop.Padding = new Padding(33, 0, 0, 0);
            titleDragDrop.Size = new Size(967, 20);
            titleDragDrop.TabIndex = 5;
            titleDragDrop.Text = "📥 Drag and drop any archive";
            titleDragDrop.TextAlign = ContentAlignment.MiddleCenter;
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
            Load += mainForm_Load;
            panelTitle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)panelTitleImage).EndInit();
            panelTitleText.ResumeLayout(false);
            panelDragDrop.ResumeLayout(false);
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
    }
}
