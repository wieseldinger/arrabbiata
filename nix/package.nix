{ buildDotnetModule
, dotnet-sdk_9
, dotnet-aspnetcore_9
}:

buildDotnetModule rec {
  pname = "arrabbiata";
  version = "0.1.0";

  src = ./..;

  projectFile = "arrabbiata/arrabbiata.csproj";
  nugetDeps = ./deps.nix;

  dotnet-sdk = dotnet-sdk_9;
  dotnet-runtime = dotnet-aspnetcore_9;

  executables = [ "arrabbiata" ];

  meta = {
    description = "Dynamic Pomodoro Timer";
    homepage = "https://github.com/wieseldinger/arrabbiata";
  };
}
