using System;
using System.Collections.Generic;

namespace Archetypical.Software.Vitruvian
{
    public class VitruvianBehaviorConfiguration
    {
        internal List<Action> StartupActions { get; set; }
        internal List<Action> ShutdownActions { get; set; }
    }
}