# Eaf.HtmlSanitizer

Módulo EAF para sanitização de HTML baseado no HtmlSanitizer, removendo scripts, atributos de eventos e URIs `javascript:` por padrão.

## Dependências

- `Abp` 10.5.0
- `HtmlSanitizer` 9.0.892

## Uso

```csharp
[DependsOn(typeof(EafHtmlSanitizerModule))]
public class MyProjectModule : AbpModule
{
}
```

```csharp
public class MyService : ITransientDependency
{
    private readonly IHtmlSanitizer _htmlSanitizer;

    public MyService(IHtmlSanitizer htmlSanitizer)
    {
        _htmlSanitizer = htmlSanitizer;
    }

    public void Process(string input)
    {
        var safeHtml = _htmlSanitizer.Sanitize(input);
    }
}
```

## Configuração

```csharp
var options = new EafHtmlSanitizerOptions
{
    AllowedTags = { "p", "strong", "em" },
    AllowedAttributes = { "style" },
    AllowedUriSchemes = { "https", "mailto" }
};

var safe = htmlSanitizer.Sanitize(input, options);
```

Quando as coleções estiverem vazias, o módulo utiliza as configurações padrão do sanitizer.
