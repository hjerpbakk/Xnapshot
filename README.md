# Xnapshot

> Xnapshot enables you to use C#, together with Xamarin.UITest, to automatically take app screenshots for you. Just derive from the abstract Screenshots class, implement one method per screenshot and use your time productively while your computer takes the screenshots.

[![Build status](https://ci.appveyor.com/api/projects/status/tkls3ydfbu9lcmqu?svg=true)](https://ci.appveyor.com/project/Sankra/xnapshot) [![Latest version](https://img.shields.io/nuget/v/Xnapshot.svg)](https://www.nuget.org/packages/Xnapshot/) [![Downloads from NuGet](https://img.shields.io/nuget/dt/Xnapshot.svg)](https://www.nuget.org/packages/Xnapshot/)

Taking screenshots of your app on every device and localization quickly becomes time-consuming. With 2 languages, 6 different iPhones and 10 screenshots, you are faced with 120 screenshots per release. If we increase the number of languages to 10 and add iPad support, this number explodes to 10 (languages) x 11 (devices) x 10 (screenshots) = **1 100 screenshots**!

`Xnapshot` enables you to use C#, together with Xamarin.UITest, to automatically take the screenshots for you. Just derive from the abstract [Screenshots](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot/Screenshots.cs#L49) class, implement one method per screenshot and use your time productively while your computer takes the screenshots.

## Complete working example

Download this repository, open the solution in Visual Studio for Mac and run [Xnapshot.Example.Usage](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot.Example.Usage/Program.cs) to see a working example. The solution contains the Xnapshot source, an iOS app to be screenshotted and the configuration of Xnapshot to actually take screenshots.

## Usage

### Prerequisites

- Create your [awesome iOS app](https://itunes.apple.com/no/app/id953899091?at=11l5UV&ct=website) using C# and Xamarin.
- Add the [Xamarin.TestCloud.Agent](https://www.nuget.org/packages/Xamarin.TestCloud.Agent/) NuGet package to your iOS project and update your `AppDelegate` class to enable [Calabash](https://github.com/calabash), the remote control for your app, while running in debug mode.

```cs
public override void FinishedLaunching(UIApplication application) {
  #if DEBUG
    Xamarin.Calabash.Start();
  #endif
…
```

### Configuration

I use the settings for one of my own apps, [Golden Ratio Calculator](https://itunes.apple.com/no/app/id953899091?at=11l5UV&ct=website), to illustrate the usage of Xnapshot.

- Add a new .Net Framework `Console project` to your solution and add the [Xnapshot](https://www.nuget.org/packages/Xnapshot/) NuGet package. I name such projects `[AppName].Screens`. The project must a .Net Framework project, version 4.5 or newer. .Net Core is not supported as Xamarin.UITests, which is needed to automate the before each screenshot, does not support .Net Core.
- Create a new class, `[AppName]Screenshots` and derive from the abstract [Xnapshot.Screenshots](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot/Screenshots.cs#L49) class.
- Add your iOS version, screenshots folder, the path to your App bundle and devices you wish to screenshot as constructor arguments. The devices listed below covers all iPhone screen sizes of the time of writing.

```cs
public class GoldenRatioScreenshots : Screenshots {
  public GoldenRatioScreenshots() : base(
    "iOS-12-2",
    "/Users/sankra/Projects/GoldenRatioCalculator/screenshots/en-US",
    "/Users/sankra/Projects/GoldenRatioCalculator/iOS/bin/iPhoneSimulator/Debug/GoldenRatioCalculatoriOS.app",
    new[] {
      "iPhone-XS-Max",
      "iPhone-XS",
      "iPhone-XR",
      "iPhone-8-Plus",
      "iPhone-8",
      "iPhone-SE"
    }) {
  }
}
```

- It’s now time to implement the [SetAppStateForScreenshotX](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot/Screenshots.cs#L124) methods in `[AppName]Screenshots`. [Xamarin.UITest](https://docs.microsoft.com/en-us/appcenter/test-cloud/uitest/) is used in these methods to automate your [app](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot.Example.Usage/ExampleScreenshots.cs#L24), putting it in the correct state before each screenshot. `SetAppStateForScreenshot1` is empty below because the first screenshot is of the first screen.

```cs
protected override void SetAppStateForScreenshot1() {
}

protected override void SetAppStateForScreenshot2() {
  App.Tap(c => c.Marked("ratioPicker"));
  App.Tap(v => v.Text("Silver Ratio"));
  App.Tap(c => c.Marked("Done"));
}

protected override void SetAppStateForScreenshot3() {
  App.Tap(c => c.Marked("ratioPicker"));
  App.Tap(v => v.Text("Bronze Ratio"));
  App.Tap(c => c.Marked("Done"));
}

protected override void SetAppStateForScreenshot4() {
  App.Tap(c => c.Marked("ratioPicker"));
  App.Tap(v => v.Text("Yamato Ratio"));
  App.Tap(c => c.Marked("Done"));
  App.Tap(c => c.Marked("rotateButton"));
}

protected override void SetAppStateForScreenshot5() {
  App.Tap(c => c.Marked("Cog.png"));
}
```

- Add the `TakeScreenshots()` method of your class to [Program.cs](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot.Example.Usage/Program.cs#L7) and run your console app to take the screenshots.

```cs
public static void Main(string[] args) {
  var screenshots = new GoldenRatioScreenshots();
  screenshots.TakeScreenshots();

  Environment.Exit(0);
}
```

My screenshots look like this after this example app has run:

<img src="https://hjerpbakk.com/img/Xnapshot/2example_screenshots_small.png" alt="example_screenshots_small" width="651.0" height="239.5">

And the screenshots folder contains screenshots for all configured devices:

<img src="https://hjerpbakk.com/img/Xnapshot/0example_screenshots_folder.png" alt="example_screenshots_folder" width="676.0" height="456.0">

## Advanced Options

The abstract [Screenshots](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot/Screenshots.cs) class has a couple of advanced options that can be optionally set in your `[AppName]Screenshots` constructor:

```cs
public class ExampleScreenshots : Screenshots {
  public ExampleScreenshots() : base(/*removed for brevity*/) {
    SaveScreenshots = true;
    OptimizeImagesAfterSave = false;
  }
}
```

### [SaveScreenshots](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot.Example.Usage/ExampleScreenshots.cs#L18)

Set to `true` as default. Set this to `false` if you want to do a dry run, testing your `SetAppStateForScreenshotX` methods without actually taking screenshots. The methods will be run in the same order, the only difference being that nothing is saved.

### [OptimizeImagesAfterSave](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot.Example.Usage/ExampleScreenshots.cs#L21)

Set to `false` as default. Set this to `true` if you want to run   [ImageOptim](https://imageoptim.com) on every screenshot after save. ImageOptim must be installed in your Applications folder and will losslessly decrease the file size of the screenshots.

## From 1.x to 2.0

Version 2.0 contains breaking changes. Here's how you update your code from 1.x to use the new version.

- Update the Xnapshot Nuget-package to 2.0.
- Change your `[AppName]Screenshots` constructor from:

```cs
public class GoldenRatioScreenshots : Screenshots {
  public GoldenRatioScreenshots() : base(
    DeviceType.iPhone,
    "iOS-9-2",
    "/Users/sankra/Projects/GoldenRatioCalculator/screenshots/en-US",
    "/Users/sankra/Projects/GoldenRatioCalculator/iOS/bin/iPhoneSimulator/Debug/GoldenRatioCalculatoriOS.app") {
  }
}
```

to

```cs
public class GoldenRatioScreenshots : Screenshots {
  public GoldenRatioScreenshots() : base(
    "iOS-12-2",
    "/Users/sankra/Projects/GoldenRatioCalculator/screenshots/en-US",
    "/Users/sankra/Projects/GoldenRatioCalculator/iOS/bin/iPhoneSimulator/Debug/GoldenRatioCalculatoriOS.app",
    new[] {
      "iPhone-XS-Max",
      "iPhone-XS",
      "iPhone-XR",
      "iPhone-8-Plus",
      "iPhone-8",
      "iPhone-SE"
    }) {
  }
}
```

The rest of the code should work as is.
