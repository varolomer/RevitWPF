![alt text](https://github.com/varolomer/RevitWPF/blob/master/RevitWPF/Assets/Github/Banner.png)

# Revit & Modeless WPF | Two-Way Updates
![alt text](https://github.com/varolomer/RevitWPF/blob/master/RevitWPF/Assets/Github/BatchWallExportProgress.gif)

This library utilizes Revit's External Event architecture to be able to develop responsive UIs. Furthermore, the library provides the chance to pass information from UI Thread to Revit thread and vice versa using the Dispatcher Object. To be able to pass the Revit External event and Event Handler from External Application to UI Thread a wrapper abstract class is created which implements IExternalEventHandler.

To be able to have a responsive UI which works even when while Revit is busy, the UI is created in a separate thread. For communication between UI and Revit, the dispatcher object is invoked. While the UI thread is being shut down, all the ExternalEvents and handlers are disposed.

# Attribution
This library is based on a WPF example from mitevpi. Most of the credit goes there. First, I have re-written the library as a tutorial but then I have improved and re-implemented some points that was not working for me like adding the interlocking mechanism to easily pass the desired commands from UI Thread to Revit External Event.
