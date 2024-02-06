using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GaltonBoard.Model.Models;

public class ObjectNotification : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}