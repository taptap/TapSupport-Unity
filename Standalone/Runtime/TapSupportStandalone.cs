#if UNITY_STANDALONE || UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LC.Newtonsoft.Json;
using TapTap.Common;

[assembly: UnityEngine.Scripting.Preserve]
namespace TapTap.Support.Internal.Platform {
    public class TapSupportStandalone : ITapSupport {
        public void Init(string serverUrl, string productID, TapSupportDelegate supportDelegate)
        {
        }

        public void OpenSupportView(string path){
            Application.OpenURL(path);
        }

        public void CloseSupportView() {
            throw new NotImplementedException();
        }
        
    }
}

#endif
