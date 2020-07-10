using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CastleInterceptorSelectorSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new ProxyGenerator(new PersistentProxyBuilder());
            var options = new ProxyGenerationOptions(new MyProxyGenerationHook());
            options.Selector = new MyInterceptorSelector(); // the selector is injected in MyClassProxy and used each method to filter interceptors

            var proxy = generator.CreateClassProxy<MyClass>(options, new GetMethodInterceptor(), new SetMethodInterceptor(), new MethodInterceptor());
            generator.ProxyBuilder.ModuleScope.SaveAssembly();

            proxy.MyProperty = "New Value";

            Console.WriteLine($"Get: {proxy.MyProperty}");

            proxy.MyMethod();
            proxy.MyMethod2(); // not intercepted

            Console.ReadKey();
        }
    }

    public class GetMethodInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"[GetMethodInterceptor] {invocation.Method.Name}");
            invocation.Proceed();
        }
    }

    public class SetMethodInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"[SetMethodInterceptor] {invocation.Method.Name}");
            invocation.Proceed();
        }
    }

    public class MethodInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"[MethodInterceptor] {invocation.Method.Name}");
            invocation.Proceed();
        }
    }

    public class MyClass
    {
        public virtual string MyProperty { get; set; }

        public virtual void MyMethod()
        {
            Console.WriteLine("In MyMethod");
        }

        public virtual void MyMethod2()
        {
            Console.WriteLine("In MyMethod2");
        }

        public void MyMethodNotVirtual()
        {
            Console.WriteLine("In MyMethodNotVirtual");
        }
    }

    [Serializable]
    public class MyProxyGenerationHook : IProxyGenerationHook
    {

        private List<MemberInfo> nonProxyableMembers = new List<MemberInfo>();

        public void MethodsInspected()
        {
           
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
            // MyMethodNotVirtual
            nonProxyableMembers.Add(memberInfo);
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            if (type == typeof(MyClass) && methodInfo.Name == "MyMethod2")
                return false;

            return true;
        }
    }

    [Serializable]
    public class MyInterceptorSelector : IInterceptorSelector
    {
        private static readonly IInterceptor[] emptyInterceptorArray = new IInterceptor[0];

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            if (IsGetMethod(method))
            {
                var getMethodInterceptor = interceptors.First(p => p.GetType() == typeof(GetMethodInterceptor));
                return new IInterceptor[] { getMethodInterceptor };
            }
            else if (IsSetMethod(method))
            {
                var setMethodInterceptor = interceptors.First(p => p.GetType() == typeof(SetMethodInterceptor));
                return new IInterceptor[] { setMethodInterceptor };
            }
            else if (IsAddmethod(method)) { }
            else if (IsRemoveMethod(method)) { }
            else
            {
                var methodInterceptor = interceptors.First(p => p.GetType() == typeof(MethodInterceptor));
                return new IInterceptor[] { methodInterceptor };
            }
            return emptyInterceptorArray;
        }

        private bool IsGetMethod(MethodInfo method)
        {
            return method.Name.StartsWith("get_");
        }

        private bool IsSetMethod(MethodInfo method)
        {
            return method.Name.StartsWith("set_");
        }

        private bool IsAddmethod(MethodInfo method)
        {
            return method.Name.StartsWith("add_");
        }

        private bool IsRemoveMethod(MethodInfo method)
        {
            return method.Name.StartsWith("remove_");
        }
    }
}
