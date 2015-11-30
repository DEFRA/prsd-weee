namespace EA.Weee.Web.Tests.Integration.Views
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FakeItEasy;
    using Microsoft.Build.Evaluation;
    using Microsoft.Owin.Builder;
    using RazorEngine;
    using RazorEngine.Configuration;
    using RazorEngine.Templating;
    using Xunit;

    public class ViewTests
    {
        public const string WebTestProjectFolderName = "EA.Weee.Web.Tests.Integration";
        public const string WebProjectFolderName = "EA.Weee.Web";
        public const string WebProjectFileName = "EA.Weee.Web.csproj";
        public const string WebProjectAssemblyName = WebProjectFolderName + ".dll";

        [Fact(Skip = "This shouldn't be run all the time, and isn't complete however maybe useful to run locally to check for non-compiling views")]
        public void AllViewsCompile()
        {
            var test = new AppBuilder();

            LoadAssembliesIntoAppDomain();

            var folder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (folder.Parent != null && folder.Name != WebTestProjectFolderName)
            {
                folder = new DirectoryInfo(folder.Parent.FullName);
            }

            var webProjectFolder = folder.Parent.GetDirectories(WebProjectFolderName).Single();
            var webProject = new Project(webProjectFolder.GetFiles(WebProjectFileName).Single().FullName);

            var views = webProject.Items
                .Where(i => i.ItemType == "Content" 
                    && i.EvaluatedInclude.EndsWith(".cshtml"));

            foreach (var view in views)
            {
                var viewPath = Path.Combine(webProjectFolder.FullName, view.EvaluatedInclude);
                var viewNamespace = viewPath.Substring(0, viewPath.LastIndexOf("\\", StringComparison.InvariantCultureIgnoreCase));
                var viewName = viewPath.Replace(viewNamespace + "\\", string.Empty);
                var modelName = GetModelName(viewPath);

                if (!string.IsNullOrEmpty(modelName))
                {
                    // If this can be processed, the view can compile OK
                    var model = Activator.CreateInstance(typeof(Startup).Assembly.FullName, modelName).Unwrap();
                    var viewContent = GetViewContent(viewPath);

                    viewContent = string.Format("@inherits System.Web.Mvc.WebViewPage<{0}>\r\n{1}", model, viewContent);
                    viewContent = "@using System.Web.Mvc\r\n" + viewContent;
                    viewContent = "@using System.Web.Mvc.Html\r\n" + viewContent;
                    viewContent = viewContent.Replace("@model " + modelName, string.Empty);

                    var razorService = RazorEngineService.Create(new FluentTemplateServiceConfiguration(config =>
                    {
                        config.WithCodeLanguage(Language.CSharp);
                        config.WithBaseTemplateType(typeof(HtmlTemplateBase<>));
                    }));

                    try
                    {
                        razorService.RunCompile(viewContent, viewName, null, model);
                    }
                    catch (Exception)
                    {
                        // Check out exceptions here...
                    }
                }
            }
        }

        public string GetModelName(string viewPath)
        {
            using (var stream = new FileStream(viewPath, FileMode.Open))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    while (streamReader.Peek() > 0)
                    {
                        var fileLine = streamReader.ReadLine();

                        if (fileLine != null && fileLine.Contains(string.Format("@model {0}", WebProjectFolderName)))
                        {
                            return fileLine.Replace("@model ", string.Empty);
                        }
                    }
                }
            }

            return null;
        }

        public string GetViewContent(string viewPath)
        {
            using (var stream = new FileStream(viewPath, FileMode.Open))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public void LoadAssembliesIntoAppDomain()
        {
            var folder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            var files = folder.GetFiles().Where(f => f.Name.EndsWith(".dll"));

            foreach (var file in files)
            {
                AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file.FullName));
            }
        }
    }
}
