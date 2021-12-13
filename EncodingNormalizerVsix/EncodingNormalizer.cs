//------------------------------------------------------------------------------
// <copyright file="EncodingNormalizer.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using EncodingNormalizerVsx.View;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio.Shell;

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Window = System.Windows.Window;

namespace EncodingNormalizerVsx
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class EncodingNormalizer
    {
        /// <summary>
        ///     Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("0640f5ce-e6bc-43ba-b45e-497d70819a20");

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private AsyncPackage AsyncPackage { get; }

        private Window _conformWindow;
        private Window _definitionWindow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncodingNormalizer" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private EncodingNormalizer(AsyncPackage package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            AsyncPackage = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                CommandID menuCommandID = new CommandID(CommandSet, CommandId);
                MenuCommand menuItem = new MenuCommand(EncodingNormalizerCallback, menuCommandID);
                commandService.AddCommand(menuItem);

                CommandID convertCurrentFileSaveEncodingCommand = new CommandID(CommandSet, 0x0103);
                MenuCommand convertCurrentEncodingMenuCommand = new MenuCommand(ConvertCurrentFileEncoding, convertCurrentFileSaveEncodingCommand);
                commandService.AddCommand(convertCurrentEncodingMenuCommand);

                menuCommandID = new CommandID(CommandSet, 0x0101);
                menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);

                //menuCommandID = new CommandID(CommandSet, 0x0102);
                //menuItem = new MenuCommand(SaveEncoding, menuCommandID);
                //commandService.AddCommand(menuItem);
            }
        }

#pragma warning disable VSTHRD100 // 避免使用 Async Void 方法
        private async void ConvertCurrentFileEncoding(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // 避免使用 Async Void 方法
        {
            await AsyncPackage.JoinableTaskFactory.SwitchToMainThreadAsync();
            // 修改用户打开的文件的编码
            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
            Document document = dte.ActiveDocument;
            string str = document.FullName;

            new ConvertFileEncodingPage(new FileInfo(str)).Show();
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static EncodingNormalizer Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => AsyncPackage;

#pragma warning disable VSTHRD100 // 避免使用 Async Void 方法
        private async void EncodingNormalizerCallback(object sender, EventArgs e)
#pragma warning restore VSTHRD100 // 避免使用 Async Void 方法
        {
            await AsyncPackage.JoinableTaskFactory.SwitchToMainThreadAsync();
            
            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
            string file = dte.Solution.FullName;
            List<string> project = new List<string>();

            if (dte.Solution.Projects.Count > 0)
            {
                //try
                //{
                int noLoadProjectCount = await TryParseProjectAsync(dte, project);
                //}
                // catch (NotImplementedException)
                //{
                //    MessageBox.Show("The project not loaded.", "项目还没有加载完成");
                //    return;
                //}
                if (noLoadProjectCount > 0)
                {
                    if (project.Count == 0)
                    {
                        MessageBox.Show("All project not loaded.", "全部项目都没有加载完成");
                        return;
                    }
                    MessageBox.Show("存在" + noLoadProjectCount + "个工程没有加载");
                }
                else
                {
                    if (project.Count == 0)
                    {
                        MessageBox.Show("Cant find any project.", "没有发现工程");
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Cant find the solution.", "少年，听说你没有打开工程");
                return;
            }

            ConformWindow(file, project);
        }

        private async Task<int> TryParseProjectAsync(DTE dte, List<string> project)
        {
            await AsyncPackage.JoinableTaskFactory.SwitchToMainThreadAsync();

            int noLoadProjectCount = 0;

            foreach (object temp in dte.Solution.Projects)
            {
                try
                {
                    if (temp is Project tempProject)
                    {
                        if ((tempProject).Kind == ProjectKinds.vsProjectKindSolutionFolder)
                        {
                            foreach(var subProject in await GetSolutionFolderProjectsAsync(tempProject))
                            {
                                var directory = await ParseProjectFolderAsync(subProject);
                                if (!string.IsNullOrEmpty(directory))
                                {
                                    project.Add(directory);
                                }
                            }
                        }
                        else
                        {
                            var directory = await ParseProjectFolderAsync(tempProject);

                            project.Add(directory);
                        }
                    }
                }
                catch (NotImplementedException)
                {
                    noLoadProjectCount++;
                }
            }
            return noLoadProjectCount;
        }

        private async Task<string> ParseProjectFolderAsync(Project project)
        {
            await AsyncPackage.JoinableTaskFactory.SwitchToMainThreadAsync();
            string file = project.FullName;
            if (!string.IsNullOrEmpty(file))
            {
                return new FileInfo(file).Directory?.FullName;
            }
            return "";
        }

        private async Task<List<Project>> GetSolutionFolderProjectsAsync(Project solutionFolder)
        {
            await AsyncPackage.JoinableTaskFactory.SwitchToMainThreadAsync();

            List<Project> project = new List<Project>();
            for (int i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                Project subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                {
                    continue;
                }

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    project.AddRange(await GetSolutionFolderProjectsAsync(subProject));
                }
                else
                {
                    project.Add(subProject);
                }
            }
            return project;
        }

        private void ConformWindow(string file, List<string> project)
        {
            if (_conformWindow != null)
            {
                _conformWindow.Focus();
                _conformWindow.Show();
                return;
            }

            string folder = "";
            if (!string.IsNullOrEmpty(file))
            {
                folder = new FileInfo(file).Directory?.FullName;
            }
            Window window = new Window()
            {
                Width = 500,
                Height = 500
            };
            ConformPage conformPage = new ConformPage();
            window.Content = conformPage;
            window.Title = "编码规范工具";
            conformPage.Closing += (_s, _e) =>
            {
                window.Close();
                _conformWindow = null;
            };
            window.Closed += (_s, _e) => { _conformWindow = null; };
            conformPage.SolutionFolder = folder;
            conformPage.Project = project;
            window.Show();
            conformPage.InspectFolderEncoding();
            _conformWindow = window;
        }

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(AsyncPackage package)
        {
            Instance = new EncodingNormalizer(package);
        }
        

        /// <summary>
        ///     This function is the callback used to execute the command when the menu item is clicked.
        ///     See the constructor to see how the menu item is associated with this function using
        ///     OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            if (_definitionWindow != null)
            {
                _definitionWindow.Focus();
                _definitionWindow.Show();
                return;
            }
            Window window = new Window();
            DefinitionPage definitionPage = new DefinitionPage();
            definitionPage.Closing += (_s, _e) =>
            {
                window.Close();
                _definitionWindow = null;
            };
            window.Closed += (_s, _e) => { _definitionWindow = null; };
            window.Title = "编码规范工具设置";
            window.Content = definitionPage;
            window.Show();

            _definitionWindow = window;
        }
    }
}