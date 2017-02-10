// The MIT License (MIT)
//
// Copyright (c) 2016 Runar Ovesen Hjerpbakk
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Xamarin.UITest;
using Xamarin.UITest.iOS;

namespace Xnapshot {
    /// <summary>
    /// Inherit from this class to automatically create screenshots 
    /// for all devices supported by your app. Override the 
    /// SetAppStateForScreenshotX-methods to automate your app, 
    /// creating the correct state for screenshots to be taken.
    /// </summary>
    public abstract class Screenshots {
        readonly string deviceType;
        readonly string osVersion;
        readonly DirectoryInfo screenshotsDirectory;
        readonly string appBundlePath;

        uint screenshotIndex;

        /// <summary>
        /// You need to supply the device type (iPhone or iPad), the iOS version to use, 
        /// path to a folder to save the screenshots and the path to App bundle to run. 
        /// </summary>
        /// <param name="deviceType">The device type (iPhone or iPad).</param>
        /// <param name="osVersion">The iOS version to use.</param>
        /// <param name="screenshotsPath">Path to a folder to save the screenshots.</param>
        /// <param name="appBundlePath">Path to App bundle to run.</param>
        protected Screenshots(DeviceType deviceType, string osVersion, string screenshotsPath, string appBundlePath) {
            VerifyArguments(osVersion, screenshotsPath, appBundlePath);

            this.deviceType = Enum.GetName(typeof(DeviceType), deviceType);
            this.osVersion = osVersion;
            screenshotsDirectory = new DirectoryInfo(screenshotsPath);
            this.appBundlePath = appBundlePath;

            OptimizeImagesAfterSave = false;
            SaveScreenshots = true;
        }

        /// <summary>
        /// The app to automate. Use Xamarin.UITest to interact with the app,
        /// making the app ready for screenshots.
        /// </summary>
        /// <value>The app to automate.</value>
        protected iOSApp App { get; private set; }

        /// <summary>
        /// Set to true if you want to use ImageOptim to optimize the screenshots after saving. 
        /// ImageOptim needs to be installed in the Applications-folder.
        /// 
        /// Set to false by default.
        /// 
        /// ImageOptim can be installed from https://imageoptim.com
        /// </summary>
        /// <value><c>true</c> if images are to be optimized after save; otherwise, <c>false</c>.</value>
        protected bool OptimizeImagesAfterSave { private get; set; }

        /// <summary>
        /// Set to false if you don’t want the screenshots to be saved and want to try a dry run. 
        /// The different app states will still be run in the correct sequence.
        ///
        /// Set to true by default.
        /// </summary>
        /// <value><c>true</c> if screenshots are to be saved; otherwise, <c>false</c>.</value>
        protected bool SaveScreenshots { private get; set; }

        /// <summary>
        /// Takes the screenshots using the currently configured options.
        /// 
        /// The app will be automated using the SetAppStateForScreenshotX methods
        /// for all available simulators given the selected device type.
        /// 
        /// The SetAppStateForScreenshotX methods will be run in sequence
        /// and screenshots taken after each one if configured.
        /// </summary>
        public void TakeScreenshots() {
            ClearScreenshotsDirectory();
            var simulators = DeviceSetParser.GetAvailableSimulators(osVersion, deviceType);
            foreach (var device in simulators) {
                App = ConfigureApp
                    .Debug().EnableLocalScreenshots()
                    .iOS
                    .AppBundle(appBundlePath)
                    .DeviceIdentifier(device.UUID)
                    .StartApp();

                TakeScreenShot(device.Name);
            }
        }

        /// <summary>
        /// Implement this in your derived class to make the app ready for the first screenshot.
        /// If the method is empty, the app will maintain its default state.
        /// After this method has run, a screenshot will be taken if configured.
        /// </summary>
        protected abstract void SetAppStateForScreenshot1();

        /// <summary>
        /// Implement this in your derived class to make the app ready for the second screenshot.
        /// If the method is empty, the app will maintain its last state.
        /// After this method has run, a screenshot will be taken if configured.
        /// </summary>
        protected abstract void SetAppStateForScreenshot2();

        /// <summary>
        /// Implement this in your derived class to make the app ready for the third screenshot.
        /// If the method is empty, the app will maintain its last state.
        /// After this method has run, a screenshot will be taken if configured.
        /// </summary>
        protected abstract void SetAppStateForScreenshot3();

        /// <summary>
        /// Implement this in your derived class to make the app ready for the fourth screenshot.
        /// If the method is empty, the app will maintain its last state.
        /// After this method has run, a screenshot will be taken if configured.
        /// </summary>
        protected abstract void SetAppStateForScreenshot4();

        /// <summary>
        /// Implement this in your derived class to make the app ready for the fifth screenshot.
        /// If the method is empty, the app will maintain its last state.
        /// After this method has run, a screenshot will be taken if configured.
        /// </summary>
        protected abstract void SetAppStateForScreenshot5();

