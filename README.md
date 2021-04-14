# async-void-no-more

async-void-no-more is a simple tool to track down async void tests that should be async Task in C#.

## Example

This test:

```csharp
        public void UrlQueriesArrayMultiValid() => TestStatus(async (a, b) => await new lib.Search(a, b));
```

should return a Task, or else exceptions thrown will be lost and sadness will ensure.

## Isn't there a roslyn analyzer for this?

You'd thing, wouldn't you?!?

- There apparently [used to be ASYNC0003](https://roslyn-analyzers.readthedocs.io/en/latest/analyzers-info/async/avoid-async-void-methods.html) but I couldn't find it in the latest release, and it was unconditional.  
- [vs-threading](https://github.com/microsoft/vs-threading/blob/main/doc/analyzers/index.md) has VSTHRD100 but I was unable to get working in a reasonable time in my project.
