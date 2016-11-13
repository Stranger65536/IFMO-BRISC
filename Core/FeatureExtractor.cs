using System;
using System.Collections.Generic;
using System.Text;

namespace BRISC.Core
{
    interface FeatureExtractor
    {
        void ExtractFeatures(LIDCNodule nodule);
    }
}
