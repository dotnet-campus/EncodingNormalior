// lindexi
// 15:58

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace EncodingNormalizerVsx.ViewModel
{
    /// <summary>
    ///     提供继承通知UI改变值
    /// </summary>
    public class NotifyProperty : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateProper<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
        {
            if (Equals(properValue, newValue))
                return;

            properValue = newValue;
            OnPropertyChanged(properName);
        }

        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            var handler = PropertyChanged;
            try
            {
                SynchronizationContext.SetSynchronizationContext(new
                    DispatcherSynchronizationContext(Application.Current.Dispatcher));
                SynchronizationContext.Current.Send(
                    obj => { handler?.Invoke(this, new PropertyChangedEventArgs(name)); }, null);
            }
            catch (Exception)
            {
                handler?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}