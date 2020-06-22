using Prism.Commands;

namespace CastleNotifyPropertyChangedMixinSample.ViewModels
{
    public interface IMainWindowViewModel
    {
        string Title { get; set; }
        DelegateCommand UpdateTitleCommand { get; }
    }

    public class MainWindowViewModel : IMainWindowViewModel
    {
        private DelegateCommand updateTitleCommand;

        public MainWindowViewModel()
        {
            Title = "Main Window";
        }

        public virtual string Title { get; set; }

        public DelegateCommand UpdateTitleCommand
        {
            get
            {
                if (updateTitleCommand == null)
                    updateTitleCommand = new DelegateCommand(ExecuteUpdateTitleCommand);
                return updateTitleCommand;
            }
        }

        private void ExecuteUpdateTitleCommand()
        {
            Title += "!";
        }
    }
}
