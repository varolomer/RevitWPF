using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Windows;

namespace RevitWPF
{
    class Commands
    {
        /// <summary>
        /// If the task is used Revit must be reached only as read-only. If there is any
        /// attempt to open a transaction or modify Revit it will throw exception.
        /// </summary>
        /// <param name="uiapp"></param>
        public static void CountWallsTask(UIApplication uiapp)
        {
            Task.Run(() => CountWalls(uiapp));
        }

        public static void CountWalls(UIApplication uiapp)
        {
            Document doc = uiapp.ActiveUIDocument.Document;

            var elements = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .ToList();

            //TaskDialog.Show("Basic External EventHandler", $"{elements.Count.ToString()} wall instances exist in the documents.");

            MessageBox.Show($"{elements.Count.ToString()} wall instances exist in the documents.");
        }

        public static void CreateRandomWalls(UIApplication uiapp)
        {
            //Get the document
            Document doc = uiapp.ActiveUIDocument.Document;

            //Collect a wall type
            WallType wallType = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsElementType()
                .Cast<WallType>()
                .FirstOrDefault();

            //Get a level
            Level level = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .FirstOrDefault();

            //Make sure the wall type is not null
            if (wallType == null) throw new Exception("No wall type");

            //Create a random instance
            Random random = new Random();

            //Define a bound
            int bound = (int)UnitUtils.ConvertToInternalUnits(15, UnitTypeId.Meters);

            //Get some random points within bound
            XYZ p1 = new XYZ(random.Next(0, bound), random.Next(0, bound), 0);
            XYZ p2 = new XYZ(random.Next(0, bound), random.Next(0, bound), 0);

            //Create a line
            Line line = Line.CreateBound(p1, p2);

            //Create the wall
            using (Transaction trans = new Transaction(doc, "Create Wall"))
            {
                trans.Start();
                Wall.Create(doc, line, level.Id, false);
                trans.Commit();
            }
        }

        public static void BatchWalls(UIApplication uiapp, ModelessWPF argsUI)
        {
            //Get the value from UI
            int wallCount = 0;
            int singleAdvance = 0;
            argsUI.Dispatcher.Invoke(() =>
            {
                wallCount = (int)argsUI.slider.Value;
                singleAdvance = (int)argsUI.slider.Maximum / wallCount;

                //Set progress to zero first
                argsUI.progressBar.Value = 0;
            });

            //Get the document
            Document doc = uiapp.ActiveUIDocument.Document;

            //Collect a wall type
            WallType wallType = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsElementType()
                .Cast<WallType>()
                .FirstOrDefault();

            //Get a level
            Level level = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Levels)
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .FirstOrDefault();

            //Make sure the wall type is not null
            if (wallType == null) throw new Exception("No wall type");

            //Create a random instance
            Random random = new Random();

            //Define a bound
            int bound = (int)UnitUtils.ConvertToInternalUnits(15, UnitTypeId.Meters);

            for (int i = 0; i < wallCount; i++)
            {
                //Get some random points within bound
                XYZ p1 = new XYZ(random.Next(0, bound), random.Next(0, bound), 0);
                XYZ p2 = new XYZ(random.Next(0, bound), random.Next(0, bound), 0);

                //Create a line
                Line line = Line.CreateBound(p1, p2);

                //Create the wall
                using (Transaction trans = new Transaction(doc, $"Create Wall-{i.ToString()}"))
                {
                    trans.Start();
                    Wall.Create(doc, line, level.Id, false);
                    trans.Commit();
                }

                argsUI.Dispatcher.Invoke(() => argsUI.progressBar.Value += singleAdvance);
            }

            //Set progress to 100 when it is done.
            argsUI.Dispatcher.Invoke(() => argsUI.progressBar.Value = 100);
        }

        public static void ExportImage(UIApplication uiapp)
        {
            //Get document
            Document doc = uiapp.ActiveUIDocument.Document;

            //Create export options
            ImageExportOptions ops = new ImageExportOptions();
            ops.ExportRange = ExportRange.VisibleRegionOfCurrentView;
            ops.HLRandWFViewsFileType = ImageFileType.PNG;
            ops.ZoomType = ZoomFitType.FitToPage;
            ops.PixelSize = 3600;
            ops.FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Test.png");

            //Export image
            doc.ExportImage(ops);
        }
    }
}
