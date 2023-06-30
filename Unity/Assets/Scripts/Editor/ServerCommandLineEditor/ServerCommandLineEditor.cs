using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public enum DevelopMode
    {
        正式 = 0,
        开发 = 1,
        压测 = 2,
    }
    
    public class ServerCommandLineEditor: EditorWindow
    {
        [MenuItem("ET/ServerTools")]
        public static void ShowWindow()
        {
            GetWindow<ServerCommandLineEditor>(DockDefine.Types);
        }
        
        private int selectStartConfigIndex = 0;
        private string[] startConfigs;
        private string startConfig;
        private DevelopMode developMode;

        public void OnEnable()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo("../Config/Excel/StartConfig");
            this.startConfigs = directoryInfo.GetDirectories().Select(x => x.Name).ToArray();
        }

        public void OnGUI()
        {
            if (Logger.Instance == null)
            {
                Game.AddSingleton<Options>().Console = 1;
                Game.AddSingleton<Logger>().ILog = new UnityLogger();
            }

            selectStartConfigIndex = EditorGUILayout.Popup(selectStartConfigIndex, this.startConfigs);
            this.startConfig = this.startConfigs[this.selectStartConfigIndex];
            this.developMode = (DevelopMode) EditorGUILayout.EnumPopup("起服模式：", this.developMode);

            string dotnet = "dotnet.exe";
            
#if UNITY_EDITOR_OSX
            dotnet = "dotnet";
#endif
            
            if (GUILayout.Button("Start Server(Single Process)"))
            {
                string arguments = $"App.dll --Process=1 --StartConfig=StartConfig/{this.startConfig} --Console=1";
                ProcessHelper.Run(dotnet, arguments, "../Bin/");
            }
            
            if (GUILayout.Button("Start Watcher"))
            {
                string arguments = $"App.dll --AppType=Watcher --StartConfig=StartConfig/{this.startConfig} --Console=1";
                ProcessHelper.Run(dotnet, arguments, "../Bin/");
            }

            if (GUILayout.Button("Start Mongo"))
            {
                ProcessHelper.Run("mongod", @"--dbpath=db", "../Database/bin/");
            }
        }
    }
}
