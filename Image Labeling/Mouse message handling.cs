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

namespace Image_Labeling
{
    public partial class ImageLabeling : Form
    {
        enum DragMode { DragNone, DragImage, DrawRectangle, DragRectangle, DragLeftEdge, DragTopEdge, DragRightEdge, DragBottomEdge,
                        DragUpperLeftCorner, DragUpperRightCorner, DragLowerLeftCorner, DragLowerRightCorner };
        enum FourEdges { LeftEdge, TopEdge, RightEdge, BottomEdge, NotAnEdge, DragEdge };
        enum FourCorners { UpperLeftCorner, UpperRightCorner, LowerLeftCorner, LowerRightCorner, NotACorner };

        private const int KBIMAGESTEP = 10;                 // Step size when using arrow keys to move the image
        private const int KBRECTANGLESTEP = 3;              // Step size when using arrow keys to move active rectangle
        private const int KBENLARGESTEP = 2;                // Step size when using Ctrl+arrow keys to enlarge/shrink the active rectangle
        private const int DETECTLINEWIDTH = 15;              // Distance to find an edge of a rectangle
        private const int MINRECTWIDTH = 20;                // Minimum width of a field (rectangle)
        private const int MINRECTHEIGHT = 25;               // Minimum height of a field (rectangle)
        public const int MAXROWNUMBER = 25;                 // Maximum number of rows permitted in multi-row/column define field
        public const int MAXCOLUMNNUMBER = 20;              // Maximum number of columns permitted in multi-row/column define field
        private const int IGNOREWIDTH = 5;                  // Distance between mouse down and mouse up will be ignored, essentially will make activeRectangle -1
        private const int IGNOREHEIGHT = 5;                 // Distance between mouse down and mouse up will be ignored, essentially will make activeRectangle -1
        private const int ACTIVECORNER = 7;                 // Dimension of the four corners of active rectangle
        public const int MAXENTRYDISPLAY = 11;              // Maximum number of defined entried to display on define form, above this number will add vertical scroll bar
        private const float MINIMUMDISPLAYSCALE = (float)0.2;
        private const float MAXIMUMDISPLAYSCALE = (float)2.0;
        private int mouseX = 0, mouseY = 0;                 // Description in DrawImageLabelingScreen()
        private int dragX = 0, dragY = 0;                   // Description in DrawImageLabelingScreen()
        private int dispX = 0, dispY = 0;                   // Description in DrawImageLabelingScreen()
        private int rectX = 0, rectY = 0;                   // Description in DrawImageLabelingScreen()
        private int rectWidth = 0, rectHeight = 0;          // Description in DrawImageLabelingScreen()
        private DragMode dragMode = DragMode.DragNone;
        private float displayScale = (float)1.0;            // Controls the scale of the image in define form screen, it's limit from 0.2 to 2.0
        private bool bStartCount = false;
        private bool imageIsVisible = true;                 // Determines display or hide the image
        private int activeRectangle = -1;                   // Active rectangle index, -1 means background image is active
        private int displayStartIndex = 0;                  // When number of entries is over MAXENTRYDISPLAY, vscroll bar is shown, and user may scroll up
        private int leftEntry = 0;
        private int topEntry = 0;
        private int widthEntry = 0;
        private int heightEntry = 0;
        private Bitmap srcBitmap = null;                    // For pboxImage use
        private Bitmap processedBitmap = null;              // For pboxImage use, normally after brightness and contrast processing
        private Point ptEditField = new Point(0, 0);
        private Rectangle resizeRect = new Rectangle(0, 0, 0, 0);   // Description in DrawImageLabelingScreen()
        private Rectangle cloneRect = new Rectangle(0, 0, 0, 0);    // Description in DrawImageLabelingScreen()
        private Rectangle imageViewableRect = new Rectangle();      // Description in DrawImageLabelingScreen()

        private List<ImageLabelsList> imageLabelsList = new List<ImageLabelsList>();


        private void pboxImage_MouseDown2(object sender, MouseEventArgs e)
        {
            int screenX, screenY;           // A point with coordinates relative to upper left of imageViewableRect
            int imageX, imageY;             // Equivalent coordinates of (screenX,screenY) relative to image (no scaling is applied)
            
            mouseX = e.X;                                               // Use by lblCoordinates to display (x,y) of cursor
            mouseY = e.Y;                                               // Use by lblCoordinates to display (x,y) of cursor
            screenX = e.X - imageViewableRect.Left;                     // Coordinates relative to upper left of imageViewableRect
            screenY = e.Y - imageViewableRect.Top;                      // Coordinates relative to upper left of imageViewableRect
            imageX = cloneRect.Left + (int)(screenX / displayScale);    // Coordinates relative to upper left of image in 100% scale
            imageY = cloneRect.Top + (int)(screenY / displayScale);     // Coordinates relative to upper left of image in 100% scale

            // Ignore this mouse down message if it's outside the image display area
            if (!imageViewableRect.Contains(e.X, e.Y))
                return;
            
            if (e.Button == MouseButtons.Left)
            {
                // Detect whether the 'control' key is depressed when mouse down occurs, if yes, enter DRAGIMAGE mode immediately
                if (ModifierKeys.HasFlag(Keys.Control))
                {
                    dragMode = DragMode.DragImage;
                    dragX = imageX;
                    dragY = imageY;
                }
                else
                {
                    if (activeRectangle != -1)
                    {
                        ImageLabelsList imageLabel = imageLabelsList[activeRectangle];
                        FourEdges edge = FindEdges(imageX, imageY, imageLabel.left, imageLabel.top, imageLabel.width, imageLabel.height);
                        FourCorners corner = FindCorners(imageX, imageY, imageLabel.left, imageLabel.top, imageLabel.width, imageLabel.height);
                        if (corner != FourCorners.NotACorner)       // Detect corners first, then detect edges, remember this order, important
                        {
                            if (corner == FourCorners.UpperLeftCorner)
                                dragMode = DragMode.DragUpperLeftCorner;
                            else if (corner == FourCorners.UpperRightCorner)
                                dragMode = DragMode.DragUpperRightCorner;
                            else if (corner == FourCorners.LowerLeftCorner)
                                dragMode = DragMode.DragLowerLeftCorner;
                            else                // corner = FourCorners.LowerRightCorner
                                dragMode = DragMode.DragLowerRightCorner;
                            resizeRect.X = imageLabel.left;
                            resizeRect.Y = imageLabel.top;
                            resizeRect.Width = imageLabel.width;
                            resizeRect.Height = imageLabel.height;
                            dragX = imageX;
                            dragY = imageY;
                            return;
                        }
                        else if (edge != FourEdges.NotAnEdge)
                        {
                            if (edge == FourEdges.LeftEdge)
                                dragMode = DragMode.DragLeftEdge;
                            else if (edge == FourEdges.RightEdge)
                                dragMode = DragMode.DragRightEdge;
                            else if (edge == FourEdges.TopEdge)
                                dragMode = DragMode.DragTopEdge;
                            else if (edge == FourEdges.BottomEdge)
                                dragMode = DragMode.DragBottomEdge;
                            else                // edge = FourEdges.DragEdge
                                dragMode = DragMode.DragRectangle;
                            resizeRect.X = imageLabel.left;
                            resizeRect.Y = imageLabel.top;
                            resizeRect.Width = imageLabel.width;
                            resizeRect.Height = imageLabel.height;
                            dragX = imageX;
                            dragY = imageY;
                            return;
                        }
                    }

                    dragMode = DragMode.DrawRectangle;
                    bStartCount = true;
                    timer1.Start();                     // Start counting time
                    rectX = dragX = imageX;             // Use coordinates relative to image in 100% scale,     no need to include dragX
                    rectY = dragY = imageY;             // Use coordinates relative to image in 100% scale
                    rectWidth = rectHeight = 0;
                }
            }
            
            UpdateCoordinatesLabel();
        }

