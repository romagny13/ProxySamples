using Castle.DynamicProxy;
using CastleNotifyPropertyChangedMixinSample.ViewModels;
using System;
using System.ComponentModel;

namespace CastleNotifyPropertyChangedMixinSample
{
    public class ViewModelLocator
    {
        private ProxyGenerator generator;
        private MainWindowViewModel mainWindowViewModel;

        public ViewModelLocator()
        {
            generator = new ProxyGenerator(new PersistentProxyBuilder());

            Initialize();

            generator.ProxyBuilder.ModuleScope.SaveAssembly();
        }

        private void Initialize()
        {
            // MainWindowViewModel
            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(new PropertyChangedNotifier());
            mainWindowViewModel = generator.CreateClassProxyWithTarget<MainWindowViewModel>(new MainWindowViewModel(), options, new PropertyChangedInterceptor());
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get { return mainWindowViewModel; }
        }
    }

    public interface IPropertyChangedNotifier : INotifyPropertyChanged
    {
        void OnPropertyChanged(object target, string propertyName);
    }

    [Serializable]
    public class PropertyChangedNotifier : IPropertyChangedNotifier
    {
        public void OnPropertyChanged(object target, string propertyName)
        {
            PropertyChanged?.Invoke(target, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class PropertyChangedInterceptor : IInterceptor
    {
        private const string SetConstant = "set_";

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            if (invocation.Method.Name.StartsWith(SetConstant))
            {
                var notifier = invocation.Proxy as IPropertyChangedNotifier;
                if (notifier != null)
                {
                    string propertyName = invocation.Method.Name.Substring(SetConstant.Length);
                    notifier.OnPropertyChanged(invocation.Proxy, propertyName);
                }
            }
        }
    }
}
