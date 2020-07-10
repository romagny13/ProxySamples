# ProxySamples
 
 > RealProxy, DispatchProxy, Unity Interception, Castle, Fody and From Scratch Samples


## Overview

### Mono Cecil

Create a build task and update the assembly at compilation. Great for AOP

:arrow_right: **Fody** for example.

### Reflection Emit

Create an assembly and return proxies. 

Require virtual members for class proxies. Cannot call Static method

:arrow_right: **Castle Core (DynamicProxy)**


### DynamicMetaObject / IDynamicMetaObjectProvider

DLR. Keyword dynamic. No InstelliSense

:arrow_right: **KingAOP** for example

### And more

:arrow_right: **RealProxy**, **DispatchProxy**: class inherits from MarshalByRefObject (or use interface).

:arrow_right: **Unity Interception**: use proxies MS and more.

**Reflection**: it's possible

**Code generation** with **T4 Templates** / **CodeDom**

:arrow_right: **PostSharp** (commercial)

**ContextBoundObject**

and more projects (like Spring.Net, etc.)

## Samples

### RealProxy 

[Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.remoting.proxies.realproxy?view=netframework-4.8)

* .NET Framework 4.8 4.7.2 4.7.1 4.7 4.6.2 4.6.1 4.6 4.5.2 4.5.1 4.5 4.0 3.5 3.0 2.0 1.1
* Xamarin.Android 7.1
* Xamarin.iOS 10.8
* Xamarin.Mac 3.0

### DispatchProxy 

[Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy?view=netcore-3.1)

* .NET 5.0 Preview 4
* .NET Core 3.1 3.0 2.2 2.1 2.0 1.1 1.0
* .NET Platform Extensions 2.1
* .NET Standard 2.1
* UWP 10.0
* Xamarin.Android 7.1
* Xamarin.iOS 10.8

### Unity

* [Unity](https://github.com/unitycontainer/container), [Abstractions](https://github.com/unitycontainer/abstractions)
* [Unity.Interception](https://github.com/unitycontainer/interception)


### Fody

* [Fody](https://github.com/Fody/Fody), 
* [Mimick.Fody](https://github.com/Epoque/Mimick.Fody) [wiki](https://github.com/Epoque/Mimick.Fody/wiki)

### Castle

* [Castle Windsor](https://github.com/castleproject/Windsor)
* [Castle Core](https://github.com/castleproject/Core)