        private void pboxImage_MouseUp2(object sender, MouseEventArgs e)
        {
            int screenX, screenY;           // A point with coordinates relative to upper left of imageViewableRect
            int imageX, imageY;             // Equivalent coordinates of (screenX,screenY) relative to image (no scaling is applied)
            
            mouseX = e.X;                                               // Use by lblCoordinates to display (x,y) of cursor
            mouseY = e.Y;                                               // Use by lblCoordinates to display (x,y) of cursor
            screenX = e.X - imageViewableRect.Left;                     // Coordinates relative to upper left of imageViewableRect
            screenY = e.Y - imageViewableRect.Top;                      // Coordinates relative to upper left of imageViewableRect
            imageX = cloneRect.Left + (int)(screenX / displayScale);    // Coordinates relative to upper left of image in 100% scale
            imageY = cloneRect.Top + (int)(screenY / displayScale);     // Coordinates relative to upper left of image in 100% scale

            if (e.Button == MouseButtons.Left)
            {
                if (srcBitmap == null)                          // Let user draw rectangle even if there's no image selected
                {
                    activeRectangle = -1;
                    this.pboxImage.Cursor = System.Windows.Forms.Cursors.Default;
                }
                else if (dragMode == DragMode.DragNone)
                {
                    activeRectangle = -1;
                    this.pboxImage.Cursor = System.Windows.Forms.Cursors.Default;
                    pboxBackground.Invalidate();
                }
                else if (dragMode == DragMode.DrawRectangle)
                {
                    // Detect if the distance between mouse up and mouse down is too close, say less that 15 pixels, ignore this DRAWRECTANGLE operation
                    if (Math.Abs(rectWidth) < MINRECTWIDTH || Math.Abs(rectHeight) < MINRECTHEIGHT)
                    {
                        if (Math.Abs(rectWidth) <= IGNOREWIDTH && Math.Abs(rectHeight) <= IGNOREHEIGHT)  // If user clicks nearly at the same point, just ignore this operation
                        {
                            // Treat as a simple click on the background or some of the rectangles
                            activeRectangle = GetActiveRectangle(imageX, imageY);
//                            if (activeRectangle != -1)
//                                MakeActiveEntryVisible();
//                            SetAllDynamicLabelColor();
                        }
                        else
                            MessageBox.Show($"Minimum width is {MINRECTWIDTH} pixels and minimum height is {MINRECTHEIGHT} pixels!");
                        dragMode = DragMode.DragNone;
                        this.pboxImage.Cursor = System.Windows.Forms.Cursors.Default;
                        bStartCount = false;
                        timer1.Stop();
                        UpdateCoordinatesLabel();
                        pboxBackground.Invalidate();
                        return;
                    }

//                    Edit_Field_Name editField = new Edit_Field_Name();
                    int left, top, width, height, defineType, rowNumber, columnNumber;
                    int x = e.X + pboxBackground.Left + pboxImage.Left;
                    int y = e.Y + pboxBackground.Top + pboxImage.Top;
                    Rectangle nRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);
                    GetUprightRect(ref nRect);
                    // Prevent rectangle is outside either of the four edges of srcBitmap
                    bool needToRefresh = false;
                    if (nRect.X < 0)
                    {
                        nRect.Width = nRect.Width + nRect.X;
                        nRect.X = 0;
                        needToRefresh = true;
                    }
                    if (nRect.Y < 0)
                    {
                        nRect.Height = nRect.Height + nRect.Y;
                        nRect.Y = 0;
                        needToRefresh = true;
                    }
                    if (nRect.X + nRect.Width >= srcBitmap.Width)
                    {
                        nRect.Width = nRect.Width - (nRect.X + nRect.Width - srcBitmap.Width) - 2;
                        needToRefresh = true;
                    }
                    if (nRect.Y + nRect.Height >= srcBitmap.Height)
                    {
                        nRect.Height = nRect.Height - (nRect.Y + nRect.Height - srcBitmap.Height) - 2;
                        needToRefresh = true;
                    }
                    if (needToRefresh)
                    {
                        rectX = nRect.X;
                        rectY = nRect.Y;
                        rectWidth = nRect.Width;
                        rectHeight = nRect.Height;
                        pboxImage.Invalidate();
                    }

                    left = nRect.X;           // Transform the coordinates relative to srcBitmap
                    top = nRect.Y;
                    width = nRect.Width;
                    height = nRect.Height;
                    leftEntry = left;               // Will be referenced in MenuItem_Click()
                    topEntry = top;                 // Will be referenced in MenuItem_Click()
                    widthEntry = width;             // Will be referenced in MenuItem_Click()
                    heightEntry = height;           // Will be referenced in MenuItem_Click()
                                                    //                    Point ptStart = PointToScreen(new Point(x, y));

                    // DisplayContextMenu() will create a context menu, display to user, if user chooses an item, will be handled by MenuItem_Click() directly
                    DisplayContextMenu(screenX+imageViewableRect.Left-18, screenY+imageViewableRect.Top-16);
//                    activeRectangle = imageLabelsList.Count;                    // Set active rectangle to the newly added rectangle
  //                  AddOneImageLabelEntry(left, top, width, height, "Blank description");

                    pboxBackground.Invalidate();
                }
                else if (dragMode == DragMode.DragImage)
                {
                    cloneRect.X = cloneRect.X - dispX;
                    cloneRect.Y = cloneRect.Y - dispY;
                    if (cloneRect.X < 0)
                        cloneRect.X = 0;
                    else if (cloneRect.X + cloneRect.Width >= srcBitmap.Width)
                        cloneRect.X = srcBitmap.Width - cloneRect.Width - 1;
                    if (cloneRect.Y < 0)
                        cloneRect.Y = 0;
                    else if (cloneRect.Y + cloneRect.Height >= srcBitmap.Height)
                        cloneRect.Y = srcBitmap.Height - cloneRect.Height - 1;
                    dispX = 0;
                    dispY = 0;

                    pboxBackground.Invalidate();
                }
                else if (dragMode == DragMode.DragLeftEdge || dragMode == DragMode.DragTopEdge || dragMode == DragMode.DragRightEdge ||
                         dragMode == DragMode.DragBottomEdge || dragMode == DragMode.DragUpperLeftCorner || dragMode == DragMode.DragUpperRightCorner ||
                         dragMode == DragMode.DragLowerLeftCorner || dragMode == DragMode.DragLowerRightCorner || dragMode == DragMode.DragRectangle)
                {
                    pboxBackground.Invalidate();           // Update data in the right panel area
                }
                dragMode = DragMode.DragNone;
                bStartCount = false;
                timer1.Stop();
                this.pboxImage.Cursor = System.Windows.Forms.Cursors.Default;
            }
            UpdateCoordinatesLabel();            
        }

        // Create a context menu at runtime in C#
        // https://www.codeproject.com/Questions/302327/Create-a-context-menu-at-runtime-in-Csharp
        //
        // How to add submenu items to menuitems (Very useful)
        // https://stackoverflow.com/questions/16146801/how-to-add-submenu-items-to-menuitems
        private void DisplayContextMenu(int x, int y)
        {
            //            ToolStrip[] ts = new ToolStrip[3];

            int count = defectDefinition.Length;

            if (count == 0)
                return;

            int index = 0;
            MenuItem[] menuItem = new MenuItem[count];

            foreach (DefectDefinition defect in defectDefinition)
            {
                MenuItem defectCategory = new MenuItem(defect.Category);
                foreach (string ss in defect.Items)
                {
                    defectCategory.MenuItems.Add(ss, MenuItem_Click);
                }
                menuItem[index] = defectCategory;
                index++;
            }

/*            MenuItem defectCategory1 = new MenuItem("客廳天花");
            defectCategory1.MenuItems.Add("輕微滲水", MenuItem_Click);
            defectCategory1.MenuItems.Add("有裂痕", MenuItem_Click);
            defectCategory1.MenuItems.Add("有水蹟", MenuItem_Click);
            MenuItem defectCategory2 = new MenuItem("廚房地板");
            defectCategory2.MenuItems.Add("輕微滲水", MenuItem_Click);
            defectCategory2.MenuItems.Add("有裂痕", MenuItem_Click);
            defectCategory2.MenuItems.Add("有水蹟", MenuItem_Click);
            MenuItem defectCategory3 = new MenuItem("露台");
            defectCategory3.MenuItems.Add("輕微滲水", MenuItem_Click);
            defectCategory3.MenuItems.Add("有裂痕", MenuItem_Click);
            defectCategory3.MenuItems.Add("有水蹟", MenuItem_Click);

            MenuItem[] menuItem = new MenuItem[3];
            menuItem[0] = defectCategory1;
            menuItem[1] = defectCategory2;
            menuItem[2] = defectCategory3;*/

            ContextMenu cm = new ContextMenu(menuItem);

            cm.Show(pboxImage, new Point(x, y));            // pboxImage is the control that owns the context menu

            return;
        }

        // If user selects an item from context menu, MenuItem_Click() will be called
        private void MenuItem_Click(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            MenuItem test = new MenuItem();
            MenuItem test2 = new MenuItem();
            if (mi != null)
            {
                string aa = "";
                string bb = "";
                string cc = "";
//                aa = mi.Parent.ToString().Text;
                Menu cctt = mi.Parent;
                test = (MenuItem)mi.Parent;
//                if (typeof(mi.Parent) == MenuItem)
//                if (test.Parent != null)
//                    test2 = (MenuItem)test.Parent;
                aa = cctt.ToString();
                aa = cctt.Name;
                aa = test.Text;
                bb = $"[{aa}] {mi.Text}";
//                bb = $"{aa} → {mi.Text}";
                //                MenuItem ccss = mi.Parent;
                activeRectangle = imageLabelsList.Count;
//                AddOneImageLabelEntry(leftEntry, topEntry, widthEntry, heightEntry, mi.Text);
                AddOneImageLabelEntry(leftEntry, topEntry, widthEntry, heightEntry, bb);
            }
        }

