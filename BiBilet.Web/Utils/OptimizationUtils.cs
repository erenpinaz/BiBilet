﻿using System.Collections.Generic;
using System.Web.Optimization;

namespace BiBilet.Web.Utils
{
    public class CustomBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}