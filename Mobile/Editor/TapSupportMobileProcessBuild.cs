using System.IO;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using TapTap.Common.Editor;

namespace TapTap.Support.Editor {
    public class TapSupportMobilePreprocessBuild : IPreprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) {
            if (!BuildTargetUtils.IsSupportMobile(report.summary.platform)) {
                return;
            }

            string linkPath = Path.Combine(Application.dataPath, "TapTap/Support/link.xml");
            LinkXMLGenerator.Generate(linkPath,
                new LinkedAssembly[] {
                    new LinkedAssembly { Fullname = "TapTap.Support.Runtime" },
                    new LinkedAssembly { Fullname = "TapTap.Support.Mobile.Runtime" }
                });
        }
    }

    public class TapSupportMobilePostprocessBuild : IPostprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report) {
            if (!BuildTargetUtils.IsSupportMobile(report.summary.platform)) {
                return;
            }

            string linkPath = Path.Combine(Application.dataPath, "TapTap/Support/link.xml");
            LinkXMLGenerator.Delete(linkPath);
        }
    }
}
