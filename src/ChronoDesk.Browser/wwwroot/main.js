import { dotnet } from './_framework/dotnet.js'

const isBrowser = typeof window !== 'undefined';
if (!isBrowser) throw new Error('ChronoDesk Browser must run in a browser.');

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();
await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
