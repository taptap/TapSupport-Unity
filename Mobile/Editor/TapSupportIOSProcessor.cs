#if UNITY_IOS

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using TapTap.Common.Editor;

namespace TapTap.Support.Editor {
    public class TapSupportIOSProcessor {
        // 添加标签，unity导出工程后自动执行该函数
        [PostProcessBuild(107)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {
            if (buildTarget != BuildTarget.iOS) return;
            // 获得工程路径
            var projPath = TapCommonCompile.GetProjPath(path);
            var proj = TapCommonCompile.ParseProjPath(projPath);
            var target = TapCommonCompile.GetUnityTarget(proj);

            if (TapCommonCompile.CheckTarget(target)) {
                Debug.LogError("Unity-iPhone is NUll");
                return;
            }

            if (TapCommonCompile.HandlerIOSSetting(path,
                Application.dataPath,
                "TapSupportResource",
                "com.taptap.tds.support",
                "Support",
                new[] { "TapSupportResource.bundle" },
                target, projPath, proj)) {
                Debug.Log("TapSupport add Bundle Success!");
                return;
            }

            Debug.LogWarning("TapSupport add Bundle Failed!");
        }
    }
}

#endif