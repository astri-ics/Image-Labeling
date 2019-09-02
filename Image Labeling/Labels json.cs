using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Labeling
{
    class Labels_json
    {
    }

    public class EditingRecordObject
    {
        public string[] processedFolders { get; set; }
        public string lastAccessedFolder { get; set; }      // This folder contains "Image Labeling Data.JSON", which contains "lastAccessedFile"
    }

    public class DefectDefinitionObject
    {
        public DefectDefinition[] defectDefinition { get; set; }
        public string NormalColor { get; set; }
        public string UndefinedColor { get; set; }
        public string OthersColor { get; set; }
        public string ColorDescription { get; set; }
    }

    public class DefectDefinition
    {
        public string Category { get; set; }
        public string[] Items { get; set; }
    }

    public class ImageLabelsAllFiles
    {
        public string lastAccessedFile { get; set; }        // Last accessed file of the specified folder
        public ImageLabelsPerFile[] allLabels { get; set; }
    }

    public class ImageLabelsPerFile
    {
        public string filename { get; set; }
        public ImageLabelsList[] imageLabels { get; set; }
    }

    public class ImageLabelsList
    {
        public int left { get; set; }
        public int top { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string labelDescription { get; set; }
    }    
}
