//------------------------------------------------------------------------------
// <copyright file="EncodingNormalizer.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace EncodingNormalizerVsx
{
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
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("0640f5ce-e6bc-43ba-b45e-497d70819a20");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingNormalizer"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private EncodingNormalizer(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.EncodingNormalizerCallback, menuCommandID);
                commandService.AddCommand(menuItem);

                menuCommandID = new CommandID(CommandSet, 0x0101);
                menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }
        private void EncodingNormalizerCallback(object sender, EventArgs e)
        {
            DTE dte = (DTE)ServiceProvider.GetService(typeof(DTE));
            var folder = dte.Solution.FullName;
            if (string.IsNullOrEmpty(folder))
            {
               // MessageBox.Show("Cant find the solution.", "少年，听说你没有打开工程");
                // return;
            }
            //ReadAccount();
            //MessageBox.Show(AppDomain.CurrentDomain.BaseDirectory, "路径");
            //string str = EncodingNormalizerPackage.DefinitionPage().CriterionEncoding.ToString();
            // MessageBox.Show(str, "路径");
           


        }

        //private void ReadAccount()
        //{
        //    var folder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
        //                 "\\EncodingNormalizer\\";
        //    var file = folder+ "Account.json";
        //    if (!Directory.Exists(folder))
        //    {
        //        Directory.CreateDirectory(folder);
        //    }
            
        //    using (StreamWriter stream = File.CreateText(file))
        //    {
        //        stream.Write("EncodingNormalizer");
        //    }

        //    using (StreamReader stream=File.OpenText(file))
        //    {
        //        string str = stream.ReadToEnd();
        //    }
        //}

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
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new EncodingNormalizer(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "EncodingNormalizer";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
