namespace XeniaCanaryUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Collections.Generic.List<System.Windows.Forms.Panel> panels = new System.Collections.Generic.List<System.Windows.Forms.Panel>();
        private System.Collections.Generic.Dictionary<string, string> iconMappings = new System.Collections.Generic.Dictionary<string, string>();
        private string[] zarFiles;
        private int minSize = 150;
        private int padding = 10;
        private string mappingsFile = "icon_mappings.txt";
        private string iconsDirectory = "icons";
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.Panel scrollPanel; 
        private DateTime lastClickTime = DateTime.MinValue;
        private System.Windows.Forms.Panel lastClickedPanel = null;
        private System.Windows.Forms.Panel selectedPanel = null;
        string xeniaCanary;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            SuspendLayout();
            AutoScaleMode = AutoScaleMode.None;
            BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            ClientSize = new Size(1280, 720);
            ForeColor = SystemColors.Control;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Xenia Canary UI";
            Load += Form1_Load;
            ResumeLayout(false);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            System.IO.Directory.CreateDirectory(iconsDirectory);
            LoadIconMappings();
            zarFiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "games"), "*.zar", System.IO.SearchOption.AllDirectories);
            xeniaCanary = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "xenia_canary.exe");
            this.Resize += new System.EventHandler(this.Form1_Resize);
            contextMenu = new System.Windows.Forms.ContextMenuStrip();
            var runAppMenuItem = new System.Windows.Forms.ToolStripMenuItem("Run Application");
            runAppMenuItem.Click += RunAppMenuItem_Click;
            var removeIconMenuItem = new System.Windows.Forms.ToolStripMenuItem("Remove Icon");
            removeIconMenuItem.Click += RemoveIconMenuItem_Click;
            contextMenu.Items.Add(runAppMenuItem);
            contextMenu.Items.Add(removeIconMenuItem);
            CreatePanels(); 
            this.MouseDown += Form1_BackgroundClick;
            scrollPanel.MouseDown += Form1_BackgroundClick;
            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                if (WindowState == FormWindowState.Normal)
                {
                    WindowState = FormWindowState.Maximized;
                    FormBorderStyle = FormBorderStyle.None;
                    TopMost = true;
                }
                else
                {
                    WindowState = FormWindowState.Normal;
                    FormBorderStyle = FormBorderStyle.Sizable;
                    TopMost = false;
                }
            }
        }
        private void Form1_BackgroundClick(object sender, MouseEventArgs e)
        {
            if (selectedPanel != null)
            {
                selectedPanel.BackColor = System.Drawing.Color.DimGray;
                selectedPanel = null;
            }
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            CreatePanels();
        }
        private void CreatePanels()
        {
            if (scrollPanel != null)
            {
                this.Controls.Remove(scrollPanel);
                scrollPanel.Dispose();
            }
            panels.Clear();

            int defaultPanelWidth = 200;
            int defaultPanelHeight = 200;

            int cols = Math.Max(1, (this.ClientSize.Width + padding) / (defaultPanelWidth + padding));
            int panelWidth = defaultPanelWidth;
            int panelHeight = defaultPanelHeight;

            int totalContentWidth = cols * panelWidth + (cols - 1) * padding;

            int verticalScrollbarWidth = SystemInformation.VerticalScrollBarWidth;
            int sideMargin = Math.Max(0, (this.ClientSize.Width - totalContentWidth - verticalScrollbarWidth) / 2);

            int rows = (int)Math.Ceiling((double)zarFiles.Length / cols);

            scrollPanel = new System.Windows.Forms.Panel();
            scrollPanel.AutoScroll = true;
            scrollPanel.Size = new System.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height);
            scrollPanel.Location = new System.Drawing.Point(0, 0);

            for (int i = 0; i < zarFiles.Length; i++)
            {
                System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
                panel.BackColor = System.Drawing.Color.DimGray;
                panel.Size = new System.Drawing.Size(panelWidth, panelHeight);
                int x = sideMargin + (i % cols) * (panelWidth + padding);
                int y = (i / cols) * (panelHeight + padding) + padding;
                panel.Location = new System.Drawing.Point(x, y);
                panel.AllowDrop = true;
                panel.Tag = zarFiles[i];
                panel.DragEnter += Panel_DragEnter;
                panel.DragDrop += Panel_DragDrop;
                panel.MouseDown += Panel_MouseDown; 
                panel.MouseEnter += Panel_MouseEnter;
                panel.MouseLeave += Panel_MouseLeave;

                bool hasIcon = false;
                if (iconMappings.TryGetValue(zarFiles[i], out string relativePath))
                {
                    string fullPath = System.IO.Path.Combine(iconsDirectory, relativePath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        SetPanelIcon(panel, fullPath);
                        hasIcon = true;
                    }
                }

                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                label.Text = System.IO.Path.GetFileNameWithoutExtension(zarFiles[i]);
                label.ForeColor = System.Drawing.Color.White;
                label.Font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                label.Dock = System.Windows.Forms.DockStyle.Fill;
                label.AutoSize = false;
                label.BackColor = System.Drawing.Color.Transparent;
                if (hasIcon)
                {
                    label.Visible = false;
                }
                label.MouseDown += Panel_MouseDown;
                panel.Controls.Add(label);
                scrollPanel.Controls.Add(panel);
                panels.Add(panel); 
                label.MouseEnter += Panel_MouseEnter;
                label.MouseLeave += Panel_MouseLeave;
            }
            this.Controls.Add(scrollPanel);
        }

        private void RunAppMenuItem_Click(object sender, EventArgs e)
        {
            var panel = contextMenu.SourceControl as System.Windows.Forms.Panel;
            if (panel != null && panel.Tag != null)
            {
                string zarPath = panel.Tag as string;
                System.Diagnostics.Process.Start(xeniaCanary, $"\"{zarPath}\"");
            }
        }
        private void RemoveIconMenuItem_Click(object sender, EventArgs e)
        {
            var panel = contextMenu.SourceControl as System.Windows.Forms.Panel;
            if (panel != null && panel.Tag != null)
            {
                string zarPath = panel.Tag as string;
                if (iconMappings.ContainsKey(zarPath))
                {
                    string iconPath = System.IO.Path.Combine(iconsDirectory, iconMappings[zarPath]);
                    if (System.IO.File.Exists(iconPath))
                    {
                        System.IO.File.Delete(iconPath);
                    }
                    iconMappings.Remove(zarPath);
                    SaveIconMappings();
                }
                panel.BackgroundImage = null;
                panel.BackColor = System.Drawing.Color.DimGray;
                foreach (System.Windows.Forms.Control control in panel.Controls)
                {
                    if (control is System.Windows.Forms.Label)
                    {
                        control.Visible = true;
                        break;
                    }
                }
                var removeIconMenuItem = contextMenu.Items[1] as System.Windows.Forms.ToolStripMenuItem;
                removeIconMenuItem.Enabled = false;
            }
        }
        private void Panel_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Panel panel = sender as System.Windows.Forms.Panel;
            if (panel == null && sender is System.Windows.Forms.Control control)
            {
                panel = control.Parent as System.Windows.Forms.Panel;
            }

            if (panel != null && panel != selectedPanel)
            {
                panel.BackColor = System.Drawing.Color.DarkGray;
            }
        }

        private void Panel_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Panel panel = sender as System.Windows.Forms.Panel;
            if (panel == null && sender is System.Windows.Forms.Control control)
            {
                panel = control.Parent as System.Windows.Forms.Panel;
            }

            if (panel != null && panel != selectedPanel)
            {
                panel.BackColor = System.Drawing.Color.DimGray;
            }
        }
        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Panel panel = sender as System.Windows.Forms.Panel;
            if (panel == null && sender is System.Windows.Forms.Control control)
            {
                panel = control.Parent as System.Windows.Forms.Panel;
            }

            if (panel != null)
            {
                if (selectedPanel != panel)
                {
                    if (selectedPanel != null)
                    {
                        selectedPanel.BackColor = System.Drawing.Color.DimGray;
                    }
                    selectedPanel = panel;
                    panel.BackColor = System.Drawing.Color.CornflowerBlue;
                }

                if (e.Button == MouseButtons.Right)
                {
                    var removeIconMenuItem = contextMenu.Items[1] as System.Windows.Forms.ToolStripMenuItem;
                    string zarPath = panel.Tag as string;

                    if (iconMappings.ContainsKey(zarPath))
                    {
                        removeIconMenuItem.Enabled = true;
                    }
                    else
                    {
                        removeIconMenuItem.Enabled = false;
                    }

                    contextMenu.Show(panel, e.Location);
                }

                if (e.Button == MouseButtons.Left)
                {
                    DateTime now = DateTime.Now;
                    if (panel == lastClickedPanel && (now - lastClickTime).TotalMilliseconds <= 500)
                    {
                        string zarPath = panel.Tag as string;
                        System.Diagnostics.Process.Start(xeniaCanary, $"\"{zarPath}\"");
                    }

                    lastClickTime = now;
                    lastClickedPanel = panel;
                }
            }
        }
        private void Panel_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
                if (files.Length == 1 && (files[0].EndsWith(".png") || files[0].EndsWith(".jpg") || files[0].EndsWith(".ico")))
                {
                    using (var img = System.Drawing.Image.FromFile(files[0]))
                    {
                        if (img.Width == img.Height)
                        {
                            e.Effect = System.Windows.Forms.DragDropEffects.Copy;
                        }
                    }
                }
            }
        }

        private void Panel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
            if (files.Length != 1) return;
            string sourcePath = files[0];
            var panel = sender as System.Windows.Forms.Panel;
            string zarPath = panel.Tag as string;
            if (!System.IO.File.Exists(sourcePath)) return;
            using (var img = System.Drawing.Image.FromFile(sourcePath))
            {
                if (img.Width != img.Height) return;
            }
            string fileNameSafe = System.IO.Path.GetFileNameWithoutExtension(zarPath);
            fileNameSafe = string.Concat(fileNameSafe.Split(System.IO.Path.GetInvalidFileNameChars()));
            string targetFileName = fileNameSafe + System.IO.Path.GetExtension(sourcePath);
            string targetPath = System.IO.Path.Combine(iconsDirectory, targetFileName);
            System.IO.File.Copy(sourcePath, targetPath, true);
            iconMappings[zarPath] = targetFileName;
            SaveIconMappings();
            SetPanelIcon(panel, targetPath);
            foreach (System.Windows.Forms.Control control in panel.Controls)
            {
                if (control is System.Windows.Forms.Label)
                {
                    control.Visible = false;
                    break;
                }
            }
        }

        private void SetPanelIcon(System.Windows.Forms.Panel panel, string imagePath)
        {
            if (!System.IO.File.Exists(imagePath)) return;
            using (var original = System.Drawing.Image.FromFile(imagePath))
            {
                if (original.Width != original.Height) return;
                System.Drawing.Bitmap resized = new System.Drawing.Bitmap(original, new System.Drawing.Size(panel.Width, panel.Height));
                panel.BackgroundImage = resized;
                panel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            }
        }

        private void SaveIconMappings()
        {
            var lines = iconMappings.Select(kv => $"{kv.Key}|{kv.Value}");
            System.IO.File.WriteAllLines(mappingsFile, lines);
        }

        private void LoadIconMappings()
        {
            iconMappings.Clear();
            if (!System.IO.File.Exists(mappingsFile)) return;
            var lines = System.IO.File.ReadAllLines(mappingsFile);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 2)
                    iconMappings[parts[0]] = parts[1];
            }
        }
    }
}
