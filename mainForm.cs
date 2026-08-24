using SharpCompress.Archives;
using SharpCompress.Common;
using System.Diagnostics;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using Timer = System.Windows.Forms.Timer;

namespace spt_mods_installer
{
    public partial class mainForm : Form
    {
        public class sptCorePost
        {
            public string? projectName { get; set; }
            public string? compatibleTarkovVersion { get; set; }
        }

        public static string currentEnv = Environment.CurrentDirectory;
        // public static string currentEnv = "D:\\SPT Iterations\\4.0.0 Host";
        public static string? bepInFolder = null;
        public static string? userFolder = null;
        public static string? sptDataFolder = null;
        public static string? sptName = null;
        public static string? sptExecutable = null;
        List<string>? completedTasks = null;

        public Color darkMode = Color.FromArgb(38, 41, 44);
        public Color lightMode = SystemColors.Control;
        public Color darkModeText = Color.LightGray;
        public Color lightModeText = Color.FromArgb(50, 50, 50);

        bool isValidLocation = false;
        bool isConditionsMet = false;
        bool doesContainOtherFileTypes = false;

        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            loadDetection();
            bepInFolder = Path.Join(currentEnv, "BepInEx");

            lblCounter.DragEnter += panelDragDrop_DragEnter;
            lblCounter.DragDrop += panelDragDrop_DragDrop;

            btnThemeSwitch.PerformClick();
        }

        public static string getAssemblyVersion(string path)
        {
            try
            {
                FileVersionInfo sptInfo = FileVersionInfo.GetVersionInfo(path);
                if (sptInfo == null) return string.Empty;
                if (sptInfo.FileVersion == null) return string.Empty;

                int major = sptInfo.FileMajorPart;
                int minor = sptInfo.FileMinorPart;
                int build = sptInfo.FileBuildPart;

                string formattedString = major + "." + minor + "." + build;
                if (string.IsNullOrEmpty(formattedString)) return string.Empty;

                return formattedString;
            }
            catch (Exception ex)
            {
                return "Error reading version: " + ex.Message.ToString();
            }
        }

        public static bool isPost400Release(string assemblyPath)
        {
            bool assemblyPathExists = Directory.Exists(assemblyPath);
            if (assemblyPathExists)
            {
                string sptFolder = Path.Join(assemblyPath, "SPT_Runtime");
                bool sptFolderExists = Directory.Exists(sptFolder);
                if (sptFolderExists) // >4.0
                {
                    return true;
                }
                else // <3.11
                {
                    return false;
                }
            }

            return true;
        }

        private void loadDetection()
        {
            panelTitleDetector.Text = $"Could not detect a functioning SPT install";
            panelTitleDetector.ForeColor = Color.Red;
            titleDragDrop.Text = "Drag and drop is disabled, see above";
            dropdownOpen.Enabled = false;
            sptName = $"SPT";

            bool currentEnvExists = Directory.Exists(currentEnv);
            if (currentEnvExists)
            {
                string sptFolder = Path.Join(currentEnv, "SPT_Runtime");
                bool sptFolderExists = Directory.Exists(sptFolder);
                if (sptFolderExists) // > 4.0
                {
                    sptExecutable = Path.Join(sptFolder, "SPT.Server.exe");
                    sptDataFolder = Path.Join(sptFolder, "SPT_Data");
                    userFolder = Path.Join(sptFolder, "user");

                    string sptVersion = getAssemblyVersion(sptExecutable);
                    if (string.IsNullOrEmpty(sptVersion))
                    {
                        return;
                    }

                    string coreJson = Path.Join(sptDataFolder, "configs", "core.json");
                    bool coreJsonExists = File.Exists(coreJson);
                    if (coreJsonExists)
                    {
                        string coreContent = File.ReadAllText(coreJson);
                        sptCorePost? core = JsonSerializer.Deserialize<sptCorePost>(coreContent);

                        string? projectName = core?.projectName;
                        string? compatibleTarkovVersion = core?.compatibleTarkovVersion;

                        isValidLocation = true;
                        sptName = $"{projectName} {sptVersion}";

                        panelTitleDetector.Text =
                            $"Detected {projectName} {sptVersion}" + Environment.NewLine +
                            $"\\" + Path.GetFileName(currentEnv);

                        panelTitleDetector.ForeColor = Color.DodgerBlue;
                        titleDragDrop.Text = "📥 Drag and drop any archive";
                        dropdownOpen.Enabled = true;

                        // adding ranges
                        dropdownOpen.Items.Add("Open server folder");
                        dropdownOpen.Items.Add("Open BepInEx folder");
                        dropdownOpen.Items.Add("Open server mods folder");
                    }
                }
            }
        }

