using Switcheroo.Core;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Switcheroo {
    public class AppWindowViewModel : INotifyPropertyChanged, IWindowText {
        public AppWindowViewModel(AppWindow appWindow) {
            AppWindow = appWindow;
        }

        public AppWindow AppWindow { get; }

        public string WindowTitle => AppWindow.Title;

        public string ProcessTitle => AppWindow.ProcessTitle;

        public IntPtr HWnd => AppWindow.HWnd;

        private string _formattedTitle;

        public string FormattedTitle {
            get => _formattedTitle;
            set {
                _formattedTitle = value;
                NotifyOfPropertyChange(() => FormattedTitle);
            }
        }

        private string _formattedProcessTitle;

        public string FormattedProcessTitle {
            get => _formattedProcessTitle;
            set {
                _formattedProcessTitle = value;
                NotifyOfPropertyChange(() => FormattedProcessTitle);
            }
        }

        private bool _isBeingClosed;

        public bool IsBeingClosed {
            get => _isBeingClosed;
            set {
                _isBeingClosed = value;
                NotifyOfPropertyChange(() => IsBeingClosed);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyOfPropertyChange<T>(Expression<Func<T>> property) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(GetPropertyName(property)));
        }

        private string GetPropertyName<T>(Expression<Func<T>> property) {
            LambdaExpression lambda = property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression unaryExpression)
                memberExpression = (MemberExpression)unaryExpression.Operand;
            else
                memberExpression = (MemberExpression)lambda.Body;

            return memberExpression.Member.Name;
        }
    }
}