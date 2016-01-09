# Xnapshot
Xnapshot - automated screenshots of your iOS app on every device using Xamarin.UITest. 

## tl;dr

- Create an [awesome iOS app](https://itunes.apple.com/no/app/id953899091?at=11l5UV&ct=website) using C# and Xamarin. 
- Add a new `Console project` to your solution and add the `Xnapshot` and [Xamarin.UITest](https://www.nuget.org/packages/Xamarin.UITest/) nuget packages. 
- Create a new class, `AppNameScreenshots` and derive from the [Xnapshot.Screenshots](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot/Screenshots.cs) abstract class.
- Add your preferred device type, iOS version, screenshots folder and path to your App bundle as constructor arguments. See [Usage](#usage) below for allowed values.

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

- It’s now time to implement the `SetAppStateForScreenshotX` methods. Use `Xamarin.UITest` to automate your app, putting it in the correct state before each screenshot. The examples below are from my Golden Ratio Calculator app. `SetAppStateForScreenshot1` is empty because the first screenshot is of the first screen.

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

- Call the `TakeScreenshots()` method of your class and run your console app to take the screenshots.

```cs
public static void Main(string[] args) {
  var screenshots = new GoldenRatioScreenshots();
  screenshots.TakeScreenshots();

  Environment.Exit(0);
}
```

The screenshots look like this after this example app has run:

<h3></h3>
<img src="http://hjerpbakk.com/s/example_screenshots_small.png" alt="example_screenshots_small" width="651.0" height="239.5">

And the screenshots folder contains screenshots for all configured devices:

<h3></h3>
<img src="http://hjerpbakk.com/s/example_screenshots_folder.png" alt="example_screenshots_folder" width="676.0" height="456.0">

## Usage

## Advanced Options

[Xnapshot.Screenshots](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot/Screenshots.cs) has a couple of advanced options that can be set in your `AppNameScreenshots` constructor.

### [OptimizeImagesAfterSave](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot/Screenshots.cs#L84)

Set to `false` as default. Set this to `true` if you want to run   [ImageOptim](https://imageoptim.com) on every screenshot after save. ImageOptim must be installed in your Applications folder and will losslessly decrease the file size of the screenshots. 

### SaveScreenshots

Set to `true` as default. Set this to `false` if you want to do a dry run, testing your `SetAppStateForScreenshotX` methods without actually taking screenshots. The methods will be run in the same order, the only difference being that nothing is saved.