using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevitWPF
{
    public class RevitRequest
    {
        // Storing the value as a plain Int makes using the interlocking mechanism simpler
        private int m_request = (int)RevitRequestId.None;

        // Interlocked. This C# class helps with threading. It safely changes the value of
        // a shared variable from multiple threads. It is part of System.Threading.

        //Using threads safely is possible with the lock statement. But you can instead use the
        //Interlocked type for simpler and faster code.

        /// <summary>
        /// The handler calls this method to obtain the current RevitRequest. Interlocked.Exchange
        /// method sets the input integer value to none (0) and still returns the current RevitRequest's
        /// integer value. So it could be thought that after returning the current requestId it sets
        /// the request id to none.
        /// </summary>
        /// <returns></returns>
        public RevitRequestId Take()
        {
            return (RevitRequestId)Interlocked.Exchange(ref m_request, (int)RevitRequestId.None);
        }

        /// <summary>
        /// This function is called when UI buttons want to execute a request. It replaces
        /// the old RevitRequests with the new one.
        /// </summary>
        /// <param name="request"></param>
        public void Make(RevitRequestId request)
        {
            Interlocked.Exchange(ref m_request, (int)request);
        }

    }

    /// <summary>
    /// Enumeration for commands. This could be regarded as old IExternalCommands in normal plugin
    /// pattern. But now, this commands will be operated in Event Handler. The reason why these commands
    /// are enumerated with integer is that it is convinient for the interlocking mechanism for thread
    /// safety. Interlocking mechanism is explained more above.
    /// </summary>
    public enum RevitRequestId
    {
        None = 0,
        CountWalls = 1,
        CreateRandomWall = 2,
        BatchWalls = 3,
        ExportImage = 4,

    }
}
