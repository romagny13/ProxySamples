using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace RealProxySample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Samples with interfaces
            var proxy1 = RepositoryProxyGenerator.CreateProxy<User>();
            proxy1.Add(new User { Id = 1, UserName = "User1" });

            var proxy2 = MyServiceProxyGenerator.CreateProxy<MyService>();
            proxy2.MyServiceMethod();

            // sample with object that implements MarshalByRefObject
            var proxy3 = ProxyGenerator.CreateProxy<MyClass>();
            proxy3.MyClassMethod();

            Console.ReadKey();
        }
    }

    // 1

    public interface IMyService
    {
        void MyServiceMethod();
    }

    public class MyService : IMyService
    {
        public void MyServiceMethod()
        {
            System.Console.WriteLine("In MyService Method");
        }
    }

    // 2

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }

    public interface IRepository<T>
    {
        void Add(T entity);
    }

    public class Repository<T> : IRepository<T>
    {
        public void Add(T entity)
        {
            Console.WriteLine($"Repository Add entity {entity}");
            // throw new Exception("My exception");
        }
    }

    // 3

    public class MyClass : MarshalByRefObject
    {
        public void MyClassMethod()
        {
            Console.WriteLine("In MyClass Method");
        }
    }

    public class MyServiceProxyGenerator
    {
        public static IMyService CreateProxy<T>() where T : IMyService, new()
        {
            IMyService service = new T();

            var aopProxy = new AopProxy<IMyService>(service);

            aopProxy.BeforeExecute += (s, e) => Console.WriteLine($"Before Execute {e.MethodCallMessage.MethodName}");
            aopProxy.AfterExecute += (s, e) => Console.WriteLine($"After Execute {e.MethodCallMessage.MethodName}");
            aopProxy.ExecutionFailed += (s, e) => Console.WriteLine($"Execution failed {e.MethodCallMessage.MethodName}");

            return aopProxy.GetTransparentProxy() as IMyService;
        }
    }

    public class RepositoryProxyGenerator
    {
        public static IRepository<T> CreateProxy<T>()
        {
            var repository = new Repository<T>();

            var aopProxy = new AopProxy<IRepository<T>>(repository);

            aopProxy.BeforeExecute += (s, e) => Console.WriteLine($"Before Execute {e.MethodCallMessage.MethodName}");
            aopProxy.AfterExecute += (s, e) => Console.WriteLine($"After Execute {e.MethodCallMessage.MethodName}");
            aopProxy.ExecutionFailed += (s, e) => Console.WriteLine($"Execution failed {e.MethodCallMessage.MethodName}");

            return aopProxy.GetTransparentProxy() as IRepository<T>;
        }
    }

    public class ProxyGenerator
    {
        public static T CreateProxy<T>() where T : MarshalByRefObject
        {
            var instance = Activator.CreateInstance<T>();

            var aopProxy = new AopProxy<T>(instance);

            aopProxy.BeforeExecute += (s, e) => Console.WriteLine($"Before Execute {e.MethodCallMessage.MethodName}");
            aopProxy.AfterExecute += (s, e) => Console.WriteLine($"After Execute {e.MethodCallMessage.MethodName}");
            aopProxy.ExecutionFailed += (s, e) => Console.WriteLine($"Execution failed {e.MethodCallMessage.MethodName}");

            return aopProxy.GetTransparentProxy() as T;
        }
    }

    // RealProxy https://docs.microsoft.com/fr-fr/dotnet/api/system.runtime.remoting.proxies.realproxy?view=netframework-4.8
    // .NET Framework 4.8 4.7.2 4.7.1 4.7 4.6.2 4.6.1 4.6 4.5.2 4.5.1 4.5 4.0 3.5 3.0 2.0 1.1
    // Xamarin.Android 7.1
    // Xamarin.iOS 10.8
    // Xamarin.Mac 3.0
    public class AopProxy<T> : RealProxy
    {
        private T instance;

        internal AopProxy(T instance) : base(typeof(T))
        {
            this.instance = instance;
        }

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;
            MethodInfo method = methodCallMessage.MethodBase as MethodInfo;
            try
            {
                OnBeforeExecute(methodCallMessage);

                object result = method.Invoke(instance, methodCallMessage.InArgs);

                OnAfterExecute(methodCallMessage, result);

                return new ReturnMessage(result, null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);
            }
            catch (Exception ex)
            {
                OnExecutionFailed(methodCallMessage, ex);
                return new ReturnMessage(ex, methodCallMessage);
            }
        }

        protected void OnBeforeExecute(IMethodCallMessage methodCallMessage)
        {
            BeforeExecute?.Invoke(this, new ExecutionEventArgs(methodCallMessage));
        }

        protected void OnAfterExecute(IMethodCallMessage methodCallMessage, object result)
        {
            AfterExecute?.Invoke(this, new AfterExecutionEventArgs(methodCallMessage, result));
        }

        protected void OnExecutionFailed(IMethodCallMessage methodCallMessage, Exception ex)
        {
            ExecutionFailed?.Invoke(this, new ExecutionFailedEventArgs(methodCallMessage, ex));
        }

        public event EventHandler<ExecutionEventArgs> BeforeExecute;
        public event EventHandler<AfterExecutionEventArgs> AfterExecute;
        public event EventHandler<ExecutionFailedEventArgs> ExecutionFailed;
    }

    public class ExecutionEventArgs : EventArgs
    {
        private IMethodCallMessage methodCallMessage;

        public ExecutionEventArgs(IMethodCallMessage methodCallMessage)
        {
            this.methodCallMessage = methodCallMessage;
        }

        public IMethodCallMessage MethodCallMessage
        {
            get { return methodCallMessage; }
        }
    }

    public class AfterExecutionEventArgs : ExecutionEventArgs
    {
        private object result;

        public AfterExecutionEventArgs(IMethodCallMessage methodCallMessage, object result)
        : base(methodCallMessage)
        {
            this.result = result;
        }

        public object Result
        {
            get { return result; }
            set { result = value; }
        }
    }

    public class ExecutionFailedEventArgs : ExecutionEventArgs
    {
        private Exception exception;

        public ExecutionFailedEventArgs(IMethodCallMessage methodCallMessage, Exception exception) : base(methodCallMessage)
        {
            this.exception = exception;
        }

        public Exception Exception
        {
            get { return exception; }
        }

    }
}