using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// In pboxImage, add events MouseUp, MouseDown, MouseMove, Paint
// timer1: used to detect the start of DragMode.DragImage mode
// timer2: used to update coordinates label
// timer3: used to detect user scroll action in listbox
namespace Image_Labeling
{
    public partial class ImageLabeling : Form
    {
        private int brightness = 5;
        private double contrast = 5;
        private int topIndex = 0;                   // Top index value of listbox 
        private bool bImageLabelsModified = false;
        private bool bCursorIsInside = false;
        private bool bDisplayDefectText = true;     // Default is always display defect text with semi-transparent white rectangle at the back
        private Color normalColor = Color.Red;
        private Color undefinedColor = Color.Red;
        private Color othersColor = Color.Red;
        private List<string> extensionList = new List<string>{ ".bmp", ".jpg", ".jpeg", ".png" };
        private string editingRecordFilename = "Image Labeling Editing Record.JSON";
        private string defectDefinitionFilename = "Image Labeling Definition.JSON";
        private string imageLabelsFilename = "Image Labeling Data.JSON";
        private List<string> processedFoldersList = new List<string>();
        private string currentFolder;
        private string currentFile;
        private DefectDefinition[] defectDefinition;
        private ImageLabelsAllFiles imageLabelsAllFiles = new ImageLabelsAllFiles();
        private List<ImageLabelsPerFile> imageLabelsPerFileList = new List<ImageLabelsPerFile>();
        private List<int> labelCountList = new List<int>();             // List of labels count in a specific foler
        private List<string> labelStringList = new List<string>();      // List of filenames in a specific folder
        private List<Label> starsLabelList = new List<Label>();          // List of Windows labels of how many stars in a specific folder

        public ImageLabeling()
        {
            InitializeComponent();
        }

        private void ImageLabeling_Load(object sender, EventArgs e)
        {
            this.Width = 1200;
            this.Height = 800;
            imageLabelsAllFiles.lastAccessedFile = "";
            imageLabelsAllFiles.allLabels = new ImageLabelsPerFile[0];
            if (!ReadEditingRecordFile())
            {
                Application.Exit();
                return;
            }
            if (!ReadDefectDefinitionFile())
            {
                Application.Exit();
                return;
            }

            SetParentChildRelationship();

//            pboxImage.MouseWheel += pboxImage_MouseWheel;
            pboxBackground.MouseWheel += pboxImage_MouseWheel;
            UpdateVariousRectangles();
        }

        private void pboxImage_MouseDown(object sender, MouseEventArgs e)
        {
            pboxImage_MouseDown2(sender, e);
        }

        private void pboxImage_MouseUp(object sender, MouseEventArgs e)
        {
            pboxImage_MouseUp2(sender, e);
        }

        private void pboxImage_MouseMove(object sender, MouseEventArgs e)
        {
            pboxImage_MouseMove2(sender, e);
        }

        private void pboxImage_Paint(object sender, PaintEventArgs e)
        {
            pboxImage_Paint2(sender, e);
        }

        private void ImageLabeling_SizeChanged(object sender, EventArgs e)
        {
            UpdateVariousRectangles();
            UpdateCoordinatesLabel();
        }

        // On mouse down event, timer starts and wait for 0.5 seconds.  If mouse moves not greater than 3 pixels within this 0.5 seconds,
        // it automatically enters DRAGIMAGE mode and cursor is changed to 'hand'.
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Wait time is long enough to set it to DRAGIMAGE mode
            if (bStartCount == true)
            {
                dragMode = DragMode.DragImage;
                this.pboxImage.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            bStartCount = false;
            timer1.Stop();
        }

        private void tbarBrightness_ValueChanged(object sender, EventArgs e)
        {
            UpdateBrightnessAndContrast();
        }

        private void tbarContrast_ValueChanged(object sender, EventArgs e)
        {
            UpdateBrightnessAndContrast();
        }

        private void UpdateBrightnessAndContrast()
        {
            if (srcBitmap == null)
                return;
            brightness = tbarBrightness.Value;
            contrast = tbarContrast.Value / 100.0;
            if (processedBitmap != null)
                processedBitmap.Dispose();          // processedBitmap will be re-generated by basicLinearTransform2()
            basicLinearTransform2(srcBitmap, ref processedBitmap, contrast, brightness);
            pboxImage.Invalidate();
            UpdateCoordinatesLabel();
        }

