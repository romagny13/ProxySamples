using System;
using System.Reflection;

namespace FromScratch
{
    class Program
    {
        static void Main(string[] args)
        {
            var target = new MyClass();
            var proxy = new MyClass_Proxy(target, new IInterceptor[] { new MyInterceptor() });

            int result;
            proxy.TryParse("10", out result);
            Console.WriteLine($"Out {result}");

            Console.ReadKey();
        }
    }

    public class MyClass
    {
        public virtual bool TryParse(string value, out int result)
        {
            Console.WriteLine($"MyClass TryParse {value}");
            return int.TryParse(value, out result);
        }
    }

    // Class Proxy

    public class MyClass_Proxy : MyClass
    {
        private readonly IInterceptor[] interceptors;
        private readonly MyClass target;

        public MyClass_Proxy(MyClass target, IInterceptor[] interceptors)
        {
            this.target = target;
            this.interceptors = interceptors;
        }

        public override bool TryParse(string value, out int result)
        {
            MethodInfo method = (MethodInfo)MethodBase.GetCurrentMethod(); 

            result = -1;
            var invocation = new MyClass_TryParse_Invocation(interceptors, method, target, new object[] { value, result });
            invocation.Proceed();

            result = (int)invocation.Parameters[1];

            return (bool)invocation.ReturnValue;
        }
    }

    public class MyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Before MyInterceptor 1 | {invocation.Method.Name}");

            invocation.Proceed();

            Console.WriteLine($"After MyInterceptor 1 | {invocation.Method.Name} {invocation.ReturnValue}");
        }
    }

    // 

    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);
    }

    // Invocations (1 per method)

    public class MyClass_TryParse_Invocation : Invocation
    {
        public MyClass_TryParse_Invocation(IInterceptor[] interceptors, MethodInfo method, MyClass target, object[] parameters)
            : base(interceptors, method, target, parameters)
        {
        }

        protected override void InvokeMethodOnTarget()
        {
            // call method on target
            int result;
            ReturnValue = Target.TryParse((string)Parameters[0], out result);
            Parameters[1] = result;
        }
    }

    public interface IInvocation
    {
        MethodInfo Method { get; }
        object[] Parameters { get; }
        object ReturnValue { get; set; }
        MyClass Target { get; }

        void Proceed();
    }

    public abstract class Invocation : IInvocation
    {
        private readonly IInterceptor[] interceptors;
        private int currentInterceptorIndex;

        public Invocation(IInterceptor[] interceptors, MethodInfo method, MyClass target, object[] parameters)
        {
            this.interceptors = interceptors;
            Method = method;
            Target = target;
            Parameters = parameters;
            currentInterceptorIndex = -1;
        }

        public MethodInfo Method { get; }
        public MyClass Target { get; }
        public object[] Parameters { get; }
        public object ReturnValue { get; set; }

        public void Proceed()
        {
            currentInterceptorIndex++;
            try
            {
                if (currentInterceptorIndex == interceptors.Length)
                    InvokeMethodOnTarget();
                else if (currentInterceptorIndex < interceptors.Length)
                    interceptors[currentInterceptorIndex].Intercept(this);
                else
                    throw new IndexOutOfRangeException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                currentInterceptorIndex--;
            }
        }

        protected abstract void InvokeMethodOnTarget();
    }

    public static class MethodBaseMethods
    {
        public static readonly MethodInfo GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) });
    }
}
