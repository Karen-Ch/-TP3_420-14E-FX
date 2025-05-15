﻿using Seismoscope.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seismoscope.Utils.Services.Interfaces
{
    public interface IAjustementService
    {
        void AppliquerRegles(Capteur capteur, List<double> lecturesRecues);
    }
}
