using System.Text.Json;
using System.IO.Compression;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations.Schema;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using Microsoft.VisualBasic.Devices;

namespace spt_mods_installer
{

    public partial class mainForm : Form
    {
        public class akiCore
        {
            public string sptVersion { get; set; }
            public string projectName { get; set; }
            public string compatibleTarkovVersion { get; set; }
        }

        public string currentEnv = Environment.CurrentDirectory;
        public string bepInFolder = null;
        public string userFolder = null;
        public string sptName = null;

        bool isValidLocation = false;
        bool isConditionsMet = false;

        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            loadDetection();

            bepInFolder = Path.Combine(currentEnv, "BepInEx");
            userFolder = Path.Combine(currentEnv, "user");
        }

        private void loadDetection()
        {
            panelTitleDetector.Text = $"Could not detect a functioning SPT install";
            panelTitleDetector.ForeColor = Color.Red;
            titleDragDrop.Text = "Drag and drop is disabled, see above";
            dropdownOpen.Enabled = false;
            sptName = $"SPT";

            bool currentEnvExists = Path.Exists(currentEnv);
            if (currentEnvExists)
            {
                string akidata = Path.Join(currentEnv, "SPT_Data");
                bool akidataExists = Path.Exists(akidata);
                if (akidataExists)
                {
                    string server = Path.Join(akidata, "Server");
                    bool serverExists = Path.Exists(server);
                    if (serverExists)
                    {
                        string configs = Path.Join(server, "configs");
                        bool configsExists = Path.Exists(configs);
                        if (configsExists)
                        {
                            string coreJson = Path.Join(configs, "core.json");
                            bool coreJsonExists = Path.Exists(coreJson);
                            if (coreJsonExists)
                            {
                                string coreContent = File.ReadAllText(coreJson);
                                akiCore core = JsonSerializer.Deserialize<akiCore>(coreContent);

                                string akiVersion = core.sptVersion;
                                string projectName = core.projectName;
                                isValidLocation = true;
                                sptName = $"{projectName} {akiVersion}";

                                panelTitleDetector.Text = $"Detected {projectName} {akiVersion}";
                                panelTitleDetector.ForeColor = Color.DodgerBlue;
                                titleDragDrop.Text = "📥 Drag and drop any archive";
                                dropdownOpen.Enabled = true;

                                // adding ranges
                                dropdownOpen.Items.Add("Open server folder");
                                dropdownOpen.Items.Add("Open BepInEx folder");
                                dropdownOpen.Items.Add("Open user mods folder");
                            }
                        }
                    }
                }
            }
        }