        private async Task extractArchiveAsync(string filepath, int currentNumber)
        {
            string? extractPath = null;

            try
            {
                extractPath = Path.Join(currentEnv, Path.GetFileNameWithoutExtension(filepath));

                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                string currentMod = Path.GetFileNameWithoutExtension(filepath);

                await Task.Run(() =>
                {
                    using (var archive = ArchiveFactory.Open(filepath))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            entry.WriteToDirectory(extractPath, new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    MessageBox.Show($"Extract error: {ex.Message}");
                }));
            }

            if (extractPath != null)
            {
                string modName = Path.GetFileName(extractPath);
                bool hasMisc = await Task.Run(() => areThereMiscFiles(extractPath));

                if (hasMisc)
                {
                    string content = "Mod " + modName + " contains unsupported filetypes." + Environment.NewLine + Environment.NewLine +
                            "It will be skipped, please urge mod authors to pack their SPT mods according to the Forge guidelines.";

                    this.Invoke((MethodInvoker)(() =>
                    {
                        MessageBox.Show(content, Text, MessageBoxButtons.OK);
                    }));

                    try
                    {
                        await Task.Run(() => Directory.Delete(extractPath, true));
                    }
                    catch { }
                    return;
                }

                await Task.Run(() => moveFolder(extractPath, currentNumber));
            }
        }

        private void moveFolder(string extractPath, int currentNumber)
        {
            string modName = Path.GetFileNameWithoutExtension(extractPath);

            int fullDelay = Convert.ToInt32(notificationDelay.Value) * 1000;
            completedTasks = new List<string>();

            bool doesFolderExist = Directory.Exists(extractPath);
            if (doesFolderExist)
            {
                string user_path = Path.Join(extractPath, "SPT_Runtime", "user");
                string BepInEx_path = Path.Join(extractPath, "BepInEx");

                // unknown folders
                List<string> otherFolders = unknownFolders(extractPath);
                for (int i = 0; i < otherFolders.Count; i++)
                {
                    string unknownName = Path.GetFileName(otherFolders[i]);
                    string unknown_path = Path.Join(extractPath, unknownName);

                    try
                    {
                        CopyFolder(unknown_path, Path.Join(currentEnv, unknownName));
                        Debug.WriteLine($"success unknown folder");
                    }
                    catch
                    {
                        Debug.WriteLine("[STANDARD] No unknown folder detected, continuing...");
                    }
                }

                // server mod
                try
                {
                    CopyFolder(user_path, Path.Join(currentEnv, Path.GetFileName(user_path)));
                    Debug.WriteLine($"success server side folder");
                }
                catch
                {
                    Debug.WriteLine("[STANDARD] No user folder could be found, continuing...");
                }

                // client mod
                try
                {
                    CopyFolder(BepInEx_path, Path.Join(currentEnv, Path.GetFileName(BepInEx_path)));
                    Debug.WriteLine($"success BepInEx plugin");
                }
                catch
                {
                    Debug.WriteLine("[STANDARD] No BepInEx folder could be found, continuing...");
                }
            }

            searchBepInExFilesManually(extractPath, modName);
            searchExecutableFilesManually(extractPath, modName);

            string addedItem = $"- {modName} installed into {Path.GetFileName(currentEnv)}";
            completedTasks.Add(addedItem);
            listHistory.Items.Add(addedItem);

            completedTasks.Clear();
            Directory.Delete(extractPath, true);
            doesContainOtherFileTypes = false;
        }

        static bool doesArchiveExceedSize(string filePath, int maxSizeInMegabytes)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                long fileSizeInBytes = fileInfo.Length;
                long maxSizeInBytes = maxSizeInMegabytes * 1024 * 1024;

