using EncodingNormalizerVsx.View;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Task = System.Threading.Tasks.Task;
using Window = System.Windows.Window;
#pragma warning disable IDE0090 // 忽略警告

namespace EncodingNormalizerVsix;

/// <summary>
/// Command handler
/// </summary>
internal sealed class EncodingNormalizer
{
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 0x0100;

    /// <summary>
    ///     Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("0640f5ce-e6bc-43ba-b45e-497d70819a20");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly AsyncPackage _package;

    private Window _conformWindow;
    private Window _definitionWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="EncodingNormalizer"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    /// <param name="commandService">Command service to add command to, not null.</param>
    private EncodingNormalizer(AsyncPackage package, OleMenuCommandService commandService)
    {
        _package = package ?? throw new ArgumentNullException(nameof(package));
        commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

        CommandID menuCommandID = new CommandID(CommandSet, CommandId);
        MenuCommand menuItem = new MenuCommand(EncodingNormalizerCallback, menuCommandID);
        commandService.AddCommand(menuItem);

        CommandID convertCurrentFileSaveEncodingCommand = new CommandID(CommandSet, 0x0103);
        MenuCommand convertCurrentEncodingMenuCommand = new MenuCommand(ConvertCurrentFileEncoding, convertCurrentFileSaveEncodingCommand);
        commandService.AddCommand(convertCurrentEncodingMenuCommand);

        menuCommandID = new CommandID(CommandSet, 0x0101);
        menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
        commandService.AddCommand(menuItem);
    }

    /// <summary>
    /// 修改用户打开的文件的编码
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ConvertCurrentFileEncoding(object sender, EventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(_package.DisposalToken);
        // 修改用户打开的文件的编码
        DTE dte = (DTE)await ServiceProvider.GetServiceAsync(typeof(DTE));
        Document document = dte.ActiveDocument;

        if (document is null)
        {
            IServiceProvider serviceProvider = (IServiceProvider)ServiceProvider;
            VsShellUtilities.ShowMessageBox(serviceProvider, "没有找到打开的文档", "修改用户打开的文件的编码失败", OLEMSGICON.OLEMSGICON_WARNING, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            return;
        }
        string str = document.FullName;

        new ConvertFileEncodingPage(new FileInfo(str)).Show();
    }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static EncodingNormalizer Instance
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the service provider from the owner package.
    /// </summary>
    private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
    {
        get
        {
            return this._package;
        }
    }

    private async void EncodingNormalizerCallback(object sender, EventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(_package.DisposalToken);
        DTE dte = (DTE)await ServiceProvider.GetServiceAsync(typeof(DTE));
        string file = dte.Solution.FullName;
        List<string> project = new List<string>();
        if (dte.Solution.Projects.Count > 0)
        {
            //try
            //{
            int noLoadProjectCount = TryParseProject(dte, project);
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
        ThreadHelper.ThrowIfNotOnUIThread();
        int noLoadProjectCount = 0;

        foreach (object temp in dte.Solution.Projects)
        {
            try
            {
                if (temp is Project)
                {
                    if (((Project)temp).Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        project.AddRange(GetSolutionFolderProjects((Project)temp).Select(ParseProjectFolder));
                    }
                    else
                    {
                        project.Add(ParseProjectFolder((Project)temp));
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

    private static string ParseProjectFolder(Project project)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
        string file = project.FullName;
        if (!string.IsNullOrEmpty(file))
        {
            return new FileInfo(file).Directory?.FullName;
        }
        return "";
    }

    private static List<Project> GetSolutionFolderProjects(Project solutionFolder)
    {
        ThreadHelper.ThrowIfNotOnUIThread();
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
                project.AddRange(GetSolutionFolderProjects(subProject));
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
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static async Task InitializeAsync(AsyncPackage package)
    {
        // Switch to the main thread - the call to AddCommand in HelloCommand's constructor requires
        // the UI thread.
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

        OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
        Instance = new EncodingNormalizer(package, commandService);
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
