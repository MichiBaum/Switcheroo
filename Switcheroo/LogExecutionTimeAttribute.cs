﻿using log4net;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System.Diagnostics;

namespace Switcheroo {

    
    [PSerializable]
    public class LogExecutionTime : OnMethodBoundaryAspect {

        private static readonly Stopwatch timer = new Stopwatch();
        [PNonSerialized]
        private static readonly ILog logger = LogManager.GetLogger(typeof(LogExecutionTime));

        public override void OnEntry(MethodExecutionArgs args) {

            timer.Start();

        }

        public override void OnExit(MethodExecutionArgs args) {

            var elapsedMilliseconds = timer.ElapsedMilliseconds;

            timer.Stop();

            logger.Debug($"Method '{args.Method.Name}' took {elapsedMilliseconds} milliseconds to run.");

        }

    }
}
