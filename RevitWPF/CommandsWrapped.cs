using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitWPF
{
    public class EventHandlerWithStringArg : RevitEventWrapper<string>
    {
        public override void Execute(UIApplication app, string args)
        {
            // Do your processing here with "args"
            TaskDialog.Show("External Event", args);
        }
    }

    public class EventHandlerWithWpfArg : RevitEventWrapper<ModelessWPF>
    {
        public override void Execute(UIApplication uiapp, ModelessWPF argsUI)
        {
            switch (RevitRequest.Take())
            {
                case RevitRequestId.None:
                    {
                        return;
                    }
                case RevitRequestId.CountWalls:
                    {
                        Commands.CountWallsTask(uiapp);
                        break;
                    }
                case RevitRequestId.CreateRandomWall:
                    {
                        Commands.CreateRandomWalls(uiapp);
                        break;
                    }
                case RevitRequestId.BatchWalls:
                    {
                        Commands.BatchWalls(uiapp, argsUI);
                        break;
                    }
                case RevitRequestId.ExportImage:
                    {
                        Commands.ExportImage(uiapp);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    }
}
