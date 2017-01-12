//------------------------------------------------------------------------------
// <copyright file="EncodingNormalizerPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace EncodingNormalizerVsx
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(EncodingNormalizerPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideProfile(typeof(DefinitionPage), "EncodingNormalizer", "DefinitionPage", 101, 104, true)]
    [ProvideOptionPage(typeof(DefinitionPage), "EncodingNormalizer", "DefinitionPage", 101, 104, true)]
    public sealed class EncodingNormalizerPackage : Package
    {
        /// <summary>
        /// EncodingNormalizerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "ffc5dabf-5ded-4433-8225-73b47e154210";

        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingNormalizer"/> class.
        /// </summary>
        public EncodingNormalizerPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            EncodingNormalizer.Initialize(this);
            base.Initialize();
        }

        public static EncodingNormalizerPackage EnsureEncodingNormalizerPackage()
        {
            IVsShell shell = Package.GetGlobalService(typeof(SVsShell)) as IVsShell;
            if (shell != null)
            {
                IVsPackage package;
                Guid guid = new Guid(EncodingNormalizerPackage.PackageGuidString);
                if (ErrorHandler.Succeeded(shell.IsPackageLoaded(ref guid, out package)))
                {
                    return package as EncodingNormalizerPackage;
                }
                if (ErrorHandler.Succeeded(shell.LoadPackage(ref guid, out package)))
                {
                    return package as EncodingNormalizerPackage;
                }
            }
            return null;
        }

        public static DefinitionPage DefinitionPage()
        {
            EncodingNormalizerPackage package = EnsureEncodingNormalizerPackage();
            return package?.GetDialogPage(typeof(DefinitionPage)) as DefinitionPage;
        }


        #endregion
    }
}
