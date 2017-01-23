using System;
using System.Windows;
using System.Windows.Controls;
using EncodingNormalizerVsx.ViewModel;

namespace EncodingNormalizerVsx.View
{
    /// <summary>
    ///     DefinitionPage.xaml 的交互逻辑
    /// </summary>
    public partial class DefinitionPage : UserControl
    {
        /// <summary>
        ///     通知窗口关闭
        /// </summary>
        public EventHandler Closing;

        public DefinitionPage()
        {
            ViewModel = new DefinitionModel();
            InitializeComponent();
        }

        public DefinitionModel ViewModel { set; get; }

        private void WriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            //保存数据
            if (ViewModel.WriteAccount())
                Closing?.Invoke(this, null);
        }

        private void AbandonButton_OnClick(object sender, RoutedEventArgs e)
        {
            Closing?.Invoke(this, null);
        }
    }
}