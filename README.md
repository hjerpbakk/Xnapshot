# Xnapshot
Xnapshot - automating screenshots of your iOS app on every device using Xamarin.UITest

## tl;dr

- Create an [awesome iOS app](https://itunes.apple.com/no/app/id953899091?at=11l5UV&ct=website) using C# and Xamarin. 
- Add a new ‘Console’-project to your solution and add the `Xnapshot` and [Xamarin.UITest](https://www.nuget.org/packages/Xamarin.UITest/) nuget packages. 
- Create a new class, `AppNameScreenshots` and derive from the [Xnapshot.Screenshots](https://github.com/Sankra/Xnapshot/blob/master/Xnapshot/Screenshots.cs) abstract class.
- Add your preferred device type, iOS version, screenshots folder and path to your App bundle as constructor arguments. 

<script src="https://gist.github.com/Sankra/0f72b26eba5ceb4be57c.js"></script>

- Use Xamarin.UITest to implement the `SetAppStateForScreenshotX` methods. This should automate your app, putting it in the correct state for each screenshot. 

<script src="https://gist.github.com/Sankra/0e3c6589a439d9ce541a.js"></script>

- Call the `AppNameScreenshots.TakeScreenshots()` method and run your console app to take the screenshots.

```
public static void Main(string[] args) {
   var screenshots = new GoldenRatioScreenshots();
   screenshots.TakeScreenshots();

   Environment.Exit(0);
}
```

The screenshots look like this after the console app has run:



And the screenshots folder contains screenshots for all configured devices:



## Usage

## Advanced Options