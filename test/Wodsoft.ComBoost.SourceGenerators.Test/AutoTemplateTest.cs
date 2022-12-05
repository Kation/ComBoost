using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.SourceGenerators.Test
{
    public class AutoTemplateTest
    {
        [Fact]
        public void NormalTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public partial class GreeterService : DomainService
    {
        public Task Test(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }

    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(IGreeterService))]
    public partial class GreeterService { }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void AsyncTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public partial class GreeterService : DomainService
    {
        public async Task Test(Guid id)
        {

        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }

    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(IGreeterService))]
    public partial class GreeterService { }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void PrivateMethodTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public partial class GreeterService : DomainService
    {
        public Task Test(Guid id)
        {
            return Task.CompletedTask;
        }

        private Test PrivateMethod()
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }

    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(IGreeterService))]
    public partial class GreeterService { }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void FromServiceTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public partial class GreeterService : DomainService
    {
        public Task Test([FromService] IServiceProvider serviceProvider, Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }

    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(IGreeterService))]
    public partial class GreeterService { }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void AliasTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;
using AT = Wodsoft.ComBoost.AutoTemplateAttribute;

namespace Wodsoft.ComBoost.Test
{
    [AT]
    public partial class GreeterService : DomainService
    {
        public Task Test(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;
using AT = Wodsoft.ComBoost.AutoTemplateAttribute;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }

    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(IGreeterService))]
    public partial class GreeterService { }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void HalfNamespaceTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft
{
    [ComBoost.AutoTemplate]
    public partial class GreeterService : ComBoost.DomainService
    {
        public Task Test(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }

    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(IGreeterService))]
    public partial class GreeterService { }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void TemplateNameTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate(TemplateName = ""ITestService"")]
    public partial class GreeterService : DomainService
    {
        public Task Test(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface ITestService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }

    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(ITestService))]
    public partial class GreeterService { }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void NewNamespaceTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate(Namespace = ""Test.Templates"")]
    public partial class GreeterService : DomainService
    {
        public Task Test(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Test;

namespace Test.Templates
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }
}

namespace Wodsoft.ComBoost.Test
{
    [global::Wodsoft.ComBoost.DomainTemplateImplementer(typeof(global::Test.Templates.IGreeterService))]
    public partial class GreeterService { }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void NotPartialTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public class GreeterService : DomainService
    {
        public Task Test(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test(Guid id);
    }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void IncludeTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public class GreeterService : DomainService
    {
        [AutoTemplateMethod(IsIncluded = true)]
        public Task Test1(Guid id)
        {
            return Task.CompletedTask;
        }

        public Task Test2(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test1(Guid id);
    }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void ExcludeTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public class GreeterService : DomainService
    {
        public Task Test1(Guid id)
        {
            return Task.CompletedTask;
        }

        [AutoTemplateMethod(IsExcluded = true)]
        public Task Test2(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test1(Guid id);
    }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void IncludeMixExcludeTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    public class GreeterService : DomainService
    {
        [AutoTemplateMethod(IsExcluded = true, IsIncluded = true)]
        public Task Test1(Guid id)
        {
            return Task.CompletedTask;
        }

        [AutoTemplateMethod(IsIncluded = true)]
        public Task Test2(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Single(texts);
            var generatedCode = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test1(Guid id);
        Task Test2(Guid id);
    }
}";
            Assert.Equal(generatedCode, texts[0]);
        }

        [Fact]
        public void GroupIncludeTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    [AutoTemplate(Group = ""Internal"", TemplateName = ""IGreeterInternalService"")]
    public class GreeterService : DomainService
    {
        [AutoTemplateMethod(IsIncluded = true)]
        public Task Test1(Guid id)
        {
            return Task.CompletedTask;
        }

        [AutoTemplateMethod(IsIncluded = true, Group = ""Internal"")]
        public Task Test2(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Equal(2, texts.Count);
            var generatedCode1 = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test1(Guid id);
    }
}";
            Assert.Equal(generatedCode1, texts[0]);

            var generatedCode2 = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterInternalService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test2(Guid id);
    }
}";
            Assert.Equal(generatedCode2, texts[1]);
        }

        [Fact]
        public void GroupExcludeTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    [AutoTemplate]
    [AutoTemplate(Group = ""Internal"", TemplateName = ""IGreeterInternalService"")]
    public class GreeterService : DomainService
    {
        public Task Test1(Guid id)
        {
            return Task.CompletedTask;
        }

        [AutoTemplateMethod(IsExcluded = true)]
        public Task Test2(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Equal(2, texts.Count);
            var generatedCode1 = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test1(Guid id);
    }
}";
            Assert.Equal(generatedCode1, texts[0]);

            var generatedCode2 = @"// ComBoost auto generated domain template interface.
