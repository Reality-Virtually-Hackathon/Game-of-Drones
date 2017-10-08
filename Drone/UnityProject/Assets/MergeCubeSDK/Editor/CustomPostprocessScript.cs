#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Xml;
using System.IO;

using Images2Video.Editor;

public class CustomPostprocessScript {
	[PostProcessBuildAttribute(1080)]//To make sure the other scripts are executed.
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject) {
		if (pathToBuildProject.StartsWith("./") || !pathToBuildProject.StartsWith("/"))   // Fix three erroneous path cases on Unity 5.4f03
        {
            pathToBuildProject = Path.Combine(Application.dataPath.Replace("Assets", ""), pathToBuildProject.Replace("./", ""));
        }
        else if (pathToBuildProject.Contains("./"))
        {
            pathToBuildProject = pathToBuildProject.Replace("./", "");
        }

		UnityEngine.Debug.Log("----Executing post process build phase----");
		PBXModifier mod = new PBXModifier();
		string pbxproj = Path.Combine(pathToBuildProject, "Unity-iPhone.xcodeproj/project.pbxproj");
		if (!File.Exists(pbxproj))
			return;
			
		var lines = mod.applyToAssetLibrary (pbxproj);
		File.WriteAllLines (pbxproj, lines);

		UnityEngine.Debug.Log("----Finished executing post process build phase");

		ProcessInfoPList(pathToBuildProject);
	}

	/// <summary>
    /// Ref : EveryplayPostprocessor.cs 
    /// </summary>
    /// <param name="path">Project path</param>
	private static void ProcessInfoPList(string path)
	{
		try {
			string file = System.IO.Path.Combine(path, "Info.plist");
			if (!File.Exists(file))
				return;

			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(file);
			XmlNode dict = xmlDocument.SelectSingleNode("plist/dict");

			if (dict != null) {

				PListItem photoLibraryUsageDesc = GetPlistItem(dict, "NSPhotoLibraryUsageDescription");
				if (photoLibraryUsageDesc == null)
				{
					XmlElement key = xmlDocument.CreateElement("key");
					key.InnerText = "NSPhotoLibraryUsageDescription";

					XmlElement str = xmlDocument.CreateElement("string");
					str.InnerText = "Save Game Recording requires access to the photo library";

					dict.AppendChild(key);
					dict.AppendChild(str);
				}

				xmlDocument.Save(file);

				// Remove extra gargabe added by the XmlDocument save
                UpdateStringInFile(file, "dtd\"[]>", "dtd\">");
			} else {
				Debug.Log("Info.plist is not valid");
			}
		} catch (Exception e) {
			Debug.Log("Unable to update Info.plist : " + e);
		}
	}

	public class PListItem
    {
        public XmlNode itemKeyNode;
        public XmlNode itemValueNode;

        public PListItem(XmlNode keyNode, XmlNode valueNode)
        {
            itemKeyNode = keyNode;
            itemValueNode = valueNode;
        }
    }

    public static PListItem GetPlistItem(XmlNode dict, string name)
    {
        for (int i = 0; i < dict.ChildNodes.Count - 1; i++)
        {
            XmlNode node = dict.ChildNodes.Item(i);

            if (node.Name.ToLower().Equals("key") && node.InnerText.ToLower().Equals(name.Trim().ToLower()))
            {
                XmlNode valueNode = dict.ChildNodes.Item(i + 1);

                if (!valueNode.Name.ToLower().Equals("key"))
                {
                    return new PListItem(node, valueNode);
                }
                else
                {
                    Debug.Log("Value for key missing in Info.plist");
                }
            }
        }

        return null;
    }

	private static void UpdateStringInFile(string file, string subject, string replacement)
    {
        try
        {
            if (!File.Exists(file))
            {
                return;
            }

            string processedContents = "";

            using (StreamReader sr = new StreamReader(file))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    processedContents += line.Replace(subject, replacement) + "\n";
                }
            }

            File.Delete(file);

            using (StreamWriter streamWriter = File.CreateText(file))
            {
                streamWriter.Write(processedContents);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Unable to update string in file: " + e);
        }
    }
}
#endif
	 