        private void AddOneImageLabelEntry(int left, int top, int width, int height, string strDescription)
        {
            ImageLabelsList imageLabel = new ImageLabelsList();

            imageLabel.left = left;
            imageLabel.top = top;
            imageLabel.width = width;
            imageLabel.height = height;
            imageLabel.labelDescription = strDescription;
            imageLabelsList.Add(imageLabel);
            bImageLabelsModified = true;
            UpdateStarsContent();
        }

        private void DeleteOneImageLabelEntry(int index)
        {
            if (index < 0 || index >= imageLabelsList.Count)
            {
                MessageBox.Show("Incorrect index value", "DeleteOneDefineFormEntry error");
                return;
            }
/*            DefineFormFieldsList fieldList = defineFormFieldsList[index];
            fieldList.dynamicLabel.Dispose();
            this.Controls.Remove(fieldList.dynamicLabel);
            fieldList.dynamicTextbox.Dispose();
            this.Controls.Remove(fieldList.dynamicTextbox);
            fieldList.dynamicCombobox.Dispose();
            this.Controls.Remove(fieldList.dynamicCombobox);
            fieldList.dynamicEditButton.Dispose();
            this.Controls.Remove(fieldList.dynamicEditButton);
            fieldList.dynamicDeleteButton.Dispose();
            this.Controls.Remove(fieldList.dynamicDeleteButton);*/
            imageLabelsList.RemoveAt(index);
            if (activeRectangle == imageLabelsList.Count)      // If the removed item is the last item
                activeRectangle--;                                  // If the removed item is the last and only item in the list, it will be -1

            // Following case will only happen when delete index is smaller than activeRectangle, usually by pressing a dynamic delete button
            // e.g. count=9, activeRectangle=7, delete index=3, then after deletion, activeRectangle should be equal to 6
/*            if (index < activeRectangle)            // If the delete entry is smaller than active rectangle, we need to adjust the active rectangle
                activeRectangle--;
            ReorderDynamicLabelIndex();
            SetAllDynamicLabelColor();*/

//            int count = defineFormFieldsList.Count;
            // Count:             1    2    3    4    5    6    7    8    9    10    11    12    13    14    15    16    17    18 ...
            // Max.:              0    1    2    3    4    5    6    7    8     9    10    11    12    13    14    15    16    17 ...
            // LargeChange:       1    2    3    4    5    6    7    8    9    10    11    11    11    11    11    11    11    11 ...
            // Handle the scroll bar
            /*            if (count <= MAXENTRYDISPLAY)
                        {
                            displayStartIndex = 0;
                        }
                        else
                        {
                            vScrollDefineForm.LargeChange = (count <= MAXENTRYDISPLAY) ? count : MAXENTRYDISPLAY;
                            vScrollDefineForm.Maximum = defineFormFieldsList.Count - 1;
                        }*/
//            MakeActiveEntryVisible();                       // displayStartIndex may be modified, scroll bar handling included
  //          UpdateDynamicControlsCoordinates();             // This will also hide controls outside the visible area
            pboxBackground.Invalidate();
            bImageLabelsModified = true;
            UpdateStarsContent();
        }

        // When labels are added or removed, we update the 'stars' content
        // 
        private void UpdateStarsContent()
        {
           foreach (string filename in labelStringList)
            {
                if (String.Equals(filename, currentFile))
                {
                    int index = labelStringList.IndexOf(filename);
                    labelCountList[index] = imageLabelsList.Count;
                    UpdateStarsLabel();
                    break;
                }
            }
            UpdateTotalProcessedLabel();
        }

        private void UpdateTotalProcessedLabel()
        {
            int processedCount = 0;
            foreach (int count in labelCountList)
            {
                if (count > 0)
                    processedCount++;
            }
            lblTotalProcessed.Text = $"Total: {labelCountList.Count},   Processed: {processedCount}";
        }

