using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using EncodingNormalizerVsx.ViewModel;

namespace EncodingNormalizerVsx.View
{
    /// <summary>
    ///     ConformPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConformPage : UserControl
    {
        public EventHandler Closing;

        public ConformPage()
        {
            ViewModel = new ConformModel();
            InitializeComponent();
            ViewModel.Closing += (s, e) => Closing?.Invoke(s, e);
            ViewModel.InspectCompleted += InspectCompleted;
            ProgressVisbilityGrid.Visibility = Visibility.Visible;
            VisibilityGrid.Visibility = Visibility.Collapsed;
        }

        private void InspectCompleted(object sender, EventArgs s)
        {
            Dispatcher.Invoke(() =>
            {
                ProgressGrid.BeginStoryboard(ProgressGrid.Resources["ProgressGridStory"] as Storyboard);
            });
        }

        /// <summary>
        ///     项目文件夹
        /// </summary>
        public string SolutionFolder { set; get; }

        public List<string> Project { set; get; }

        public ConformModel ViewModel { set; get; }

        public void InspectFolderEncoding()
        {
            if (Project.Count > 0)
                ViewModel.InspectFolderEncoding(Project);
            else
                ViewModel.InspectFolderEncoding(SolutionFolder);
        }

        private void WriteCriterionButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.WriteCriterionEncoding();
        }

        private void ProgressGrid_OnCompleted(object sender, EventArgs e)
        {
            ProgressVisbilityGrid.Visibility = Visibility.Collapsed;
            VisibilityGrid.Visibility = Visibility.Visible;
        }
    }
}