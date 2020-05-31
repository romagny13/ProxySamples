using System;
using System.Collections.Generic;
using Unity;
using Unity.Interception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.InterceptionBehaviors;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.Interceptors.InstanceInterceptors.TransparentProxyInterception;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.PolicyInjection;
using Unity.Interception.PolicyInjection.MatchingRules;
using Unity.Interception.PolicyInjection.Pipeline;

namespace UnityInterceptionSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Methods public / Not Static + Interfaces or virtual or MarshalByRefObject

            IUnityContainer container = new UnityContainer();

            // extension
            container.AddNewExtension<Interception>();

            // define a custom policy (optional)
            container.Configure<Interception>()
              .AddPolicy("MyUnityInterceptionPolicy")
              .AddMatchingRule(new CustomAttributeMatchingRule(typeof(InterceptionIncludeAttribute), true))
              .AddCallHandler(new MyInterceptionCallHandler());

            // Sample 1 - With interface
            Console.WriteLine("-------------- Interfaces - InterfaceInterceptor --------------");
            // interceptor
            container.RegisterType<IMyService, MyService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<MyInterceptor>());
            // Or with InterceptionCallHandler
            // container.RegisterType<IMyService, MyService>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            // Or 
            //container.RegisterType<IMyService, MyService>(new Interceptor<InterfaceInterceptor>(), 
            //    new InterceptionBehavior<MyInterceptor>(), 
            //    new InterceptionBehavior<PolicyInjectionBehavior>());
          
            var myServiceProxy = container.Resolve<IMyService>(); // {DynamicModule.ns.Wrapped_IMyClass_816ff84e4acf4048b2efcfa233b3b480}
            var result = myServiceProxy.MethodA("World");
            myServiceProxy.MethodB();

            // Sample 2 - With virtual Methods  
            Console.WriteLine("-------------- virtual Methods - VirtualMethodInterceptor> --------------");
            // container.RegisterType<MyClass>(new Interceptor<VirtualMethodInterceptor>(), new InterceptionBehavior<MyInterceptor>());
            // Or with InterceptionCallHandler
            container.RegisterType<MyClass>(new Interceptor<VirtualMethodInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            var myClassProxy = container.Resolve<MyClass>(); // {DynamicModule.ns.Wrapped_MyClass_cad8926869a14814aa6ae0cc904f2d25}
            myClassProxy.MethodA();
            myClassProxy.MethodB();

            // Sample 3 - With MarshalByReflObject        
            Console.WriteLine("-------------- MarshalByRefObject - TransparentProxyInterceptor and RealProxy --------------");
            container.RegisterType<MyClassMarshalByRefObject>(new Interceptor<TransparentProxyInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            var myClassMarshalByRefObjectProxy = container.Resolve<MyClassMarshalByRefObject>();
            myClassMarshalByRefObjectProxy.MethodA();
            myClassMarshalByRefObjectProxy.MethodB();

            Console.ReadKey();
        }
    }
    
    public interface IMyService
    {
        [InterceptionInclude]
        string MethodA(string name);

        void MethodB();
    }

    public class MyService : IMyService
    {
        public string MethodA(string name)
        {
            Console.WriteLine("In MyService MethodA");

            return $"Hello {name}!";
        }

        public void MethodB()
        {
            Console.WriteLine("In MyService MethodB");
        }
    }

    public class MyClass
    {
        [InterceptionInclude]
        public virtual void MethodA()
        {
            Console.WriteLine("In MyClass MethodA");
        }

        public virtual void MethodB()
        {
            Console.WriteLine("In MyClass MethodB");
        }
    }

    public class MyClassMarshalByRefObject: MarshalByRefObject
    {
        [InterceptionInclude]
        public void MethodA()
        {
            Console.WriteLine("In MyClassMarshalByRefObject MethodA");
        }

        public void MethodB()
        {
            Console.WriteLine("In MyClassMarshalByRefObject MethodB");
        }
    }


    public class MyInterceptionCallHandler : ICallHandler
    {
        public int Order { get; set; }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            Console.WriteLine($"Interception Before by {nameof(MyInterceptionCallHandler)}");

            IMethodReturn methodReturn = getNext()(input, getNext);

            Console.WriteLine($"Interception After by {nameof(MyInterceptionCallHandler)}");

            return methodReturn;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class InterceptionIncludeAttribute : Attribute
    {
    }

    public class MyInterceptor : IInterceptionBehavior
    {
        public bool WillExecute
        {
            get { return true; }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            string arguments = input.Arguments.Count > 0 ? input.Arguments[0].ToString() : "No Arguments";
            Console.WriteLine(value: $"Interception Before by {nameof(MyInterceptor)} '{input.MethodBase}','{arguments}'");

            var result = getNext()(input, getNext);

            Console.WriteLine($"Interception After  by {nameof(MyInterceptor)} '{input.MethodBase}', return value: '{result.ReturnValue}'");

            return result;
        }
    }
}