                return fileSizeInBytes > maxSizeInBytes;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false; // Exception means that the file doesn't exceed the size limit
            }
        }

        private static List<string> unknownFolders(string originFolder)
        {
            string[] excludedFolders = { "SPT_Runtime", "BepInEx" };
            List<string> items = new List<string>();

            try
            {
                string[] subFolders = Directory.GetDirectories(originFolder);
                for (int i = 0; i < subFolders.Length; i++)
                {
                    string folderName = Path.GetFileName(subFolders[i]);
                    string fullFolder = subFolders[i];

                    bool isExcludedFolder = excludedFolders.Any(name =>
                        name.Equals(folderName, StringComparison.OrdinalIgnoreCase)
                    );

                    if (!isExcludedFolder)
                    {
                        items.Add(fullFolder);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                Debug.WriteLine("Error: folder not found: " + originFolder);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return items;
        }

        private static bool areThereMiscFiles(string originFolder)
        {
            string[] excludedTypes = { ".dll", ".exe" };
            string[] files = Directory.GetFiles(originFolder);

            for (int i = 0; i < files.Length; i++)
            {
                string fullFile = files[i];
                string fileName = Path.GetFileName(files[i]);
                string fileExtension = Path.GetExtension(files[i]);

                bool isExcludedType = excludedTypes.Any(ext =>
                    ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)
                );

                if (!isExcludedType)
                {
                    return true;
                }
            }

            return false;
        }

        private void searchBepInExFilesManually(string originFolder, string modName)
        {
            string searchPattern = "*.dll";
            string bepPath = Path.Join(bepInFolder, "plugins");

            string[] subFolderFiles = Directory.GetFiles(
                originFolder,
                searchPattern,
                SearchOption.TopDirectoryOnly
            );

            for (int i = 0; i < subFolderFiles.Length; i++)
            {
                if (subFolderFiles.Length > 0)
                {
                    string plugin = subFolderFiles[i];
                    string pluginName = Path.GetFileName(subFolderFiles[i]);

                    try
                    {
                        File.Copy(plugin, Path.Join(bepPath, pluginName));
                        Debug.WriteLine($"Successfully copied file " + pluginName);
                        isConditionsMet = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        Debug.WriteLine("[CUSTOM FILE] Failed to copy file " + pluginName + ", continuing...");
                    }

                }
            }
        }

        private void searchExecutableFilesManually(string originFolder, string modName)
        {
            string searchPattern = "*.exe";

            string[] subFolderFiles = Directory.GetFiles(
                originFolder,
                searchPattern,
                SearchOption.TopDirectoryOnly
            );

            for (int i = 0; i < subFolderFiles.Length; i++)
            {
                if (subFolderFiles.Length > 0)
                {
                    string file = subFolderFiles[i];
                    string fileName = Path.GetFileName(subFolderFiles[i]);

                    try
                    {
                        File.Copy(file, Path.Join(currentEnv, fileName));
                        Debug.WriteLine($"Successfully copied executable " + fileName);
                        isConditionsMet = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        Debug.WriteLine("[CUSTOM FILE] Failed to copy executable " + modName + ", continuing...");
                    }

                }
            }
        }

        private void copyBepInEx(string originalFolder, bool is400)
        {
            string originalBepIn = string.Empty;
            originalBepIn = Path.Join(bepInFolder, "plugins");
            CopyFolder(originalFolder, originalBepIn);
            isConditionsMet = true;
        }

        private void copyUser(string originalFolder, bool standardMethod)
        {
            if (!string.IsNullOrEmpty(userFolder))
            {
                if (!standardMethod)
                {
                    string originalFolderName = Path.GetFileNameWithoutExtension(originalFolder);
                    bool userFolderExists = Directory.Exists(userFolder);
                    if (userFolderExists)
                    {
                        string originalMods = Path.Join(userFolder, "mods");
                        bool originalModsExists = Directory.Exists(originalMods);
                        if (originalModsExists)
                        {
                            string fullModPath = Path.Join(originalMods, originalFolderName);
                            CopyFolder(originalFolder, fullModPath);
                            isConditionsMet = true;
                        }
                    }
                }
                else
                {
                    CopyFolder(originalFolder, userFolder);
                    isConditionsMet = true;
                }
            }
        }

        private void processExtractedDirectory(string extractedRootDirectory, string gameInstallPath)
        {
            foreach (string subfolder in Directory.GetDirectories(extractedRootDirectory))
            {
                string folderName = Path.GetFileName(subfolder);
                string destinationFolder = Path.Join(gameInstallPath, folderName);

                CopyFolder(subfolder, destinationFolder);
            }

            foreach (string file in Directory.GetFiles(extractedRootDirectory))
            {
                string destFile = Path.Join(gameInstallPath, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
        }

        static void CopyFolder(string sourceFolder, string destinationFolder)
        {
            try
            {
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                foreach (string file in Directory.GetFiles(sourceFolder))
                {
                    string destFile = Path.Join(destinationFolder, Path.GetFileName(file));
                    File.Copy(file, destFile, true);
                }

                foreach (string subfolder in Directory.GetDirectories(sourceFolder))
                {
                    string destSubfolder = Path.Join(destinationFolder, Path.GetFileName(subfolder));
                    CopyFolder(subfolder, destSubfolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Copy Error: {ex.Message}");
            }
        }

        static bool IsFolderEmpty(string folderPath)
        {
            return Directory.GetFiles(folderPath).Length == 0 && Directory.GetDirectories(folderPath).Length == 0;
        }

        static async Task CopyFolderAsync(string sourceFolder, string destinationFolder)
        {
            try
            {
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                string[] files = Directory.GetFiles(sourceFolder);
                string[] subfolders = Directory.GetDirectories(sourceFolder);
                await Task.WhenAll(files.Select(file => CopyFileAsync(file, destinationFolder)));
                await Task.WhenAll(subfolders.Select(subfolder => CopyFolderAsync(subfolder, Path.Join(destinationFolder, Path.GetFileName(subfolder)))));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Async copy error: {ex.Message}");
            }
        }

        static async Task CopyFileAsync(string sourceFile, string destinationFolder)
        {
            try
            {
                string destinationFilePath = Path.Join(destinationFolder, Path.GetFileName(sourceFile));
                using (var sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
                using (var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Async single-copy Error: {ex.Message}");
            }
        }

        private void panelSeparator1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.FromArgb(150, 150, 150), 1);
            int bottomY = panelSeparator1.Height - 1;
            g.DrawLine(pen, 0, bottomY, panelSeparator1.Width, bottomY);
            pen.Dispose();
        }

        private void panelDragDrop_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop) && isValidLocation)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private async void panelDragDrop_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop) || !isValidLocation)
                return;

            if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
            {
                titleDragDrop.Visible = false;
                listHistory.Visible = true;
                lblCounter.Visible = true;
                lblLoadingDots.Visible = true;

                var validExtensions = new[] { ".rar", ".zip", ".7z" };
                var extractQuantity = files
                    .Where(f => validExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .ToList();

                int unsupportedCount = files.Length - extractQuantity.Count;
                if (unsupportedCount > 0 && extractQuantity.Count == 0)
                {
                    MessageBox.Show(Text, "Only .rar, .zip, and .7z archives can be extracted with this software.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int totalArchivesCount = extractQuantity.Count;

                try
                {
                    for (int i = 0; i < totalArchivesCount; i++)
                    {
                        string file = extractQuantity[i];
                        int currentNumber = i + 1;

                        updateStatus($"({currentNumber}/{totalArchivesCount}) Extracting and installing {Path.GetFileName(file)}");
                        updateCounterStatus($"{currentNumber}/{totalArchivesCount}");

                        bool shouldExtract = true;

                        if (chkDisplayWarning.Checked)
                        {
                            int largeArchive = 35;
                            if (doesArchiveExceedSize(file, largeArchive))
                            {
                                DialogResult result = MessageBox.Show(
                                    $"Archive ({currentNumber}/{totalArchivesCount}): '{Path.GetFileName(file)}' exceeds 35 megabytes and may take longer to install." + Environment.NewLine + Environment.NewLine +
                                    "Do you wish to proceed?",
                                    "Large archive detected",
                                    MessageBoxButtons.YesNo
                                );

                                if (result != DialogResult.Yes)
                                {
                                    shouldExtract = false;
                                }
                            }
                        }

                        if (shouldExtract)
                        {
                            await extractArchiveAnimated(file, currentNumber, totalArchivesCount);
                        }
                    }

                    if (chkSkipPostLog.Checked)
                    {
                        updateCounterStatus(string.Empty);
                        updateStatus(string.Empty);
                        listHistory.Items.Clear();
                        titleDragDrop.Visible = true;
                        listHistory.Visible = false;
                        lblCounter.Visible = false;
                        lblLoadingDots.Visible = false;
                        return;
                    }

                    updateStatus($"({totalArchivesCount}/{totalArchivesCount}) Extraction complete!");
                    listHistory.Items.Add("");
                    listHistory.Items.Add("Extraction complete! Cleaning up...");

                    decimal fullDelay = notificationDelay.Value * 1000;
                    int convertedDelay = Convert.ToInt32(fullDelay);

                    await Task.Delay(convertedDelay);
                    updateCounterStatus(string.Empty);
                    listHistory.Items.Clear();
                    titleDragDrop.Visible = true;
                    listHistory.Visible = false;
                    lblCounter.Visible = false;
                    lblLoadingDots.Visible = false;
                }
                finally
                {
                    updateStatus(string.Empty);
                }
            }
        }

        private void updateStatus(string notification)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => this.Text = notification));
            }
            else
            {
                this.Text = notification;
            }
        }

        private void updateCounterStatus(string notification)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => lblCounter.Text = notification));
            }
            else
            {
                lblCounter.Text = notification;
            }
        }

        private void updateLoadingDots(string notification)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => lblLoadingDots.Text = notification));
            }
            else
            {
                lblLoadingDots.Text = notification;
            }
        }

        private void dropdownOpen_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = dropdownOpen.SelectedIndex;
            if (selected == 0)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Path.Join(currentEnv, "SPT_Runtime"),
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            else if (selected == 1)
            {
                if (!string.IsNullOrEmpty(bepInFolder))
                {
                    string? plugins = Path.Join(bepInFolder, "plugins");
                    bool pluginsExists = Directory.Exists(plugins);
                    if (pluginsExists)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = plugins,
                            UseShellExecute = true
                        };

                        Process.Start(startInfo);
                    }
                }
            }
            else if (selected == 2)
            {

                if (!string.IsNullOrEmpty(userFolder))
                {
                    string mods = Path.Join(userFolder, "mods");
                    bool modsExists = Directory.Exists(mods);
                    if (modsExists)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = mods,
                            UseShellExecute = true
                        };

                        Process.Start(startInfo);
                    }
                }
            }

            dropdownOpen.SelectedIndex = -1;
        }

        private void btnThemeSwitch_Click(object sender, EventArgs e)
        {
            if (btnThemeSwitch.Text == "○")
            {
                btnThemeSwitch.Text = "●";

                panelTitleName.ForeColor = darkModeText;
                panelTitleNotice.ForeColor = darkModeText;
                lblDelayTitle.ForeColor = darkModeText;
                lblDelayTitle2.ForeColor = darkModeText;
                notificationDelay.ForeColor = darkModeText;
                chkDisplayWarning.ForeColor = darkModeText;
                chkSkipPostLog.ForeColor = darkModeText;
                lblCounter.ForeColor = darkModeText;
                lblLoadingDots.ForeColor = darkModeText;

                notificationDelay.BackColor = darkMode;
                listHistory.BackColor = darkMode;
                listHistory.ForeColor = darkModeText;

                this.BackColor = Color.FromArgb(38, 41, 44);
                foreach (Panel pnl in this.Controls)
                {
                    if (pnl.Name.ToLower() != "panelseparator1")
                    {
                        pnl.BackColor = darkMode;
                    }
                }
            }
            else if (btnThemeSwitch.Text == "●")
            {
                btnThemeSwitch.Text = "○";

                panelTitleName.ForeColor = SystemColors.ControlText;
                panelTitleNotice.ForeColor = lightModeText;
                lblDelayTitle.ForeColor = lightModeText;
                lblDelayTitle2.ForeColor = lightModeText;
                notificationDelay.ForeColor = SystemColors.ControlText;
                chkDisplayWarning.ForeColor = lightModeText;
                chkSkipPostLog.ForeColor = lightModeText;
                lblCounter.ForeColor = lightModeText;
                lblLoadingDots.ForeColor = lightModeText;

                notificationDelay.BackColor = lightMode;
                listHistory.BackColor = lightMode;
                listHistory.ForeColor = lightModeText;

                this.BackColor = SystemColors.Control;
                foreach (Panel pnl in this.Controls)
                {
                    if (pnl.Name.ToLower() != "panelseparator1")
                    {
                        pnl.BackColor = lightMode;
                    }
                }
            }

            panelTitleName.Select();
        }

        private void mainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void timerConfirmation_Tick(object sender, EventArgs e)
        {
            timerConfirmation.Stop();
            timerConfirmation.Dispose();
        }

        private void btnClearServerCache_Click(object sender, EventArgs e)
        {
            List<string> completedTasks = new List<string>();

            string cacheFolder = Path.Join(currentEnv, "SPT_Runtime", "user", "cache");
            bool doesCacheExist = Directory.Exists(cacheFolder);
            if (doesCacheExist)
            {
                Directory.Delete(cacheFolder, true);
                completedTasks.Add($"Server cache cleared for the current installation.");

                int fullDelay = Convert.ToInt32(notificationDelay.Value) * 1000;
                timerConfirmation.Interval = fullDelay;
                timerConfirmation.Start();
            }
        }

        private async void btnBrowseForMod_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog cf = new OpenFileDialog())
            {
                cf.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                cf.Multiselect = true;
                cf.Title = "Select a mod (zip archive) to install";
                cf.Filter = "Zip archives (*.zip)|*.zip|7z archives (*.7z)|*.7z|RAR archives (*.rar)|*.rar";

                if (cf.ShowDialog() == DialogResult.OK)
                {
                    titleDragDrop.Visible = false;
                    listHistory.Visible = true;
                    lblCounter.Visible = true;
                    lblLoadingDots.Visible = true;

                    string[] selectedFiles = cf.FileNames;
                    int totalArchivesCount = selectedFiles.Length;

                    try
                    {
                        for (int i = 0; i < totalArchivesCount; i++)
                        {
                            string file = Path.GetFullPath(selectedFiles[i]);
                            string fileName = Path.GetFileName(file);
                            int currentNumber = i + 1;
                            bool shouldExtract = true;

                            updateStatus($"({currentNumber}/{totalArchivesCount}) Extracting and installing {Path.GetFileName(file)}");
                            updateCounterStatus($"{currentNumber}/{totalArchivesCount}");

                            int largeArchive = 35;
                            if (chkDisplayWarning.Checked && doesArchiveExceedSize(file, largeArchive))
                            {
                                DialogResult result = MessageBox.Show(
                                    $"Archive ({currentNumber.ToString()}/{totalArchivesCount}): '{fileName}' exceeds 35 megabytes, and may take longer to install." + Environment.NewLine + Environment.NewLine +
                                    "Do you wish to proceed?",
                                    "Large archive detected",
                                    MessageBoxButtons.YesNo
                                );

                                if (result != DialogResult.Yes)
                                {
                                    shouldExtract = false;
                                }
                            }

                            if (shouldExtract)
                            {
                                await extractArchiveAnimated(file, currentNumber, totalArchivesCount);
                            }
                        }

                        if (chkSkipPostLog.Checked)
                        {
                            updateCounterStatus(string.Empty);
                            updateStatus(string.Empty);
                            listHistory.Items.Clear();
                            titleDragDrop.Visible = true;
                            listHistory.Visible = false;
                            lblCounter.Visible = false;
                            lblLoadingDots.Visible = false;
                            return;
                        }

                        updateStatus($"({totalArchivesCount}/{totalArchivesCount}) Extraction complete!");
                        listHistory.Items.Add("");
                        listHistory.Items.Add("Extraction complete! Cleaning up...");

                        decimal fullDelay = notificationDelay.Value * 1000;
                        int convertedDelay = Convert.ToInt32(fullDelay);

                        await Task.Delay(convertedDelay);
                        updateCounterStatus(string.Empty);
                        listHistory.Items.Clear();
                        titleDragDrop.Visible = true;
                        listHistory.Visible = false;
                        lblCounter.Visible = false;
                        lblLoadingDots.Visible = false;
                    }
                    finally
                    {
                        updateStatus(string.Empty);
                    }
                }
            }
        }

        private async Task extractArchiveAnimated(string file, int currentNumber, int totalArchivesCount)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                string fileName = Path.GetFileName(file);
                Task animationTask = Task.Run(async () =>
                {
                    int dotCount = 1;
                    while (!cts.Token.IsCancellationRequested)
                    {
                        string dots = new string('.', dotCount);

                        this.Invoke(new Action(() =>
                        {
                            updateLoadingDots(dots);
                        }));

                        dotCount = (dotCount % 3) + 1;

                        try
                        {
                            await Task.Delay(550, cts.Token); // tick speed (400ms)
                        }
                        catch (TaskCanceledException)
                        {
                            break; // expected when token cancels out
                        }
                    }
                });

                await extractArchiveAsync(file, 1);
                cts.Cancel();

                try
                {
                    await animationTask; // post-clean to ensure handled exceptions go well
                }
                catch (TaskCanceledException) { }
            }
        }

        private void listHistory_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                listHistory.ClearSelected();
                listHistory.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void lblCounter_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop) && isValidLocation)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void lblCounter_DragDrop(object sender, DragEventArgs e)
        {

        }
    }
}