using System;
using System.Threading.Tasks;

namespace Wodsoft.ComBoost.Test
{
    public partial interface IGreeterInternalService : global::Wodsoft.ComBoost.IDomainTemplate
    {
        Task Test1(Guid id);
        Task Test2(Guid id);
    }
}";
            Assert.Equal(generatedCode2, texts[1]);
        }

        [Fact]
        public void DoNotGenerateTest()
        {
            var sourceCode = @"
using System;
using System.Threading.Tasks;
using Wodsoft.ComBoost;

namespace Test.ABC
{
    public interface IGreeterService { }

    [DomainTemplateImplementer(typeof(""IGreeterService""))]
    public class GreeterService : DomainService
    {
        public Task Test1(Guid id)
        {
            return Task.CompletedTask;
        }

        [AutoTemplateMethod(IsExcluded = true)]
        public Task Test2(Guid id)
        {
            return Task.CompletedTask;
        }
    }
}
";
            var texts = Run<DomainTemplateSourceGenerator>(sourceCode);
            Assert.Empty(texts);
        }

        private static List<string> Run<T>(params string[] sources) where T : ISourceGenerator, new()
        {
            var inputCompilation = CreateCompilation(sources);

            GeneratorDriver driver = CSharpGeneratorDriver.Create(new T());

            driver = driver
                .WithUpdatedAnalyzerConfigOptions(
                    CreateAnalyzerConfigOptionsProvider(new Dictionary<string, string>()
                    {
                        ["build_property.projectdir"] =
                            AppContext.BaseDirectory[..AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal)]
                    })
                )
                .RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            var outDiagnostics = outputCompilation.GetDiagnostics();
            foreach (var diagnostic in outDiagnostics)
            {

            }

            var runResult = driver.GetRunResult();

            var exception = runResult.Results.Select(m => m.Exception).FirstOrDefault();
            if (exception != null)
            {
                throw new Exception(exception.Message, exception);
            }

            return runResult.GeneratedTrees.Select(m => m.GetText().ToString()).ToList();
        }

        private static Compilation CreateCompilation(IEnumerable<string> sources)
        {
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var assemblies = ReferenceAssemblies.NetStandard.NetStandard20.ResolveAsync(null, CancellationToken.None)
                .Result.ToList();
            assemblies.Add(MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location));
            assemblies.Add(MetadataReference.CreateFromFile(typeof(IDomainService).Assembly.Location));

            var syntaxTrees = sources.Select(m => CSharpSyntaxTree.ParseText(m));
            var compilation = CSharpCompilation.Create("compilation", syntaxTrees, assemblies,
                options
            );

            return compilation;
        }

        private static AnalyzerConfigOptionsProvider CreateAnalyzerConfigOptionsProvider(Dictionary<string, string> dictionary) =>
            new MockAnalyzerConfigOptionsProvider(new MockAnalyzerConfigOptions(dictionary));

        public class MockAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
        {
            public MockAnalyzerConfigOptionsProvider(AnalyzerConfigOptions options)
            {
                GlobalOptions = options;
            }

            public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
            {
                return GlobalOptions;
            }

            public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
            {
                return GlobalOptions;
            }

            public override AnalyzerConfigOptions GlobalOptions { get; }
        }

        public class MockAnalyzerConfigOptions : AnalyzerConfigOptions
        {
            private readonly Dictionary<string, string> _dictionary;

            public MockAnalyzerConfigOptions(Dictionary<string, string> dictionary)
            {
                this._dictionary = dictionary;
            }

            public override bool TryGetValue(string key, out string value)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }
    }
}
