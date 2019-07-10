using System;

namespace Xnapshot.Example.Usage {
    class MainClass {
        public static void Main() {
            var screenshots = new ExampleScreenshots();
            screenshots.TakeScreenshots();

            Environment.Exit(0);
        }
    }
}