        private void extractArchive(string filepath)
        {
            string extractPath = null;

            try
            {
                // string extractPath = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath));
                extractPath = Path.Combine(currentEnv, Path.GetFileNameWithoutExtension(filepath));

                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                string currentMod = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath).ToLower();

                using (var archive = ArchiveFactory.Open(filepath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            entry.WriteToDirectory(extractPath, new ExtractionOptions
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Extract error: {ex.Message}");
            }

            moveFolder(extractPath);
        }

        private void moveFolder(string extractPath)
        {
            bool userChecked = false;

            List<string> completedTasks = new List<string>();
            string fileName = Path.GetFileNameWithoutExtension(extractPath);

            bool doesFolderExist = Directory.Exists(extractPath);
            if (doesFolderExist)
            {
                string[] inFolder = Directory.GetDirectories(extractPath);

                foreach (string folder in inFolder)
                {
                    if (Path.GetFileNameWithoutExtension(folder) == "user")
                    {
                        string BepInEx_path = Path.Combine(extractPath, "BepInEx");
                        string user_path = Path.Combine(extractPath, "user");
                        bool BepInEx_exists = Directory.Exists(BepInEx_path);
                        bool user_path_exists = Directory.Exists(user_path);

                        if (BepInEx_exists)
                        {
                            CopyFolder(BepInEx_path, Path.Combine(currentEnv, Path.GetFileName(BepInEx_path)));
                            Debug.WriteLine($"success BepInEx");
                            completedTasks.Add($"Client mod of {fileName} installed");
                        }

                        if (user_path_exists)
                        {
                            CopyFolder(user_path, Path.Combine(currentEnv, Path.GetFileName(user_path)));
                            Debug.WriteLine($"success user/mods");
                            completedTasks.Add($"Server mod of {fileName} installed");
                            userChecked = true;
                        }
                        break;
                    }
                    else
                        continue;
                }

                if (!userChecked)
                {
                    searchUserManually(extractPath);
                }

                titleHistory.Text = string.Join(Environment.NewLine, completedTasks);

                int fullDelay = Convert.ToInt32(notificationDelay.Value) + 1000;
                timerConfirmation.Interval = fullDelay;
                timerConfirmation.Start();
            }

            Directory.Delete(extractPath, true);
        }

        private void detectModFolders(string originFolder)
        {
            searchBep(originFolder);
            searchUser(originFolder);

            if (!isConditionsMet)
            {
                searchUserManually(originFolder);
            }

            if (isConditionsMet)
            {
                string currentMod = Path.GetFileNameWithoutExtension(originFolder);
                MessageBox.Show($"{currentMod} successfully installed into {sptName}", "SPT Mod Installer", MessageBoxButtons.OK);
            }
            else
            {
                string currentMod = Path.GetFileNameWithoutExtension(originFolder);
                MessageBox.Show($"{currentMod} could not be installed. Make sure it\'s a valid mod, then try again", "SPT Mod Installer", MessageBoxButtons.OK);
            }
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
                return false; // Exception means that the file doesn't exceed the size limit
            }
        }

        private void searchBep(string originFolder)
        {
            string keyword = "bepinex";

            try
            {
                string[] subfolders = Directory.GetDirectories(originFolder);
                foreach (string subfolder in subfolders)
                {
                    if (Path.GetFileNameWithoutExtension(subfolder).ToLower() == keyword)
                    {
                        copyBepInEx(subfolder);
                        break;
                    }
                    else
                        searchBep(subfolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BepInEx search error: {ex.Message}");
            }
        }

        private void searchUser(string originFolder)
        {
            string keyword = "user";

            try
            {
                string[] subfolders = Directory.GetDirectories(originFolder);
                foreach (string subfolder in subfolders)
                {
                    if (Path.GetFileNameWithoutExtension(subfolder).ToLower() == keyword)
                    {
                        copyUser(subfolder, true);
                        break;
                    }
                    else
                        searchBep(subfolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"User folder search error: {ex.Message}");
            }
        }

        private void searchUserManually(string originFolder)
        {
            string keyword = "package.json";

            try
            {
                string[] subfolders = Directory.GetDirectories(originFolder);
                foreach (string subfolder in subfolders)
                {
                    string packageFile = Path.Combine(subfolder, keyword);
                    bool packageFileExists = File.Exists(packageFile);
                    if (packageFileExists)
                    {
                        copyUser(subfolder, false);
                        isConditionsMet = true;
                        break;
                    }
                    else
                    {
                        searchUserManually(subfolder);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"User folder search error: {ex.Message}");
            }
        }

        private void copyBepInEx(string originalFolder)
        {
            string originalBepIn = Path.Combine(currentEnv, "bepinex");
            CopyFolder(originalFolder, originalBepIn);
            isConditionsMet = true;
        }

        private void copyUser(string originalFolder, bool standardMethod)
        {
            if (!standardMethod)
            {
                string originalFolderName = Path.GetFileNameWithoutExtension(originalFolder);

                string originalUser = Path.Combine(currentEnv, "user");
                bool originalUserExists = Directory.Exists(originalUser);
                if (originalUserExists)
                {
                    string originalMods = Path.Combine(originalUser, "mods");
                    bool originalModsExists = Directory.Exists(originalMods);
                    if (originalModsExists)
                    {
                        string fullModPath = Path.Combine(originalMods, originalFolderName);
                        CopyFolder(originalFolder, fullModPath);
                        isConditionsMet = true;
                    }
                }
            }
            else
            {
                string originalUser = Path.Combine(currentEnv, "user");
                bool originalUserExists = Directory.Exists(originalUser);
                if (originalUserExists)
                {
                    CopyFolder(originalFolder, originalUser);
                    isConditionsMet = true;
                }
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

                string[] files = Directory.GetFiles(sourceFolder);
                string[] subfolders = Directory.GetDirectories(sourceFolder);

                foreach (string file in files)
                {
                    string destinationFilePath = Path.Combine(destinationFolder, Path.GetFileName(file));
                    File.Copy(file, destinationFilePath, true);
                }

                foreach (string subfolder in subfolders)
                {
                    string destinationSubfolder = Path.Combine(destinationFolder, Path.GetFileName(subfolder));
                    CopyFolder(subfolder, destinationSubfolder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Copy Error: {ex.Message}");
            }
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
                await Task.WhenAll(subfolders.Select(subfolder => CopyFolderAsync(subfolder, Path.Combine(destinationFolder, Path.GetFileName(subfolder)))));
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
                string destinationFilePath = Path.Combine(destinationFolder, Path.GetFileName(sourceFile));
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

        private void panelDragDrop_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && isValidLocation)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void panelDragDrop_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && isValidLocation)
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();

                    if (extension == ".rar" || extension == ".zip" || extension == ".7z")
                    {
                        int largeArchive = 15;
                        if (doesArchiveExceedSize(file, largeArchive))
                        {
                            if (MessageBox.Show("This archive exceeds 10 megabytes, and may take longer to install. The window may freeze." + Environment.NewLine +
                                Environment.NewLine +
                                "Do you wish to proceed?",
                                "Large archive detected",
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                extractArchive(file);
                            }
                        }
                        else
                        {
                            extractArchive(file);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Only (rar, zip, 7z) formats are currently supported", "SPT Mod Installer", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void dropdownOpen_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = dropdownOpen.SelectedIndex;
            if (selected == 0)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = currentEnv,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            else if (selected == 1)
            {
                string plugins = Path.Combine(bepInFolder, "plugins");
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
            else if (selected == 2)
            {
                string mods = Path.Combine(userFolder, "mods");
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

            dropdownOpen.SelectedIndex = -1;
        }

        private void btnThemeSwitch_Click(object sender, EventArgs e)
        {
            if (btnThemeSwitch.Text == "○")
            {
                btnThemeSwitch.Text = "●";

                panelTitleName.ForeColor = Color.LightGray;
                panelTitleNotice.ForeColor = Color.LightGray;
                lblDelayTitle.ForeColor = Color.LightGray;
                lblDelayTitle2.ForeColor = Color.LightGray;
                notificationDelay.BackColor = Color.FromArgb(38, 41, 44);
                notificationDelay.ForeColor = Color.LightGray;

                this.BackColor = Color.FromArgb(38, 41, 44);
                foreach (Panel pnl in this.Controls)
                {
                    if (pnl.Name.ToLower() != "panelseparator1")
                    {
                        pnl.BackColor = Color.FromArgb(38, 41, 44);
                    }
                }
            }
            else if (btnThemeSwitch.Text == "●")
            {
                btnThemeSwitch.Text = "○";

                panelTitleName.ForeColor = SystemColors.ControlText;
                panelTitleNotice.ForeColor = Color.FromArgb(50, 50, 50);
                lblDelayTitle.ForeColor = Color.FromArgb(50, 50, 50);
                lblDelayTitle2.ForeColor = Color.FromArgb(50, 50, 50);
                notificationDelay.BackColor = SystemColors.Window;
                notificationDelay.ForeColor = Color.FromArgb(50, 50, 50);

                this.BackColor = SystemColors.Control;
                foreach (Panel pnl in this.Controls)
                {
                    if (pnl.Name.ToLower() != "panelseparator1")
                    {
                        pnl.BackColor = SystemColors.Control;
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
            titleHistory.Text = "";
        }
    }
}
