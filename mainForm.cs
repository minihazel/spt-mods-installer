using System.Text.Json;
using System.IO.Compression;
using System.Diagnostics;
using Aspose.Zip.SevenZip;
using System.ComponentModel.DataAnnotations.Schema;
using SharpCompress.Common;
using SharpCompress.Archives;

namespace spt_mods_installer
{

    public partial class mainForm : Form
    {
        public class akiCore
        {
            public string akiVersion { get; set; }
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
            panelTitleDetector.Text = $"Could not detect a functioning SPT-AKI install";
            panelTitleDetector.ForeColor = Color.Red;
            titleDragDrop.Text = "Drag and drop is disabled, see above";
            dropdownOpen.Enabled = false;
            sptName = $"SPT-AKI";

            bool currentEnvExists = Path.Exists(currentEnv);
            if (currentEnvExists)
            {
                string akidata = Path.Join(currentEnv, "Aki_Data");
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

                                string akiVersion = core.akiVersion;
                                string projectName = core.projectName;
                                isValidLocation = true;
                                sptName = $"{projectName} {akiVersion}";

                                panelTitleDetector.Text = $"Detected {projectName} {akiVersion} ({currentEnv})";
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
            try
            {
                // string extractPath = Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath));
                string extractPath = Path.Combine(currentEnv, Path.GetFileNameWithoutExtension(filepath));

                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                string currentMod = Path.GetFileNameWithoutExtension(filepath);
                string extension = Path.GetExtension(filepath).ToLower();

                if (extension == ".zip")
                {
                    ZipFile.ExtractToDirectory(filepath, extractPath);
                    detectModFolders(extractPath);

                    try
                    {
                        Directory.Delete(extractPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"ZIP delete error: {ex.Message}");
                    }
                }
                else if (extension == ".7z")
                {
                    using (SevenZipArchive archive = new SevenZipArchive(filepath))
                    {
                        archive.ExtractToDirectory(extractPath);
                    }

                    detectModFolders(extractPath);

                    try
                    {
                        Directory.Delete(extractPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"7z delete error: {ex.Message}");
                    }
                }
                else if (extension == ".rar")
                {
                    using (var archive = ArchiveFactory.Open(filepath))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            if (!entry.IsDirectory)
                            {
                                entry.WriteToDirectory(extractPath, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                            }
                        }
                    }

                    detectModFolders(extractPath);

                    try
                    {
                        Directory.Delete(extractPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"7z delete error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Extract error: {ex.Message}");
            }
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
                MessageBox.Show($"{currentMod} successfully installed into {sptName}", "AKI Mod Installer", MessageBoxButtons.OK);
            }
            else
            {
                string currentMod = Path.GetFileNameWithoutExtension(originFolder);
                MessageBox.Show($"{currentMod} could not be installed. Make sure it\'s a valid mod, then try again", "AKI Mod Installer", MessageBoxButtons.OK);
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
                        searchUserManually(subfolder);
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
                        extractArchive(file);
                    }
                    else
                    {
                        MessageBox.Show("", "AKI Mod Installer", MessageBoxButtons.OK);
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
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = bepInFolder,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            else if (selected == 2)
            {

                string mods = Path.Combine(userFolder, "mods");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = mods,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
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
    }
}
