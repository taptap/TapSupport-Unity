using System.IO;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using TapTap.Common.Editor;

namespace TapTap.Support.Editor {
    public class TapSupportStandalonePreprocessBuild : IPreprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) {
            if (!BuildTargetUtils.IsSupportStandalone(report.summary.platform)) {
                return;
            }

            string linkPath = Path.Combine(Application.dataPath, "TapTap/Support/link.xml");
            LinkXMLGenerator.Generate(linkPath,
                new LinkedAssembly[] {
                    new LinkedAssembly { Fullname = "TapTap.Support.Runtime" },
                    new LinkedAssembly { Fullname = "TapTap.Support.Standalone.Runtime" }
                });
        }
    }

    public class TapSupportStandalonePostprocessBuild : IPostprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report) {
            if (!BuildTargetUtils.IsSupportStandalone(report.summary.platform)) {
                return;
            }

            string linkPath = Path.Combine(Application.dataPath, "TapTap/Support/link.xml");
            LinkXMLGenerator.Delete(linkPath);
        }
    }
}

