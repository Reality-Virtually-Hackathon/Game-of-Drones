#if UNITY_IOS
using System.IO;
using System.Collections.Generic;

namespace Images2Video.Editor {
	public class PBXModifier {
		const string assetsLibrary1 = "6AA3AE8F1DCC7D690072F0BE /* AssetsLibrary.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 6A4B51BA1A25A3880063B77B /* AssetsLibrary.framework */; };";
		const string assetsLibrary2 = "6A4B51BA1A25A3880063B77B /* AssetsLibrary.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = AssetsLibrary.framework; path = System/Library/Frameworks/AssetsLibrary.framework; sourceTree = SDKROOT; };";
		const string assetsLibrary3 = "6AA3AE8F1DCC7D690072F0BE /* AssetsLibrary.framework in Frameworks */,";
		const string assetsLibrary4 = "6A4B51BA1A25A3880063B77B /* AssetsLibrary.framework */,";

		/// <summary>
		/// Initializes a new instance of the <see cref="Images2Video.Editor.PBXModifier"/> class.
		/// </summary>
		public PBXModifier() {}
		
		public string[] applyToAssetLibrary(string file) {
			List<string> lines = new List<string>(File.ReadAllLines(file));
			if (lines.Count < 10) {
				return lines.ToArray();
			}
			
			if (contains(lines, "AssetsLibrary.framework")) {
				return lines.ToArray();
			}
			
			var assetsLibraryIndex = indexOf (lines, "/* Begin PBXBuildFile section */");
			if (assetsLibraryIndex == -1) {//Find the last
				return lines.ToArray();
			}
			
			lines.Insert(assetsLibraryIndex + 1, assetsLibrary2);
			lines.Insert(assetsLibraryIndex + 1, assetsLibrary1);
			
			assetsLibraryIndex = indexOf (lines, "isa = PBXFrameworksBuildPhase");
			if (assetsLibraryIndex == -1) {
				return lines.ToArray();
			}
			
			lines.Insert(assetsLibraryIndex + 3, assetsLibrary3);
			
			assetsLibraryIndex = indexOf (lines, "/* CustomTemplate */ = {");
			if (assetsLibraryIndex == -1) {
				return lines.ToArray();
			}
			
			lines.Insert(assetsLibraryIndex + 3, assetsLibrary4);
			return lines.ToArray();
		}
		
		private static int indexOf (List<string> lines, string value) {
			for (int i = 0; i < lines.Count; i++) {
				if (lines[i].Contains(value))
					return i;
			}
			
			return -1;
		}

		private static bool contains(List<string> lines, string value) {
			foreach (var s in lines) {
				if (s.Contains(value))
					return true;
			}
			
			return false;
		}
	}
}
#endif