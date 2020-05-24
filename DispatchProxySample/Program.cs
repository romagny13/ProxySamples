using System;
using System.Reflection;

namespace DispatchProxySample
{

    class Program
    {
        static void Main(string[] args)
        {
            var proxy = AopProxy<IMyService>.CreateProxy(new MyService(), BeforeExcute, AfterExecute, ExecutionFailed);

            proxy.Method();

            Console.ReadKey();
        }

        private static void ExecutionFailed(MethodInfo arg1, Exception arg2)
        {
            Console.WriteLine("Execution failed");
        }

        private static void AfterExecute(MethodInfo arg1, object arg2)
        {
            Console.WriteLine("After Execute");
        }

        private static void BeforeExcute(MethodInfo obj)
        {
            Console.WriteLine("Before Execute");
        }
    }

    public interface IMyService
    {
        void Method();
    }

    public class MyService : IMyService
    {
        public void Method()
        {
            Console.WriteLine("In Method");
        }
    }

    //  DispatchProxy https://docs.microsoft.com/fr-fr/dotnet/api/system.reflection.dispatchproxy?view=netcore-3.1
    // .NET 5.0 Preview 4
    // .NET Core 3.1 3.0 2.2 2.1 2.0 1.1 1.0
    // .NET Platform Extensions 2.1
    // .NET Standard 2.1
    // UWP 10.0
    // Xamarin.Android 7.1
    // Xamarin.iOS 10.8
    public class AopProxy<TDecorated> : DispatchProxy where TDecorated : class
    {
        private TDecorated decorated;
        private Action<MethodInfo> beforeExecute;
        private Action<MethodInfo, object> afterExecute;
        private Action<MethodInfo, Exception> executionFailed;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            try
            {
                beforeExecute?.Invoke(targetMethod);

                object result = targetMethod.Invoke(decorated, args);

                afterExecute?.Invoke(targetMethod, result);
                return result;
            }
            catch (Exception ex)
            {
                executionFailed?.Invoke(targetMethod, ex);
                throw ex;
            }
        }

        public static TDecorated CreateProxy(TDecorated decorated,
            Action<MethodInfo> beforeExecute,
            Action<MethodInfo, object> afterExecute,
            Action<MethodInfo, Exception> executionFailed)
        {
            if (decorated is null)
                throw new ArgumentNullException(nameof(decorated));

            object proxy = Create<TDecorated, AopProxy<TDecorated>>();

            ((AopProxy<TDecorated>)proxy).SetParameters(decorated, beforeExecute, afterExecute, executionFailed, proxy);

            return (TDecorated)proxy;
        }

        public static TDecorated CreateProxy(TDecorated decorated)
        {
            return CreateProxy(decorated, null, null, null);
        }

        private void SetParameters(TDecorated decorated, Action<MethodInfo> beforeExecute, Action<MethodInfo, object> afterExecute, Action<MethodInfo, Exception> executionFailed, object proxy)
        {
            this.decorated = decorated;
            this.beforeExecute = beforeExecute;
            this.afterExecute = afterExecute;
            this.executionFailed = executionFailed;
        }
    }
}
