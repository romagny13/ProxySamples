using Prism.Commands;
using PropertyChanged;
using System.Threading.Tasks;

namespace FodyPropertyChangedSample.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel
    {
        private DelegateCommand updateTitleCommand;

        public MainWindowViewModel()
        {
            Title = "The title";
        }

        public string Title { get; set; }
        public bool IsBusy { get; set; }

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
            IsBusy = true;

            Task.Delay(3000).Await(() =>
            {
                Title += "!";
                IsBusy = false;
            });
        }
    }
}