        private void pboxImage_MouseMove2(object sender, MouseEventArgs e)
        {
            int screenX, screenY;           // A point with coordinates relative to upper left of imageViewableRect
            int imageX, imageY;             // Equivalent coordinates of (screenX,screenY) relative to image (no scaling is applied)

            mouseX = e.X;                                               // Use by lblCoordinates to display (x,y) of cursor
            mouseY = e.Y;                                               // Use by lblCoordinates to display (x,y) of cursor
            screenX = e.X - imageViewableRect.Left;                     // Coordinates relative to upper left of imageViewableRect
            screenY = e.Y - imageViewableRect.Top;                      // Coordinates relative to upper left of imageViewableRect
            imageX = cloneRect.Left + (int)(screenX / displayScale);    // Coordinates relative to upper left of image in 100% scale
            imageY = cloneRect.Top + (int)(screenY / displayScale);     // Coordinates relative to upper left of image in 100% scale

            if ((dragMode == DragMode.DragLeftEdge || dragMode == DragMode.DragTopEdge || dragMode == DragMode.DragRightEdge ||
                 dragMode == DragMode.DragBottomEdge || dragMode == DragMode.DragUpperLeftCorner || dragMode == DragMode.DragUpperRightCorner ||
                 dragMode == DragMode.DragLowerLeftCorner || dragMode == DragMode.DragLowerRightCorner || dragMode == DragMode.DragRectangle)
                 && activeRectangle == -1)
                MessageBox.Show("Error in activeRectangle");                    // This should not happen

            mouseX = imageX;
            mouseY = imageY;

            if (Math.Abs(dragX - imageX) > 3 || Math.Abs(dragY - imageY) > 3)
            {
                timer1.Stop();
            }
            if (dragMode == DragMode.DragNone)
            {
                #region DragNone
                FourEdges edge;         // = new FourEdges();   ???, like 'new Rectangle()'???
                FourCorners corner;     //  = new FourCorners();  ???
                // Detect the edges and corners of activeRectangle, if there is one
                if (activeRectangle != -1)
                {
                    ImageLabelsList imageLabel = imageLabelsList[activeRectangle];
                    corner = FindCorners(imageX, imageY, imageLabel.left, imageLabel.top, imageLabel.width, imageLabel.height);
                    edge = FindEdges(imageX, imageY, imageLabel.left, imageLabel.top, imageLabel.width, imageLabel.height);
                    if (corner == FourCorners.UpperLeftCorner || corner == FourCorners.LowerRightCorner)
                        pboxImage.Cursor = Cursors.SizeNWSE;
                    else if (corner == FourCorners.UpperRightCorner || corner == FourCorners.LowerLeftCorner)
                        pboxImage.Cursor = Cursors.SizeNESW;
                    else if (edge == FourEdges.TopEdge || edge == FourEdges.BottomEdge)
                        pboxImage.Cursor = Cursors.SizeNS;
                    else if (edge == FourEdges.LeftEdge || edge == FourEdges.RightEdge)
                        pboxImage.Cursor = Cursors.SizeWE;
                    else if (edge == FourEdges.DragEdge)
                        pboxImage.Cursor = Cursors.Hand;

                    // At least one corner point or edge line is detected and cursor already changed
                    if (corner != FourCorners.NotACorner || edge != FourEdges.NotAnEdge)
                        return;
                }
                // Note that the following foreach loop will loop through activeRectangle, nevertheless it has been procesed in the above
                foreach (ImageLabelsList imageLabel in imageLabelsList)
                {
                    corner = FindCorners(imageX, imageY, imageLabel.left, imageLabel.top, imageLabel.width, imageLabel.height);
                    edge = FindEdges(imageX, imageY, imageLabel.left, imageLabel.top, imageLabel.width, imageLabel.height);

                    // At lease one corner point or edge line is detected and cursor changed accordingly
                    if (corner != FourCorners.NotACorner || edge != FourEdges.NotAnEdge)
                    {
                        pboxImage.Cursor = Cursors.Hand;
                        return;
                    }
                }
                // Now the cursor is not on any of edges or corners, so change the cursor back to default
                pboxImage.Cursor = Cursors.Default;
                #endregion
            }
            else if (dragMode == DragMode.DrawRectangle)
            {
                #region DrawRectangle
                rectWidth = imageX - rectX;
                rectHeight = imageY - rectY;
                pboxImage.Invalidate();               // should confine to a small area only
                #endregion
            }
            else if (dragMode == DragMode.DragRectangle)
            {
                #region DragRectangle
                ImageLabelsList imageLabel = imageLabelsList[activeRectangle];
                int minX, maxX, minY, maxY, newX, newY, deltaX, deltaY;
                minX = 0;
                maxX = srcBitmap.Width - resizeRect.Width - 1;
                minY = 0;
                maxY = srcBitmap.Height - resizeRect.Height - 1;
                deltaX = imageX - dragX;
                deltaY = imageY - dragY;
                newX = resizeRect.Left + deltaX;
                newY = resizeRect.Top + deltaY;
                if (newX < minX)
                    deltaX = deltaX + (minX - newX);
                if (newX > maxX)
                    deltaX = deltaX - (newX - maxX);
                if (newY < minY)
                    deltaY = deltaY + (minY - newY);
                if (newY > maxY)
                    deltaY = deltaY - (newY - maxY);
                imageLabel.left = resizeRect.Left + deltaX;
                imageLabel.top = resizeRect.Top + deltaY;
                imageLabelsList[activeRectangle] = imageLabel;
                pboxImage.Invalidate();
                bImageLabelsModified = true;
                #endregion
            }
            else if (dragMode == DragMode.DragLeftEdge || dragMode == DragMode.DragRightEdge || dragMode == DragMode.DragTopEdge || dragMode == DragMode.DragBottomEdge ||
                     dragMode == DragMode.DragUpperLeftCorner || dragMode == DragMode.DragUpperRightCorner || dragMode == DragMode.DragLowerLeftCorner ||
                     dragMode == DragMode.DragLowerRightCorner)
            {
                #region DragFourEdges & DragFourCorners
                ImageLabelsList imageLabel = imageLabelsList[activeRectangle];
                int minLeft, maxLeft, minTop, maxTop, minRight, maxRight, minBottom, maxBottom, deltaX, deltaY, newX, newY;
                minLeft = maxLeft = resizeRect.Left;
                minTop = maxTop = resizeRect.Top;
                minRight = maxRight = resizeRect.Right;
                minBottom = maxBottom = resizeRect.Bottom;
                deltaX = imageX - dragX;
                deltaY = imageY - dragY;

                switch (dragMode)
                {
                    case DragMode.DragLeftEdge:
                        minLeft = 0;
                        maxLeft = resizeRect.Right - MINRECTWIDTH;
                        break;
                    case DragMode.DragTopEdge:
                        minTop = 0;
                        maxTop = resizeRect.Bottom - MINRECTHEIGHT;
                        break;
                    case DragMode.DragRightEdge:
                        minRight = resizeRect.Left + MINRECTWIDTH;
                        maxRight = srcBitmap.Width - 3;
                        break;
                    case DragMode.DragBottomEdge:
                        minBottom = resizeRect.Top + MINRECTHEIGHT;
                        maxBottom = srcBitmap.Height - 3;
                        break;
                    case DragMode.DragUpperLeftCorner:
                        minLeft = 0;
                        maxLeft = resizeRect.Right - MINRECTWIDTH;
                        minTop = 0;
                        maxTop = resizeRect.Bottom - MINRECTHEIGHT;
                        break;
                    case DragMode.DragUpperRightCorner:
                        minRight = resizeRect.Left + MINRECTWIDTH;
                        maxRight = srcBitmap.Width - 3;
                        minTop = 0;
                        maxTop = resizeRect.Bottom - MINRECTHEIGHT;
                        break;
                    case DragMode.DragLowerLeftCorner:
                        minLeft = 0;
                        maxLeft = resizeRect.Right - MINRECTWIDTH;
                        minBottom = resizeRect.Top + MINRECTHEIGHT;
                        maxBottom = srcBitmap.Height - 3;
                        break;
                    case DragMode.DragLowerRightCorner:
                        minRight = resizeRect.Left + MINRECTWIDTH;
                        maxRight = srcBitmap.Width - 3;
                        minBottom = resizeRect.Top + MINRECTHEIGHT;
                        maxBottom = srcBitmap.Height - 3;
                        break;
                }

                // Note that any single operation will not affect both left & right edges or both top & bottom edges
                if (minLeft != maxLeft)                     // != means we can move the left edge
                {
                    newX = resizeRect.Left + deltaX;
                    if (newX < minLeft) deltaX = deltaX + (minLeft - newX);
                    if (newX > maxLeft) deltaX = deltaX - (newX - maxLeft);
                    imageLabel.left = resizeRect.Left + deltaX;
                    imageLabel.width = resizeRect.Width - deltaX;
                }
                if (minRight != maxRight)                   // != means we can move the right edge, better use 'else if'
                {
                    newX = resizeRect.Right + deltaX;
                    if (newX < minRight) deltaX = deltaX + (minRight - newX);
                    if (newX > maxRight) deltaX = deltaX - (newX - maxRight);
                    imageLabel.width = resizeRect.Width + deltaX;
                }
                if (minTop != maxTop)                       // != means we can move the top edge
                {
                    newY = resizeRect.Top + deltaY;
                    if (newY < minTop) deltaY = deltaY + (minTop - newY);
                    if (newY > maxTop) deltaY = deltaY - (newY - maxTop);
                    imageLabel.top = resizeRect.Top + deltaY;
                    imageLabel.height = resizeRect.Height - deltaY;
                }
                if (minBottom != maxBottom)                 // != means we can move the bottom edge, better use 'else if'
                {
                    newY = resizeRect.Bottom + deltaY;
                    if (newY < minBottom) deltaY = deltaY + (minBottom - newY);
                    if (newY > maxBottom) deltaY = deltaY - (newY - maxBottom);
                    imageLabel.height = resizeRect.Height + deltaY;
                }
                imageLabelsList[activeRectangle] = imageLabel;
                pboxImage.Invalidate();
                bImageLabelsModified = true;
                #endregion
            }
            else if (dragMode == DragMode.DragImage)        // Drag the background image
            {
                // For special effect of dragging the image outside the picturebox leaving blank edges, 
                // set value of allowSpecialDraggingEffect to true/false
                bool allowSpecialDraggingEffect;
                allowSpecialDraggingEffect = true;                  //      <--- set either one, true or false
                //allowSpecialDraggingEffect = false;               //      <--- set either one, true or false

                if (allowSpecialDraggingEffect == false)
                {
                    int deltaX = imageX - dragX;
                    if (deltaX >= 0)                        // deltaX is positive when drag to the right
                    { if (deltaX < cloneRect.X) dispX = imageX - dragX; }
                    else                // deltaX < 0       // deltaX is negative when drag to the left
                    { if (deltaX > cloneRect.Right - srcBitmap.Width) dispX = imageX - dragX; }
                    int deltaY = imageY - dragY;
                    if (deltaY >= 0)                        // deltaY is positive when drag downwards
                    { if (deltaY < cloneRect.Y) dispY = imageY - dragY; }
                    else                // deltaY < 0       // deltaY is negative when drag upwards
                    { if (deltaY > cloneRect.Bottom - srcBitmap.Height) dispY = imageY - dragY; }
                }
                else                    // allowSpecialDraggingEffect == true
                {
                    dispX = imageX - dragX;
                    dispY = imageY - dragY;
                }
            
                pboxImage.Invalidate();
            }
            UpdateCoordinatesLabel();
        }