        private void basicLinearTransform2(Bitmap srcBitmap, ref Bitmap processedBitmap, double contrast, int brightness)
        {
            if (srcBitmap == null)
            {
                MessageBox.Show("srcBitmap is 'null'", "Error should not happen");
                return;
            }
            Image<Bgr, byte> imgOriginal = new Image<Bgr, byte>(srcBitmap);
            Mat imgMat_original = new Mat(imgOriginal.Size, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
            Mat imgMat_corrected = new Mat(imgOriginal.Size, Emgu.CV.CvEnum.DepthType.Cv8U, 3);
            imgMat_original = imgOriginal.Mat;
            imgMat_original.ConvertTo(imgMat_corrected, Emgu.CV.CvEnum.DepthType.Cv8U, contrast, brightness);       // Most important function call in the whole program
            Image<Bgr, byte> imgCorrected = new Image<Bgr, byte>(imgMat_corrected.Bitmap);
            processedBitmap = imgCorrected.ToBitmap();
            imgOriginal.Dispose();
            imgMat_original.Dispose();
            imgMat_corrected.Dispose();
            imgCorrected.Dispose();
        }

        private void SetParentChildRelationship()
        {
            pboxBackground.BackColor = Color.Transparent;

            pboxImage.Parent = pboxBackground;
            tbarBrightness.Parent = pboxBackground;
            tbarContrast.Parent = pboxBackground;
            lblBrightness.Parent = pboxBackground;
            lblContrast.Parent = pboxBackground;
            lblCoordinates.Parent = pboxBackground;
            lblProcessedFolders.Parent = pboxBackground;
            cboxProcessedFolders.Parent = pboxBackground;
            lblCurrentFolderText.Parent = pboxBackground;
            lblCurrentFolder.Parent = pboxBackground;
            lboxFilesList.Parent = pboxBackground;

            lblStars1.Parent = pboxBackground;
            lblStars2.Parent = pboxBackground;
            lblStars3.Parent = pboxBackground;
            lblStars4.Parent = pboxBackground;
            lblStars5.Parent = pboxBackground;

            lblTotalProcessed.Parent = pboxBackground;
            lblDisplayScale.Parent = pboxImage;
        }

        // processedFoldersList --> "Image Labeling Editing Record.JSON"
        private bool SaveEditingRecordFile()
        {
            try
            {
                // Prepare file content
                EditingRecordObject localObject = new EditingRecordObject();
                localObject.lastAccessedFolder = currentFolder;
                localObject.processedFolders = processedFoldersList.ToArray();
                string textJSON = JsonConvert.SerializeObject(localObject);

                string strLocalJSON = Directory.GetCurrentDirectory() + "\\" + editingRecordFilename;
                File.WriteAllText(strLocalJSON, textJSON, Encoding.UTF8);
            }
            catch (Exception expt)
            {
                MessageBox.Show(expt.Message, "Error in Save Editing Record!!");
                return false;
            }
            return true;
        }

        // "Image Labeling Editing Record.JSON" --> processedFoldersList
        private bool ReadEditingRecordFile()
        {
            string lastAccessedFolder = "";
            try
            {
                string textJSON = "";
                string strLocalJSON = Directory.GetCurrentDirectory() + "\\" + editingRecordFilename;
                try
                {
                    textJSON = File.ReadAllText(strLocalJSON);
                }
                catch (Exception expt)          // If file not found or locked by other process, will be 'catched' here
                {
                    // Still return 'true' to continue program execution
                    return true;
                }

                EditingRecordObject localObject = JsonConvert.DeserializeObject<EditingRecordObject>(textJSON);
                processedFoldersList = new List<string>(localObject.processedFolders);        // Convert string[] to a 'List' of strings
                lastAccessedFolder = localObject.lastAccessedFolder;
            }
            catch (Exception expt)
            {
                MessageBox.Show(expt.Message, "Error in json file \"" + editingRecordFilename + "\"");
                return false;
            }

            // According to processedFoldersList, fill in contents to ProcessedFoldersCombobox
            UpdateProcessedFoldersCombobox();              // processedFoldersList --> cboxProcessedFolders

            ChangeToThisFolder(lastAccessedFolder);         // Will call Save/ReadAllLabels(), ChangeToiThisFile()-->Save/ReadOneImageLabel()

            return true;
        }

        private bool ReadDefectDefinitionFile()
        {
            Dictionary<string, Color> colorTable = new Dictionary<string, Color>();
            colorTable.Add("black", Color.Black);
            colorTable.Add("white", Color.White);
            colorTable.Add("gray", Color.Gray);
            colorTable.Add("red", Color.Red);
            colorTable.Add("green", Color.Green);
            colorTable.Add("blue", Color.Blue);
            colorTable.Add("orange", Color.Orange);
            colorTable.Add("purple", Color.Purple);
            colorTable.Add("pink", Color.Pink);
            colorTable.Add("yellow", Color.Yellow);
            colorTable.Add("lightgray", Color.LightGray);
            colorTable.Add("lightgreen", Color.LightGreen);
            colorTable.Add("lightblue", Color.LightBlue);
            colorTable.Add("darkgray", Color.DarkGray);
            colorTable.Add("darkred", Color.DarkRed);
            colorTable.Add("darkgreen", Color.DarkGreen);
            colorTable.Add("darkblue", Color.DarkBlue);

            try
            {
                string textJSON = "";
                string strLocalJSON = Directory.GetCurrentDirectory() + "\\" + defectDefinitionFilename;
                try
                {
                    textJSON = File.ReadAllText(strLocalJSON);
                }
                catch (Exception expt)          // If file not found or locked by other process, will be 'catched' here
                {
                    MessageBox.Show(expt.Message, "Cannot find file");
                    return false;
                }

                DefectDefinitionObject jsonLocal = JsonConvert.DeserializeObject<DefectDefinitionObject>(textJSON);
                defectDefinition = jsonLocal.defectDefinition;
                if (defectDefinition.Length == 0)
                    MessageBox.Show("No defect definition is found", "Definition Error");       // Alert error message only, no need to quit
                if (!String.IsNullOrEmpty(jsonLocal.NormalColor))
                {
                    if (colorTable.ContainsKey(jsonLocal.NormalColor.ToLower()))
                        normalColor = colorTable[jsonLocal.NormalColor.ToLower()];                // Override the normalColor
                }
                if (!String.IsNullOrEmpty(jsonLocal.UndefinedColor))
                {
                    if (colorTable.ContainsKey(jsonLocal.UndefinedColor.ToLower()))
                        undefinedColor = colorTable[jsonLocal.UndefinedColor.ToLower()];          // Override the undefinedColor
                }
                if (!String.IsNullOrEmpty(jsonLocal.OthersColor))
                {
                    if (colorTable.ContainsKey(jsonLocal.OthersColor.ToLower()))
                        othersColor = colorTable[jsonLocal.OthersColor.ToLower()];                // Override the othersColor
                }
            }
            catch (Exception expt)
            {
                MessageBox.Show(expt.Message, "Error in json file \"" + defectDefinitionFilename + "\"");   // JSON error, need to quit
                return false;
            }
            return true;
        }

        private void chooseFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string folderName = folderDialog.SelectedPath;
                ChangeToThisFolder(folderName);
            }
        }

