using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitWPF
{
    public abstract class RevitEventWrapper<TType> : IExternalEventHandler
    {
        private readonly object _lock;
        private TType _savedArgs;
        public ExternalEvent _revitEvent;

        // The value of the latest request made by the modeless form 
        private RevitRequest m_request = new RevitRequest();

        /// <summary>
        /// A public property to access the current request value
        /// </summary>
        public RevitRequest RevitRequest
        {
            get { return m_request; }
        }

        /// <summary>
        /// Class for wrapping methods for execution within a "valid" Revit API context.
        /// </summary>
        protected RevitEventWrapper()
        {
            _revitEvent = ExternalEvent.Create(this);
            _lock = new object();
        }

        public void Execute(UIApplication uiapp)
        {
            TType args;

            lock (_lock)
            {
                args = _savedArgs;
                _savedArgs = default;
            }

            Execute(uiapp, args);
        }

        /// <summary>
        /// Execute the wrapped external event in a valid Revit API context.
        /// </summary>
        /// <param name="args">Arguments that could be passed to the execution method.</param>
        public void Raise(TType args)
        {
            lock (_lock)
            {
                _savedArgs = args;
            }

            _revitEvent.Raise();
        }

        public string GetName()
        {
            return GetType().Name;
        }

        /// <summary>
        /// Override void which wraps the "Execution" method in a valid Revit API context.
        /// </summary>
        /// <param name="app">Revit UI Application to use as the "wrapper" API context.</param>
        /// <param name="args">Arguments that could be passed to the execution method.</param>
        public abstract void Execute(UIApplication app, TType args);
    }
}
