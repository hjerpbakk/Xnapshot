using System;

namespace Xnapshot.Example.Usage {
    public class ExampleScreenshots : Screenshots {
        public ExampleScreenshots() : base(
            // The iOS version to use
            "iOS-12-2",
            // The folder to which screenshots should be saved, will be emptied before each run.
            "/Users/sankra/Downloads/Screenshots",
            // The path to a debug-version of the app
            "/Users/sankra/projects/Xnapshot/Xnapshot.Example.iOS/bin/iPhoneSimulator/Debug/Xnapshot.Example.iOS.app",
            // The devices to screenshot, must be at least one.
            // The following covers all iPhone sizes at the time of writing.
            new[] {
                "iPhone-SE",
                "iPhone-XS"
            }) {

            // Optional, whether the screenshots should be saved to disk. Default: true. 
            SaveScreenshots = true;

            // Optional, whether screenshots should be minified using ImageOptim. Default: false.
            OptimizeImagesAfterSave = false;
        }

        protected override void SetAppStateForScreenshot1() {
            // An empty method will screenshot the app as-is.
            // Since this is the first method, it will screenshot the app's first screen.
        }

        protected override void SetAppStateForScreenshot2() {
            // TODO: Press button in screenshot 2
        }

        protected override void SetAppStateForScreenshot3() {
            // An empty method which is not SetAppStateForScreenshot1,
            // will take the same screenshot as the previous method.
        }

        // Screenshots 4 through 10 are not implemented in this example.
        // Just override the other numbered methods to take more screenshots.
    }
}

