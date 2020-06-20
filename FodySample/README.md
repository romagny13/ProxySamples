
# Fody Sample

## Install

```
Install-Package Fody
Install-Package Mimick.Fody
```
Github:

* [Fody](https://github.com/Fody/Fody), 
* [Mimick.Fody](https://github.com/Epoque/Mimick.Fody) [wiki](https://github.com/Epoque/Mimick.Fody/wiki)


## Create attributes and implement interfaces

Interfaces:

* IPropertyGetInterceptor
* IPropertySetInterceptor
* IMethodInterceptor
* IParameterInterceptor

+ IINstanceAware, IMemberAware, IRequireInitialization, IMethodReturnInterceptor
+ CompilationImplements, CompilationOptions
+ Scheduled, Value, Autowire, provide, Component, Configuration

[Article](https://www.codeproject.com/Articles/1273911/Mimick-A-Fody-Aspect-Oriented-Weaving-Framework)
[dependency Injection](https://github.com/Epoque/Mimick.Fody/wiki/Components)

Sample

```cs
[AttributeUsage(AttributeTargets.Parameter)]
public class MyParameterInterceporAttribute : Attribute, IParameterInterceptor
{
    public void OnEnter(ParameterInterceptionArgs e)
    {
        if ((int)e.Value > 10)
            Console.WriteLine($"Invalid parameter {e.Parameter.Name}");
    }
}
```

## Decorate members

```cs
public class MyClass
{
    private string myProperty;
    [LogInterceptor]
    public string MyProperty
    {
        get { return myProperty; }
        set { myProperty = value; }
    }

    [LogInterceptor]
    public void MyMethod([MyParameterIntercepor] int p1)
    {
        Console.WriteLine("In MyMethod");
    }
}
```

Output

```
Get Property MyProperty
Exit Property MyProperty
Initial MyProperty:'Initial value'
Set Property MyProperty
Exit Property MyProperty
Get Property MyProperty
Exit Property MyProperty
MyProperty:'New Value'
Invalid parameter p1
Enter Method MyMethod
In MyMethod
Exit Method MyMethod with result
```

## Code generated

for property

```cs
private string myProperty;

[LogInterceptor]
public string MyProperty
{
    get
    {
        PropertyInterceptionArgs propertyInterceptionArg = null;
        string value = null;
        try
        {
            propertyInterceptionArg = new PropertyInterceptionArgs(this, <FodySample_MyClass>k__definition.<MyProperty>k__PropertyInfo, value);
            ((IPropertyGetInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnGet(propertyInterceptionArg);
            value = this.myProperty;
        }
        finally
        {
            ((IPropertyGetInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnExit(propertyInterceptionArg);
        }
        if (propertyInterceptionArg.IsDirty)
        {
            this.MyProperty = (string)propertyInterceptionArg.Value;
            value = (string)propertyInterceptionArg.Value;
        }
        return value;
    }
    set
    {
        PropertyInterceptionArgs propertyInterceptionArg = null;
        try
        {
            propertyInterceptionArg = new PropertyInterceptionArgs(this, <FodySample_MyClass>k__definition.<MyProperty>k__PropertyInfo, value);
            ((IPropertySetInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnSet(propertyInterceptionArg);
            if (propertyInterceptionArg.IsDirty)
            {
                value = (string)propertyInterceptionArg.Value;
            }
            this.myProperty = value;
        }
        finally
        {
            ((IPropertySetInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnExit(propertyInterceptionArg);
        }
    }
}
```

Method

```cs
public void MyMethod()
{
    object[] objArray = new object[0];
    MethodInterceptionArgs methodEventArgs = new MethodInterceptionArgs(this, objArray, null, <FodySample_MyClass>k__definition.<MyMethod$7E334EDE1>k__MethodInfo);
    try
    {
        try
        {
            ((IMethodInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnEnter(methodEventArgs);
            if (!methodEventArgs.Cancel)
            {
                Console.WriteLine("In MyMethod");
            }
        }
        catch (Exception exception)
        {
            ((IMethodInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnException(methodEventArgs, exception);
        }
    }
    finally
    {
        ((IMethodInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnExit(methodEventArgs);
    }
}
```

With parameter interceptor

```cs
public void MyMethod(int p1)
{
    object[] objArray = new object[] { p1 };
    MethodInterceptionArgs methodEventArgs = new MethodInterceptionArgs(this, objArray, null, <FodySample_MyClass>k__definition.<MyMethod$1B4FC0F5B2B>k__MethodInfo);
    try
    {
        try
        {
            ParameterInterceptionArgs parameterInterceptionArg = new ParameterInterceptionArgs(this, <FodySample_MyClass>k__definition.<MyMethod$1B4FC0F5B2B$0>k__ParameterInfo, (object)p1);
            ((IParameterInterceptor)SingletonContainer.<MyParameterInterceporAttribute>k__Attribute).OnEnter(parameterInterceptionArg);
            ((IMethodInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnEnter(methodEventArgs);
            if (!methodEventArgs.Cancel)
            {
                Console.WriteLine("In MyMethod");
            }
        }
        catch (Exception exception)
        {
            ((IMethodInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnException(methodEventArgs, exception);
        }
    }
    finally
    {
        ((IMethodInterceptor)SingletonContainer.<LogInterceptorAttribute>k__Attribute).OnExit(methodEventArgs);
    }
}
```