using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PWAHelper
{
    public partial class Form1 : Form
    {
        private PWAManifest? pwaManifest;
        public Form1()
        {
            InitializeComponent();

            Reset();
        }

        private void OnbuttonFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.InitialDirectory = textBoxFolder.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                SetWorkingDirectory(folderBrowserDialog1.SelectedPath);
            }
        }

        private void OnbuttonExportAssets_Click(object sender, EventArgs e)
        {
            ServiceWorkerManifest assetsManifest = new();

            var checkedNodes = GetCheckedTreeNodes(treeViewAssets.Nodes);
            using (SHA256 sha256 = SHA256.Create())
            {
                //Use a sorted dictionary to automatically sort filenames in some way.
                var fileHashes = new SortedDictionary<string, byte[]>();
                foreach (var node in checkedNodes)
                {
                    if (node.Tag is FileInfo file)
                    {
                        var hashValue = GetFileHash(sha256, file);
                        fileHashes.Add(file.FullName, hashValue);
                    }
                }

                foreach (var node in fileHashes)
                {
                    assetsManifest.assets.Add(new ServiceWorkerAsset { url = GetRelativePath((treeViewAssets.Nodes[0].Tag as DirectoryInfo)!.FullName, node.Key), hash = "sha256-" + Convert.ToBase64String(node.Value) });
                    sha256.TransformBlock(node.Value, 0, node.Value.Length, null, 0);
                }
                sha256.TransformFinalBlock([], 0, 0);
                assetsManifest.version = Convert.ToBase64String(sha256.Hash!)[..8];

            }

            var json = JsonSerializer.Serialize(assetsManifest, ServiceWorkerManifest.DefaultSerializationOptions);
            var path = Path.Combine(textBoxFolder.Text, textBoxServiceWorkerAssetsJs.Text);
            saveFileDialog1.Filter = $"JS files (*.js)|*.js|All files|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(path);
            saveFileDialog1.FileName = Path.GetFileName(path);
            if (string.IsNullOrWhiteSpace(saveFileDialog1.FileName))
            {
                saveFileDialog1.FileName = "service-worker-assets.js";
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxServiceWorkerAssetsJs.Text = !string.IsNullOrWhiteSpace(textBoxFolder.Text) ? Path.GetRelativePath(textBoxFolder.Text, saveFileDialog1.FileName) : saveFileDialog1.FileName;
                try
                {
                    File.WriteAllText(saveFileDialog1.FileName, "self.assetsManifest = " + json);
                    MessageBox.Show($"{saveFileDialog1.FileName} successfully saved.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PopulateTree()
        {
            treeViewAssets.Nodes.Clear();
            if (!string.IsNullOrWhiteSpace(textBoxFolder.Text))
            {
                var stack = new Stack<TreeNode>();
                var rootDir = new DirectoryInfo(textBoxFolder.Text);
                var rootNode = new TreeNode(rootDir.Name) { Tag = rootDir };
                stack.Push(rootNode);

                while (stack.Count > 0)
                {
                    var currentNode = stack.Pop();
                    var dirInfo = (DirectoryInfo)currentNode.Tag;
                    foreach (var dir in dirInfo.GetDirectories())
                    {
                        var dirNode = new TreeNode(dir.Name) { Tag = dir };
                        currentNode.Nodes.Add(dirNode);
                        stack.Push(dirNode);
                    }
                    foreach (var file in dirInfo.GetFiles())
                    {
                        currentNode.Nodes.Add(new TreeNode(file.Name) { Tag = file });
                    }
                }

                treeViewAssets.Nodes.Add(rootNode);
                rootNode.Expand();
            }
        }

        private void OntreeViewAssets_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode? treeNode = e.Node;
            CheckTreeNodesChildrenHierarchy(treeNode, treeNode?.Checked ?? false);
        }

        private static void CheckTreeNodesChildrenHierarchy(TreeNode? node, bool bChecked)
        {
            foreach (var item in GetAllTreeNodes(node?.Nodes))
            {
                item.Checked = bChecked;
            }
        }

        private static IEnumerable<TreeNode> GetCheckedTreeNodes(TreeNodeCollection? treeNodes)
        {
            return GetAllTreeNodes(treeNodes, x => x.Checked);
        }

        private static IEnumerable<TreeNode> GetAllTreeNodes(TreeNodeCollection? treeNodes, Predicate<TreeNode>? predicate = null)
        {
            if (treeNodes != null)
            {
                foreach (TreeNode subnode in treeNodes)
                {
                    if (subnode != null)
                    {
                        if ((predicate == null) || predicate(subnode))
                        {
                            yield return subnode;
                        }

                        foreach (var child in GetAllTreeNodes(subnode.Nodes, predicate))
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        private static string GetRelativePath(string relativeTo, string path)
        {
            string relativePath = Path.GetRelativePath(relativeTo, path);
            return relativePath.Replace("\\", "/");
        }

        private static byte[] GetFileHash(HashAlgorithm hashAlgo, FileInfo fileInfo)
        {
            using FileStream fileStream = fileInfo.OpenRead();
            try
            {
                fileStream.Position = 0;
                return hashAlgo.ComputeHash(fileStream);
            }
            catch (IOException e)
            {
                Console.WriteLine($"I/O Exception: {e.Message}");
                throw;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Access Exception: {e.Message}");
                throw;
            }
        }

        private void OnbuttonLoadImage_Click(object sender, EventArgs e)
        {
            var loadImageExtensions = string.Join(";", ImageCodecInfo.GetImageDecoders().Select(dec => dec.FilenameExtension?.ToLowerInvariant()).Where(x => !string.IsNullOrWhiteSpace(x)));
            openFileDialog1.Filter = $"Image files ({loadImageExtensions})|{loadImageExtensions}|All files|*.*";
            openFileDialog1.FilterIndex = 1;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxImage.Text = openFileDialog1.FileName;
                pictureBox1.ImageLocation = openFileDialog1.FileName;
            }
        }

        private void OnbuttonExportIcons_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                folderBrowserDialog1.InitialDirectory = textBoxFolder.Text;
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    var iconsDir = folderBrowserDialog1.SelectedPath;

                    int numsaved = 0;
                    foreach (string resolution in PWAManifest.IconResolutions)
                    {
                        try
                        {
                            var res = resolution.Split('x');
                            if (res.Length == 2)
                            {
                                int width = int.Parse(res[0]);
                                int height = int.Parse(res[1]);
                                var bmp = ResizeImage(pictureBox1.Image, width, height);
                                var fullFileName = Path.Combine(iconsDir, $"icon-{resolution}.png");
                                if (checkBoxOverwriteIcons.Checked || !File.Exists(fullFileName))
                                {
                                    using var imageStream = File.OpenWrite(fullFileName);
                                    bmp.Save(imageStream, ImageFormat.Png);
                                    numsaved++;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    switch (numsaved)
                    {
                        case 0:
                            MessageBox.Show($"No icons saved.");
                            break;

                        case 1:
                            MessageBox.Show($"{numsaved} icon successfully saved.");
                            break;

                        default:
                            MessageBox.Show($"{numsaved} icons successfully saved.");
                            break;
                    }
                }
            }
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return destImage;
        }

        private void OnpropertyGrid1_DragDrop(object sender, DragEventArgs e)
        {
            string[]? fileList = (string[]?)e.Data?.GetData(DataFormats.FileDrop, false);
            if (fileList?.Length == 1)
            {
                LoadManifest(fileList[0]);
            }
        }

        private void OnpropertyGrid1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Reset()
        {
            pwaManifest = new PWAManifest();
            propertyGrid1.SelectedObject = pwaManifest;
            pictureBox1.Image = null;
            textBoxFolder.Text = string.Empty;
            textBoxManifest.Text = "manifest.webmanifest";
        }

        private void OnnewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void OnaboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutBox = new AboutBox1
            {
                StartPosition = FormStartPosition.CenterParent
            };
            aboutBox.ShowDialog(this);
        }

        private void OnexitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnbuttonExportManifest_Click(object sender, EventArgs e)
        {
            ExportManifest();
        }

        private void SetWorkingDirectory(string directory)
        {
            textBoxFolder.Text = directory;
            fileSystemWatcher1.Path = directory;
            PopulateTree();
        }

        private void LoadManifest(string fileName)
        {
            try
            {
                string rootDir = textBoxFolder.Text;
                if (string.IsNullOrWhiteSpace(rootDir))
                {
                    rootDir = Path.GetDirectoryName(fileName)!;
                }

                string relativePath = Path.GetRelativePath(rootDir, fileName);
                if (Path.IsPathFullyQualified(relativePath) || relativePath.Contains("..\\"))
                {
                    MessageBox.Show($"{fileName} is not under ${rootDir} folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                pwaManifest = PWAManifest.FromJson(File.ReadAllText(fileName)) ?? new PWAManifest();
                propertyGrid1.SelectedObject = pwaManifest;
                textBoxManifest.Text = relativePath;
                SetWorkingDirectory(rootDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportManifest()
        {
            if (pwaManifest == null)
            {
                MessageBox.Show("Invalid manifest configuration.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxFolder.Text) || !Directory.Exists(textBoxFolder.Text))
            {
                MessageBox.Show("Please specify an existing dir.");
                return;
            }

            var path = Path.Combine(textBoxFolder.Text, textBoxManifest.Text);
            saveFileDialog1.Filter = $"PWA manifest files (*.webmanifest;*.json)|*.webmanifest;*.json|All files|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(path);
            saveFileDialog1.FileName = Path.GetFileName(path);
            if (string.IsNullOrWhiteSpace(saveFileDialog1.FileName))
            {
                saveFileDialog1.FileName = "manifest.webmanifest";
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string manifestFile = saveFileDialog1.FileName;

                string[] ValidExtensions = [".webmanifest", ".json"];

                if (!ValidExtensions.Contains(Path.GetExtension(manifestFile).ToLower()))
                {
                    MessageBox.Show("Please specify a valid manifest file name, such as manifest.webmanifest or manifest.json.");
                    return;
                }

                textBoxManifest.Text = Path.GetRelativePath(textBoxFolder.Text, manifestFile);

                try
                {
                    string json = pwaManifest.ToJson();
                    File.WriteAllText(manifestFile, json);
                    MessageBox.Show($"{manifestFile} successfully saved.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OnbuttonImportManifest_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = $"Manifest files (*.webmanifest;*.json)|*.webmanifest;*.json|All files|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.InitialDirectory = textBoxFolder.Text;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                LoadManifest(openFileDialog1.FileName);
            }
        }

        private void OnfileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            UpdateTreeView(null, null);
        }

        private void OnfileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            UpdateTreeView(null, null);
        }

        private void OnfileSystemWatcher1_Renamed(object sender, RenamedEventArgs e)
        {
            UpdateTreeView(e.OldFullPath, e.FullPath);
        }

        private void UpdateTreeView(string? oldFilename, string? newFilename)
        {
            //Get all checked nodes
            var checkedNodes = GetCheckedTreeNodes(treeViewAssets.Nodes);
            var checkedFiles = checkedNodes.Select(x => (x.Tag as FileInfo)?.FullName?.ToLower()).Where(x => x != null).ToList();
            if ((oldFilename != null) && (newFilename != null))
            {
                if (checkedFiles.Remove(oldFilename.ToLower()))
                {
                    checkedFiles.Add(newFilename.ToLower());
                }
            }

            PopulateTree();

            //Restore all checked nodes if available
            var nodes = GetAllTreeNodes(treeViewAssets.Nodes);
            foreach (var node in nodes)
            {
                var fileName = (node.Tag as FileInfo)?.FullName?.ToLower();
                if (fileName != null && checkedFiles.Contains(fileName))
                {
                    node.Checked = true;
                }
            }
        }
    }

    internal class ServiceWorkerAsset
    {
        public string url { get; set; } = "";
        public string hash { get; set; } = "";
    }

    internal class ServiceWorkerManifest
    {
        public List<ServiceWorkerAsset> assets { get; set; } = [];
        public string version { get; set; } = "";

        public static JsonSerializerOptions DefaultSerializationOptions = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    }
}