        static void VerifyArguments(string osVersion, string screenshotsPath, string appBundlePath) {
            if (osVersion == null) {
                throw new ArgumentNullException("osVersion");
            }

            var regex = new Regex("([a-z][A-Z]*)+[-]([0-9]*)[-][0-9]");
            if (!regex.IsMatch(osVersion)) {
                const string Message = "osVersion must be OS name followed by OS version, seperated by \"-\". ";
                var example = "Example: \"iOS-9-2\". osVersion was: \"" + osVersion + "\".";
                throw new ArgumentException(Message + example, "osVersion");
            }

            if (screenshotsPath == null) {
                throw new ArgumentNullException("screenshotsPath");
            }

            if (!Directory.Exists(screenshotsPath)) {
                Directory.CreateDirectory(screenshotsPath);
            }

            if (appBundlePath == null) {
                throw new ArgumentNullException("appBundlePath");
            }

            if (!Directory.Exists(appBundlePath)) {
                throw new ArgumentException("Could not find App bundle at: \"" + appBundlePath + "\".", 
                    "appBundlePath");
            }
        }

        void TakeScreenShot(string deviceName) {
            TakeScreenshot(SetAppStateForScreenshot1, deviceName);
            TakeScreenshot(SetAppStateForScreenshot2, deviceName);
            TakeScreenshot(SetAppStateForScreenshot3, deviceName);
            TakeScreenshot(SetAppStateForScreenshot4, deviceName);
            TakeScreenshot(SetAppStateForScreenshot5, deviceName);
        }

        void TakeScreenshot(Action readyAppForScreenshot, string deviceName) {
            readyAppForScreenshot();
            if (!SaveScreenshots) {
                return;
            }

            ++screenshotIndex;
            var screenshotFile = App.Screenshot("temp");
            var filename = screenshotIndex + " " + deviceName + Path.GetExtension(screenshotFile.FullName);
            var destinationFileName = Path.Combine(screenshotsDirectory.FullName, filename);
            screenshotFile.MoveTo(destinationFileName);
            if (OptimizeImagesAfterSave) {
                ImageOptimizer.OptimizeImage(destinationFileName);
            }
        }

        void ClearScreenshotsDirectory() {
            if (!SaveScreenshots) {
                return;
            }

            foreach (var file in screenshotsDirectory.EnumerateFileSystemInfos()) {
                file.Delete();
            }
        }

        static class DeviceSetParser {
            public static IEnumerable<Device> GetAvailableSimulators(string osVersion, string deviceType) {
                var simulatorPath = Path.Combine("/Users",
                    Environment.UserName,
                    "Library/Developer/CoreSimulator/Devices/");
                var plistPath = Path.Combine(simulatorPath, "device_set.plist");

                var deviceSet = ReadDeviceSetPlist(plistPath);
                var defaultDevices = (Dictionary<string, object>)deviceSet["DefaultDevices"];

                var fulliOSVersion = defaultDevices.Keys.SingleOrDefault(
                    k => k.EndsWith(osVersion, StringComparison.InvariantCulture));
                var devicesForVersion = (Dictionary<string, object>)defaultDevices[fulliOSVersion];

                var availableSimulators = new List<Device>();
                foreach (var device in devicesForVersion.Keys.Where(
                    k => k.Contains(deviceType) 
					// NOTE: Change this to include other models if needed
					&& (k.Contains("7") || k.Contains("7plus") || k.Contains("SE")))) {
                    availableSimulators.Add(new Device((string)devicesForVersion[device], device));
                }

                return availableSimulators;
            }

            static Dictionary<string, object> ReadDeviceSetPlist(string path) {
                using (var deviceSet = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    var xml = new XmlDocument();
                    xml.XmlResolver = null;
                    xml.Load(deviceSet);

                    var rootNode = xml.DocumentElement.ChildNodes[0];
                    return (Dictionary<string, object>)Parse(rootNode);
                }
            }

            static object Parse(XmlNode node) {
                switch (node.Name) {
                    case "dict":
                        return parseDictionary(node);
                    case "string":
                        return node.InnerText;
                    case "integer":
                        return Convert.ToInt32(node.InnerText, System.Globalization.NumberFormatInfo.InvariantInfo);
                    case "true":
                    case "false":
                        return bool.Parse(node.Name);
                    default:
                        throw new InvalidOperationException("Unexpted content in device_set.plist " + node.Name);
                }
            }

            static Dictionary<string, object> parseDictionary(XmlNode node) {
                var children = node.ChildNodes;
                if (children.Count % 2 != 0) {
                    throw new DataMisalignedException(
                        "Dictionary elements must have an even number of child nodes, was " + children.Count);
                }

                var dict = new Dictionary<string, object>();
                for (int i = 0; i < children.Count; i += 2) {
                    XmlNode keynode = children[i];
                    XmlNode valnode = children[i + 1];
                    if (keynode.Name != "key") {
                        throw new InvalidOperationException("Expected a key node, was " + keynode.Name);
                    }

                    var result = Parse(valnode);
                    if (result != null) {
                        dict.Add(keynode.InnerText, result);
                    }
                }

                return dict;
            }
        }

        struct Device {
            public Device(string uuid, string name) {
                UUID = uuid;
                Name = name;
            }

            public string UUID { get; }
            public string Name { get; }
        }

        static class ImageOptimizer {
            public static void OptimizeImage(string filePath) {
                var optimizeImage = new ProcessStartInfo {
                    FileName = "/Applications/ImageOptim.app/Contents/MacOS/ImageOptim",
                    UseShellExecute = false,
                    Arguments = "\"" + filePath + "\""
                };

                Process.Start(optimizeImage);
            }
        }
    }

    /// <summary>
    /// Contains the supported device types.
    /// </summary>
    public enum DeviceType {
        /// <summary>
        /// The iPhone.
        /// </summary>
        iPhone,

        /// <summary>
        /// The iPad.
        /// </summary>
        iPad
    }
}