        private void UpdateVariousRectangles()
        {
            // https://stackoverflow.com/questions/2022660/how-to-get-the-size-of-a-winforms-form-titlebar-height
            Rectangle screenRectangle = RectangleToScreen(this.ClientRectangle);
            int titleHeight = screenRectangle.Top - this.Top;

            int menuHeight = menuStrip1.Height;

            pboxBackground.Left = 0;
            pboxBackground.Top = menuHeight;
            pboxBackground.Width = base.ClientRectangle.Width;
            pboxBackground.Height = base.ClientRectangle.Height - menuHeight;

            // Label coordinates
            lblCoordinates.Left = 20;
            lblCoordinates.Top = 10;

            // pboxImage
            int rightBlockWidth = 400;
            int lowerBlockHeight = 60;
            pboxImage.Left = 20;
            if (lblCoordinates.Visible == true)
                pboxImage.Top = 40;
            else
                pboxImage.Top = 10;
            pboxImage.Width = pboxBackground.Width - pboxImage.Left - rightBlockWidth;
            pboxImage.Height = pboxBackground.Height - pboxImage.Top - lowerBlockHeight;

            // Display scale
            lblDisplayScale.Left = pboxImage.Width / 2 - lblDisplayScale.Width / 2;
            lblDisplayScale.Top = 15;

            // The text "Brightness"
            int middle = pboxImage.Left + pboxImage.Width / 2;
            lblBrightness.Left = middle - 230;
            lblBrightness.Top = pboxImage.Bottom + 15;

            // Slider bar for Brightness
            tbarBrightness.Left = lblBrightness.Left + 100;
            tbarBrightness.Top = lblBrightness.Top;

            // The text "Contrast"
            lblContrast.Left = middle + 25;
            lblContrast.Top = lblBrightness.Top;

            // Slider bar for Contrast
            tbarContrast.Left = lblContrast.Left + 80;
            tbarContrast.Top = lblBrightness.Top;

            // Label 'Processed Folders'
            lblProcessedFolders.Left = pboxImage.Right + 15;
            lblProcessedFolders.Top = pboxImage.Top + 20;
            cboxProcessedFolders.Left = lblProcessedFolders.Left;
            cboxProcessedFolders.Top = lblProcessedFolders.Bottom + 7;
            cboxProcessedFolders.Width = 350;

            // Label 'Current Folder and File'
            lblCurrentFolderText.Left = lblProcessedFolders.Left;
            lblCurrentFolderText.Top = cboxProcessedFolders.Bottom + 15;
            lblCurrentFolder.Left = lblCurrentFolderText.Left;
            lblCurrentFolder.Top = lblCurrentFolderText.Bottom + 2;
            lblCurrentFolder.Width = 350;

            // Listbox for listing file entries
            lboxFilesList.Left = lblCurrentFolder.Left + 110;
            lboxFilesList.Top = lblCurrentFolder.Bottom + 20;
            lboxFilesList.Width = cboxProcessedFolders.Width - 110;
            lboxFilesList.Height = pboxBackground.Height - lboxFilesList.Top - (27+15+5);

            // Label 'Total:<>, Processed:<>'
            lblTotalProcessed.Width = 335;
            lblTotalProcessed.Height = 32;
            lblTotalProcessed.Left = pboxImage.Right + 30;
            lblTotalProcessed.Top = lboxFilesList.Bottom + 10;

            // Temporary only
            lblStars1.Left = lblCurrentFolder.Left + 10;
            lblStars1.Top = lblCurrentFolder.Bottom + 20;
            lblStars5.Left = lblStars4.Left = lblStars3.Left = lblStars2.Left = lblStars1.Left;
            lblStars2.Top = lblStars1.Top + lboxFilesList.ItemHeight;
            lblStars3.Top = lblStars2.Top + lboxFilesList.ItemHeight;
            lblStars4.Top = lblStars3.Top + lboxFilesList.ItemHeight;
            lblStars5.Top = lblStars4.Top + lboxFilesList.ItemHeight;

            // Remove and re-create dynamic labels for stars
            for (int i = 0; i < starsLabelList.Count; i++)
                starsLabelList[i].Dispose();             // Release memory of List of Windows Labels
            starsLabelList.Clear();

            int starLabelLeft = lblCurrentFolder.Left + 10;
            int starLabelTop = lblCurrentFolder.Bottom + 20;
            int count = lboxFilesList.ClientRectangle.Height / lboxFilesList.ItemHeight;
            for (int i = 0; i < count; i++)
            {
                Label newLabel = new Label();
                newLabel.Left = starLabelLeft;
                newLabel.Top = starLabelTop + i * lboxFilesList.ItemHeight;
                newLabel.Width = 98;
                newLabel.Height = 20;
                newLabel.Size = new Size(98, 20);
                newLabel.AutoSize = false;
                newLabel.BorderStyle = BorderStyle.None;
                newLabel.TextAlign = ContentAlignment.MiddleRight;
                newLabel.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                newLabel.ForeColor = System.Drawing.Color.Red;

                newLabel.Parent = pboxBackground;
                starsLabelList.Add(newLabel);            // Create a list of 'Label's to the left of listbox
            }
            UpdateStarsLabel();

            // According to the values of displayScale and pboxDefineFormImage.Width/Height, calculate imageViewableRect
            // then calculate cloneRect
            UpdateImageViewableRectAndCloneRect();
        }

        // Scroll value is from 120 if scroll forward, and -120 if scroll backward
        // Forward scroll 120 means scale factor enlarge by 0.1
        // Backward scroll 120 means scale factor decrease by 0.1
        private void pboxImage_MouseWheel(object sender, MouseEventArgs e)
        {
            float scale = (float)e.Delta / 1200;
            //            displayScale += scale;
            if (e.Delta > 0)
                displayScale += (float)0.05;
            else if (e.Delta < 0)
                displayScale -= (float)0.05;
            if (displayScale < MINIMUMDISPLAYSCALE)
                displayScale = MINIMUMDISPLAYSCALE;
            else if (displayScale > MAXIMUMDISPLAYSCALE)
                displayScale = MAXIMUMDISPLAYSCALE;

            //            scrollDelta = e.Delta;

            UpdateImageViewableRectAndCloneRect();
            UpdateDisplayScaleLabel();
            UpdateVariousRectangles();
            pboxImage.Invalidate();
        }

        // Input:  displayScale and pboxDefineFormImage
        // Output: imageViewableRect and cloneRect
        // Method: 
        // Note: This function is called by mouse wheel messageto scale up/down and window resize message
        private void UpdateImageViewableRectAndCloneRect()
        {
            if (srcBitmap == null)
            {
                cloneRect.X = imageViewableRect.X = 0;
                cloneRect.Y = imageViewableRect.Y = 0;
                cloneRect.Width = imageViewableRect.Width = pboxImage.ClientRectangle.Width;
                cloneRect.Height = imageViewableRect.Height = pboxImage.ClientRectangle.Height;
                return;
            }

            // Calculate viewable image portion inside pboxDefineFormImage
            int scaledWidth = (int)(srcBitmap.Width * displayScale);
            int scaledHeight = (int)(srcBitmap.Height * displayScale);
            if (scaledWidth <= pboxImage.ClientSize.Width)
            {
                imageViewableRect.X = (pboxImage.ClientSize.Width - scaledWidth) / 2;
                imageViewableRect.Width = scaledWidth;
            }
            else
            {
                imageViewableRect.X = 0;
                imageViewableRect.Width = pboxImage.ClientSize.Width;
            }
            if (scaledHeight <= pboxImage.ClientSize.Height)
            {
                imageViewableRect.Y = (pboxImage.ClientSize.Height - scaledHeight) / 2;
                imageViewableRect.Height = scaledHeight;
            }
            else
            {
                imageViewableRect.Y = 0;
                imageViewableRect.Height = pboxImage.ClientSize.Height;
            }

            // Assume no need to change cloneRect.X and cloneRect.Y, calculate cloneRect.Width and cloneRect.Height according to displayScale and imageViewableRect
            cloneRect.Width = (int)(imageViewableRect.Width / displayScale);
            if (cloneRect.Width > srcBitmap.Width)
                cloneRect.Width = srcBitmap.Width;
            cloneRect.Height = (int)(imageViewableRect.Height / displayScale);
            if (cloneRect.Height > srcBitmap.Height)
                cloneRect.Height = srcBitmap.Height;
            if (cloneRect.Right >= srcBitmap.Width)
                cloneRect.X = srcBitmap.Width - cloneRect.Width - 1;
            if (cloneRect.Bottom >= srcBitmap.Height)
                cloneRect.Y = srcBitmap.Height - cloneRect.Height - 1;
            if (cloneRect.X < 0)
                cloneRect.X = 0;
            if (cloneRect.Y < 0)
                cloneRect.Y = 0;
        }

        private void UpdateDisplayScaleLabel()
        {
            lblDisplayScale.Text = $"Display scale: {Math.Round(displayScale * 100)}%";
        }

        // (x,y) is relative to srcBitmap
        private int GetActiveRectangle(int x, int y)
        {
            FourEdges edge;
            FourCorners corner;
            
            foreach (ImageLabelsList imageLabel in imageLabelsList)
            {
                edge = FindEdges(x, y, imageLabel.left, imageLabel.top, imageLabel.width, imageLabel.height);
                corner = FindCorners(x, y, imageLabel.left, imageLabel.top, imageLabel.width, imageLabel.height);
                if (edge != FourEdges.NotAnEdge)         // One of the four edges is detected
                    return imageLabelsList.IndexOf(imageLabel);
            }
            
            return -1;              // No active rectangle, -1 means the background image*/
        }

        // An edge is divided into three equal portions, left and right portion is for dragging and middle protion is for resizing
        private FourEdges FindEdges(int x, int y, int left, int top, int width, int height)
        {
            // (x1,y1) is the upper left corner, and (x2,y2) is the lower right corner
            int x1 = left;
            int y1 = top;
            int x2 = left + width - 1;
            int y2 = top + height - 1;
            // x3 and x4 divides the horizontal edges into three portions, y3 and y4 divides the vertical edges into three portions
            int x3 = x1 + width / 3;
            int x4 = x1 + (width * 2) / 3;
            int y3 = y1 + height / 3;
            int y4 = y1 + (height * 2) / 3;
            if (x1 <= x && x <= x2)           // Detect TOPLINE and BOTTOMLINE
            {
                if (y >= y1 - DETECTLINEWIDTH && y <= y1 + DETECTLINEWIDTH)
                    if (x >= x3 && x <= x4)
                        return FourEdges.TopEdge;
                    else
                        return FourEdges.DragEdge;
                else if (y >= y2 - DETECTLINEWIDTH && y <= y2 + DETECTLINEWIDTH)
                    if (x >= x3 && x <= x4)
                        return FourEdges.BottomEdge;
                    else
                        return FourEdges.DragEdge;
            }
            if (y1 <= y && y <= y2)           // Detect LEFTLINE and RIGHTLINE
            {
                if (x >= x1 - DETECTLINEWIDTH && x <= x1 + DETECTLINEWIDTH)
                    if (y >= y3 && y <= y4)
                        return FourEdges.LeftEdge;
                    else
                        return FourEdges.DragEdge;
                else if (x >= x2 - DETECTLINEWIDTH && x <= x2 + DETECTLINEWIDTH)
                    if (y >= y3 && y <= y4)
                        return FourEdges.RightEdge;
                    else
                        return FourEdges.DragEdge;
            }
            return FourEdges.NotAnEdge;
        }

