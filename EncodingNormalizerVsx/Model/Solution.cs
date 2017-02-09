using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EncodingNormalizerVsx.Model
{
    public class Solution
    {
        //internal class SolutionParser
        //Name: Microsoft.Build.Construction.SolutionParser
        //Assembly: Microsoft.Build, Version=4.0.0.0

        static readonly Type s_SolutionParser;
        static readonly PropertyInfo s_SolutionParser_solutionReader;
        static readonly MethodInfo s_SolutionParser_parseSolution;
        static readonly PropertyInfo s_SolutionParser_projects;

        static Solution()
        {
            //s_SolutionParser_projects.GetValue()
            s_SolutionParser = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            if (s_SolutionParser != null)
            {
                s_SolutionParser_solutionReader = s_SolutionParser.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
                s_SolutionParser_projects = s_SolutionParser.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
                s_SolutionParser_parseSolution = s_SolutionParser.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public List<SolutionProject> Projects { get; private set; }

        public Solution(string solutionFileName)
        {
            if (s_SolutionParser == null)
            {
                throw new InvalidOperationException("Can not find type 'Microsoft.Build.Construction.SolutionParser' are you missing a assembly reference to 'Microsoft.Build.dll'?");
            }
            var solutionParser = s_SolutionParser.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);
            using (var streamReader = new StreamReader(solutionFileName))
            {
                s_SolutionParser_solutionReader.SetValue(solutionParser, streamReader, null);
                s_SolutionParser_parseSolution.Invoke(solutionParser, null);
            }
            var projects = new List<SolutionProject>();
            var array = (Array)s_SolutionParser_projects.GetValue(solutionParser, null);
            for (int i = 0; i < array.Length; i++)
            {
                projects.Add(new SolutionProject(array.GetValue(i)));
            }
            this.Projects = projects;
            GetProjectFullName(solutionFileName);
            //Object cfgArray = //s_SolutionParser_configurations.GetValue
            //    s_SolutionParser_projects.GetValue(solutionParser, null);
            //PropertyInfo[] pInfos = null;
            //pInfos = cfgArray.GetType().GetProperties();
            //int count = (int)pInfos[1].GetValue(cfgArray, null);

            //var configs = new List<SolutionConfiguration>();
            //for (int i = 0; i < count; i++)
            //{
            //    configs.Add(new SolutionConfiguration(pInfos[2].GetValue(cfgArray, new object[] { i })));
            //}

            //this.Configurations = configs;
        }

        private void GetProjectFullName(string solutionFileName)
        {
            DirectoryInfo solution = (new FileInfo(solutionFileName)).Directory;
            foreach (var temp in Projects.Where
                //(temp=>temp.RelativePath.EndsWith("csproj"))
                (temp => !temp.RelativePath.Equals(temp.ProjectName))
            )
            {
                GetProjectFullName(solution, temp);
            }
        }

        private void GetProjectFullName(DirectoryInfo solution, SolutionProject project)
        {
            //Uri newUri =new Uri(,UriKind./*Absolute*/);
            //if(project.RelativePath)

            project.FullName = System.IO.Path.Combine(solution.FullName, project.RelativePath);
        }
    }

    [DebuggerDisplay("{ProjectName}, {RelativePath}, {ProjectGuid}")]
    public class SolutionProject
    {
        static readonly Type s_ProjectInSolution;
        static readonly PropertyInfo s_ProjectInSolution_ProjectName;
        static readonly PropertyInfo s_ProjectInSolution_RelativePath;
        static readonly PropertyInfo s_ProjectInSolution_ProjectGuid;
        static readonly PropertyInfo s_ProjectInSolution_ProjectType;

        static SolutionProject()
        {
            s_ProjectInSolution = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            if (s_ProjectInSolution != null)
            {
                s_ProjectInSolution_ProjectName = s_ProjectInSolution.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_RelativePath = s_ProjectInSolution.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectGuid = s_ProjectInSolution.GetProperty("ProjectGuid", BindingFlags.NonPublic | BindingFlags.Instance);
                s_ProjectInSolution_ProjectType = s_ProjectInSolution.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        public string ProjectName { get; private set; }
        public string RelativePath { get; private set; }
        public string ProjectGuid { get; private set; }
        public string ProjectType { get; private set; }
        public string FullName { set; get; }

        public SolutionProject(object solutionProject)
        {
            this.ProjectName = s_ProjectInSolution_ProjectName.GetValue(solutionProject, null) as string;
            this.RelativePath = s_ProjectInSolution_RelativePath.GetValue(solutionProject, null) as string;
            this.ProjectGuid = s_ProjectInSolution_ProjectGuid.GetValue(solutionProject, null) as string;
            this.ProjectType = s_ProjectInSolution_ProjectType.GetValue(solutionProject, null).ToString();
        }
    }
}