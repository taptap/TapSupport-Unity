using System;
using UnityEditor.Build.Reporting;
using TapTap.Common.Editor;

namespace TapTap.Support.Editor {
    public class TapSupportStandaloneProcessBuild : SDKLinkProcessBuild {
        public override int callbackOrder => 0;

        public override string LinkPath => "TapTap/Support/link.xml";

        public override LinkedAssembly[] LinkedAssemblies => new LinkedAssembly[] {
                    new LinkedAssembly { Fullname = "TapTap.Support.Runtime" },
                    new LinkedAssembly { Fullname = "TapTap.Support.Standalone.Runtime" }
                };

        public override Func<BuildReport, bool> IsTargetPlatform => (report) => {
            return BuildTargetUtils.IsSupportStandalone(report.summary.platform);
        };
    }
}

