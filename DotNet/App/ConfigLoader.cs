using System;
using System.Collections.Generic;
using System.IO;

namespace ET.Server
{
    [Invoke]
    public class GetAllConfigBytes: AInvokeHandler<ConfigComponent.GetAllConfigBytes, Dictionary<Type, byte[]>>
    {
        public override Dictionary<Type, byte[]> Handle(ConfigComponent.GetAllConfigBytes args)
        {
            Dictionary<Type, byte[]> output = new Dictionary<Type, byte[]>();
            HashSet<Type> configTypes = EventSystem.Instance.GetTypes(typeof (ConfigAttribute));
            foreach (Type configType in configTypes)
            {
                string configFilePath = GetConfigFilePath.getConfigFilePath(configType.Name);
                output[configType] = File.ReadAllBytes(configFilePath);
            }

            return output;
        }
    }

    [Invoke]
    public class GetOneConfigBytes: AInvokeHandler<ConfigComponent.GetOneConfigBytes, byte[]>
    {
        public override byte[] Handle(ConfigComponent.GetOneConfigBytes args)
        {
            string configFilePath = GetConfigFilePath.getConfigFilePath(args.ConfigName);
            return File.ReadAllBytes(configFilePath);
        }
    }

    public static class GetConfigFilePath
    {
        public static string getConfigFilePath(string configName)
        {
            string configFilePath;
            switch (configName)
            {
                case "StartMachineConfigCategory":
                case "StartProcessConfigCategory":
                case "StartSceneConfigCategory":
                case "StartZoneConfigCategory":
                    configFilePath = $"../Config/Excel/{Options.Instance.StartConfig}/{configName}.bytes";
                    break;
                default:
                    configFilePath = $"../Config/Excel/{configName}.bytes";
                    break;
            }

            return configFilePath;
        }
    }
}