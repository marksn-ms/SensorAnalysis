using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Reflection;

namespace WorldSim
{
    public class AgentsConfigSectionHandler : IConfigurationSectionHandler
    {
        public AgentsConfigSectionHandler()
        {
        }

        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            try
            {
                Dictionary<string, Assembly> lstAssemblies = new Dictionary<string, Assembly>();
                Dictionary<string, Type> lstAgentTypes = new Dictionary<string, Type>();
                foreach (XmlNode node in section.SelectNodes("Agent"))
                {
                    string strName = ((XmlElement)node).GetAttribute("name");
                    string strType = ((XmlElement)node).GetAttribute("type");
                    string strAssembly = ((XmlElement)node).GetAttribute("assembly");

                    Assembly a = null;
                    try
                    {
                        if (lstAssemblies.ContainsKey(strAssembly))
                            a = lstAssemblies[strAssembly];
                        else
                            a = Assembly.LoadFrom(strAssembly);
                    }
                    catch 
                    { 
                        throw new ApplicationException("Unable to load assembly for " + strName + " agent.");
                    }

                    Type tAgent = a.GetType(strType);
                    lstAgentTypes.Add(strName, tAgent);
                }
                return lstAgentTypes;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }

    public class WatchersConfigSectionHandler : IConfigurationSectionHandler
    {
        public WatchersConfigSectionHandler()
        {
        }

        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            try
            {
                Dictionary<string, Assembly> lstAssemblies = new Dictionary<string, Assembly>();
                Dictionary<string, Type> lstAgentTypes = new Dictionary<string, Type>();
                foreach (XmlNode node in section.SelectNodes("Watcher"))
                {
                    string strName = ((XmlElement)node).GetAttribute("name");
                    string strType = ((XmlElement)node).GetAttribute("type");
                    string strAssembly = ((XmlElement)node).GetAttribute("assembly");

                    Assembly a = null;
                    try
                    {
                        if (lstAssemblies.ContainsKey(strAssembly))
                            a = lstAssemblies[strAssembly];
                        else
                            a = Assembly.LoadFrom(strAssembly);
                    }
                    catch
                    {
                        throw new ApplicationException("Unable to load assembly for " + strName + " watcher.");
                    }

                    Type tAgent = a.GetType(strType);
                    lstAgentTypes.Add(strName, tAgent);
                }
                return lstAgentTypes;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }

    public class DeployersConfigSectionHandler : IConfigurationSectionHandler
    {
        public DeployersConfigSectionHandler()
        {
        }

        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            try
            {
                Dictionary<string, Assembly> lstAssemblies = new Dictionary<string, Assembly>();
                Dictionary<string, Type> lstAgentTypes = new Dictionary<string, Type>();
                foreach (XmlNode node in section.SelectNodes("Deployer"))
                {
                    string strName = ((XmlElement)node).GetAttribute("name");
                    string strType = ((XmlElement)node).GetAttribute("type");
                    string strAssembly = ((XmlElement)node).GetAttribute("assembly");

                    Assembly a = null;
                    try
                    {
                        if (lstAssemblies.ContainsKey(strAssembly))
                            a = lstAssemblies[strAssembly];
                        else
                            a = Assembly.LoadFrom(strAssembly);
                    }
                    catch
                    {
                        throw new ApplicationException("Unable to load assembly for " + strName + " deployer.");
                    }

                    Type tAgent = a.GetType(strType);
                    lstAgentTypes.Add(strName, tAgent);
                }
                return lstAgentTypes;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
