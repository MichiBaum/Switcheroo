// Copyright by Switcheroo

#region

using System;

#endregion

namespace Switcheroo {
    public class AutoStartException : Exception {
        public AutoStartException() {
        }

        public AutoStartException(string message)
            : base(message) {
        }

        public AutoStartException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}