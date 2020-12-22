using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Switcheroo {

    public sealed class PortableSettingsProvider : SettingsProvider, IApplicationSettingsProvider {
        private const string _rootNodeName = "settings";
        private const string _localSettingsNodeName = "localSettings";
        private const string _globalSettingsNodeName = "globalSettings";
        private const string _className = "PortableSettingsProvider";
        private XmlDocument _xmlDocument;

        private string _filePath {
            get {
                return Path.Combine(Path.GetDirectoryName(Application.ExecutablePath),
                    string.Format("{0}.settings", ApplicationName));
            }
        }

        private XmlNode _localSettingsNode {
            get {
                XmlNode settingsNode = GetSettingsNode(_localSettingsNodeName);
                XmlNode machineNode = settingsNode.SelectSingleNode(Environment.MachineName.ToLowerInvariant());

                if (machineNode == null) {
                    machineNode = RootDocument.CreateElement(Environment.MachineName.ToLowerInvariant());
                    settingsNode.AppendChild(machineNode);
                }

                return machineNode;
            }
        }

        private XmlNode GlobalSettingsNode {
            get { return GetSettingsNode(_globalSettingsNodeName); }
        }

        private XmlNode RootNode {
            get { return RootDocument.SelectSingleNode(_rootNodeName); }
        }

        private XmlDocument RootDocument {
            get {
                if (_xmlDocument == null) {
                    try {
                        _xmlDocument = new XmlDocument();
                        _xmlDocument.Load(_filePath);
                    } catch (Exception) {
                        // TODO do something with the exception
                    }

                    if (_xmlDocument.SelectSingleNode(_rootNodeName) != null)
                        return _xmlDocument;

                    _xmlDocument = GetBlankXmlDocument();
                }

                return _xmlDocument;
            }
        }

        public override string ApplicationName {
            get { return Path.GetFileNameWithoutExtension(Application.ExecutablePath); }
            set { }
        }

        public override string Name {
            get { return _className; }
        }

        public override void Initialize(string name, NameValueCollection config) {
            base.Initialize(Name, config);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection) {
            foreach (SettingsPropertyValue propertyValue in collection)
                SetValue(propertyValue);

            try {
                RootDocument.Save(_filePath);
            } catch (Exception) {
                /* 
                 * If this is a portable application and the device has been 
                 * removed then this will fail, so don't do anything. It's 
                 * probably better for the application to stop saving settings 
                 * rather than just crashing outright. Probably.
                 */
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
            SettingsPropertyCollection collection) {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            foreach (SettingsProperty property in collection) {
                values.Add(new SettingsPropertyValue(property) {
                    SerializedValue = GetValue(property)
                });
            }

            return values;
        }

        private void SetValue(SettingsPropertyValue propertyValue) {
            XmlNode targetNode = IsGlobal(propertyValue.Property)
                ? GlobalSettingsNode
                : _localSettingsNode;

            XmlNode settingNode = targetNode.SelectSingleNode(string.Format("setting[@name='{0}']", propertyValue.Name));

            if (settingNode != null) {
                settingNode.InnerText = propertyValue.SerializedValue.ToString();
            } else {
                settingNode = RootDocument.CreateElement("setting");

                XmlAttribute nameAttribute = RootDocument.CreateAttribute("name");
                nameAttribute.Value = propertyValue.Name;

                settingNode.Attributes.Append(nameAttribute);
                settingNode.InnerText = propertyValue.SerializedValue.ToString();

                targetNode.AppendChild(settingNode);
            }
        }

        private string GetValue(SettingsProperty property) {
            XmlNode targetNode = IsGlobal(property) ? GlobalSettingsNode : _localSettingsNode;
            XmlNode settingNode = targetNode.SelectSingleNode(string.Format("setting[@name='{0}']", property.Name));

            if (settingNode == null)
                return property.DefaultValue != null ? property.DefaultValue.ToString() : string.Empty;

            return settingNode.InnerText;
        }

        private bool IsGlobal(SettingsProperty property) {
            foreach (DictionaryEntry attribute in property.Attributes) {
                if ((Attribute)attribute.Value is SettingsManageabilityAttribute)
                    return true;
            }

            return false;
        }

        private XmlNode GetSettingsNode(string name) {
            XmlNode settingsNode = RootNode.SelectSingleNode(name);

            if (settingsNode == null) {
                settingsNode = RootDocument.CreateElement(name);
                RootNode.AppendChild(settingsNode);
            }

            return settingsNode;
        }

        public XmlDocument GetBlankXmlDocument() {
            XmlDocument blankXmlDocument = new XmlDocument();
            blankXmlDocument.AppendChild(blankXmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
            blankXmlDocument.AppendChild(blankXmlDocument.CreateElement(_rootNodeName));

            return blankXmlDocument;
        }

        public void Reset(SettingsContext context) {
            _localSettingsNode.RemoveAll();
            GlobalSettingsNode.RemoveAll();

            _xmlDocument.Save(_filePath);
        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property) {
            // do nothing
            return new SettingsPropertyValue(property);
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties) {
        }
    }
}