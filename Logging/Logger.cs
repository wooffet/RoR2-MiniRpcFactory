﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MiniRpcFactory.Logging
{
    internal class Logger
    {
        public static Action<object> LogInfo;
        public static Action<object> LogDebug;
        public static Action<object> LogWarning;
        public static Action<object> LogError;
    }
}
