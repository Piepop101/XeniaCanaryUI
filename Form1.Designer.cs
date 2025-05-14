namespace XeniaCanaryUI
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Collections.Generic.List<System.Windows.Forms.Panel> panels = new System.Collections.Generic.List<System.Windows.Forms.Panel>();
        private System.Collections.Generic.Dictionary<string, string> iconMappings = new System.Collections.Generic.Dictionary<string, string>();
        private string[] zarFiles;
        private int padding = 10;
        private string mappingsFile = "icon_mappings.txt";
        private string iconsDirectory = "icons";
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.Panel scrollPanel;
        private DateTime lastClickTime = DateTime.MinValue;
        private System.Windows.Forms.Panel lastClickedPanel = null;
        private System.Windows.Forms.Panel selectedPanel = null;
        string xeniaCanary;
        private int panelSize = 100;
        private bool isFullscreen = false; 
        private string defaultScreenSize = "1280x720";
        Dictionary<string, Size> screenSizeMap = new Dictionary<string, Size> { { "800x800", new Size(800, 800) }, { "1280x720", new Size(1280, 720) }, { "1920x1080", new Size(1920, 1080) }, { "2560x1440", new Size(2560, 1440) }, { "3840x2160", new Size(3840, 2160) }, { "720x1280", new Size(720, 1280) }, { "1080x1920", new Size(1080, 1920) }, { "1440x2560", new Size(1440, 2560) }, { "2160x3840", new Size(2160, 3840) } };
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
            BackColor = Color.FromArgb(50, 50, 50); 
            if (screenSizeMap.ContainsKey(defaultScreenSize)) ClientSize = screenSizeMap[defaultScreenSize];
            else if (defaultScreenSize.Contains("x") && int.TryParse(defaultScreenSize.Split('x')[0], out int w) && int.TryParse(defaultScreenSize.Split('x')[1], out int h)) ClientSize = new Size(w, h);
            else ClientSize = new Size(1280, 720);
            ForeColor = SystemColors.Control;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Xenia Canary UI";
            Load += Form1_Load;

            System.Windows.Forms.Button settingsButton = new System.Windows.Forms.Button();
            settingsButton.Text = "Settings";
            settingsButton.TextAlign = ContentAlignment.MiddleCenter;
            settingsButton.Size = new Size(100, 37);
            settingsButton.Location = new Point(0, -7);
            settingsButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            settingsButton.FlatStyle = FlatStyle.Flat;
            settingsButton.FlatAppearance.BorderSize = 0;
            settingsButton.BackColor = Color.FromArgb(50, 50, 50);
            settingsButton.ForeColor = Color.White;
            settingsButton.UseVisualStyleBackColor = false;
            this.Controls.Add(settingsButton);

            scrollPanel = new System.Windows.Forms.Panel();
            scrollPanel.AutoScroll = true;
            scrollPanel.Location = new Point(0, 30);
            scrollPanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 30);
            scrollPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(scrollPanel);
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            settingsButton.Click += (s, e) => IconSizeForm();
        }
        private void IconSizeForm()
        {
            Form popup = new Form();
            popup.Text = "Settings";
            popup.Size = new Size(550, 350);
            popup.FormBorderStyle = FormBorderStyle.FixedDialog;
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.MaximizeBox = false;
            popup.MinimizeBox = false;
            popup.TopMost = true;

            int marginLeft = 20;
            int labelWidth = 150;
            int inputX = marginLeft + labelWidth + 10;
            int currentY = 20;
            int rowHeight = 40;
            int rowSpacing = 10;

            Label iconSize = new Label();
            iconSize.Text = "Icon Size:";
            iconSize.TextAlign = ContentAlignment.MiddleLeft;
            iconSize.Size = new Size(labelWidth, rowHeight);
            iconSize.Location = new Point(marginLeft, currentY);

            TextBox sizeInput = new TextBox();
            sizeInput.Text = panelSize.ToString();
            sizeInput.Size = new Size(80, 20);
            sizeInput.Location = new Point(inputX, currentY + 10);

            currentY += rowHeight + rowSpacing;

            Label screenSizeLabel = new Label();
            screenSizeLabel.Text = "Window Size:";
            screenSizeLabel.TextAlign = ContentAlignment.MiddleLeft;
            screenSizeLabel.Size = new Size(labelWidth, rowHeight);
            screenSizeLabel.Location = new Point(marginLeft, currentY);

            ComboBox screenSizeDropdown = new ComboBox();
            screenSizeDropdown.Items.AddRange(screenSizeMap.Keys.ToArray());
            screenSizeDropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            screenSizeDropdown.Size = new Size(130, 25);
            screenSizeDropdown.Location = new Point(inputX, currentY + 8);
            screenSizeDropdown.SelectedItem = defaultScreenSize;

            currentY += rowHeight + rowSpacing;

            Label startFullscreen = new Label();
            startFullscreen.Text = "Start Fullscreen:";
            startFullscreen.TextAlign = ContentAlignment.MiddleLeft;
            startFullscreen.Size = new Size(labelWidth + 50, rowHeight);
            startFullscreen.Location = new Point(marginLeft, currentY);

            CheckBox fullscreenCheckbox = new CheckBox();
            fullscreenCheckbox.Location = new Point(inputX, currentY + 11);
            fullscreenCheckbox.Size = new Size(20, 20);
            fullscreenCheckbox.Checked = isFullscreen;

            Button saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.TextAlign = ContentAlignment.BottomRight;
            saveButton.Size = new Size(80, 40);
            saveButton.Location = new Point(popup.ClientSize.Width - saveButton.Width - 20, popup.ClientSize.Height - saveButton.Height - 20);
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            saveButton.Click += (sender2, e2) =>
            {
                if (int.TryParse(sizeInput.Text, out int newSize) && newSize > 0)
                {
                    panelSize = newSize;
                    isFullscreen = fullscreenCheckbox.Checked;
                    defaultScreenSize = screenSizeDropdown.SelectedItem?.ToString();

                    if (screenSizeMap.TryGetValue(screenSizeDropdown.SelectedItem?.ToString(), out Size selectedSize) && ClientSize != selectedSize)
                    {
                        ClientSize = selectedSize;
                        this.CenterToScreen();
                    }

                    SaveSettings();
                    foreach (var panel in panels)
                    {
                        panel.Dispose();
                    }
                    panels.Clear();
                    scrollPanel.Controls.Clear();
                    CreatePanels();
                    popup.Close();
                }
                else
                {
                    MessageBox.Show("Please enter an 'Icon Size' greater than 0.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            popup.Controls.Add(iconSize);
            popup.Controls.Add(sizeInput);
            popup.Controls.Add(screenSizeLabel);
            popup.Controls.Add(screenSizeDropdown);
            popup.Controls.Add(startFullscreen);
            popup.Controls.Add(fullscreenCheckbox);
            fullscreenCheckbox.BringToFront();
            popup.Controls.Add(saveButton);
            popup.ShowDialog(this);
        }

        private void SaveSettings()
        {
            string filePath = "XeniaCanaryUISettings.txt";
            try
            {
                string settingsData = $"{panelSize},{defaultScreenSize},{isFullscreen}";
                System.IO.File.WriteAllText(filePath, settingsData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadSettings()
        {
            string filePath = "XeniaCanaryUISettings.txt";
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    string settingsData = System.IO.File.ReadAllText(filePath);
                    string[] settings = settingsData.Split(',');

                    if (settings.Length == 3)
                    {
                        if (int.TryParse(settings[0], out int savedSize))
                        {
                            panelSize = savedSize;
                        }

                        string loadedScreenSize = settings[1];
                        defaultScreenSize = loadedScreenSize;
                        if (screenSizeMap.TryGetValue(loadedScreenSize, out Size loadedSize))
                        {
                            ClientSize = loadedSize;
                        }
                        else if (loadedScreenSize.Contains("x") && int.TryParse(loadedScreenSize.Split('x')[0], out int w) && int.TryParse(loadedScreenSize.Split('x')[1], out int h))
                        {
                            ClientSize = new Size(w, h);
                        }

                        bool.TryParse(settings[2], out isFullscreen);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SavePanelSize(int size)
        {
            string filePath = "XeniaCanaryUISettings.txt";
            try
            {
                System.IO.File.WriteAllText(filePath, size.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save panel size: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadPanelSize()
        {
            string filePath = "XeniaCanaryUISettings.txt";
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    string savedSize = System.IO.File.ReadAllText(filePath);
                    if (int.TryParse(savedSize, out int size))
                    {
                        panelSize = size;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load panel size: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSettings();
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
            if (isFullscreen) ScreenMode("Fullscreen");
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                if (WindowState == FormWindowState.Normal)
                {
                    ScreenMode("Fullscreen");
                }
                else
                {
                    ScreenMode("Normal");
                }
            }
        }
        private void ScreenMode(string screenMode)
        {
            if (screenMode == "Fullscreen")
            {
                TopMost = true;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                TopMost = false;
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
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

        private void Form1_Resize(object sender, EventArgs e)
        {
            UpdatePanelLayout();
        }

        private void CreatePanels()
        {
            if (panels.Count > 0) return;
            //scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.Controls.Clear();
            panels.Clear();

            int defaultPanelWidth = panelSize * 2;
            int defaultPanelHeight = panelSize * 2;

            for (int i = 0; i < zarFiles.Length; i++)
            {
                System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
                panel.BackColor = System.Drawing.Color.DimGray;
                panel.Size = new System.Drawing.Size(defaultPanelWidth, defaultPanelHeight);
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
                label.MouseEnter += Panel_MouseEnter;
                label.MouseLeave += Panel_MouseLeave;
                panel.Controls.Add(label);

                scrollPanel.Controls.Add(panel);
                panels.Add(panel);
            }

            UpdatePanelLayout();
        }

        private void UpdatePanelLayout()
        {
            if (zarFiles == null || scrollPanel == null) return;

            int defaultPanelWidth = panelSize * 2;
            int defaultPanelHeight = panelSize * 2;

            int cols = Math.Max(1, (this.ClientSize.Width + padding) / (defaultPanelWidth + padding));
            int panelWidth = defaultPanelWidth;
            int panelHeight = defaultPanelHeight;

            int totalContentWidth = cols * panelWidth + (cols - 1) * padding;
            int verticalScrollbarWidth = SystemInformation.VerticalScrollBarWidth;
            int sideMargin = Math.Max(0, (this.ClientSize.Width - totalContentWidth - verticalScrollbarWidth) / 2);

            for (int i = 0; i < panels.Count; i++)
            {
                int x = sideMargin + (i % cols) * (panelWidth + padding);
                int y = (i / cols) * (panelHeight + padding) + padding;
                panels[i].Location = new Point(x, y);
            }

            scrollPanel.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 30);
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

        private void SetPanelIcon(Panel panel, string imagePath)
        {
            if (!File.Exists(imagePath)) return;

            using (var original = Image.FromFile(imagePath))
            {
                if (original.Width != original.Height) return;

                panel.BackgroundImage?.Dispose();
                panel.BackgroundImage = new Bitmap(original, new Size(panel.Width, panel.Height));
                panel.BackgroundImageLayout = ImageLayout.Stretch;
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
