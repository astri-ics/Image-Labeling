# Image-Labeling
#### Image Labeling software written in C# for __Windows 10__

## Steps to run *Image Labeling.exe*
1. Clone this repository to your local drive
2. Create a folder 'C:\temp'
3. Copy four demo images, 'Defect photo 01.jpg' .. 'Defect photo 04.jpg' to 'C:\temp'
4. Copy 'Image Labeling Data.json' to 'C:\temp'
5. *(A)* You may run 'Image Labeling\bin\x64\Debug\Image Labeling.exe' directly
6. *(B)* If you have Visual Studio 2015 Community Edition installed and want to compile/run, double click on the solution file 'Image Labeling.sln' and press F5 to run.
7. Simple mouse/keyboard shortcut keys are included (Shortcut keys.png).


![Sample output screen](https://github.com/astri-ics/Image-Labeling/blob/master/Sampleoutputscreen05.png "Sample output screen")


![Shortcut keys](https://github.com/astri-ics/Image-Labeling/blob/master/Shortcutkeys.png "Shortcut keys")

## Changes in Version 1.1
1. Initial window size set to 1200x800
2. Add Help->Shortcut Keys
3. Add F1 hotkey to bring up Shortcut keys dialog
4. New defect definition (refer to Image Labeling Definition.json)

## Changes in Version 1.2
1. Add "未分類的瑕疵"
2. Add NormalColor, UndefinedColor, and OthersColor
3. Add hotkey F2 to hide defect definition text display
4. Modify Shortcut keys description
5. Change version number from V1.1 to V1.2 in About dialog box
6. Use Dictionary to implement 'acceptable color'
7. Change app name from 'Image Labeling' to 'Defect Labeling'

## Changes in Version 1.3
1. Solve the bug found in cboxProcessedFolders_SelectedIndexChanged()