        // Read the file "Image Labeling Data.JSON" under 'folderName' and update imageLabelsPerFileList list
        // Two cases: 1. Called from ReadEditingRecordFile --> 
        //            2. Called from ChooseFolder dialog box --> 
        private bool ChangeToThisFolder(string folderName)
        {
            if (folderName == null)
                return false;

            if (String.IsNullOrEmpty(folderName))
                return false;

            if (Directory.Exists(folderName))
            {
                try
                {
                    SaveAllLabels(currentFolder);                // Will reference current 'currentFolder', save to "Image Labeling Data.JSON" file
                    currentFolder = folderName;
                    bImageLabelsModified = false;
                    string lastAccessedFile = ReadAllLabels(currentFolder);     // Will read "Image Labeling Data.JSON" in new currentFolder, and update imageLabelsPerFileList

                    // Update entries of listbox
                    string supportedExtensions = "*.jpg,*.jpeg,*.png,*.bmp";
                    var files = Directory.GetFiles(folderName, "*.*", SearchOption.TopDirectoryOnly).Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()));
                    lboxFilesList.Items.Clear();
                    labelCountList.Clear();
                    labelStringList.Clear();
                    List<string> noLabelsStringList = new List<string>();       // A list (for filename) to store entries which has no labels defined
                    List<int> noLabelsCountList = new List<int>();              // A list (for count) to store entries which has no labels defined
                    foreach (string file in files)
                    {
                        string ff = Path.GetFileName(file);
                        int labelCount = GetLabelNumbers(ff);
                        if (labelCount > 0)
                        {
                            labelStringList.Add(ff);
                            labelCountList.Add(labelCount);
                        }
                        else
                        {
                            noLabelsStringList.Add(ff);
                            noLabelsCountList.Add(labelCount);          // Should be all zeros
                        }
                    }
                    labelStringList.AddRange(noLabelsStringList);       // Other options are .Concat() and .Union()
                    labelCountList.AddRange(noLabelsCountList);
                    // Better set the list data source to point to labelStringList
                    foreach (string filename in labelStringList)
                        lboxFilesList.Items.Add(filename);          // lboxFilesList.Items.Add(filename + "  " + labelCountList[i++]);

                    // Update pboxImage
                    ChangeToThisFile(currentFolder, lastAccessedFile);

                    // Update selected item in the listbox
                    int index = lboxFilesList.FindString(lastAccessedFile);
                    if (index != -1)
                        lboxFilesList.SetSelected(index, true);

                    // Update the text content in lblCurrentFolder
                    UpdateFolderNameText(currentFolder, currentFile);   

                    // Update cboxProcessedFolders
                    if (!processedFoldersList.Contains(currentFolder))
                    {
                        processedFoldersList.Add(currentFolder);
                        processedFoldersList.Sort();
                        UpdateProcessedFoldersCombobox();
                    }

                    // Update 'stars' label
                    UpdateStarsLabel();
                    UpdateTotalProcessedLabel();

                    return true;
                }
                catch (Exception expt)
                {
                    string message = expt.Message;
                    MessageBox.Show(message, "Folder access error");
                    return false;
                }
            }
            else                // Folder does not exist, user might have deleted or moved the folder
            {
                DialogResult userResponse = MessageBox.Show($"Folder {folderName} does not exist!!\n\nPress 'Yes' to delete this entry or 'No' to skip ", "Folder Error", MessageBoxButtons.YesNo);
                if (userResponse == DialogResult.Yes)
                {
                    int index = processedFoldersList.IndexOf(folderName);
                    processedFoldersList.RemoveAt(index);
                    UpdateProcessedFoldersCombobox();          // processedFoldersList --> cboxProcessedFolders
                    SaveEditingRecordFile();                    // processedFoldersList --> file
                }
                else if (userResponse == DialogResult.No)
                {
                    // Do nothing
                }
                return false;
            }
        }

        private void UpdateStarsLabel()
        {
            topIndex = lboxFilesList.TopIndex;
            for (int i=0; i<starsLabelList.Count; i++)
            {
                if (i+topIndex < labelCountList.Count)
                    starsLabelList[i].Text = GetStarString(labelCountList[i + topIndex]);
                else
                    starsLabelList[i].Text = "";
            }
        }

        private string GetStarString(int count)
        {
            string stars = "";

            if (count <= 6)
            {
                for (int i = 0; i < count; i++)
                    stars += "★";
            }
            else
                stars = "★★ Many ★★";
            return stars;
        }

        // processedFoldersList --> cboxProcessedFolders combobox
        private void UpdateProcessedFoldersCombobox()
        {
            cboxProcessedFolders.Items.Clear();
            foreach (string strFolderName in processedFoldersList)
                cboxProcessedFolders.Items.Add(strFolderName);
        }

        private void UpdateFolderNameText(string folderName, string fileName)
        {
            lblCurrentFolder.Text = folderName + "\\" + fileName;
        }

        private int GetLabelNumbers(string filename)
        {
            foreach (ImageLabelsPerFile imageLabel in imageLabelsPerFileList)
            {
                if (String.Equals(imageLabel.filename, filename))
                    return imageLabel.imageLabels.Length;
            }
            return 0;
        }

        // Access imageLabelsPerFileList list and update imageLabelsList
        private bool ChangeToThisFile(string folderName, string fileName)
        {
            string fullFilename = folderName + "\\" + fileName;
            string strExtension = Path.GetExtension(fileName);

            bImageLabelsModified = true;
            if (bImageLabelsModified == true)           // Note that imageLabelsList.Count may be zero, happen when user delete all labels previously defined
            {
                SaveOneImageLabel(currentFile);         // imageLabelsList --> imageLabelsPerFileList (All are in memory, not file)
                SaveAllLabels(currentFolder);
                ReadAllLabels(currentFolder);
//kok                Update
                UpdateStarsLabel();
            }
            if (srcBitmap != null)
                srcBitmap.Dispose();
            srcBitmap = null;                           // Should set to null, UpdateImageViewableRectAndCloneRect() will reference it
            if (processedBitmap != null)
                processedBitmap.Dispose();
            activeRectangle = -1;
            displayScale = 1;
            cloneRect.X = 0;
            cloneRect.Y = 0;
            currentFile = fileName;
            tbarBrightness.Value = 0;
            tbarContrast.Value = 100;
            bImageLabelsModified = false;
            imageLabelsList.Clear();
            if (File.Exists(fullFilename))
            {
                if (extensionList.Contains(strExtension))
                {
                    try
                    {
                        srcBitmap = new Bitmap(fullFilename);
                        processedBitmap = (Bitmap)srcBitmap.Clone();
                        ReadOneImageLabel(currentFile);             // Note that currentFile is just updated, imageLabelsPerFileList --> imageLabelsList
                    }
                    catch (Exception expt)
                    {
                        MessageBox.Show(expt.Message, "File Format Error");
                    }
                }
            }

            //imageLabelsList.Sort()
            UpdateImageViewableRectAndCloneRect();
            UpdateDisplayScaleLabel();
            UpdateFolderNameText(currentFolder, currentFile);
            pboxImage.Invalidate();
            return true;
//            return false;
        }

        // imageLabelsList --> imageLabelsPerFileList (All are in memory, not file)
        private void SaveOneImageLabel(string filename)
        {
            // Search the list recorded by imageLabelsPerFileList, if file exist --> update, if file not exist --> append
            foreach (ImageLabelsPerFile imagePerFile in imageLabelsPerFileList)
            {
                if (String.Equals(filename, imagePerFile.filename))
                {
                    // Replace the list with updated labels
                    int index = imageLabelsPerFileList.IndexOf(imagePerFile);
                    imageLabelsPerFileList.RemoveAt(index);
                    if (imageLabelsList.Count > 0)          // Some labels are left, then insert back to the imageLabelsPerFileList
                    {
                        ImageLabelsPerFile newImageLabelPerFile = new ImageLabelsPerFile();
                        newImageLabelPerFile.filename = filename;
                        newImageLabelPerFile.imageLabels = imageLabelsList.ToArray();
                        imageLabelsPerFileList.Insert(index, newImageLabelPerFile);
                    }
                    return;
                }
            }

            // 'filename' does not exist in the list, append at the end if one or more labels have been defined
            if (imageLabelsList.Count > 0)          // Some labels defined
            {
                ImageLabelsPerFile newImageLabelPerFile = new ImageLabelsPerFile();
                newImageLabelPerFile.filename = filename;
                newImageLabelPerFile.imageLabels = imageLabelsList.ToArray();
                imageLabelsPerFileList.Add(newImageLabelPerFile);
            }
        }

        // imageLabelsPerFileList --> imageLabelsList (All are in memory, not file)
        private void ReadOneImageLabel(string filename)
        {
            foreach (ImageLabelsPerFile imagePerFile in imageLabelsPerFileList)
            {
                if (String.Equals(filename, imagePerFile.filename))
                {
                    imageLabelsList = new List<ImageLabelsList>(imagePerFile.imageLabels);
                    break;
                }
            }
        }

        // imageLabelsPerFileList --> File save
        // Saves lastAccessedFile and imageLabelsPerFileList to the file "Image Labeling Data.JSON", will be called when changing 'working folder'
        private void SaveAllLabels(string folderName)
        {
            if (folderName == null)
                return;

            SaveOneImageLabel(currentFile);             // In case user changes folder immediately after defining a label

            string imageLabelsFullFilename = folderName + "\\" + imageLabelsFilename;

            int count = imageLabelsAllFiles.allLabels.Count();
            ImageLabelsAllFiles newImageLabelsAllFiles = new ImageLabelsAllFiles();


            newImageLabelsAllFiles.lastAccessedFile = currentFile;
            newImageLabelsAllFiles.allLabels = imageLabelsPerFileList.ToArray();

            string jsonText = JsonConvert.SerializeObject(newImageLabelsAllFiles);
            File.WriteAllText(imageLabelsFullFilename, jsonText);
        }

        // File read --> imageLabelsPerFileList
        // Read the file "Image Labeling Data.JSON", which contains "lastAccessedFile", "filename + imageLabels[]", "filename + imageLabels[]", ...
        // Read the image labeling data for this folder, will be called when changing 'working folder'
        private string ReadAllLabels(string folderName)
        {
            string imageLabelsFullFilename = folderName + "\\" + imageLabelsFilename;       // folder name + "Image Labeling Data.JSON"

            if (File.Exists(imageLabelsFullFilename))
            {
                string jsonText = File.ReadAllText(imageLabelsFullFilename);
                try
                {
                    imageLabelsAllFiles = JsonConvert.DeserializeObject<ImageLabelsAllFiles>(jsonText);
                }
                catch (Exception expt)
                {
                    MessageBox.Show(expt.Message, "JSON error");
                }
                imageLabelsPerFileList.Clear();
                imageLabelsPerFileList = new List<ImageLabelsPerFile>(imageLabelsAllFiles.allLabels);   // Convert array to list
                return imageLabelsAllFiles.lastAccessedFile;
            }
            else
            {
                imageLabelsPerFileList.Clear();
                return "";
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            UpdateCoordinatesLabel();
        }

        private void lboxFilesList_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void lboxFilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFilename = lboxFilesList.SelectedItem.ToString();
            ChangeToThisFile(currentFolder, selectedFilename);
        }

        private void cboxProcessedFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(cboxProcessedFolders.Text))
            {
                string newFolder = cboxProcessedFolders.Text;
                ChangeToThisFolder(newFolder);
            }
        }

        private void ImageLabeling_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAllLabels(currentFolder);
            SaveEditingRecordFile();
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (topIndex != lboxFilesList.TopIndex)
                UpdateStarsLabel();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About_Box aboutBox = new Image_Labeling.About_Box();
            aboutBox.ShowDialog();
        }

        private void pboxImage_MouseEnter(object sender, EventArgs e)
        {
            if (bCursorIsInside == false)
            {
                bCursorIsInside = true;
                InvalidateActiveCorners();
                UpdateCoordinatesLabel();
            }
        }

        private void pboxImage_MouseLeave(object sender, EventArgs e)
        {
            if (bCursorIsInside == true)
            {
                bCursorIsInside = false;
                InvalidateActiveCorners();
                UpdateCoordinatesLabel();
            }
        }

        private void InvalidateActiveCorners()
        {
            int width = 15;
            int height = 15;
            Rectangle rect = new Rectangle();

            rect.Width = width;
            rect.Height = height;

            rect.X = 0;
            rect.Y = 0;
            pboxImage.Invalidate(rect);             // Invalidate the upper left corner
            rect.X = pboxImage.Width - width;
            pboxImage.Invalidate(rect);             // Invalidate the upper right corner
            rect.Y = pboxImage.Height - height;
            pboxImage.Invalidate(rect);             // Invalidate the lower right corner
            rect.X = 0;
            pboxImage.Invalidate(rect);             // Invalidate the lower left corner
        }

        private void shortcutKeystoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Shortcut_keys shortcutKeys = new Shortcut_keys();
            shortcutKeys.ShowDialog();
        }
    }
}