        private FourCorners FindCorners(int x, int y, int left, int top, int width, int height)
        {
            int x1 = left;
            int y1 = top;
            int x2 = left + width - 1;
            int y2 = top + height - 1;
            if (y >= y1 - DETECTLINEWIDTH && y <= y1 + DETECTLINEWIDTH)     // Detect UpperLeft and UpperRight corner
            {
                if (x >= x1 - DETECTLINEWIDTH && x <= x1 + DETECTLINEWIDTH)
                    return FourCorners.UpperLeftCorner;
                else if (x >= x2 - DETECTLINEWIDTH && x <= x2 + DETECTLINEWIDTH)
                    return FourCorners.UpperRightCorner;
            }
            else if (y >= y2 - DETECTLINEWIDTH && y <= y2 + DETECTLINEWIDTH)    // Detect LowerLeft and LowerRight corner
            {
                if (x >= x1 - DETECTLINEWIDTH && x <= x1 + DETECTLINEWIDTH)
                    return FourCorners.LowerLeftCorner;
                else if (x >= x2 - DETECTLINEWIDTH && x <= x2 + DETECTLINEWIDTH)
                    return FourCorners.LowerRightCorner;
            }
            return FourCorners.NotACorner;
        }

        private void pboxImage_Paint2(object sender, PaintEventArgs e)
        {
            DrawImageLabelingScreen(sender, e);
        }

        // Refer to the drawing "Form Processing\Pictures\cloneRect & imageViewableRect.png" for detail description
        // (dispX, dispY)     --> Displacement of rectangle relative to position (dragX,dragY), coordinates relative to image in 100% scale
        // (dragX, dragY)     --> Start position of a dragging action, coordinates relative to image in 100% scale
        // (rectX, rectY)     --> Start position of drawing a rectangle, coordinates relative to image in 100% scale
        // rectWidth /Height  --> Width and height of the 'drawing rectangle', coordinates relative to image in 100% scale
        // resizeRect         --> 
        // cloneRect          --> The display area in image, coordinates relative to image in 100% scale, will be updated by arrow keys, drag image, and zoom in/out
        // drawingRect        --> 
        // imageViewableRect  --> A rectangle that specifies the displayale area within pboxDefineFormImage, mousedown only valid within this rectangle
        //                        Size of viewable image inside pboxDefineFormImage, updated in UpdateVariousRectangles
        //
        // When will cloneRect be updated?
        // Ans.: cloneRect will be updated by arrow keys, drag image, and zoom in/out
        //
        // When will imageViewableRect be updated?
        // Ans.: 
        //
        // What will happen when size of image is smaller than imageViewableRect?
        // Ans.:
        //
        // What are the steps to implement 'DragCorner'?
        // Ans.: 
        // 
        // Note:
        // 1. Mouse coordinates are transformed to screen coordinates relative to upper left of imageViewableRect
        // 2. Coordinates are scaled in displayScale when stored to defineFormFieldsList
        // 3. cloneRect specifies what area to display from source image, always at original size, i.e. 100% scale
        // 4. imageViewableRect determines where to display the image in pboxDefineFormImage, imageViewableRect is always smaller than pboxDefineFormImage

        private void DrawImageLabelingScreen(object sender, PaintEventArgs e)
        {
            int left, top, width, height;
            SizeF sizeF;
            int scaledLeft, scaledTop, scaledWidth, scaledHeight;
            Font font = new Font("Arial", 12);
            Brush alphaBrush = new SolidBrush(Color.FromArgb(158, 255, 255, 255));

            Graphics g = e.Graphics;

            // 1. Draw the background image first (at the bottom z-order)
            if (srcBitmap != null)
            {
                if (imageIsVisible == true)
                {
                    if (dragMode == DragMode.DragImage)
                    {
                        Rectangle tempRect = new Rectangle();
                        tempRect.X = cloneRect.X - dispX;
                        tempRect.Y = cloneRect.Y - dispY;
                        tempRect.Width = cloneRect.Width;
                        tempRect.Height = cloneRect.Height;
                        g.DrawImage(processedBitmap, imageViewableRect, tempRect, GraphicsUnit.Pixel);
                    }
                    else
                    {
                        g.DrawImage(processedBitmap, imageViewableRect, cloneRect, GraphicsUnit.Pixel);
                    }
                }
            }

            sizeF = g.MeasureString(lblDisplayScale.Text, font);
            Rectangle scaledLabelRectangle = new Rectangle();
            scaledLabelRectangle.Width = (int)sizeF.Width;
            scaledLabelRectangle.Height = (int)sizeF.Height;
            scaledLabelRectangle.X = pboxImage.Width / 2 - scaledLabelRectangle.Width / 2;
            scaledLabelRectangle.Y = lblDisplayScale.Top;

            g.FillRectangle(alphaBrush, scaledLabelRectangle);
//            g.FillRectangle(alphaBrush, new Rectangle(300, 300, 300, 300));
//            g.FillRectangle(alphaBrush, new Rectangle(395, 395, 30, 30));
//            g.FillRectangle(alphaBrush, new Rectangle(400, 400, 300, 300));

            // 2. Draw the already defined labels with text description above the rectangle
            foreach (ImageLabelsList imageLabel in imageLabelsList)
            {
                left = 0;
                top = 0;
                width = imageLabel.width;
                height = imageLabel.height;
                if (dragMode == DragMode.DragNone) { left = imageLabel.left - cloneRect.X; top = imageLabel.top - cloneRect.Y; }
                else if (dragMode == DragMode.DrawRectangle) { left = imageLabel.left - cloneRect.X; top = imageLabel.top - cloneRect.Y; }
                else if (dragMode == DragMode.DragRectangle) { left = imageLabel.left - cloneRect.X; top = imageLabel.top - cloneRect.Y; }
                else if (dragMode == DragMode.DragImage) { left = imageLabel.left - (cloneRect.X - dispX); top = imageLabel.top - (cloneRect.Y - dispY); }
                else if (dragMode == DragMode.DragTopEdge || dragMode == DragMode.DragLeftEdge || dragMode == DragMode.DragRightEdge ||
                         dragMode == DragMode.DragBottomEdge || dragMode == DragMode.DragUpperLeftCorner || dragMode == DragMode.DragUpperRightCorner ||
                         dragMode == DragMode.DragLowerLeftCorner || dragMode == DragMode.DragLowerRightCorner)
                { left = imageLabel.left - cloneRect.X; top = imageLabel.top - cloneRect.Y; }
                else
                {
                    MessageBox.Show($"Value of dragMode is {dragMode}", "dragMode error");              // Safety only
                    return;
                }

                Color frameColor = Color.Red;                     // Default color
                scaledLeft = (int)(left * displayScale) + imageViewableRect.Left;
                scaledTop = (int)(top * displayScale) + imageViewableRect.Top;
                scaledWidth = (int)(width * displayScale);
                scaledHeight = (int)(height * displayScale);
                if (imageLabel.labelDescription != null)
                {
                    frameColor = GetFrameColor(imageLabel.labelDescription);         // GetColor() will return one of normalColor, undefinedColor, or othersColor
                    if (bDisplayDefectText == true)
                    {
                        sizeF = g.MeasureString(imageLabel.labelDescription, font);
                        g.FillRectangle(alphaBrush, new Rectangle(scaledLeft, scaledTop - (int)sizeF.Height, (int)sizeF.Width, (int)sizeF.Height));
                        g.DrawString(imageLabel.labelDescription, font, Brushes.Red, scaledLeft, scaledTop - (int)sizeF.Height + 1);
                    }
                }
                Pen pen = new Pen(frameColor);
                g.DrawRectangle(pen, scaledLeft, scaledTop, scaledWidth, scaledHeight);
            }

            // 3. Draw the rectangle being defined, not yet in the defineFormFieldsList
            if (dragMode == DragMode.DrawRectangle)
            {
                int x = (int)((rectX - cloneRect.Left) * displayScale) + imageViewableRect.Left;
                int y = (int)((rectY - cloneRect.Top) * displayScale) + imageViewableRect.Top;
                width = (int)(rectWidth * displayScale);
                height = (int)(rectHeight * displayScale);
                Rectangle nRect = new Rectangle(x, y, width, height);
                GetUprightRect(ref nRect);
                g.DrawRectangle(Pens.Red, nRect);
            }
            else                // Draw the four corners of activeRectangle, if any
            {
                if (activeRectangle != -1)
                {
                    ImageLabelsList imageLabel = new ImageLabelsList();
                    imageLabel = imageLabelsList[activeRectangle];
                    left = 0;
                    top = 0;
                    if (dragMode == DragMode.DragNone)
                    {
                        left = imageLabel.left - cloneRect.X;
                        top = imageLabel.top - cloneRect.Y;
                    }
                    else if (dragMode == DragMode.DragImage)
                    {
                        left = imageLabel.left - (cloneRect.X - dispX);
                        top = imageLabel.top - (cloneRect.Y - dispY);
                    }
                    else if (dragMode == DragMode.DragTopEdge || dragMode == DragMode.DragLeftEdge || dragMode == DragMode.DragRightEdge ||
                             dragMode == DragMode.DragBottomEdge || dragMode == DragMode.DragUpperLeftCorner || dragMode == DragMode.DragUpperRightCorner ||
                             dragMode == DragMode.DragLowerLeftCorner || dragMode == DragMode.DragLowerRightCorner || dragMode == DragMode.DragRectangle)
                    {
                        left = imageLabel.left - cloneRect.X;
                        top = imageLabel.top - cloneRect.Y;
                    }
                    Point ptUpperLeft = new Point(), ptLowerRight = new Point();
                    ptUpperLeft.X = (int)(left * displayScale) + imageViewableRect.Left;
                    ptUpperLeft.Y = (int)(top * displayScale) + imageViewableRect.Top;
                    ptLowerRight.X = (int)((left + imageLabel.width) * displayScale) + imageViewableRect.Left;
                    ptLowerRight.Y = (int)((top + imageLabel.height) * displayScale) + imageViewableRect.Top;
                    DrawFourCorners(g, ptUpperLeft, ptLowerRight);
                }
            }

            // 4. Draw the four corners on pboxImage to signify the active state of pboxImage
            if (bCursorIsInside)
            {
                width = 10;
                height = 10;
                g.FillRectangle(Brushes.Red, 0, 0, width, height);
                g.FillRectangle(Brushes.Red, pboxImage.ClientRectangle.Width - width, 0, width, height);
                g.FillRectangle(Brushes.Red, pboxImage.ClientRectangle.Width - width, pboxImage.ClientRectangle.Height - height, width, height);
                g.FillRectangle(Brushes.Red, 0, pboxImage.ClientRectangle.Height - height, width, height);
            }
        }

