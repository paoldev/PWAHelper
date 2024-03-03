namespace PWAHelper
{
    partial class Form1
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
            treeViewAssets = new TreeView();
            buttonFolder = new Button();
            textBoxFolder = new TextBox();
            buttonExportAssets = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            saveFileDialog1 = new SaveFileDialog();
            groupBox1 = new GroupBox();
            label3 = new Label();
            textBoxServiceWorkerAssetsJs = new TextBox();
            label2 = new Label();
            groupBox2 = new GroupBox();
            checkBoxOverwriteIcons = new CheckBox();
            pictureBox1 = new PictureBox();
            buttonExportIcons = new Button();
            textBoxImage = new TextBox();
            buttonLoadImage = new Button();
            openFileDialog1 = new OpenFileDialog();
            groupBox3 = new GroupBox();
            buttonImportManifest = new Button();
            buttonExportManifest = new Button();
            textBoxManifest = new TextBox();
            label1 = new Label();
            propertyGrid1 = new PropertyGrid();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            groupBox4 = new GroupBox();
            fileSystemWatcher1 = new FileSystemWatcher();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox3.SuspendLayout();
            menuStrip1.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            SuspendLayout();
            // 
            // treeViewAssets
            // 
            treeViewAssets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeViewAssets.CheckBoxes = true;
            treeViewAssets.Location = new Point(6, 177);
            treeViewAssets.Name = "treeViewAssets";
            treeViewAssets.Size = new Size(996, 661);
            treeViewAssets.TabIndex = 0;
            treeViewAssets.AfterCheck += OntreeViewAssets_AfterCheck;
            // 
            // buttonFolder
            // 
            buttonFolder.Location = new Point(9, 28);
            buttonFolder.Name = "buttonFolder";
            buttonFolder.Size = new Size(172, 34);
            buttonFolder.TabIndex = 1;
            buttonFolder.Text = "Select folder";
            buttonFolder.UseVisualStyleBackColor = true;
            buttonFolder.Click += OnbuttonFolder_Click;
            // 
            // textBoxFolder
            // 
            textBoxFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBoxFolder.Location = new Point(187, 30);
            textBoxFolder.Name = "textBoxFolder";
            textBoxFolder.Size = new Size(823, 31);
            textBoxFolder.TabIndex = 2;
            // 
            // buttonExportAssets
            // 
            buttonExportAssets.Location = new Point(6, 82);
            buttonExportAssets.Name = "buttonExportAssets";
            buttonExportAssets.Size = new Size(271, 32);
            buttonExportAssets.TabIndex = 3;
            buttonExportAssets.Text = "Generate service-worker-assets.js";
            buttonExportAssets.UseVisualStyleBackColor = true;
            buttonExportAssets.Click += OnbuttonExportAssets_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(textBoxServiceWorkerAssetsJs);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(treeViewAssets);
            groupBox1.Controls.Add(buttonExportAssets);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1008, 858);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 133);
            label3.Name = "label3";
            label3.Size = new Size(869, 25);
            label3.TabIndex = 6;
            label3.Text = "Select files for offline caching. Usually service-worker.js and service-worker-assets.js don't need to be cached.";
            // 
            // textBoxServiceWorkerAssetsJs
            // 
            textBoxServiceWorkerAssetsJs.Location = new Point(83, 30);
            textBoxServiceWorkerAssetsJs.Name = "textBoxServiceWorkerAssetsJs";
            textBoxServiceWorkerAssetsJs.PlaceholderText = "Usually service-worker-assets.js";
            textBoxServiceWorkerAssetsJs.Size = new Size(919, 31);
            textBoxServiceWorkerAssetsJs.TabIndex = 5;
            textBoxServiceWorkerAssetsJs.Text = "service-worker-assets.js";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 33);
            label2.Name = "label2";
            label2.Size = new Size(52, 25);
            label2.TabIndex = 4;
            label2.Text = "js file";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(checkBoxOverwriteIcons);
            groupBox2.Controls.Add(pictureBox1);
            groupBox2.Controls.Add(buttonExportIcons);
            groupBox2.Controls.Add(textBoxImage);
            groupBox2.Controls.Add(buttonLoadImage);
            groupBox2.Dock = DockStyle.Fill;
            groupBox2.Location = new Point(3, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1002, 852);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            // 
            // checkBoxOverwriteIcons
            // 
            checkBoxOverwriteIcons.AutoSize = true;
            checkBoxOverwriteIcons.Location = new Point(687, 272);
            checkBoxOverwriteIcons.Name = "checkBoxOverwriteIcons";
            checkBoxOverwriteIcons.Size = new Size(160, 29);
            checkBoxOverwriteIcons.TabIndex = 4;
            checkBoxOverwriteIcons.Text = "Overwrite icons";
            checkBoxOverwriteIcons.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.BackgroundImageLayout = ImageLayout.None;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(9, 70);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(468, 461);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // buttonExportIcons
            // 
            buttonExportIcons.Location = new Point(533, 268);
            buttonExportIcons.Name = "buttonExportIcons";
            buttonExportIcons.Size = new Size(148, 34);
            buttonExportIcons.TabIndex = 2;
            buttonExportIcons.Text = "Export icons";
            buttonExportIcons.UseVisualStyleBackColor = true;
            buttonExportIcons.Click += OnbuttonExportIcons_Click;
            // 
            // textBoxImage
            // 
            textBoxImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxImage.Location = new Point(160, 32);
            textBoxImage.Name = "textBoxImage";
            textBoxImage.Size = new Size(839, 31);
            textBoxImage.TabIndex = 1;
            // 
            // buttonLoadImage
            // 
            buttonLoadImage.Location = new Point(9, 30);
            buttonLoadImage.Name = "buttonLoadImage";
            buttonLoadImage.Size = new Size(148, 34);
            buttonLoadImage.TabIndex = 0;
            buttonLoadImage.Text = "Load image";
            buttonLoadImage.UseVisualStyleBackColor = true;
            buttonLoadImage.Click += OnbuttonLoadImage_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(buttonImportManifest);
            groupBox3.Controls.Add(buttonExportManifest);
            groupBox3.Controls.Add(textBoxManifest);
            groupBox3.Controls.Add(label1);
            groupBox3.Controls.Add(propertyGrid1);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(3, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(1002, 852);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            // 
            // buttonImportManifest
            // 
            buttonImportManifest.Location = new Point(9, 83);
            buttonImportManifest.Name = "buttonImportManifest";
            buttonImportManifest.Size = new Size(199, 34);
            buttonImportManifest.TabIndex = 5;
            buttonImportManifest.Text = "Import Manifest";
            buttonImportManifest.UseVisualStyleBackColor = true;
            buttonImportManifest.Click += OnbuttonImportManifest_Click;
            // 
            // buttonExportManifest
            // 
            buttonExportManifest.Location = new Point(797, 83);
            buttonExportManifest.Name = "buttonExportManifest";
            buttonExportManifest.Size = new Size(199, 34);
            buttonExportManifest.TabIndex = 3;
            buttonExportManifest.Text = "Export Manifest";
            buttonExportManifest.UseVisualStyleBackColor = true;
            buttonExportManifest.Click += OnbuttonExportManifest_Click;
            // 
            // textBoxManifest
            // 
            textBoxManifest.Location = new Point(120, 31);
            textBoxManifest.Name = "textBoxManifest";
            textBoxManifest.PlaceholderText = "Usually manifest.webmanifest or manifest.json";
            textBoxManifest.Size = new Size(876, 31);
            textBoxManifest.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 34);
            label1.Name = "label1";
            label1.Size = new Size(108, 25);
            label1.TabIndex = 1;
            label1.Text = "manifest file";
            // 
            // propertyGrid1
            // 
            propertyGrid1.AllowDrop = true;
            propertyGrid1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            propertyGrid1.Location = new Point(9, 149);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.PropertySort = PropertySort.NoSort;
            propertyGrid1.Size = new Size(987, 697);
            propertyGrid1.TabIndex = 0;
            propertyGrid1.ToolbarVisible = false;
            propertyGrid1.DragDrop += OnpropertyGrid1_DragDrop;
            propertyGrid1.DragEnter += OnpropertyGrid1_DragEnter;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1037, 33);
            menuStrip1.TabIndex = 7;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, toolStripSeparator1, aboutToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(164, 34);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += OnnewToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(161, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(164, 34);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += OnaboutToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(161, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(164, 34);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += OnexitToolStripMenuItem_Click;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(buttonFolder);
            groupBox4.Controls.Add(textBoxFolder);
            groupBox4.Location = new Point(9, 36);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(1016, 83);
            groupBox4.TabIndex = 8;
            groupBox4.TabStop = false;
            groupBox4.Text = "PWA working directory";
            // 
            // fileSystemWatcher1
            // 
            fileSystemWatcher1.EnableRaisingEvents = true;
            fileSystemWatcher1.IncludeSubdirectories = true;
            fileSystemWatcher1.SynchronizingObject = this;
            fileSystemWatcher1.Created += OnfileSystemWatcher1_Created;
            fileSystemWatcher1.Deleted += OnfileSystemWatcher1_Deleted;
            fileSystemWatcher1.Renamed += OnfileSystemWatcher1_Renamed;
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(9, 125);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1016, 896);
            tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox3);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1008, 858);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "manifest.webmanifest";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1008, 858);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "icons";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(groupBox1);
            tabPage3.Location = new Point(4, 34);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1008, 858);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "service-worker-assets.js";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1037, 1033);
            Controls.Add(tabControl1);
            Controls.Add(groupBox4);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "PWA Helper";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)fileSystemWatcher1).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView treeViewAssets;
        private Button buttonFolder;
        private TextBox textBoxFolder;
        private Button buttonExportAssets;
        private FolderBrowserDialog folderBrowserDialog1;
        private SaveFileDialog saveFileDialog1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button buttonExportIcons;
        private TextBox textBoxImage;
        private Button buttonLoadImage;
        private PictureBox pictureBox1;
        private OpenFileDialog openFileDialog1;
        private CheckBox checkBoxOverwriteIcons;
        private GroupBox groupBox3;
        private PropertyGrid propertyGrid1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private GroupBox groupBox4;
        private FileSystemWatcher fileSystemWatcher1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TextBox textBoxManifest;
        private Label label1;
        private Button buttonExportManifest;
        private Button buttonImportManifest;
        private TextBox textBoxServiceWorkerAssetsJs;
        private Label label2;
        private Label label3;
    }
}
