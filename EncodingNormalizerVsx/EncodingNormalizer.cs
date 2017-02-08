//------------------------------------------------------------------------------
// <copyright file="EncodingNormalizer.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Windows;
using EncodingNormalizerVsx.View;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
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
        private readonly Package package;

        private Window _conformWindow;
        private Window _definitionWindow;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncodingNormalizer" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private EncodingNormalizer(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(EncodingNormalizerCallback, menuCommandID);
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(CommandSet, 0x0101);
                menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static EncodingNormalizer Instance { get; private set; }

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get { return package; }
        }

        private void EncodingNormalizerCallback(object sender, EventArgs e)
        {
            var dte = (DTE)ServiceProvider.GetService(typeof(DTE));
            var file = dte.Solution.FullName;
            var project = new List<string>();

            if (dte.Solution.Projects.Count > 0)
            {
                //try
                //{
                var noLoadProjectCount = TryParseProject(dte, project);
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

        private static int TryParseProject(DTE dte, List<string> project)
        {
            int noLoadProjectCount = 0;
            foreach (var temp in dte.Solution.Projects)
            {
                try
                {
                    //file = ((Project)temp).FileName;
                    //if (file.EndsWith(".csproj"))
                    //{

                    //}
                    var file = ((Project)temp).FullName;

                    if (!string.IsNullOrEmpty(file))
                    {
                        project.Add(new FileInfo(file).Directory?.FullName);
                    }
                }
                catch (NotImplementedException)
                {
                    noLoadProjectCount++;
                }
            }
            return noLoadProjectCount;
        }

        private void ConformWindow(string file, List<string> project)
        {
            if (_conformWindow != null)
            {
                _conformWindow.Focus();
                _conformWindow.Show();
                return;
            }

            var folder = "";
            if (!string.IsNullOrEmpty(file))
            {
                folder = new FileInfo(file).Directory?.FullName;
            }
            var window = new Window()
            {
                Width = 500,
                Height = 500
            };
            var conformPage = new ConformPage();
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
        public static void Initialize(Package package)
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
            var window = new Window();
            var definitionPage = new DefinitionPage();
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