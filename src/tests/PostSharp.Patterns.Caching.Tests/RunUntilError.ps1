DO
{
    & mstest.exe /testcontainer:bin\Debug\PostSharp.Patterns.Caching.Tests.dll
} While ( $lastExitCode -eq 0 )

echo "Exit code: " $lastExitCode