        private Color GetFrameColor(string desc)
        {
            //            Color frameColor = normalColor;

            if (desc.Contains("[其他]"))
                return othersColor;
            else if (desc.Contains("未分類的瑕疵"))
                return undefinedColor;
            else
                return normalColor;
        }

        // Draw the four corners specified by pt1 (upper left corner) and pt2 (lower right corner)
        private void DrawFourCorners(Graphics g, Point pt1, Point pt2)
        {
            // Draw the testing crosses at pt1 and pt2
            //            g.DrawLine(Pens.Green, pt1.X - 8, pt1.Y, pt1.X + 8, pt1.Y);
            //            g.DrawLine(Pens.Green, pt1.X, pt1.Y - 8, pt1.X, pt1.Y + 8);
            //            g.DrawLine(Pens.Green, pt2.X - 8, pt2.Y, pt2.X + 8, pt2.Y);
            //            g.DrawLine(Pens.Green, pt2.X, pt2.Y - 8, pt2.X, pt2.Y + 8);

            // Draw the four corners
            g.FillRectangle(Brushes.Red, pt1.X - 2, pt1.Y - 2, ACTIVECORNER, ACTIVECORNER);
            g.FillRectangle(Brushes.Red, pt2.X - 4, pt1.Y - 2, ACTIVECORNER, ACTIVECORNER);
            g.FillRectangle(Brushes.Red, pt1.X - 2, pt2.Y - 4, ACTIVECORNER, ACTIVECORNER);
            g.FillRectangle(Brushes.Red, pt2.X - 4, pt2.Y - 4, ACTIVECORNER, ACTIVECORNER);
        }

        private void GetUprightRect(ref Rectangle rect)
        {
            if (rect.Width < 0)
            {
                rect.X += rect.Width;
                rect.Width = Math.Abs(rect.Width);
            }
            if (rect.Height < 0)
            {
                rect.Y += rect.Height;
                rect.Height = Math.Abs(rect.Height);
            }
        }

