using Mimick;
using Mimick.Aspect;
using System;

namespace FodySample
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest();

            Console.ReadKey();
        }

        private static void RunTest()
        {
            var instance = new MyClass();

            Console.WriteLine($"Initial MyProperty:'{instance.MyProperty}'");

            instance.MyProperty = "New Value";
            Console.WriteLine($"MyProperty:'{instance.MyProperty}'");

            instance.MyMethod(20);
        }
    }

    public class MyClass
    {
        [Value("Initial value")]
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

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class LogInterceptorAttribute : Attribute,
        IMethodInterceptor,
        IPropertyGetInterceptor,
        IPropertySetInterceptor
    {
        public void OnEnter(MethodInterceptionArgs e)
        {
            Console.WriteLine($"Enter Method {e.Method.Name}");
        }

        public void OnException(MethodInterceptionArgs e, Exception ex)
        {
            Console.WriteLine($"Exception Method {e.Method.Name}, {ex.Message}");
        }

        public void OnException(PropertyInterceptionArgs e, Exception ex)
        {
            throw new NotImplementedException();
        }

        public void OnExit(MethodInterceptionArgs e)
        {
            Console.WriteLine($"Exit Method {e.Method.Name} with result {e.Return}");
        }

        public void OnExit(PropertyInterceptionArgs e)
        {
            Console.WriteLine($"Exit Property {e.Property.Name}");
        }

        public void OnGet(PropertyInterceptionArgs e)
        {
            Console.WriteLine($"Get Property {e.Property.Name}");
        }

        public void OnSet(PropertyInterceptionArgs e)
        {
            Console.WriteLine($"Set Property {e.Property.Name}");
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class MyParameterInterceporAttribute : Attribute, IParameterInterceptor
    {
        public void OnEnter(ParameterInterceptionArgs e)
        {
            if ((int)e.Value > 10)
                Console.WriteLine($"Invalid parameter {e.Parameter.Name}");
        }
    }
}
