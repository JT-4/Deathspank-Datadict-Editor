using System;
using System.Windows.Input;

namespace Datadict_Editor
{
    public class RelayCommand : ICommand
    {
        private Action mAction;
        public event EventHandler CanExecuteChanged;

        //relay commands can always execute
        public bool CanExecute(object parameter)
        {
            return true;
        }

        //set the action to be performed
        public RelayCommand(Action action)
        {
            mAction = action;
        }

        //execute the action
        public void Execute(object parameter)
        {
            mAction();
        }
    }
}
