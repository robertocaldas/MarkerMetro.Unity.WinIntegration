MarkerMetro.Unity.WinIntegration
================================

Plugin Libraries to support implementations of Windows 8 or Windows Phone platform specific functionality

Why?
================================
Not all platform specific features are implemented, or implemented well by either Unity or other commercial plugins. 

Where this is the case, we have implemented plugins to help


What?
================================
This plugin helps with: Store Integration, Helper to Get App Version, (More TBC!)

## Store Integration

Add a using statement to include the Store APIs.

```csharp
using MarkerMetro.Unity.WinIntegration.Store;
```

Firstly, ensure you initialise the store manager in App.xaml.cs, specifying whether you want the simulated IAP or not. You can do this alongside the call to initialize the plugin's Dispatcher (see "How?" below).

The simulator will expect IAP to be provided in an xml file on the project root called "iap_simulator.xml". Use the correct formatting for this file and note that it differs between Win 8.1 and WP8. Copying a previous project will be the quickest route here.

```csharp
void StoreManager.Initialise(bool useSimulator)
```

There is a single Store API for both Win 8.1 and WP8:

Determine whether the app has a currently active trial:

```csharp
bool StoreManager.Instance.IsActiveTrial
```

Attempt to purchase the application. The Receipt object returned in the delegate will have a StatusCode of Success or ExceptionThrown if anything went wrong.

```csharp
void StoreManager.Instance.PurchaseApplication(PurchaseDelegate callback)
```

Attempt to list all IAP products available. This will be a list of products or null if anything went wrong.

```csharp
void StoreManager.Instance.RetrieveProducts(ProductListDelegate callback)
```

Attempt to purchase an IAP product. The receipt object returned in the delegate will have a StatusCode of Success or ExceptionThrow if something went badly wrong. 

Specifically for WP8, the only other StatusCode used is NotReady when after a successful purchase the license information does not appear to be valid. Windows 8.1 uses all the other status codes as more information is available.

```csharp
void StoreManager.Instance.PurchaseProduct(PurchaseDelegate callback)
```

## Helper

Add a using statement to include the  APIs.

```csharp
using MarkerMetro.Unity.WinIntegration;
```

The following method will return the manifest version of the app:

```csharp
Helper.Instance.GetAppVersion()
```

The following method will show the Share charm (Win 8.1) or invoke the Share task (WP8)

```csharp
Helper.Instance.ShowShareUI()
```

Get User Device ID equivalent. This uses a roaming Guid on Win 8.1 or the actual device id for WP8

```csharp
Helper.Instance.GetUserDeviceId()
```

Get the Store Uri. Note this only returns a valid value once the app has been published

```csharp
Helper.Instance.GetAppStoreUri()
```

This method returns true if running on Arm Windows 8.1 or a low memory device on Windows Phone 8

```csharp
Helper.Instance.IsLowEndDevice()
```

## IntegrationManager

This singleton offers an easy way to hook MonoBehaviours in a Game Object. This particular Game Object is created during the singleton initialization and it's
not destroyed between scene changes. It also provides means to run on MonoBehaviour-related events (Update, key pressed, etc.) without requiring a MonoBehaviour.

Add a using statement to include the  manager:

```csharp
using MarkerMetro.Unity.WinIntegration;
```

and call this method to initialize the manager:

```csharp
IntegrationManager.Instance.Init()
```

Please refer to the code documentation for more information.

How?
================================
This library is published on the Marker Metro NuGet Feed (https://github.com/MarkerMetro/MarkerMetro.ProcessAutomation/wiki)

Always remember to initialize the plugin's Dispatcher for marshalling threads:

You can use the App.xaml.cs for Windows 8.1 when AppCallbacks has been initialized:
https://github.com/MarkerMetro/SportsJeopardy/blob/windows/WindowsSolution/WindowsStoreApps/Sports%20Jeopardy!/App.xaml.cs

And in Windows Phone 8, you can use Mainpage.xaml.cs in the Unity_Loaded handler:
https://github.wdig.com/MarkerMetro/BlockSumProduction/blob/windows/WindowsSolution/WindowsStoreApps/LostLight/MainPage.xaml.cs

To update and use a NuGet plugin on a project see an example here:
https://github.com/MarkerMetro/SportsJeopardy#marker-metro-nuget-access

Generally speaking once set up, you can push changes, run a new build via the build server

1. Push Changes
2. Run a new build via the http://mmbuild1.cloudapp.net/ build server
3. Run the bat file in your project which will copy new versions into Unity plugins folders