        private void UpdateCoordinatesLabel()
        {
            string strCoordinates = "";
            strCoordinates += $"mouseX: ({mouseX}, {mouseY}), {dragMode}, ";
            if (srcBitmap != null)
                strCoordinates += $"srcBitmap: ({srcBitmap.Width}, {srcBitmap.Height}), ";

            int itemHeight = lboxFilesList.ItemHeight;
            int topIndex = lboxFilesList.TopIndex;
            int selectedIndex = lboxFilesList.SelectedIndex;
            int left = lboxFilesList.Left;
            int top = lboxFilesList.Top;
            int width = lboxFilesList.Width;
            int height = lboxFilesList.Height;
            int texst = lboxFilesList.IndexFromPoint(mouseX, mouseY);
            int clientWidth = lboxFilesList.ClientSize.Width;
            int clientHeight = lboxFilesList.ClientSize.Height;

            //            strCoordinates += $"Listbox:({itemHeight}, {topIndex}, {selectedIndex}, {texst}, {left}, {top}, {width}, {height}, {clientWidth}, {clientHeight})";


            //            strCoordinates += $"pBox: ({pboxImage.Width}, {pboxImage.Height}), ";
            //            strCoordinates += $"Brightness: {brightness}, Contrast: {contrast}, ";
            //            strCoordinates += $"dragX: ({dragX}, {dragY}), ";
            //            strCoordinates += $"dispX: ({dispX}, {dispY}), ";
            //            strCoordinates += $"resizeRect: ({resizeRect.X}, {resizeRect.Y}, {resizeRect.Width}, {resizeRect.Height}), ";
            //            strCoordinates += $"cloneRect: ({cloneRect.X}, {cloneRect.Y}, {cloneRect.Width}, {cloneRect.Height}), ";
            //            strCoordinates += $"imgVwRect: ({imageViewableRect.X}, {imageViewableRect.Y}, {imageViewableRect.Width}, {imageViewableRect.Height}), ";
            //            strCoordinates += $"rectX: ({rectX}, {rectY}, {rectWidth}, {rectHeight}), ";
            //            strCoordinates += $"delta: {scrollDelta}, ";
            strCoordinates += $"currentFile: {currentFile}";
            strCoordinates += $"bModified: {bImageLabelsModified}, ";
            strCoordinates += $"labelCount: {starsLabelList.Count}, ";
            strCoordinates += bCursorIsInside ? "Inside, " : "Outside, ";
            strCoordinates += $"active: {activeRectangle}";
            lblCoordinates.Text = strCoordinates;
        }

//        private bool bHandleArrowKeys = true;
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)        // For ImageLabeling form
        {
            int kbImageStep = (int)(KBIMAGESTEP / displayScale);
            int kbRectangleStep = (int)(KBRECTANGLESTEP / displayScale);
            int kbEnlargeStep = (int)(KBENLARGESTEP / displayScale);

            //            if (cboxProcessedFolders.ContainsFocus == true)
            //              return base.ProcessCmdKey(ref msg, keyData);

            if (keyData == Keys.F1)
            {
                Shortcut_keys shortcutKeys = new Shortcut_keys();
                shortcutKeys.ShowDialog();
                return base.ProcessCmdKey(ref msg, keyData);
            }

            if (keyData == Keys.F2)
            {
                bDisplayDefectText = !bDisplayDefectText;
                pboxImage.Invalidate();
                return base.ProcessCmdKey(ref msg, keyData);
            }
            
            if (srcBitmap == null)
                return base.ProcessCmdKey(ref msg, keyData);

            if (bCursorIsInside == true)
            {
                if (keyData == Keys.Up)                                 // Move the active rectangle up, by KBRECTANGLESTEP pixels
                {
                    #region Keys.Up
                    if (activeRectangle == -1)          // Move the image up, not rectangles
                    {
                        if (cloneRect.Y + kbImageStep + cloneRect.Height < srcBitmap.Height)
                        {
                            cloneRect.Y += kbImageStep;
                            pboxImage.Invalidate();
                            UpdateCoordinatesLabel();
                            return true;
                        }
                    }
                    else                // Move active rectangle up
                    {
                        ImageLabelsList imageLabel = new ImageLabelsList();
                        imageLabel = imageLabelsList[activeRectangle];
                        if (imageLabel.top - kbRectangleStep >= 0)
                        {
                            imageLabel.top -= kbRectangleStep;
                            imageLabelsList.RemoveAt(activeRectangle);
                            imageLabelsList.Insert(activeRectangle, imageLabel);
                            pboxBackground.Invalidate();
                            UpdateCoordinatesLabel();
                            bImageLabelsModified = true;
                        }
                        return true;
                    }
                    #endregion
                }
                else if (keyData == Keys.Down)                          // Move the active rectangle down, by KBRECTANGLESTEP pixels
                {
                    #region Keys.Down
                    if (activeRectangle == -1)          // Move the image down, not rectangles
                    {
                        if (cloneRect.Y - kbImageStep >= 0)
                        {
                            cloneRect.Y -= kbImageStep;
                            pboxImage.Invalidate();
                            UpdateCoordinatesLabel();
                            return true;
                        }
                    }
                    else                // Move active rectangle down
                    {
                        ImageLabelsList imageLabel = new ImageLabelsList();
                        imageLabel = imageLabelsList[activeRectangle];
                        if (imageLabel.top + kbRectangleStep + imageLabel.height < srcBitmap.Height)
                        {
                            imageLabel.top += kbRectangleStep;
                            imageLabelsList.RemoveAt(activeRectangle);
                            imageLabelsList.Insert(activeRectangle, imageLabel);
                            pboxBackground.Invalidate();
                            UpdateCoordinatesLabel();
                            bImageLabelsModified = true;
                        }
                        return true;
                    }
                    #endregion
                }                
                else if (keyData == Keys.Left)                          // Move the active rectangle to the left, by KBRECTANGLESTEP pixels
                {
                    #region Keys.Left
                    if (activeRectangle == -1)          // Move the image, not rectangles, to the left
                    {
                        if (cloneRect.X + kbImageStep + cloneRect.Width < srcBitmap.Width)
                        {
                            cloneRect.X += kbImageStep;
                            pboxImage.Invalidate();
                            UpdateCoordinatesLabel();
                            return true;
                        }
                    }
                    else                // Move active rectangle to the left
                    {
                        ImageLabelsList imageLabel = new ImageLabelsList();
                        imageLabel = imageLabelsList[activeRectangle];
                        if (imageLabel.left - kbRectangleStep >= 0)
                        {
                            imageLabel.left -= kbRectangleStep;
                            imageLabelsList.RemoveAt(activeRectangle);
                            imageLabelsList.Insert(activeRectangle, imageLabel);
                            pboxBackground.Invalidate();
                            UpdateCoordinatesLabel();
                            bImageLabelsModified = true;
                        }
                        return true;
                    }
                    #endregion
                }
                else if (keyData == Keys.Right)                         // Move the active rectangle to the right, by KBRECTANGLESTEP pixels
                {
                    #region Keys.Right
                    if (activeRectangle == -1)          // Move the image, not rectangles, to the right
                    {
                        if (cloneRect.X - kbImageStep >= 0)
                        {
                            cloneRect.X -= kbImageStep;
                            pboxImage.Invalidate();
                            UpdateCoordinatesLabel();
                            return true;
                        }
                    }
                    else                // Move active rectangle to the right
                    {
                        ImageLabelsList imageLabel = new ImageLabelsList();
                        imageLabel = imageLabelsList[activeRectangle];
                        if (imageLabel.left + kbRectangleStep + imageLabel.width < srcBitmap.Width)                        {
                            imageLabel.left += kbRectangleStep;
                            imageLabelsList.RemoveAt(activeRectangle);
                            imageLabelsList.Insert(activeRectangle, imageLabel);
                            pboxBackground.Invalidate();
                            UpdateCoordinatesLabel();
                            bImageLabelsModified = true;
                        }
                        return true;
                    }
                    #endregion
                }
                else if (keyData == (Keys.Control | Keys.Up))           // Increase in height of active rectangle
                {
                    #region Ctrl+Keys.Up
                    if (activeRectangle != -1)
                    {
                        ImageLabelsList imageLabel = new ImageLabelsList();
                        imageLabel = imageLabelsList[activeRectangle];
                        if (imageLabel.height + kbEnlargeStep < srcBitmap.Height)
                        {
                            imageLabel.height += kbEnlargeStep;
                            imageLabel.top = imageLabel.top - kbEnlargeStep / 2;            // Make the rectangle enlarge in both top edge and bottom edge
                            if (imageLabel.top < 0)
                                imageLabel.top = 0;                                     // Make sure the defined rectangle is always within srcBitmap
                            else if (imageLabel.top + imageLabel.height >= srcBitmap.Height)
                                imageLabel.top = srcBitmap.Height - imageLabel.height - 1;      // Make sure the defined rectangle is always within srcBitmap
                            imageLabelsList.RemoveAt(activeRectangle);
                            imageLabelsList.Insert(activeRectangle, imageLabel);
                            pboxBackground.Invalidate();
                            UpdateCoordinatesLabel();
                            bImageLabelsModified = true;
                        }
                    }
                    return true;
                    #endregion
                }
                else if (keyData == (Keys.Control | Keys.Down))         // Decrease in height of active rectangle
                {
                    #region Ctrl+Keys.Down
                    if (activeRectangle != -1)
                    {
                        ImageLabelsList imageLabel = new ImageLabelsList();
                        imageLabel = imageLabelsList[activeRectangle];
                        if (imageLabel.height - kbEnlargeStep > MINRECTHEIGHT)
                        {
                            imageLabel.height -= kbEnlargeStep;
                            imageLabel.top = imageLabel.top + kbEnlargeStep / 2;                // Make the rectangle shrink in both top edge and bottom edge
                            imageLabelsList.RemoveAt(activeRectangle);
                            imageLabelsList.Insert(activeRectangle, imageLabel);
                            pboxBackground.Invalidate();
                            UpdateCoordinatesLabel();
                            bImageLabelsModified = true;
                        }
                    }
                    return true;
                    #endregion
                }
                else if (keyData == (Keys.Control | Keys.Left))         // Decrease in width of active rectangle
                {
                    #region Ctrl+Keys.Left
                    if (activeRectangle != -1)
                    {
                        ImageLabelsList imageLabel = new ImageLabelsList();
                        imageLabel = imageLabelsList[activeRectangle];
                        if (imageLabel.width - kbEnlargeStep > MINRECTWIDTH)
                        {
                            imageLabel.width -= kbEnlargeStep;
                            imageLabel.left = imageLabel.left + kbEnlargeStep / 2;              // Make the rectangle shrink in both left edge and right edge
                            imageLabelsList.RemoveAt(activeRectangle);
                            imageLabelsList.Insert(activeRectangle, imageLabel);
                            pboxBackground.Invalidate();
                            UpdateCoordinatesLabel();
                            bImageLabelsModified = true;
                        }
                    }
                    return true;
                    #endregion
                }
                else if (keyData == (Keys.Control | Keys.Right))        // Increase in width of active rectangle
                {
                    #region Ctrl+Keys.Up
                    if (activeRectangle != -1)
                    {
                        ImageLabelsList imageLabel = new ImageLabelsList();
                        imageLabel = imageLabelsList[activeRectangle];
                        if (imageLabel.width + kbEnlargeStep < srcBitmap.Width)
                        {
                            imageLabel.width += kbEnlargeStep;
                            imageLabel.left = imageLabel.left - kbEnlargeStep / 2;              // Make the rectangle enlarge in both left edge and right edge
                            if (imageLabel.left < 0)
                                imageLabel.left = 0;                                    // Make sure the defined rectangle is always within srcBitmap
                            else if (imageLabel.left + imageLabel.width >= srcBitmap.Width)
                                imageLabel.left = srcBitmap.Width - imageLabel.width - 1;       // Make sure the defined rectangle is always within srcBitmap
                            imageLabelsList.RemoveAt(activeRectangle);
                            imageLabelsList.Insert(activeRectangle, imageLabel);
                            pboxBackground.Invalidate();
                            UpdateCoordinatesLabel();
                            bImageLabelsModified = true;
                        }
                    }
                    return true;
                    #endregion
                }
                else if (keyData == Keys.PageUp)                        // Set active rectangle to the previous one
                {
                    #region Keys.PageUp
                    if (activeRectangle > 0)
                    {
                        activeRectangle--;
                        pboxBackground.Invalidate();
                    }
                    return true;
                    #endregion
                }
                else if (keyData == Keys.PageDown)                      // Set active rectangle to the next one
                {
                    #region Keys.PageDown
                    if (activeRectangle < imageLabelsList.Count - 1)
                    {
                        activeRectangle++;
                        pboxBackground.Invalidate();
                    }
                    return true;
                    #endregion
                }
                else if (keyData == Keys.Delete)                        // Delete current active rectangle
                {
                    #region Keys.Delete
                    if (activeRectangle != -1)
                    {
                        DeleteOneImageLabelEntry(activeRectangle);
                        UpdateCoordinatesLabel();
                        bImageLabelsModified = true;
                    }
                    return true;
                    #endregion
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
